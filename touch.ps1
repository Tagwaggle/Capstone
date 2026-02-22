Write-Host "=== LANCE MUD CLEANUP SCRIPT ===" -ForegroundColor Cyan

# 1. Stop dotnet processes
Write-Host "Stopping dotnet processes..."
Get-Process dotnet -ErrorAction SilentlyContinue | Stop-Process -Force

# 2. Close Visual Studio if open
Write-Host "Closing Visual Studio..."
Get-Process devenv -ErrorAction SilentlyContinue | Stop-Process -Force

# 3. Set your project path here
$projectPath = "C:\WGU\Capstone\LanceMudCapstone"

Write-Host "Cleaning project at: $projectPath"

# 4. Delete bin and obj
$pathsToDelete = @(
    "$projectPath\bin",
    "$projectPath\obj",
    "$projectPath\.vs"
)

foreach ($path in $pathsToDelete) {
    if (Test-Path $path) {
        Write-Host "Deleting $path ..."
        Remove-Item -Recurse -Force $path
    }
}

# 5. Clear NuGet caches
Write-Host "Clearing NuGet caches..."
dotnet nuget locals all --clear

# 6. Rebuild solution
Write-Host "Rebuilding solution..."
dotnet build $projectPath

Write-Host "=== CLEANUP COMPLETE ===" -ForegroundColor Green
Write-Host "You can now reopen Visual Studio and run the project."
