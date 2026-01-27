# PowerShell script to start all services
# Run: .\start_all.ps1

Write-Host "Loading environment variables from .env..." -ForegroundColor Cyan

# Load .env file
if (Test-Path ".env") {
    Get-Content ".env" | ForEach-Object {
        if ($_ -match "^([^#=]+)=(.*)$") {
            $name = $matches[1].Trim()
            $value = $matches[2].Trim().Trim('"')
            [Environment]::SetEnvironmentVariable($name, $value, "Process")
        }
    }
}

# Load Next.js local environment variables
if (Test-Path "Clients/Ecommerce.NextJS/.env.local") {
    Write-Host "Loading Next.js local environment variables..." -ForegroundColor Cyan
    Get-Content "Clients/Ecommerce.NextJS/.env.local" | ForEach-Object {
        if ($_ -match "^([^#=]+)=(.*)$") {
            $name = $matches[1].Trim()
            $value = $matches[2].Trim().Trim('"')
            [Environment]::SetEnvironmentVariable($name, $value, "Process")
        }
    }
}

$jobs = @()

Write-Host "Starting IdentityServer..." -ForegroundColor Green
$jobs += Start-Job -ScriptBlock { 
    Set-Location $using:PWD
    dotnet run --project Services/Identity/Ecommerce.IdentityServer/Ecommerce.IdentityServer.csproj --urls "http://localhost:5001" 2>&1 | Out-File identity.log
}

Write-Host "Starting Ocelot Gateway..." -ForegroundColor Green
$jobs += Start-Job -ScriptBlock { 
    Set-Location $using:PWD
    dotnet run --project Gateways/Ecommerce.OcelotGateway/Ecommerce.OcelotGateway.csproj --urls "http://*:5000" 2>&1 | Out-File ocelot.log
}

Write-Host "Starting Catalog Service..." -ForegroundColor Green
$jobs += Start-Job -ScriptBlock { 
    Set-Location $using:PWD
    dotnet run --project Services/Catalog/Ecommerce.Catalog/Ecommerce.Catalog.csproj --urls "http://localhost:7220" 2>&1 | Out-File catalog.log
}

Write-Host "Starting Cart Service..." -ForegroundColor Green
$jobs += Start-Job -ScriptBlock { 
    Set-Location $using:PWD
    dotnet run --project Services/Cart/Ecommerce.Cart/Ecommerce.Cart.csproj --urls "http://localhost:7224" 2>&1 | Out-File cart.log
}

Write-Host "Starting Discount Service..." -ForegroundColor Green
$jobs += Start-Job -ScriptBlock { 
    Set-Location $using:PWD
    dotnet run --project Services/Discount/Ecommerce.Discount/Ecommerce.Discount.csproj --urls "http://localhost:7221" 2>&1 | Out-File discount.log
}

Write-Host "Starting Order Service..." -ForegroundColor Green
$jobs += Start-Job -ScriptBlock { 
    Set-Location $using:PWD
    dotnet run --project Services/Order/Presentation/Ecommerce.Order.WebApi/Ecommerce.Order.WebApi.csproj --urls "http://localhost:7222" 2>&1 | Out-File order.log
}

Write-Host "Starting Message Service..." -ForegroundColor Green
$jobs += Start-Job -ScriptBlock { 
    Set-Location $using:PWD
    dotnet run --project Services/Message/Ecommerce.Message/Ecommerce.Message.csproj --urls "http://localhost:7228" 2>&1 | Out-File message.log
}

Write-Host "Starting Cargo Service..." -ForegroundColor Green
$jobs += Start-Job -ScriptBlock { 
    Set-Location $using:PWD
    dotnet run --project Services/Cargo/Ecommerce.Cargo.WebAPI/Ecommerce.Cargo.WebAPI.csproj --urls "http://localhost:7223" 2>&1 | Out-File cargo.log
}

Write-Host "Starting Payment Service..." -ForegroundColor Green
$jobs += Start-Job -ScriptBlock { 
    Set-Location $using:PWD
    dotnet run --project Services/Payment/Ecommerce.Payment/Ecommerce.Payment.csproj --urls "http://localhost:7226" 2>&1 | Out-File payment.log
}

Write-Host "Starting Review Service..." -ForegroundColor Green
$jobs += Start-Job -ScriptBlock { 
    Set-Location $using:PWD
    dotnet run --project Services/Review/Ecommerce.Review/Ecommerce.Review.csproj --urls "http://localhost:7225" 2>&1 | Out-File review.log
}

Write-Host "Starting Images Service..." -ForegroundColor Green
$jobs += Start-Job -ScriptBlock { 
    Set-Location $using:PWD
    dotnet run --project Services/Images/Ecommerce.Images/Ecommerce.Images.csproj --urls "http://localhost:7227" 2>&1 | Out-File images.log
}

Write-Host "Starting BFF Service..." -ForegroundColor Green
$jobs += Start-Job -ScriptBlock { 
    Set-Location $using:PWD
    dotnet run --project Services/BFF/Ecommerce.BFF/Ecommerce.BFF.csproj --urls "http://*:5500" 2>&1 | Out-File bff.log
}

Write-Host "Starting Next.js UI..." -ForegroundColor Green
$jobs += Start-Job -ScriptBlock { 
    Set-Location "$using:PWD/Clients/Ecommerce.NextJS"
    npm run dev 2>&1 | Out-File ../../nextjs.log
}

Write-Host "Starting WebUI..." -ForegroundColor Green
$jobs += Start-Job -ScriptBlock { 
    Set-Location $using:PWD
    dotnet run --project Clients/Ecommerce.WebUI/Ecommerce.WebUI.csproj --launch-profile https 2>&1 | Out-File webui.log
}

Write-Host ""
Write-Host "All services started! Check logs (*.log) for details." -ForegroundColor Yellow
Write-Host "Press Ctrl+C to stop all services..." -ForegroundColor Yellow
Write-Host ""

# Wait for Ctrl+C
try {
    Wait-Job $jobs
} finally {
    Write-Host "Stopping all services..." -ForegroundColor Red
    $jobs | Stop-Job
    $jobs | Remove-Job -Force
}
