# Symbolica Build Image

## Building the image

From this directory run the following command:

```sh
docker build . -t symbolica/build:latest
```

## Running the image

```sh
docker run -v $(pwd)/<path-to-user-code>:/code symbolica/build:latest
```
