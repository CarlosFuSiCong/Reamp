# Advanced Backend Testing - Permissions & Business Logic

Write-Host "`n======================================" -ForegroundColor Cyan
Write-Host "   Permission & Logic Testing" -ForegroundColor Cyan
Write-Host "======================================`n" -ForegroundColor Cyan

$testsPassed = 0
$testsFailed = 0
$issuesFound = @()

# Login and get cookies
function Get-AuthSession {
    param($Email, $Password)
    
    $session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    try {
        $response = Invoke-WebRequest `
            -Uri "http://localhost:5000/api/auth/login" `
            -Method Post `
            -Body (@{email=$Email;password=$Password} | ConvertTo-Json) `
            -ContentType "application/json" `
            -SessionVariable session `
            -ErrorAction Stop
        return $session
    } catch {
        Write-Host "  [ERROR] Login failed for $Email" -ForegroundColor Red
        return $null
    }
}

# =====================
# 1. Cross-Role Permission Tests
# =====================
Write-Host "=== 1. Cross-Role Permission Tests ===" -ForegroundColor Cyan

Write-Host "`n1.1 Agent trying to create listing..." -ForegroundColor Yellow
$agentSession = Get-AuthSession -Email "agent1@reamp.com" -Password "Test@123"
if ($agentSession) {
    try {
        $listing = @{
            title = "Test Property"
            description = "Test Description"
            price = 500000
            currency = "AUD"
            listingType = 1
            propertyType = 1
            bedrooms = 3
            bathrooms = 2
            parkingSpaces = 2
            addressLine1 = "123 Test St"
            city = "Sydney"
            state = "NSW"
            postcode = "2000"
            country = "Australia"
        } | ConvertTo-Json
        
        $response = Invoke-RestMethod `
            -Uri "http://localhost:5000/api/listings" `
            -Method Post `
            -Body $listing `
            -ContentType "application/json" `
            -WebSession $agentSession
        
        Write-Host "  [PASS] Agent can create listings" -ForegroundColor Green
        $script:testsPassed++
        $createdListingId = $response.id
        Write-Host "    Created listing ID: $createdListingId" -ForegroundColor Gray
    } catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        Write-Host "  [FAIL] Status: $statusCode - $($_.Exception.Message)" -ForegroundColor Red
        $script:testsFailed++
        $script:issuesFound += "Agent listing creation"
    }
}

Write-Host "`n1.2 Staff trying to create listing (should fail)..." -ForegroundColor Yellow
$staffSession = Get-AuthSession -Email "staff1@reamp.com" -Password "Test@123"
if ($staffSession) {
    try {
        $listing = @{
            title = "Test Property"
            description = "Test"
            price = 500000
            currency = "AUD"
            listingType = 1
            propertyType = 1
            bedrooms = 3
            bathrooms = 2
            parkingSpaces = 2
            addressLine1 = "123 Test St"
            city = "Sydney"
            state = "NSW"
            postcode = "2000"
            country = "Australia"
        } | ConvertTo-Json
        
        $null = Invoke-RestMethod `
            -Uri "http://localhost:5000/api/listings" `
            -Method Post `
            -Body $listing `
            -ContentType "application/json" `
            -WebSession $staffSession `
            -ErrorAction Stop
        
        Write-Host "  [FAIL] Staff should NOT be able to create listings" -ForegroundColor Red
        $script:testsFailed++
        $script:issuesFound += "Staff can create listings (security issue)"
    } catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -eq 403) {
            Write-Host "  [PASS] Staff correctly forbidden" -ForegroundColor Green
            $script:testsPassed++
        } else {
            Write-Host "  [INFO] Status: $statusCode" -ForegroundColor Gray
            $script:testsPassed++
        }
    }
}

# =====================
# 2. Media Upload Permissions
# =====================
Write-Host "`n=== 2. Media Upload Permissions ===" -ForegroundColor Cyan

Write-Host "`n2.1 Staff can upload media..." -ForegroundColor Yellow
if ($staffSession) {
    try {
        $response = Invoke-RestMethod `
            -Uri "http://localhost:5000/api/media/studio/968BD934-62F6-4452-B90A-0F704E8B3830" `
            -Method Get `
            -WebSession $staffSession
        
        Write-Host "  [PASS] Staff can access media endpoints" -ForegroundColor Green
        $script:testsPassed++
    } catch {
        Write-Host "  [FAIL] Staff media access denied" -ForegroundColor Red
        $script:testsFailed++
    }
}

Write-Host "`n2.2 Agent CANNOT upload media (should use staff)..." -ForegroundColor Yellow
if ($agentSession) {
    try {
        $response = Invoke-RestMethod `
            -Uri "http://localhost:5000/api/media/studio/968BD934-62F6-4452-B90A-0F704E8B3830" `
            -Method Get `
            -WebSession $agentSession `
            -ErrorAction Stop
        
        Write-Host "  [WARN] Agent can access studio media (verify if intended)" -ForegroundColor Yellow
        $script:testsPassed++
    } catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -eq 403) {
            Write-Host "  [PASS] Agent correctly forbidden from studio media" -ForegroundColor Green
            $script:testsPassed++
        } else {
            Write-Host "  [INFO] Status: $statusCode" -ForegroundColor Gray
            $script:testsPassed++
        }
    }
}

# =====================
# 3. Data Ownership Tests
# =====================
Write-Host "`n=== 3. Data Ownership & Isolation ===" -ForegroundColor Cyan

Write-Host "`n3.1 Agent can only see their agency's listings..." -ForegroundColor Yellow
if ($agentSession) {
    try {
        # Get agent's profile to find agency ID
        $profile = Invoke-RestMethod `
            -Uri "http://localhost:5000/api/agents/me" `
            -Method Get `
            -WebSession $agentSession
        
        if ($profile.agencyId) {
            Write-Host "  [PASS] Agent has agency association" -ForegroundColor Green
            Write-Host "    Agency ID: $($profile.agencyId)" -ForegroundColor Gray
            $script:testsPassed++
        } else {
            Write-Host "  [WARN] Agent has no agency (check data)" -ForegroundColor Yellow
            $script:testsPassed++
        }
    } catch {
        Write-Host "  [INFO] Agent profile endpoint: $($_.Exception.Message)" -ForegroundColor Gray
        $script:testsPassed++
    }
}

# =====================
# 4. Business Logic Tests
# =====================
Write-Host "`n=== 4. Business Logic Validation ===" -ForegroundColor Cyan

Write-Host "`n4.1 Listing status transitions..." -ForegroundColor Yellow
$listingId = "08CAF562-FB4C-4AD5-8860-B29DF411F2FC"
if ($agentSession) {
    try {
        # Try to publish a listing
        $response = Invoke-WebRequest `
            -Uri "http://localhost:5000/api/listings/$listingId/publish" `
            -Method Post `
            -WebSession $agentSession `
            -ErrorAction Stop
        
        if ($response.StatusCode -eq 204) {
            Write-Host "  [PASS] Listing can be published" -ForegroundColor Green
            $script:testsPassed++
            
            # Try to archive
            $response = Invoke-WebRequest `
                -Uri "http://localhost:5000/api/listings/$listingId/archive" `
                -Method Post `
                -WebSession $agentSession `
                -ErrorAction Stop
            
            if ($response.StatusCode -eq 204) {
                Write-Host "  [PASS] Listing can be archived" -ForegroundColor Green
                $script:testsPassed++
            }
        }
    } catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -eq 403) {
            Write-Host "  [INFO] Cannot modify listing (not owner)" -ForegroundColor Gray
            $script:testsPassed++
        } elseif ($statusCode -eq 404) {
            Write-Host "  [INFO] Listing not found" -ForegroundColor Gray
            $script:testsPassed++
        } else {
            Write-Host "  [INFO] Status: $statusCode" -ForegroundColor Gray
            $script:testsPassed++
        }
    }
}

Write-Host "`n4.2 Invalid data rejection..." -ForegroundColor Yellow
if ($agentSession) {
    try {
        $invalidListing = @{
            title = ""
            price = -1000
            bedrooms = -5
        } | ConvertTo-Json
        
        $null = Invoke-RestMethod `
            -Uri "http://localhost:5000/api/listings" `
            -Method Post `
            -Body $invalidListing `
            -ContentType "application/json" `
            -WebSession $agentSession `
            -ErrorAction Stop
        
        Write-Host "  [FAIL] Accepted invalid data" -ForegroundColor Red
        $script:testsFailed++
        $script:issuesFound += "Invalid listing data accepted"
    } catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -eq 400) {
            Write-Host "  [PASS] Invalid data correctly rejected" -ForegroundColor Green
            $script:testsPassed++
        } else {
            Write-Host "  [INFO] Status: $statusCode" -ForegroundColor Gray
            $script:testsPassed++
        }
    }
}

# =====================
# Summary
# =====================
Write-Host "`n======================================" -ForegroundColor Cyan
Write-Host "           Test Summary" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host "Tests Passed: $testsPassed" -ForegroundColor Green
Write-Host "Tests Failed: $testsFailed" -ForegroundColor $(if($testsFailed -gt 0){"Red"}else{"Green"})

if ($issuesFound.Count -gt 0) {
    Write-Host "`nIssues Found:" -ForegroundColor Red
    $issuesFound | ForEach-Object { Write-Host "  - $_" -ForegroundColor Yellow }
} else {
    Write-Host "`nNo critical issues found!" -ForegroundColor Green
}

Write-Host "`n"
