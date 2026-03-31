@echo off
echo Setting up Migrations for LearnConnect...

cd LearnConnect.API

echo Installing dotnet-ef tool...
dotnet tool install --global dotnet-ef
REM Ignore error if tool is already installed

echo Dropping existing database (if matches old schema) to ensure clean migration state...
dotnet ef database drop -f
IF %ERRORLEVEL% NEQ 0 (dotnet ef database updat
    echo.
    echo [ERROR] Failed to drop the database. It might be in use by another program (like SQL Server Management Studio).
    echo Please close any open connections to 'LearnConnectDB' and try again.
    echo.
    pause
    exit /b %ERRORLEVEL%
)

echo Adding InitialCreate migration...
dotnet ef migrations add InitialCreate
IF %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Failed to create migration.
    echo.
    pause
    exit /b %ERRORLEVEL%
)

echo Updating database...
dotnet ef database update
IF %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Failed to update database.
    echo.
    pause
    exit /b %ERRORLEVEL%
)

echo Done!
pause
