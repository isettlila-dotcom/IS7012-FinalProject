
# Auth

## Request these details
CLERK_CLIENT_ID=
CLERK_CLIENT_SECRET=
CLERK_AUTHORITY=

## Set up secrets
dotnet user-secrets init

dotnet user-secrets set "Clerk:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "Clerk:ClientSecret" "YOUR_CLIENT_SECRET"
dotnet user-secrets set "Clerk:Authority" "https://YOUR-CLERK-DOMAIN"

dotnet user-secrets list

## Download OIDC
dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect --version 10.0.0

## Run
dotnet build
dotnet run --launch-profile https

# Data

## Load data and migrations
dotnet ef database update