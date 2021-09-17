FROM alpine:3.14.2 as base
ENV HOME /home/me
RUN apk add --no-cache binutils
COPY --from=symbolica/clang-builder /tmp/clang/install/bin/clang-12 /usr/local/bin/
COPY --from=symbolica/clang-builder /tmp/clang/install/bin/llvm-link /usr/local/bin/

FROM base as musl-builder
RUN apk add --no-cache git make

WORKDIR $HOME/.symbolica/build
COPY . .
RUN ./musl.sh

FROM base
LABEL maintainer "Symbolica <dev@symbolica.dev>"

# TODO: Only copy across the built musl artifacts
COPY --from=musl-builder $HOME/.symbolica/build $HOME/.symbolica/build

WORKDIR /code
ENTRYPOINT [ "./symbolica.sh" ]
