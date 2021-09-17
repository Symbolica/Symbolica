FROM alpine:3.14.2
LABEL maintainer "Symbolica <dev@symbolica.dev>"
RUN apk add --no-cache binutils
COPY --from=symbolica/clang-builder /tmp/clang/install/bin/llvm-extract /usr/local/bin/llvm-extract-12
COPY --from=symbolica/clang-builder /tmp/clang/install/bin/opt /usr/local/bin/opt-12

COPY . /.symbolica/

WORKDIR /code
ENTRYPOINT [ "/.symbolica/translate" ]
