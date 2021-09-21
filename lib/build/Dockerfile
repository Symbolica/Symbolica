FROM alpine:3.14.2
RUN apk add --no-cache clang git make

ENV HOME /home/me
WORKDIR $HOME/.symbolica/build
COPY . .
RUN ./musl.sh

WORKDIR /code
ENTRYPOINT [ "./symbolica.sh" ]
