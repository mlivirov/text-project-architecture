# Script for launching Stupid Chat projects on Windows
Write-Host "Starting Stupid Chat..." -ForegroundColor Green

# Navigate to project directory
$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition

# Launch .NET API in background
Write-Host "Starting .NET API..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$projectRoot\api\StupidChat'; dotnet run" -WindowStyle Normal

# Wait a bit for API to start
Start-Sleep -Seconds 3

# Launch Angular application
Write-Host "Starting Angular application..." -ForegroundColor Yellow
Set-Location "$projectRoot\web"

# Check if node_modules exists
if (!(Test-Path "node_modules")) {
    Write-Host "Installing npm dependencies..." -ForegroundColor Cyan
    npm install
}

# Start Angular dev server
npm start

Write-Host "Applications started!" -ForegroundColor Green
Write-Host "API: https://localhost:5298" -ForegroundColor Cyan
Write-Host "Web: http://localhost:4200" -ForegroundColor Cyan
