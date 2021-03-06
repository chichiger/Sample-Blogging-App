| Services      | Platform      | author|
| ------------- |:-------------:| -----:|
| service-fabic | dotnet        | chichiger |

# Sample-Blogging-App
The sample blogging application shows how to build an end-to-end Service Fabric application with a front end stateless service and a backend stateful service. 
# Building and Deploying
First, [set up your development enviornment](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-get-started)
 with Visual Studio 2017. Make sure you have at least version 15.1 of Visual Studio 2017 installed.

This sample application can be built and deployed immediately using Visual Studio 2017. To deploy on the local cluster, you can simply hit F5 to debug the sample. If you'd like to try publishing the sample to an Azure cluster:

1. Right-click on the application project in Solution Explorer and choose Publish.
2. Sign-in to the Microsoft account associated with your Azure subscription.
3. Choose the cluster you'd like to deploy to.

# About the Sample App
This sample app contains two services and demonstrates how to use key parts of Service Fabric
## Stateless Frontend Service
  ### Key Concepts
  * Communicating with back end over HTTP using Service Proxy
## Stateful Backend Service
This service demonstrates the use of [Reliable Collections](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-reliable-services-reliable-collections) in a stateful service.

[Service Remoting](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-reliable-services-communication-remoting) is also used here to communicate between the services
  ### Key Concepts
  * Reliable Collections
  * MVC Controllers
  
#### Flowchart of the stateless front end interacting with the stateful back end
![Imgur Image](http://i.imgur.com/Yd3aceo.png)

---
*This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact opencode@microsoft.com with any additional questions or comments.*
