#!/bin/bash
set -e

name=$1
version=$2
isLatest=$3

versionedTag="$name:$version"
latestTag="$name:latest"

tags=$([[ "$isLatest" = true ]] && echo "$versionedTag,$latestTag" || echo $versionedTag)

echo "::set-output name=tags::$(echo $tags)"
