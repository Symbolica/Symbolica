FROM alpine:3.14.2 as builder

COPY aports /tmp/aports

RUN apk add --no-cache alpine-sdk

RUN adduser -D apk && \
    adduser apk abuild && \
    su - apk && \
    abuild-keygen -an && \
    cp /home/apk/.abuild/*.rsa.pub /etc/apk/keys

RUN cd tmp/aports/main/clang && \
    abuild -r

RUN apk add /home/apk/packages/main/$(uname -m)/*.apk

# FROM alpine:3.14.2
# LABEL maintainer "Symbolica <dev@symbolica.dev>"
# # FIXME: Install all packages you want to have in your release container.
# # A minimal useful installation should include at least libstdc++ and binutils.
# RUN apk add --no-cache binutils libstdc++

# COPY --from=builder /tmp/clang/install/ /usr/local/
