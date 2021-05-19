# Symbolica Build Image

## Building the image

From the root of this repository run the following command:

```sh
docker build . -f docker/build/Dockerfile -t symbolica/build:latest
```

## Running the image

```sh
docker run -v $(pwd)/<path-to-user-code>:/code ssvm-build:latest </path/to/user/build.sh>
```

For instance, if you want to generate the bc files for `/tests/pass/c/average` you should run the following command, assuming you're in the root of this repository.

```sh
docker run -v $(pwd)/tests/pass/c/average:/code symbolica/build:latest
```
