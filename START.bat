@echo off
cls
echo.
echo ========================================
echo    LearnConnect - Learning Platform
echo ========================================
echo.
echo Starting the application...
echo.

cd /d "%~dp0LearnConnect.API"

echo [Step 1/2] Building project...
dotnet build --verbosity quiet

if %errorlevel% neq 0 (
    echo.
    echo ERROR: Build failed!
    echo Please make sure .NET 8.0 SDK is installed.
    echo.
    pause
    exit /b %errorlevel%
)

echo [Step 2/2] Starting server...
echo.
echo ========================================
echo   Application is starting!
echo ========================================
echo.
echo   Open your browser and go to:
echo   https://localhost:5001
echo.
echo   Default Admin Login:
echo   Email: admin@learnconnect.com
echo   Password: Admin123!
echo.
echo   Press Ctrl+C to stop the server
echo ========================================
echo.

start https://localhost:5001

dotnet run --no-build --urls="http://0.0.0.0:5000;https://0.0.0.0:5001"

pause
