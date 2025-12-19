#!/bin/sh
set -e

echo "Starting Next.js application with runtime environment variables..."

# Function to replace placeholder with actual value
replace_placeholder() {
  local placeholder=$1
  local env_var=$2
  local value=$(eval echo \$$env_var)
  
  if [ -n "$value" ] && [ "$value" != "" ]; then
    echo "Replacing $placeholder with $value"
    
    # Replace in standalone server files
    find /app/.next/static -type f -name "*.js" -exec sed -i "s|$placeholder|$value|g" {} +
    
    # Also check standalone server.js and other JS files in root
    find /app -maxdepth 2 -type f -name "*.js" -exec sed -i "s|$placeholder|$value|g" {} +
  else
    echo "Warning: $env_var is not set, keeping placeholder $placeholder"
  fi
}

# Replace placeholders with actual environment variables
replace_placeholder "__NEXT_PUBLIC_API_URL__" "NEXT_PUBLIC_API_URL"
replace_placeholder "__NEXT_PUBLIC_APP_URL__" "NEXT_PUBLIC_APP_URL"
replace_placeholder "__NEXT_PUBLIC_GOOGLE_MAPS_API_KEY__" "NEXT_PUBLIC_GOOGLE_MAPS_API_KEY"

echo "Environment variable replacement complete!"
echo "Starting server..."

# Execute the main command
exec "$@"

