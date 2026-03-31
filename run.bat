@echo off
echo ========================================
echo LearnConnect - Learning Platform
echo ========================================
echo.

cd LearnConnect.API

echo [1/3] Restoring NuGet packages...
dotnet restore
if %errorlevel% neq 0 (
    echo Failed to restore packages!
    pause
    exit /b %errorlevel%
)

echo.
echo [2/3] Building project...
dotnet build
if %errorlevel% neq 0 (
    echo Build failed!
    pause
    exit /b %errorlevel%
)

echo.
echo [3/3] Starting application...
echo.
echo Application will be available at:
echo   - https://localhost:5001
echo   - http://localhost:5000
echo.
echo Default Admin Account:
echo   Email: admin@learnconnect.com
echo   Password: Admin123!
echo.
echo Press Ctrl+C to stop the server
echo.

dotnet run --urls="http://0.0.0.0:5000;https://0.0.0.0:5001"

pause
