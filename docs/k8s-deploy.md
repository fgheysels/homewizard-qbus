# Introduction

The program is packaged as a container and can be deployed in a Kubernetes cluster

## Deploy in Kubernetes

Deploy the `solar-prod-alert.yml` file that can be found in the `deploy/k8s` folder.  This manifest will create a deployment in the Kubernetes cluster that creates one pod:

```
kubectl apply -f solar-prod-alert.yml -n solar-alert
```

Verify that the pod is running:

```
kubectl get pods -n solar-alert
```

Troubleshoot:

```
kubectl describe pod <podname> -n solar-alert

kubectl logs <podname> -n solar-alert
```