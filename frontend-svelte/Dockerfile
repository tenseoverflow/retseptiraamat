# Build stage
FROM node:20-alpine as build

WORKDIR /app

# Copy package files and install dependencies
COPY package*.json ./
RUN npm install

# Copy source files and build the application
COPY . .
RUN npm run build

# Production stage
FROM nginx:alpine

# Copy nginx configuration (if you have any custom nginx config)
# COPY ./nginx.conf /etc/nginx/conf.d/default.conf

# Copy build files from the build stage
# For SvelteKit, the build output is typically in build directory
COPY --from=build /app/build /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]