# Introduction

The goal of this project is to inform me and the members of my family if our solar-panels are producing 

# How does it work

A process that runs indefinitely reads out the measurements of the HomeWizard P1 device at a certain interval.
If it turns out that the solar-panels are producing that much energy that we have a surplus of energy, a signal is triggered in the QBus home automation system that informs the habitants about the energy surplus.

# Build and run

## Build the image for a Windows operating system

- Build the project as a container image

  ```bash
  docker build . -f Dockerfile-amd64 -t solarpoweralerter:<tag>
  ```

- Run the program

  ```bash
  docker run solarpoweralerter:001 --network="host"
  ```

  The `--network="host"` argument makes sure that the container connects to the host's network.
  https://docs.docker.com/engine/api/v1.32/#tag/Container/operation/ContainerList


## Building for ARM32 devices

If you want to run the image on a Raspberry PI, you need to use the `Dockerfile-arm32` for building the image.

If you build this container for arm32 on a Windows system, [use docker buildx to build the image](https://docs.docker.com/build/install-buildx/).

You also need the required emulators.  Find information on how to install them [here](https://docs.docker.com/build/building/multi-platform/#build-and-run-multi-architecture-images).

Make sure that you have the linux/arm/v7 emulator.  Verify this by executing `docker buildx ls`.

https://github.com/dotnet/dotnet-docker/blob/main/samples/dotnetapp/README.md#build-an-image-for-arm32-and-arm64


https://github.com/dotnet/dotnet-docker/blob/main/samples/dotnetapp/Dockerfile.debian-arm32

https://github.com/dotnet/sdk/issues/28971

Once everything is in place, build the image using this command:

```
docker buildx build . -f .\Dockerfile-arm32 -t solarpoweralerter:<tag>
```

## Push to dockerhub

- Build the container locally
- Tag the container so that it can be pushed to dockerhub:
  ```
  docker tag solarpoweralerter:001 docker.io/fgheysels/solarpoweralerter:001
  ```
- Make sure to be logged in with Docker Hub via `docker login`
- Push the image to the repository `docker push fgheysels/solarpoweralerter:001`

- Pull image `docker pull docker.io/fgheysels/solarpoweralerter:001`

## Kubernetes sources

Deploy the component on a Kubernetes cluster by simply deploying the deployment manifest.

```
kubectl apply -f .\deploy\k8s\solar-prod-alert.yml -n solar-alert
```

https://www.alibabacloud.com/help/en/container-service-for-kubernetes/latest/use-the-host-network