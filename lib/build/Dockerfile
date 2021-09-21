FROM alpine:3.14.2
RUN apk add --no-cache binutils clang git llvm make

ENV HOME /home/me
WORKDIR $HOME/.symbolica/build
COPY . .
RUN ./musl.sh

WORKDIR /code
ENTRYPOINT [ "./symbolica.sh" ]
