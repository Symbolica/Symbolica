FROM symbolica/clang:latest as clang
# RUN apk add --no-cache clang make
ENV HOME /home/me

FROM clang as builder
RUN apk add --no-cache git make

WORKDIR $HOME/.symbolica/build
COPY . .
RUN ./musl.sh

FROM clang
COPY --from=builder /home/me/.symbolica/build .

WORKDIR /code
ENTRYPOINT [ "./symbolica.sh" ]
