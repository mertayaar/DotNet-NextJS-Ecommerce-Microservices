#!/bin/bash

# Function to kill all background processes on exit
cleanup() {
    echo "Stopping all services..."
    pkill -P $$
    exit 0
}

# Trap SIGINT (Ctrl+C) to run cleanup
trap cleanup SIGINT

echo "Loading environment variables from .env..."
set -a
[ -f .env ] && . .env
set +a

echo "Loading Next.js local environment variables..."
if [ -f Clients/Ecommerce.NextJS/.env.local ]; then
    set -a
    . Clients/Ecommerce.NextJS/.env.local
    set +a
fi
echo "Starting IdentityServer..."
dotnet run --project Services/Identity/Ecommerce.IdentityServer/Ecommerce.IdentityServer.csproj --urls "http://localhost:5001" > identity.log 2>&1 &

echo "Starting Ocelot Gateway..."
dotnet run --project Gateways/Ecommerce.OcelotGateway/Ecommerce.OcelotGateway.csproj --urls "http://*:5000" > ocelot.log 2>&1 &

echo "Starting Catalog Service..."
dotnet run --project Services/Catalog/Ecommerce.Catalog/Ecommerce.Catalog.csproj --urls "http://localhost:7220" > catalog.log 2>&1 &

echo "Starting Cart Service..."
dotnet run --project Services/Cart/Ecommerce.Cart/Ecommerce.Cart.csproj --urls "http://localhost:7224" > cart.log 2>&1 &

echo "Starting Discount Service..."
dotnet run --project Services/Discount/Ecommerce.Discount/Ecommerce.Discount.csproj --urls "http://localhost:7221" > discount.log 2>&1 &

echo "Starting Order Service..."
dotnet run --project Services/Order/Presentation/Ecommerce.Order.WebApi/Ecommerce.Order.WebApi.csproj --urls "http://localhost:7222" > order.log 2>&1 &

echo "Starting Message Service..."
dotnet run --project Services/Message/Ecommerce.Message/Ecommerce.Message.csproj --urls "http://localhost:7228" > message.log 2>&1 &

echo "Starting Cargo Service..."
dotnet run --project Services/Cargo/Ecommerce.Cargo.WebAPI/Ecommerce.Cargo.WebAPI.csproj --urls "http://localhost:7223" > cargo.log 2>&1 &

echo "Starting Payment Service..."
dotnet run --project Services/Payment/Ecommerce.Payment/Ecommerce.Payment.csproj --urls "http://localhost:7226" > payment.log 2>&1 &

echo "Starting Review Service..."
dotnet run --project Services/Review/Ecommerce.Review/Ecommerce.Review.csproj --urls "http://localhost:7225" > review.log 2>&1 &

echo "Starting Images Service..."
dotnet run --project Services/Images/Ecommerce.Images/Ecommerce.Images.csproj --urls "http://localhost:7227" > images.log 2>&1 &


echo "Starting BFF Service..."
dotnet run --project Services/BFF/Ecommerce.BFF/Ecommerce.BFF.csproj --urls "http://*:5500" > bff.log 2>&1 &

echo "Starting Next.js UI..."
cd Clients/Ecommerce.NextJS
npm run dev > ../../nextjs.log 2>&1 &
cd ../..

echo "Starting WebUI..."
dotnet run --project Clients/Ecommerce.WebUI/Ecommerce.WebUI.csproj --launch-profile https > webui.log 2>&1 &

echo "All services started! Check logs (*.log) for details."
wait
