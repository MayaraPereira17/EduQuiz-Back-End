#!/bin/bash

IMAGE_NAME=eduquizapi

echo "Construindo a imagem Docker..."
docker build -t $IMAGE_NAME .

if [ "$(docker ps -q -f name=$IMAGE_NAME)" ]; then
    echo "Parando container existente..."
    docker stop $IMAGE_NAME
    docker rm $IMAGE_NAME
fi

echo "Rodando o container..."
docker run -d -p 8080:8080 --name $IMAGE_NAME $IMAGE_NAME

echo "Aplicação rodando em http://localhost:8080"
