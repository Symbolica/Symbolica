#!/bin/bash
set -e

mkdir -p ~/.docker/machine/cache
curl -Lo ~/.docker/machine/cache/boot2docker.iso https://github.com/boot2docker/boot2docker/releases/download/v19.03.12/boot2docker.iso
brew install docker docker-machine
docker-machine create --driver virtualbox default
docker-machine env default
eval "$(docker-machine env default)"
docker run hello-world
