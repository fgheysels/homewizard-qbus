# Introduction

The goal of this project is to inform me and the members of my family if our solar-panels are producing more electricity than what we're currently consuming.

# How does it work

A process that runs indefinitely reads out the measurements of the HomeWizard P1 device at a certain interval.
If it turns out that the solar-panels are producing that much energy that we have a surplus of energy, a signal is triggered in the QBus home automation system that informs the habitants about the energy surplus.

I have this process running on a Raspberry Pi where it is hosted in Kubernetes (k3s).  This is actually overkill, but I wanted to experiment with Kubernetes as well.  Information on how to install k3s on a Raspberry Pi can be found [here](https://github.com/fgheysels/pi_k3s).

# Build and run

You can pull the image from [Dockerhub](https://hub.docker.com/r/fgheysels/solarpoweralerter/tags).

## Build the image for a Windows operating system

- Build the project as a container image

  ```bash
  docker build . -f Dockerfile-amd64 -t solarpoweralerter:<tag>
  ```

- Run the program

  ```bash
  docker run solarpoweralerter:001 --network="host"
  ```

  The `--network="host"` argument makes sure that the [container connects to the host's network](https://docs.docker.com/engine/reference/run/#network-settings).


## Building for ARM32 devices

If you want to run the image on a Raspberry PI, you need to use the `Dockerfile-arm32` for building the image.

If you build this container for arm32 on a Windows system, [use docker buildx to build the image](https://docs.docker.com/build/install-buildx/).

You also need the required emulators.  Find information on how to install them [here](https://docs.docker.com/build/building/multi-platform/#build-and-run-multi-architecture-images).

Make sure that you have the linux/arm/v7 emulator.  Verify this by executing `docker buildx ls`.

Additional background information for this can be found [here](https://github.com/dotnet/dotnet-docker/blob/main/samples/dotnetapp/README.md#build-an-image-for-arm32-and-arm64).

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
- Push the image to the repository `docker push fgheysels/solarpoweralerter:0.0.1`

- Pull image `docker pull docker.io/fgheysels/solarpoweralerter:0.0.1`

## Deploy to Kubernetes

Deploy the component on a Kubernetes cluster by simply deploying the deployment manifest.

```
kubectl apply -f .\deploy\k8s\solar-prod-alert.yml -n solar-alert
```

## Configuration

Following configuration settings are required:

|Setting Name|Value|Description|
|-|-|-|
|HomeWizard__P1HostName|p1meter_015ABC|The hostname of the HomeWizard P1 name. We try to find the HomeWizard in the network via this name|
|QBus__IpAddress|192.168.1.14|The IP address of the QBus controller|
|QBus__Port|8444|The port at which the QBus EqoWeb API is listening|
|QBus__Username|QBUS|The username of the account that must be used to connect to QBus EqoWeb. Note that the username is case-sensitive|
|QBus__Password||The password of the account that is used to connect to QBus EqoWeb|
|QBus__SolarIndicators||A comma-separated string that lists the QBus devices that must be notified on Power Usage state changes|
|PowerUsageThresholds__NotEnoughProduction|200|The amount of electricity power (in watt) that must be exceeded to determine that we're consuming more electricity than that we're producing|
|PowerUsageThresholds__OverProduction|-800|The amount of electricity powser (in watt) that must be passed to determine that we're producing more electricity than that we're consuming|
|PowerUsageThresholds__ExtremeOverProduction|-2500|The amount of electricity power (in watt) that must be passed to determine that we're producing a whole lot more electricity than that we're consuming|

When the power usage (in watt) is between `PowerUsageThresholds__NotEnoughProduction` and `PowerUsageThresholds__OverProduction`, we consider this as a 'break-even' state.