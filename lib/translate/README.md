# Symbolica Translate Image

## Building the image

From this directory run the following command:

```sh
docker build . -t symbolica/translate:latest
```

## Running the image

```sh
docker run -v $(pwd)/<path-to-user-code>:/code symbolica/translate:latest <path-to-bc-output-from-build-step>
```
