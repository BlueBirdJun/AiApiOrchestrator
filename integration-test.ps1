# AI API Orchestrator Integration Test Script
# This script performs comprehensive integration testing

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "AI API Orchestrator Integration Test" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Test configuration
$baseUrl = "http://localhost:5152"
$testResults = @()

# Function to test API endpoint
function Test-ApiEndpoint {
    param(
        [string]$Name,
        [string]$Method,
        [string]$Url,
        [object]$Body = $null
    )
    
    Write-Host "Testing: $Name" -ForegroundColor Yellow
    
    try {
        $params = @{
            Uri = $Url
            Method = $Method
            ContentType = "application/json"
            TimeoutSec = 10
        }
        
        if ($Body) {
            $params.Body = ($Body | ConvertTo-Json)
        }
        
        $response = Invoke-RestMethod @params
        Write-Host "Success" -ForegroundColor Green
        Write-Host "Response: $($response | ConvertTo-Json -Depth 3)" -ForegroundColor Gray
        Write-Host ""
        
        return @{
            Name = $Name
            Status = "Success"
            Response = $response
        }
    }
    catch {
        Write-Host "Failed: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
        
        return @{
            Name = $Name
            Status = "Failed"
            Error = $_.Exception.Message
        }
    }
}

# Start the application in background
Write-Host "Starting application..." -ForegroundColor Cyan
$appProcess = Start-Process -FilePath "dotnet" -ArgumentList "run", "--project", ".", "--no-build" -PassThru -NoNewWindow

# Wait for application to start
Write-Host "Waiting for application to start (10 seconds)..." -ForegroundColor Cyan
Start-Sleep -Seconds 10

try {
    # Test 1: Sample API - Weather
    Write-Host "Test 1: Sample API - Weather" -ForegroundColor Cyan
    $testResults += Test-ApiEndpoint -Name "Weather API" -Method "GET" -Url "$baseUrl/api/sample/weather?location=Seoul"
    
    # Test 2: Sample API - Stock
    Write-Host "Test 2: Sample API - Stock" -ForegroundColor Cyan
    $testResults += Test-ApiEndpoint -Name "Stock API" -Method "GET" -Url "$baseUrl/api/sample/stock?symbol=AAPL"
    
    # Test 3: Sample API - News
    Write-Host "Test 3: Sample API - News" -ForegroundColor Cyan
    $testResults += Test-ApiEndpoint -Name "News API" -Method "GET" -Url "$baseUrl/api/sample/news?topic=Technology"
    
    # Test 4: Agent Mode - Weather keyword
    Write-Host "Test 4: Agent Mode - Weather Prompt" -ForegroundColor Cyan
    $testResults += Test-ApiEndpoint -Name "Agent Mode (Weather)" -Method "POST" -Url "$baseUrl/api/ai/process" -Body @{
        prompt = "What is the weather in Seoul today?"
        mode = 1
    }
    
    # Test 5: Agent Mode - Stock keyword
    Write-Host "Test 5: Agent Mode - Stock Prompt" -ForegroundColor Cyan
    $testResults += Test-ApiEndpoint -Name "Agent Mode (Stock)" -Method "POST" -Url "$baseUrl/api/ai/process" -Body @{
        prompt = "Tell me about Apple stock information"
        mode = 1
    }
    
    # Test 6: Agent Mode - News keyword
    Write-Host "Test 6: Agent Mode - News Prompt" -ForegroundColor Cyan
    $testResults += Test-ApiEndpoint -Name "Agent Mode (News)" -Method "POST" -Url "$baseUrl/api/ai/process" -Body @{
        prompt = "Show me the latest news"
        mode = 1
    }
    
    # Test 7: Agent Mode - No keyword match
    Write-Host "Test 7: Agent Mode - No Keyword Match" -ForegroundColor Cyan
    $testResults += Test-ApiEndpoint -Name "Agent Mode (No Match)" -Method "POST" -Url "$baseUrl/api/ai/process" -Body @{
        prompt = "Hello, how are you?"
        mode = 1
    }
    
    # Test 8: Ollama Mode - Check if Ollama server is running
    Write-Host "Test 8: Ollama Mode Test" -ForegroundColor Cyan
    Write-Host "Checking if Ollama server is running on http://localhost:11434..." -ForegroundColor Yellow
    
    try {
        $ollamaCheck = Invoke-RestMethod -Uri "http://localhost:11434/api/tags" -Method GET -TimeoutSec 3
        Write-Host "Ollama server is running" -ForegroundColor Green
        
        # Test with Ollama mode
        Write-Host "Testing Ollama mode with weather prompt..." -ForegroundColor Yellow
        $testResults += Test-ApiEndpoint -Name "Ollama Mode (Weather)" -Method "POST" -Url "$baseUrl/api/ai/process" -Body @{
            prompt = "What is the weather in Seoul?"
            mode = 0
        }
    }
    catch {
        Write-Host "Ollama server is not running" -ForegroundColor Red
        Write-Host "Testing error handling when Ollama is not available..." -ForegroundColor Yellow
        
        $testResults += Test-ApiEndpoint -Name "Ollama Mode (Server Not Running)" -Method "POST" -Url "$baseUrl/api/ai/process" -Body @{
            prompt = "What is the weather in Seoul?"
            mode = 0
        }
    }
    
    # Test Summary
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Test Summary" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    
    $successCount = ($testResults | Where-Object { $_.Status -eq "Success" }).Count
    $failCount = ($testResults | Where-Object { $_.Status -eq "Failed" }).Count
    
    Write-Host "Total Tests: $($testResults.Count)" -ForegroundColor White
    Write-Host "Passed: $successCount" -ForegroundColor Green
    Write-Host "Failed: $failCount" -ForegroundColor Red
    Write-Host ""
    
    foreach ($result in $testResults) {
        if ($result.Status -eq "Success") {
            Write-Host "[PASS] $($result.Name)" -ForegroundColor Green
        } else {
            Write-Host "[FAIL] $($result.Name)" -ForegroundColor Red
        }
    }
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Manual Testing Instructions" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "The application is still running at: $baseUrl" -ForegroundColor Yellow
    Write-Host "You can now open a browser and test the UI manually." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Test scenarios to try in the browser:" -ForegroundColor White
    Write-Host "1. Navigate to $baseUrl" -ForegroundColor Gray
    Write-Host "2. Select 'Agent' mode and try these prompts:" -ForegroundColor Gray
    Write-Host "   - 'What is the weather in Seoul?'" -ForegroundColor Gray
    Write-Host "   - 'Tell me about Apple stock'" -ForegroundColor Gray
    Write-Host "   - 'Show me the latest technology news'" -ForegroundColor Gray
    Write-Host "3. Select 'Ollama' mode and try a prompt (requires Ollama server)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Press any key to stop the application..." -ForegroundColor Yellow
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}
finally {
    # Stop the application
    Write-Host ""
    Write-Host "Stopping application..." -ForegroundColor Cyan
    Stop-Process -Id $appProcess.Id -Force
    Write-Host "Application stopped." -ForegroundColor Green
}
