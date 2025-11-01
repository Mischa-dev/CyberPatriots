@echo off
echo CyberPatriot Security Helper
echo ==============================
echo.
echo This application requires Administrator privileges to function properly.
echo.
echo Starting application...
echo.

REM Check if running as administrator
net session >nul 2>&1
if %errorLevel% == 0 (
    echo Running with Administrator privileges...
    dotnet run --project CyberPatriotHelper\CyberPatriotHelper.csproj
) else (
    echo ERROR: This script must be run as Administrator!
    echo.
    echo Right-click this file and select "Run as administrator"
    echo.
    pause
    exit /b 1
)
