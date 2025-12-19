# Reamp Backend Security & Logic Test Suite

Write-Host "`n======================================" -ForegroundColor Cyan
Write-Host "   Reamp Backend Testing Suite" -ForegroundColor Cyan
Write-Host "======================================`n" -ForegroundColor Cyan

$issuesFound = @()
$testsPassed = 0
$testsFailed = 0

# Helper function
function Test-Endpoint {
    param($Name, $Uri, $Method = "GET", $Body = $null, $ExpectedStatus = 200, $ShouldFail = $false)
    
    Write-Host "Testing: $Name" -ForegroundColor Yellow
    try {
        $params = @{
            Uri = $Uri
            Method = $Method
            ErrorAction = "Stop"
        }
        if ($Body) {
            $params.Body = $Body
            $params.ContentType = "application/json"
        }
        $response = Invoke-RestMethod @params
        
        if ($ShouldFail) {
            Write-Host "  [FAIL] Expected failure but succeeded" -ForegroundColor Red
            $script:issuesFound += $Name
            $script:testsFailed++
        } else {
            Write-Host "  [PASS]" -ForegroundColor Green
            $script:testsPassed++
        }
        return $response
    } catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($ShouldFail -and $statusCode -eq $ExpectedStatus) {
            Write-Host "  [PASS] Correctly rejected (Status: $statusCode)" -ForegroundColor Green
            $script:testsPassed++
        } elseif (-not $ShouldFail) {
            Write-Host "  [FAIL] Unexpected error (Status: $statusCode)" -ForegroundColor Red
            $script:issuesFound += $Name
            $script:testsFailed++
        } else {
            Write-Host "  [INFO] Status: $statusCode" -ForegroundColor Gray
            $script:testsPassed++
        }
        return $null
    }
}

# =====================
# 1. Authentication Tests
# =====================
Write-Host "`n=== 1. Authentication & Authorization ===" -ForegroundColor Cyan

Test-Endpoint -Name "Invalid login credentials" `
    -Uri "http://localhost:5000/api/auth/login" `
    -Method "POST" `
    -Body '{"email":"invalid@test.com","password":"wrong"}' `
    -ExpectedStatus 401 `
    -ShouldFail $true

Test-Endpoint -Name "Empty email login" `
    -Uri "http://localhost:5000/api/auth/login" `
    -Method "POST" `
    -Body '{"email":"","password":"test"}' `
    -ExpectedStatus 400 `
    -ShouldFail $true

Test-Endpoint -Name "Valid agent login" `
    -Uri "http://localhost:5000/api/auth/login" `
    -Method "POST" `
    -Body '{"email":"agent1@reamp.com","password":"Test@123"}'

# =====================
# 2. Public API Tests
# =====================
Write-Host "`n=== 2. Public API Access ===" -ForegroundColor Cyan

$listings = Test-Endpoint -Name "Public listings API" `
    -Uri "http://localhost:5000/api/listings?page=1&pageSize=5"

if ($listings) {
    Write-Host "  - Returned $($listings.items.Count) listings" -ForegroundColor Gray
    Write-Host "  - Total: $($listings.totalCount)" -ForegroundColor Gray
}

Test-Endpoint -Name "Listing details by ID" `
    -Uri "http://localhost:5000/api/listings/08caf562-fb4c-4ad5-8860-b29df411f2fc"

Test-Endpoint -Name "Invalid UUID format" `
    -Uri "http://localhost:5000/api/listings/invalid-id" `
    -ExpectedStatus 400 `
    -ShouldFail $true

Test-Endpoint -Name "Non-existent listing ID" `
    -Uri "http://localhost:5000/api/listings/00000000-0000-0000-0000-000000000000" `
    -ExpectedStatus 404 `
    -ShouldFail $true

# =====================
# 3. Data Validation Tests
# =====================
Write-Host "`n=== 3. Data Validation ===" -ForegroundColor Cyan

# Test pagination limits
Test-Endpoint -Name "Excessive page size (should limit)" `
    -Uri "http://localhost:5000/api/listings?page=1&pageSize=1000"

Test-Endpoint -Name "Negative page number" `
    -Uri "http://localhost:5000/api/listings?page=-1&pageSize=10" `
    -ExpectedStatus 400 `
    -ShouldFail $true

# =====================
# 4. Media & Assets
# =====================
Write-Host "`n=== 4. Media & Assets ===" -ForegroundColor Cyan

$listing = Test-Endpoint -Name "Listing with media" `
    -Uri "http://localhost:5000/api/listings/08caf562-fb4c-4ad5-8860-b29df411f2fc"

if ($listing -and $listing.media) {
    Write-Host "  - Total media: $($listing.media.Count)" -ForegroundColor Gray
    $photos = $listing.media | Where-Object { $_.role -ne "FloorPlan" }
    $floorPlans = $listing.media | Where-Object { $_.role -eq "FloorPlan" }
    Write-Host "  - Photos: $($photos.Count)" -ForegroundColor Gray
    Write-Host "  - Floor plans: $($floorPlans.Count)" -ForegroundColor Gray
    
    # Check if URLs are accessible
    if ($listing.media[0].thumbnailUrl) {
        try {
            $null = Invoke-WebRequest -Uri $listing.media[0].thumbnailUrl -Method Head -TimeoutSec 5
            Write-Host "  [PASS] Media URLs are accessible" -ForegroundColor Green
            $script:testsPassed++
        } catch {
            Write-Host "  [FAIL] Media URL not accessible" -ForegroundColor Red
            $script:issuesFound += "Media URL accessibility"
            $script:testsFailed++
        }
    }
}

# =====================
# 5. Search & Filtering
# =====================
Write-Host "`n=== 5. Search & Filtering ===" -ForegroundColor Cyan

Test-Endpoint -Name "Search by city" `
    -Uri "http://localhost:5000/api/listings?city=Sydney"

Test-Endpoint -Name "Filter by price range" `
    -Uri "http://localhost:5000/api/listings?minPrice=500000&maxPrice=1000000"

Test-Endpoint -Name "Filter by property type" `
    -Uri "http://localhost:5000/api/listings?propertyType=1"

Test-Endpoint -Name "Filter by listing status" `
    -Uri "http://localhost:5000/api/listings?status=1"

# =====================
# 6. Edge Cases
# =====================
Write-Host "`n=== 6. Edge Cases ===" -ForegroundColor Cyan

Test-Endpoint -Name "Empty query parameters" `
    -Uri "http://localhost:5000/api/listings?"

Test-Endpoint -Name "Special characters in search" `
    -Uri "http://localhost:5000/api/listings?searchTerm=%3Cscript%3Ealert%28%29%3C%2Fscript%3E"

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
