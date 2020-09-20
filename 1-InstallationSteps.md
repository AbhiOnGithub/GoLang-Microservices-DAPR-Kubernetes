# Installation of DAPR (Distributed Application Runtime)
An event-driven, portable runtime for building microservices on cloud and edge.

## Installing DAPR CLI on *Windows*

1) Open Windows Powershell
2) Run
   
```cmd
Set-ExecutionPolicy RemoteSigned -scope CurrentUser
```

3) Run 

``` cmd
powershell -Command "iwr -useb https://raw.githubusercontent.com/dapr/cli/master/install/install.ps1 | iex"
```

output -
<img src="images/1-InstallingDaprOnWindows.PNG "/>

<hr/>

## Installing DAPR CLI on *Linux*
Install the latest linux Dapr CLI to /usr/local/bin

```cmd
wget -q https://raw.githubusercontent.com/dapr/cli/master/install/install.sh -O - | /bin/bash
```

<hr/>

## Installing DAPR CLI on *MacOS*

Install the latest darwin Dapr CLI to /usr/local/bin

```cmd
curl -fsSL https://raw.githubusercontent.com/dapr/cli/master/install/install.sh | /bin/bash
```

Or install via Homebrew

```cmd
brew install dapr/tap/dapr-cli
```
<hr/>

## Installing Dapr in self hosted mode
**Initialize Dapr using the CLI**
By default, during initialization the Dapr CLI will install the Dapr binaries as well as setup a developer environment to help you get started easily with Dapr. This environment uses Docker containers, therefore Docker is listed as a prerequisite.

```cmd
dapr init
```
<img src="images/2-Dapr-Init-SelfHostedMode.PNG" />

To see that Dapr has been installed successfully, from a command prompt run the **docker ps** command and check that the daprio/dapr:latest and redis container images are both running.

<img src="images/3-Docker-Init-For-DAPR.png" />

<hr/>

## On Kubernetes Clusters

When setting up Kubernetes, you can do this either via the Dapr CLI or Helm.

Dapr installs the following pods:

* dapr-operator: Manages component updates and kubernetes services endpoints for Dapr (state stores, pub-subs, etc.)
* dapr-sidecar-injector: Injects Dapr into annotated deployment pods
* dapr-placement: Used for actors only. Creates mapping tables that map actor instances to pods
* dapr-sentry: Manages mTLS between services and acts as a certificate authority

## Set up an Azure Kubernetes Service cluster

## Prerequisites

- [Docker](https://docs.docker.com/install/)
- [kubectl](https://kubernetes.io/docs/tasks/tools/install-kubectl/)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest)

## Deploy an Azure Kubernetes Service cluster

This section walks you through installing an Azure Kubernetes Service (AKS) cluster. 

1. Login to Azure

```bash
az login
```

2. Set the default subscription

```bash
az account set -s [your_subscription_id]
```

3. Create a resource group

```bash
az group create --name [your_resource_group] --location [region]
ex : az group create --name kubernetes-dapr-rg --location westus
```

4. Create an Azure Kubernetes Service cluster

Use 1.17.9 or newer version of Kubernetes with `--kubernetes-version`

```bash
az aks create --resource-group [your_resource_group] --name [your_aks_cluster_name] --node-count 2 --kubernetes-version 1.17.9 --enable-addons http_application_routing --enable-rbac --generate-ssh-keys
```

This is how I got my K8s Instance (AKS) provisioned on Azure

<img src="images/6-Provisioned-AKS.png" />

Get the access credentials for the Azure Kubernetes cluster

```bash
az aks get-credentials -n [your_aks_cluster_name] -g [your_resource_group]
ex : az aks get-credentials -n dapr-aks-cluster -g kubernetes-dapr-rg
```

<img src="images/4-GetContextOfAKS.PNG" />

5. Finally, also Create Azure Container Registry

```cmd
az acr create --resource-group kubernetes-dapr-rg --name daprk8sreg --sku Basic
```

we will use this container registry later

<hr/>

## (optional) Install Helm v3

Install Helm v3 client

```cmd
choco install kubernetes-helm
```
In case you need permissions the kubernetes dashboard (i.e. configmaps is forbidden: User "system:serviceaccount:kube-system:kubernetes-dashboard" cannot list configmaps in the namespace "default", etc.) execute this command

```bash
kubectl create clusterrolebinding kubernetes-dashboard -n kube-system --clusterrole=cluster-admin --serviceaccount=kube-system:kubernetes-dashboard
```

for me it already exists as shown below :

<img src="images/5-Kubernetes-Dashboard-Permissions.png" />

<hr/>

## Install Dapr to Kubernetes **(AKS)**

## Option A (Using DAPR CLI)
## Option B (Using HELM)

**Option A** Using the Dapr CLI

1. Install Dapr to Kubernetes

```cmd
dapr init -k
```

2. Check Status of

```cmd
dapr status -k
```

As shown here
<img src="images/7-Dapr-Installation-In-K8s.PNG" />

3. Uninstall like this **(if required)**

```cmd
dapr uninstall --kubernetes
```

### Verify installation

Once the chart installation is complete, verify the dapr-operator, dapr-placement, dapr-sidecar-injector and dapr-sentry pods are running in the `dapr-system` namespace:

```bash
$ kubectl get pods -n dapr-system -w
```

<img src="images/8-Get-Dapr-System-PODS.PNG" />

### Sidecar annotations

To see all the supported annotations for the Dapr sidecar on Kubernetes, visit [this](../howto/configure-k8s/README.md) how to guide.

#### Uninstall Dapr on Kubernetes **(Only if required)**

Helm 3

```bash
helm uninstall dapr -n dapr-system
```

<hr/>