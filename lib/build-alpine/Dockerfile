FROM alpine:3.14.2 as base
RUN apk add --no-cache clang make

FROM base as builder
RUN apk add --no-cache git

ENV HOME /home/me
WORKDIR $HOME/.symbolica/build
COPY . .
RUN ./musl.sh

FROM base
ENV HOME /home/me
WORKDIR $HOME/.symbolica/build
COPY --from=builder /home/me/.symbolica/build .

WORKDIR /code
ENTRYPOINT [ "./symbolica.sh" ]
