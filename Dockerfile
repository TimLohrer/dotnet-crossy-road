# --- build stage ---
FROM oven/bun:1 AS build
WORKDIR /app

COPY package.json bun.lockb* bun.lock* ./
RUN bun install --frozen-lockfile || bun install

COPY . .
RUN bun run build

# --- runtime stage ---
FROM oven/bun:1-slim AS runtime
WORKDIR /app
ENV NODE_ENV=production
ENV PORT=3000
ENV HOST=0.0.0.0

# Only ship what the bun adapter needs at runtime.
COPY --from=build /app/build ./build
COPY --from=build /app/package.json ./package.json

EXPOSE 3000
CMD ["bun", "run", "build/index.js"]
