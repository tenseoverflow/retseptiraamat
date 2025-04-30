# Build stage
FROM node:23-slim as build

WORKDIR /app

# Copy package files and install dependencies
COPY package*.json ./
RUN npm ci

ARG VITE_API_URL
ENV VITE_API_URL=${VITE_API_URL}

# Copy source files and build the application
COPY . .
RUN npm run build
RUN npm prune --omit=dev

# Production stage
FROM node:23-slim

WORKDIR /app

# Copy build files from the build stage
COPY --from=build /app/build build/
#COPY --from=build /app/node_modules node_modules/
COPY package.json .

EXPOSE 3000

ENV NODE_ENV=production
ENV ORIGIN=http://localhost:8099

CMD ["node", "build"]