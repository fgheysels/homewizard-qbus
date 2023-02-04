# Introduction

The goal of this project is to inform me and the members of my family if our solar-panels are producing 

# How does it work

# Build and run

- Build the project as a container image

  ```bash
  docker build . -f Dockerfile -t solarpoweralerter:001
  ```

- Run the program

  ```bash
  docker run solarpoweralerter:001
  ```

- Push to dockerhub
  - Make sure to be logged in with Docker Hub via `docker login`
  - Push the image to the repository `docker push fgheysels/solarpoweralerter:001`

- Pull image `docker pull docker.io/fgheysels/solarpoweralerter:001`

## Building for ARM32

If you want to run the image on a Raspberry PI, you need to use the `Dockerfile-arm32` for building the image.

If you build this container on a Windows system, use docker buildx to build the image: https://docs.docker.com/build/install-buildx/

You also need the required emulators: https://docs.docker.com/build/building/multi-platform/#build-and-run-multi-architecture-images
Make sure that you have the linux/arm/v7 emulator.  Verify this by executing `docker buildx ls`.

https://github.com/dotnet/dotnet-docker/blob/main/samples/dotnetapp/README.md#build-an-image-for-arm32-and-arm64


https://github.com/dotnet/dotnet-docker/blob/main/samples/dotnetapp/Dockerfile.debian-arm32
https://github.com/dotnet/sdk/issues/28971

If you have, build the image:

```
docker buildx build --platform=linux/arm/v7 --progress=plain . -f .\Dockerfile-arm32 -t solarpoweralerter:003
```