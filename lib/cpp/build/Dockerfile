ARG BUILD_BASE_IMG_TAG
FROM symbolica/build:${BUILD_BASE_IMG_TAG}
LABEL maintainer "Symbolica <dev@symbolica.dev>"

COPY . $HOME/.symbolica

RUN apk add --no-cache --virtual libcxx-build-deps cmake git ninja python3 && \
    cd /tmp && \
    $HOME/.symbolica/install-libcxx.sh && \
    rm -rf /tmp/* $HOME/.symbolica/install-libcxx.sh && \
    apk --purge del libcxx-build-deps

WORKDIR /code
ENTRYPOINT [ "./symbolica.sh" ]
