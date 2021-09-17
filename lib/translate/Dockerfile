FROM alpine:3.14.2
LABEL maintainer "Symbolica <dev@symbolica.dev>"
RUN apk add --no-cache binutils
COPY --from=symbolica/clang-builder /tmp/clang/install/bin/llvm-extract-12 /usr/local/bin/
COPY --from=symbolica/clang-builder /tmp/clang/install/bin/opt /usr/local/bin/

COPY . /.symbolica/

WORKDIR /code
ENTRYPOINT [ "/.symbolica/translate" ]
