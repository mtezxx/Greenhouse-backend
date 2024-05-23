#!/bin/sh
docker build --no-cache -t sep4backendregistry.azurecr.io/backend:v1 .
docker image push sep4backendregistry.azurecr.io/backend:v1