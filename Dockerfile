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

# svelte-adapter-bun keeps several packages external (cookie, @sveltejs/kit, ...).
# Install the full dependency tree in the runtime image so they resolve.
COPY package.json bun.lockb* bun.lock* ./
RUN bun install --frozen-lockfile || bun install

RUN bun add cookie devalue set-cookie-parser @sveltejs/kit

COPY --from=build /app/build ./build

EXPOSE 3000
CMD ["bun", "run", "build/index.js"]
