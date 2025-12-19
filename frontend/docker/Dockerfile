# Stage 1: Dependencies
FROM node:20-alpine AS deps
RUN apk add --no-cache libc6-compat
WORKDIR /app

# Copy package files
COPY package.json pnpm-lock.yaml* ./

# Install pnpm and dependencies
RUN corepack enable pnpm && pnpm install --frozen-lockfile

# Stage 2: Builder
FROM node:20-alpine AS builder
WORKDIR /app

# Build with placeholder values that will be replaced at runtime
ENV NEXT_PUBLIC_API_URL=__NEXT_PUBLIC_API_URL__
ENV NEXT_PUBLIC_API_TIMEOUT=30000
ENV NEXT_PUBLIC_APP_NAME=Reamp
ENV NEXT_PUBLIC_APP_URL=__NEXT_PUBLIC_APP_URL__
ENV NEXT_PUBLIC_ENABLE_DEBUG=false
ENV NEXT_PUBLIC_GOOGLE_MAPS_API_KEY=__NEXT_PUBLIC_GOOGLE_MAPS_API_KEY__

# Copy dependencies from deps stage
COPY --from=deps /app/node_modules ./node_modules
COPY . .

# Build the application
RUN corepack enable pnpm && pnpm run build

# Stage 3: Runner
FROM node:20-alpine AS runner
WORKDIR /app

ENV NODE_ENV=production
ENV NEXT_TELEMETRY_DISABLED=1

RUN addgroup --system --gid 1001 nodejs
RUN adduser --system --uid 1001 nextjs

# Copy startup script first (before switching user)
COPY docker-entrypoint.sh /docker-entrypoint.sh
# Convert line endings from CRLF to LF
RUN sed -i 's/\r$//' /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh

# Copy necessary files from builder
COPY --from=builder /app/public ./public
COPY --from=builder /app/.next/standalone ./
COPY --from=builder /app/.next/static ./.next/static

# Set correct permissions
RUN chown -R nextjs:nodejs /app

USER nextjs

EXPOSE 3000

ENV PORT=3000
ENV HOSTNAME="0.0.0.0"

ENTRYPOINT ["/docker-entrypoint.sh"]
CMD ["node", "server.js"]
