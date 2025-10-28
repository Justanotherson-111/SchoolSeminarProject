# -------------------------------
# Build React app
# -------------------------------
FROM node:20-alpine AS builder

# Set working directory
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies (clean & reproducible)
RUN npm ci

# Copy all source files
COPY . .

# Build production-ready static files
RUN npm run build


# -------------------------------
# Serve via Nginx
# -------------------------------
FROM nginx:1.25-alpine

# Remove default config
RUN rm /etc/nginx/conf.d/default.conf

# Copy custom Nginx config
COPY default.conf /etc/nginx/conf.d/default.conf

# Copy build output from previous stage
COPY --from=builder /app/dist /usr/share/nginx/html

# Expose port 80
EXPOSE 80

# Run Nginx in foreground
CMD ["nginx", "-g", "daemon off;"]
