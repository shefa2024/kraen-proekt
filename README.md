# LearnConnect - Learning Platform

A comprehensive learning platform built with ASP.NET Core and modern web technologies. Connect students with expert teachers for personalized one-on-one lessons.

## Features

### User Roles
- **Students**: Browse teachers, book lessons, write reviews
- **Teachers**: Manage profile, accept bookings, conduct lessons
- **Admin**: Platform oversight, user management, analytics

### Core Functionality
- **Teacher Discovery**: Search and filter teachers by subject, price, rating
- **Booking System**: Request and confirm lesson reservations
- **Lesson Management**: Track upcoming, ongoing, and completed lessons
- **Review System**: Rate and review teachers after lessons
- **Schedule Management**: Teachers can set their availability
- **Real-time Dashboard**: Role-based dashboards for all users

## Technology Stack

### Backend
- **ASP.NET Core 8.0** - Web API
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **JWT Authentication** - Secure token-based auth
- **BCrypt** - Password hashing

### Frontend
- **HTML5** - Semantic markup
- **CSS3** - Modern styling with CSS variables
- **Vanilla JavaScript** - No framework dependencies
- **Responsive Design** - Mobile-first approach

## Project Structure

```
LearnConnect.API/
├── Controllers/          # API endpoints
│   ├── AuthController.cs
│   ├── TeachersController.cs
│   ├── LessonsController.cs
│   ├── ReservationsController.cs
│   ├── ReviewsController.cs
│   ├── AdminController.cs
│   └── SubjectsController.cs
├── Models/              # Data models
│   ├── User.cs
│   ├── Student.cs
│   ├── Teacher.cs
│   ├── Lesson.cs
│   ├── Reservation.cs
│   ├── Review.cs
│   ├── Schedule.cs
│   └── Subject.cs
├── Data/                # Database context
│   └── ApplicationDbContext.cs
├── DTOs/                # Data transfer objects
│   └── DTOs.cs
├── Services/            # Business logic
│   └── AuthService.cs
├── wwwroot/             # Frontend files
│   ├── index.html
│   ├── css/
│   │   └── styles.css
│   └── js/
│       ├── config.js
│       ├── api.js
│       ├── auth.js
│       └── app.js
├── Program.cs           # Application entry point
└── appsettings.json     # Configuration
```

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server or SQL Server LocalDB
- Modern web browser

### Installation

1. **Restore packages**
   ```bash
   cd LearnConnect.API
   dotnet restore
   ```

2. **Update database connection** (if needed)
   Edit `appsettings.json` and update the connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LearnConnectDB;Trusted_Connection=true;TrustServerCertificate=true"
   }
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Access the application**
   Open your browser and navigate to:
   - `https://localhost:5001` (HTTPS)
   - `http://localhost:5000` (HTTP)

### Default Admin Account
- **Email**: admin@learnconnect.com
- **Password**: Admin123!

## API Documentation

Once running, access the Swagger documentation at:
- `https://localhost:5001/swagger`

### Key Endpoints

#### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user

#### Teachers
- `GET /api/teachers` - Get all teachers (with filters)
- `GET /api/teachers/{id}` - Get teacher profile
- `PUT /api/teachers/profile` - Update teacher profile
- `GET /api/teachers/{id}/reviews` - Get teacher reviews

#### Lessons
- `GET /api/lessons/my-lessons` - Get user's lessons
- `PUT /api/lessons/{id}/status` - Update lesson status

#### Reservations
- `POST /api/reservations` - Create booking request
- `GET /api/reservations/my-reservations` - Get user's reservations
- `PUT /api/reservations/{id}/status` - Confirm/cancel reservation

#### Reviews
- `POST /api/reviews` - Create review (students only)

#### Admin
- `GET /api/admin/dashboard` - Get platform statistics
- `GET /api/admin/users` - Get all users
- `PUT /api/admin/users/{id}/toggle-active` - Activate/deactivate user

## Database Schema

### Main Entities
1. **Users** - Base user accounts with roles
2. **Students** - Student-specific profile data
3. **Teachers** - Teacher profiles with rates and bio
4. **Subjects** - Available teaching subjects
5. **Lessons** - Scheduled teaching sessions
6. **Reservations** - Booking requests
7. **Reviews** - Student feedback
8. **Schedules** - Teacher availability

## Features Implementation

### Authentication & Authorization
- JWT token-based authentication
- Role-based access control (Student, Teacher, Admin)
- Secure password hashing with BCrypt

### Search & Filtering
- Full-text search on teacher names and bios
- Filter by subject, price range, rating
- Pagination support

### Booking Flow
1. Student browses teachers
2. Student sends booking request
3. Teacher receives notification
4. Teacher confirms/declines
5. Lesson is created upon confirmation
6. Both parties can manage the lesson

### Review System
- Students can review teachers after completed lessons
- Automatic rating calculation
- Reviews visible on teacher profiles

## Development

### Adding New Features
1. Create/update models in `Models/`
2. Update `ApplicationDbContext.cs`
3. Create DTOs in `DTOs/`
4. Implement controller in `Controllers/`
5. Update frontend in `wwwroot/`

### Database Migrations
```bash
# Add migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update
```

## Testing

The project includes:
- API endpoint testing via Swagger
- Manual testing through the web interface
- Role-based access testing

### Test Scenarios
1. User registration and login
2. Teacher profile creation and updates
3. Student booking flow
4. Teacher accepting/declining bookings
5. Lesson completion and reviews
6. Admin dashboard access

## Deployment

### Production Checklist
- [ ] Update JWT secret in `appsettings.json`
- [ ] Configure production database connection
- [ ] Enable HTTPS
- [ ] Set up CORS for production domain
- [ ] Configure logging
- [ ] Set up email notifications (future feature)

## Future Enhancements

- Real-time chat between students and teachers
- Video conferencing integration
- Payment processing
- Email notifications
- Mobile app
- Advanced scheduling with calendar integration
- Teacher certifications and verification
- Course packages and subscriptions

## License

This project is created for educational purposes.

## Support

For issues and questions, please create an issue in the repository.
