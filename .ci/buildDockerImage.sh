#!/bin/bash
set -e
set -u
set -x

docker build --file ./Tokens.API/Dockerfile --tag ghcr.io/nmshd/bkb-tokens:${TAG-temp} .
