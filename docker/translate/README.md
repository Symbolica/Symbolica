# Symbolica Translate Image

## Building the image

From the root of this repository run the following command:

```sh
docker build . -f docker/translate/Dockerfile -t symbolica/translate:latest
```

## Running the image

```sh
docker run -v $(pwd)/<path-to-user-code>:/code symbolica/translate:latest
```
