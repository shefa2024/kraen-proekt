using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using LearnConnect.API.Data;
using LearnConnect.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

// Configure Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "LearnConnect API", 
        Version = "v1",
        Description = "API for the LearnConnect learning platform"
    });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure Database - Use SQL Server for local development, PostgreSQL for production
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrEmpty(databaseUrl))
{
    var databaseUri = new Uri(databaseUrl);
    var userInfo = databaseUri.UserInfo.Split(':');
    var port = databaseUri.Port > 0 ? databaseUri.Port : 5432;
    connectionString = $"Host={databaseUri.Host};Port={port};Database={databaseUri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};";
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (connectionString != null && (connectionString.Contains("localdb", StringComparison.OrdinalIgnoreCase) || connectionString.Contains("sqlexpress", StringComparison.OrdinalIgnoreCase)))
    {
        options.UseSqlServer(connectionString);
    }
    else
    {
        options.UseNpgsql(
            connectionString,
            npgsqlOptions => npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
    }
});

// Configure JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "YourSuperSecretKeyForJWTTokenGeneration12345!";
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGamificationService, GamificationService>();
builder.Services.AddHostedService<ReminderService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.SetIsOriginAllowed(_ => true)
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials()
               .WithExposedHeaders("X-Total-Count");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LearnConnect API V1");
});

app.MapGet("/api/debug/db", (ApplicationDbContext context, IConfiguration config) => 
{
    try 
    {
        var canConnect = context.Database.CanConnect();
        var connStr = config.GetConnectionString("DefaultConnection") ?? Environment.GetEnvironmentVariable("DATABASE_URL") ?? "Not found";
        // Mask password
        if (connStr.Contains("Password", StringComparison.OrdinalIgnoreCase)) {
            var parts = connStr.Split(';');
            for (int i=0; i<parts.Length; i++) {
                if (parts[i].TrimStart().StartsWith("Password", StringComparison.OrdinalIgnoreCase) || parts[i].TrimStart().StartsWith("Pwd", StringComparison.OrdinalIgnoreCase)) {
                    parts[i] = "Password=***";
                }
            }
            connStr = string.Join(";", parts);
        }
        
        // Also mask URL password
        if (connStr.StartsWith("postgres://")) {
            var uri = new Uri(connStr);
            var userInfo = uri.UserInfo.Split(':');
            if (userInfo.Length > 1) {
                connStr = connStr.Replace(userInfo[1], "***");
            }
        }
        
        return Results.Ok(new { CanConnect = canConnect, ConnectionString = connStr, Provider = context.Database.ProviderName });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.ToString());
    }
});

// app.UseCors
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Serve static files from wwwroot
app.UseStaticFiles();

// Fallback to index.html for SPA routing
app.MapFallbackToFile("index.html");

app.MapControllers();
app.MapHub<LearnConnect.API.Hubs.WhiteboardHub>("/hubs/whiteboard");
app.MapHub<LearnConnect.API.Hubs.NotebookHub>("/hubs/notebook");
app.MapHub<LearnConnect.API.Hubs.VideoCallHub>("/hubs/videocall");

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        if (context.Database.IsSqlServer())
        {
            context.Database.EnsureCreated();
            
            // Apply automated fix for LessonPackages schema mismatch since we lack SQL Server migrations
            try {
                context.Database.ExecuteSqlRaw(@"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LessonPackages')
                    BEGIN
                        CREATE TABLE LessonPackages (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            StudentId INT NOT NULL,
                            TeacherId INT NOT NULL,
                            SubjectId INT NOT NULL,
                            TotalLessons INT NOT NULL,
                            RemainingLessons INT NOT NULL,
                            TotalPrice DECIMAL(18,2) NOT NULL,
                            Status INT NOT NULL,
                            CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                            CONSTRAINT FK_LessonPackages_Students FOREIGN KEY (StudentId) REFERENCES Students(Id) ON DELETE NO ACTION,
                            CONSTRAINT FK_LessonPackages_Teachers FOREIGN KEY (TeacherId) REFERENCES Teachers(Id) ON DELETE NO ACTION,
                            CONSTRAINT FK_LessonPackages_Subjects FOREIGN KEY (SubjectId) REFERENCES Subjects(Id) ON DELETE CASCADE
                        );
                    END
                    
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Lessons') AND name = 'LessonPackageId')
                    BEGIN
                        ALTER TABLE Lessons ADD LessonPackageId INT NULL;
                        ALTER TABLE Lessons ADD CONSTRAINT FK_Lessons_LessonPackages FOREIGN KEY (LessonPackageId) REFERENCES LessonPackages(Id);
                    END
                ");
            } catch (Exception) { /* Ignore if it fails */ }
        }
        else
        {
            context.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred migrating the DB.");
    }
}

app.Run();
