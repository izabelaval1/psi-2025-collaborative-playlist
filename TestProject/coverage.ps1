# 1) Clean old coverage files
Write-Host "Cleaning old coverage output..." -ForegroundColor Yellow
Remove-Item -Recurse -Force ".\TestProject\TestResults\" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force ".\coverage-report\" -ErrorAction SilentlyContinue

# 2) Run tests AND collect coverage
Write-Host "Running dotnet tests with coverage..." -ForegroundColor Yellow
dotnet test --collect:"XPlat Code Coverage" `
    --results-directory ".\TestProject\TestResults"

# 3) Grab the coverage file path automatically
$coverageFile = Get-ChildItem -Recurse -Filter "coverage.cobertura.xml" | Select-Object -First 1

if (-not $coverageFile) {
    Write-Error "Coverage file not found. Something failed."
    exit 1
}

# 4) Run ReportGenerator to build HTML
Write-Host "Generating HTML coverage report..." -ForegroundColor Yellow
reportgenerator `
    -reports:$coverageFile.FullName `
    -targetdir:"coverage-report" `
    -reporttypes:Html

# 5) Open the report in your browser
$reportPath = ".\coverage-report\index.html"
if (Test-Path $reportPath) {
    Write-Host "Opening coverage report..." -ForegroundColor Green
    Start-Process $reportPath
} else {
    Write-Error "index.html not found. Something failed."
}
