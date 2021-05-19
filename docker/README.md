# Docker Images

This folder contains the docker images that can be used to build user code and translate it into a format that can be consumed by Symbolica.
Each sub-folder of this directory contains a separate image.
The images should be built in the context of the root of this repository as they often require files that are useful in contexts outside of these docker images, for example when building on a bare metal Linux machine.
Therefore, if you were to build the images from this directory you would need to do it like so:

```sh
docker build .. -f build/Dockerfile -t symbolica/build:latest
```
