# Build stage
FROM node:20-alpine AS build
WORKDIR /frontend
# Install deps
COPY package*.json ./
RUN npm install
# Copy source and build
COPY . ./
RUN npm run build
# Nginx stage to serve static files
FROM nginx:alpine AS runtime
COPY --from=build /frontend/dist /usr/share/nginx/html
# Copy custom nginx config for frontend (optional)
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
