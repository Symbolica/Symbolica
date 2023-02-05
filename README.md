# Symbolica

Run your code forall inputs

## What is Symbolica?
Symbolica is a symbolic code executor hosted as a 'CI-like' service. We run your code, but instead of feeding it concrete inputs like 1 or "Hello World", we feed it symbols (variables) that represent all of the possible inputs to your program. This allows us to explore all the code paths at the same time, which allows us to do all kinds of interesting things, such as:

- check if two different programs are logically equivalent
- find out if any inputs lead to buffer overflows
- detect inputs that cause undefined behaviour

## Quickstart

### Prerequisites

- [Dotnet 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Docker](https://www.docker.com/)

### Clone Symbolica

```sh
git clone https://github.com/Symbolica/Symbolica.git
cd ./Symboica
```

### Set env variables
Set the [latest versions](https://hub.docker.com/u/symbolica) of the Symbolica build and translate images.

For example:
```sh
export SYMBOLICA_BUILD_IMAGE=symbolica/build:0.2.0-master0183
export SYMBOLICA_TRANSLATE_IMAGE=symbolica/translate:0.2.0-master0183
```

### Run the example
```sh
dotnet run --project src/Application './example'
```


[![Build history](https://buildstats.info/github/chart/SymbolicaDev/Symbolica?branch=master)](https://github.com/SymbolicaDev/Symbolica/actions)

## NuGet Packages

### Symbolica.Abstraction

[![NuGet Badge](https://buildstats.info/nuget/Symbolica.Abstraction)](https://www.nuget.org/packages/Symbolica.Abstraction/)

### Symbolica.Collection

[![NuGet Badge](https://buildstats.info/nuget/Symbolica.Collection)](https://www.nuget.org/packages/Symbolica.Collection/)

### Symbolica.Computation

[![NuGet Badge](https://buildstats.info/nuget/Symbolica.Computation)](https://www.nuget.org/packages/Symbolica.Computation/)

### Symbolica.Deserialization

[![NuGet Badge](https://buildstats.info/nuget/Symbolica.Deserialization)](https://www.nuget.org/packages/Symbolica.Deserialization/)

### Symbolica.Expression

[![NuGet Badge](https://buildstats.info/nuget/Symbolica.Expression)](https://www.nuget.org/packages/Symbolica.Expression/)

### Symbolica.Implementation

[![NuGet Badge](https://buildstats.info/nuget/Symbolica.Implementation)](https://www.nuget.org/packages/Symbolica.Implementation/)

### Symbolica.Representation

[![NuGet Badge](https://buildstats.info/nuget/Symbolica.Representation)](https://www.nuget.org/packages/Symbolica.Representation/)

## Docker Images

### symbolica/build

```sh
docker build lib/c/build -t symbolica/build:latest
```

```sh
docker run -v <path-to-user-code>:/code symbolica/build:latest
```

[![Docker Image Version (latest semver)](https://img.shields.io/docker/v/symbolica/build?sort=semver&logo=Docker)](https://hub.docker.com/repository/docker/symbolica/build)
[![Docker Pulls](https://img.shields.io/docker/pulls/symbolica/build?logo=Docker&label=pulls)](https://hub.docker.com/repository/docker/symbolica/build)

### symbolica/build-cpp

```sh
docker build lib/cpp/build --build-arg BUILD_BASE_IMG_TAG=latest -t symbolica/build-cpp:latest
```

```sh
docker run -v <path-to-user-code>:/code symbolica/build-cpp:latest
```

[![Docker Image Version (latest semver)](https://img.shields.io/docker/v/symbolica/build-cpp?sort=semver&logo=Docker)](https://hub.docker.com/repository/docker/symbolica/build-cpp)
[![Docker Pulls](https://img.shields.io/docker/pulls/symbolica/build-cpp?logo=Docker&label=pulls)](https://hub.docker.com/repository/docker/symbolica/build-cpp)

### symbolica/translate

```sh
docker build lib/translate -t symbolica/translate:latest
```

```sh
docker run -v <path-to-user-code>:/code symbolica/translate:latest <declarations>
```

[![Docker Image Version (latest semver)](https://img.shields.io/docker/v/symbolica/translate?sort=semver&logo=Docker)](https://hub.docker.com/repository/docker/symbolica/translate)
[![Docker Pulls](https://img.shields.io/docker/pulls/symbolica/translate?logo=Docker&label=pulls)](https://hub.docker.com/repository/docker/symbolica/translate)

Note, we don't publish a `latest` tag because we prefer to use SemVer instead, even if Docker doesn't natively support it.
Our stable images will be tagged according to the tags in this repository and the versions are aligned with the NuGet package versions.
If you're using the docker images in conjunction with the NuGet packages, it's best to make sure you're using a tag for the docker images that matches the NuGet package version you're using.
