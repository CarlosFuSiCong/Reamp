# Security Configuration Guide

## ⚠️ Important: Cloudinary Credentials

**DO NOT commit your Cloudinary credentials to version control!**

The Cloudinary API key and secret have been removed from `appsettings.json` and `appsettings.Development.json` for security reasons.

### Setup Instructions

#### Option 1: User Secrets (Recommended for Development)

```bash
cd src/Reamp/Reamp.Api
dotnet user-secrets init
dotnet user-secrets set "CloudinarySettings:CloudName" "your_cloud_name"
dotnet user-secrets set "CloudinarySettings:ApiKey" "your_api_key"
dotnet user-secrets set "CloudinarySettings:ApiSecret" "your_api_secret"
```

#### Option 2: Environment Variables

Set the following environment variables:

**Windows (PowerShell):**
```powershell
$env:CloudinarySettings__CloudName="your_cloud_name"
$env:CloudinarySettings__ApiKey="your_api_key"
$env:CloudinarySettings__ApiSecret="your_api_secret"
```

**Linux/macOS:**
```bash
export CloudinarySettings__CloudName="your_cloud_name"
export CloudinarySettings__ApiKey="your_api_key"
export CloudinarySettings__ApiSecret="your_api_secret"
```

#### Option 3: Azure Key Vault / AWS Secrets Manager (Production)

For production deployments, use a proper secrets management service:
- Azure Key Vault
- AWS Secrets Manager
- HashiCorp Vault

### Get Your Cloudinary Credentials

1. Log in to your Cloudinary account: https://cloudinary.com/console
2. Copy your **Cloud Name**, **API Key**, and **API Secret**
3. Configure them using one of the methods above

### ⚠️ Security Notice

**If you accidentally committed credentials:**

1. **Immediately rotate your Cloudinary API keys** at https://cloudinary.com/console/settings/security
2. Remove the credentials from Git history (or make the repo private)
3. Never commit the new credentials

### Configuration Priority

ASP.NET Core loads configuration in this order (later sources override earlier ones):
1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. User Secrets (Development only)
4. Environment Variables
5. Command-line arguments

### Other Secrets

The same approach should be used for:
- JWT Secret Key
- Database Connection Strings (with passwords)
- Any other sensitive configuration

## Example Configuration Files

### appsettings.json (Production)
```json
{
  "CloudinarySettings": {
    "CloudName": "",
    "ApiKey": "",
    "ApiSecret": "",
    "UseHttps": true,
    "SecureDistribution": true,
    "Folder": "reamp"
  }
}
```

### appsettings.Development.json (Development)
```json
{
  "CloudinarySettings": {
    "CloudName": "",
    "ApiKey": "",
    "ApiSecret": "",
    "UseHttps": true,
    "SecureDistribution": false,
    "Folder": "reamp-dev"
  }
}
```

**Leave credentials empty in these files and provide them via User Secrets or Environment Variables!**






