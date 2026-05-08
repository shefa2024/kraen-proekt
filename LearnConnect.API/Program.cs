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
var parsedConnectionString = ""; // Store for debug

// If DATABASE_URL is not set but DefaultConnection contains a postgresql URL, use that
if (string.IsNullOrEmpty(databaseUrl) && connectionString != null && connectionString.StartsWith("postgres", StringComparison.OrdinalIgnoreCase))
{
    databaseUrl = connectionString;
}

if (!string.IsNullOrEmpty(databaseUrl) && databaseUrl.StartsWith("postgres", StringComparison.OrdinalIgnoreCase))
{
    // Normalize postgresql:// to postgres:// for Uri parsing
    var normalizedUrl = databaseUrl.Replace("postgresql://", "postgres://");
    var databaseUri = new Uri(normalizedUrl);
    var userInfo = databaseUri.UserInfo.Split(':');
    var host = databaseUri.Host;
    var port = databaseUri.Port > 0 ? databaseUri.Port : 5432;
    var database = databaseUri.LocalPath.TrimStart('/');
    var username = userInfo[0];
    var password = userInfo.Length > 1 ? userInfo[1] : "";
    
    // Render internal URLs (dpg-xxx-a) do NOT support SSL
    // Render external URLs (dpg-xxx-a.region-postgres.render.com) REQUIRE SSL
    var isInternal = !host.Contains(".");
    var sslMode = isInternal ? "Disable" : "Require";
    var sslExtra = isInternal ? "" : "Trust Server Certificate=true;";
    
    connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode={sslMode};{sslExtra}Timeout=30;Command Timeout=30;";
    parsedConnectionString = $"Host={host};Port={port};Database={database};Username={username};Password=***;SSL Mode={sslMode};{sslExtra}Timeout=30;Command Timeout=30;";
    
    Console.WriteLine($"[DB] PostgreSQL URL detected. Host={host}, Internal={isInternal}, SSL={sslMode}");
}
else
{
    Console.WriteLine($"[DB] No PostgreSQL URL found. Using DefaultConnection as-is.");
    parsedConnectionString = connectionString ?? "Not configured";
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (connectionString != null && (connectionString.Contains("localdb", StringComparison.OrdinalIgnoreCase) || connectionString.Contains("sqlexpress", StringComparison.OrdinalIgnoreCase)))
    {
        Console.WriteLine("[DB] Using SQL Server provider");
        options.UseSqlServer(connectionString);
    }
    else
    {
        Console.WriteLine("[DB] Using PostgreSQL (Npgsql) provider");
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

app.MapGet("/api/debug/db", (ApplicationDbContext context) => 
{
    try 
    {
        var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL") ?? "Not set";
        bool canConnect = false;
        string error = "";
        try {
            canConnect = context.Database.CanConnect();
        } catch (Exception connEx) {
            error = connEx.Message;
        }
        
        return Results.Ok(new { 
            CanConnect = canConnect, 
            ParsedConnectionString = parsedConnectionString,
            OriginalUrl = databaseUrl,
            Provider = context.Database.ProviderName,
            Error = error
        });
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

// Initialize database with retry for Render cold starts
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    const int maxRetries = 5;
    
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            
            if (context.Database.IsSqlServer())
            {
                logger.LogInformation("[DB] Using SQL Server – calling EnsureCreated");
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
                logger.LogInformation("[DB] Using PostgreSQL – running Migrate() (attempt {Attempt}/{Max})", attempt, maxRetries);
                context.Database.Migrate();
                logger.LogInformation("[DB] Migration complete.");
                
                // Runtime seeder for Render if tables are empty
                if (!context.Subjects.Any())
                {
                    logger.LogInformation("[DB] Database is empty. Applying runtime seed data...");
                    DataSeeder.SeedData(context);
                    logger.LogInformation("[DB] Runtime seed data applied successfully.");
                }
                else
                {
                    logger.LogInformation("[DB] Database already has data – skipping seed.");
                }
            }
            
            break; // Success – exit retry loop
        }
        catch (Exception ex) when (attempt < maxRetries)
        {
            var logger2 = services.GetRequiredService<ILogger<Program>>();
            logger2.LogWarning(ex, "[DB] Attempt {Attempt}/{Max} failed. Retrying in 3 seconds...", attempt, maxRetries);
            Thread.Sleep(3000);
        }
        catch (Exception ex)
        {
            var logger2 = services.GetRequiredService<ILogger<Program>>();
            logger2.LogError(ex, "[DB] All {Max} migration attempts failed. App will start but DB may be unavailable.", maxRetries);
        }
    }
}

app.Run();
