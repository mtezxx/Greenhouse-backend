FROM --platform=linux/amd64 node:18-slim as build

WORKDIR /usr/src/app

COPY green-house-dashboard/package.json green-house-dashboard/package-lock.json ./

RUN npm install

COPY green-house-dashboard .

RUN npm run build

FROM --platform=linux/amd64 nginx:1.25.5-alpine
COPY --from=build /usr/src/app/build /app
COPY Greenhouse-backend/proxy/nginx.conf /etc/nginx/conf.d/default.conf