# Quick test to verify the application is working
Write-Host "Starting application..." -ForegroundColor Cyan
$appProcess = Start-Process -FilePath "dotnet" -ArgumentList "run", "--no-build" -PassThru -NoNewWindow

Write-Host "Waiting for application to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 8

try {
    Write-Host "`nTesting HTTP endpoint (http://localhost:5152)..." -ForegroundColor Cyan
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5152" -Method GET -TimeoutSec 5
        Write-Host "HTTP Status: $($response.StatusCode)" -ForegroundColor Green
        Write-Host "Content Length: $($response.Content.Length) bytes" -ForegroundColor Green
        Write-Host "Success! Application is working on HTTP." -ForegroundColor Green
    }
    catch {
        Write-Host "HTTP test failed: $($_.Exception.Message)" -ForegroundColor Red
    }

    Write-Host "`nTesting HTTPS endpoint (https://localhost:7192)..." -ForegroundColor Cyan
    try {
        $response = Invoke-WebRequest -Uri "https://localhost:7192" -Method GET -TimeoutSec 5 -SkipCertificateCheck
        Write-Host "HTTPS Status: $($response.StatusCode)" -ForegroundColor Green
        Write-Host "Content Length: $($response.Content.Length) bytes" -ForegroundColor Green
        Write-Host "Success! Application is working on HTTPS." -ForegroundColor Green
    }
    catch {
        Write-Host "HTTPS test failed: $($_.Exception.Message)" -ForegroundColor Red
    }

    Write-Host "`nApplication is running. Open your browser to:" -ForegroundColor Yellow
    Write-Host "  HTTP:  http://localhost:5152" -ForegroundColor Cyan
    Write-Host "  HTTPS: https://localhost:7192" -ForegroundColor Cyan
    Write-Host "`nPress any key to stop..." -ForegroundColor Yellow
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}
finally {
    Write-Host "`nStopping application..." -ForegroundColor Cyan
    Stop-Process -Id $appProcess.Id -Force
    Write-Host "Done." -ForegroundColor Green
}
