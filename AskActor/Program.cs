using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using StructureMap;
using StructureMap.Pipeline;

namespace AskActor
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // This line registers an Actor Service to host your actor class with the Service Fabric runtime.
                // The contents of your ServiceManifest.xml and ApplicationManifest.xml files
                // are automatically populated when you build this project.
                // For more information, see https://aka.ms/servicefabricactorsplatform
                var cosmosDB = FabricRuntime.GetActivationContext().GetConfigurationPackageObject("Config").Settings.Sections["CosmosDB"];
                Container container = new Container();
                container.Configure(config =>
                {
                    var configPackage = 
                    config.For<AskActor>().Use<AskActor>();
                });
                ActorRuntime.RegisterActorAsync<AskActor> (
                   (context, actorType) => new ActorService(context, actorType, (service, actorId) => 
                    container.GetInstance<AskActor>(
                        new ExplicitArguments().Set(service).Set(actorId)
                        ))).GetAwaiter().GetResult();

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
