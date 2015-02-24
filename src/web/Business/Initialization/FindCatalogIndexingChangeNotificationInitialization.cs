/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Linq;
using EPiServer.Events.ChangeNotification;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging;
using EPiServer.ServiceLocation;

namespace OxxCommerceStarterKit.Web.Business.Initialization
{
    /// <summary>
    /// NOTE! This is an example on how to handle catalog changes using the change notification system.
    /// The site does not yet do actual Find indexing based on this.
    /// </summary>
    [ModuleDependency(typeof(EPiServer.Commerce.Initialization.InitializationModule))]
    public class FindCatalogIndexingChangeNotificationInitialization : IConfigurableModule
    {
        private ILogger _log = LogManager.GetLogger();

        public void Initialize(InitializationEngine context)
        {
            // We need to make sure the processor is in a
            // valid state. Note! This could mean we start
            // indexing 
            _log.Debug("Try start recovery.");
            var changeManager = ServiceLocator.Current.GetInstance<IChangeNotificationManager>();
            var processor = changeManager.GetProcessorInfo().SingleOrDefault(ep => ep.ProcessorId == FindCatalogIndexingChangeNotificationProcessor.ProcessorId);
            if (processor != null)
            {
                processor.TryStartRecovery();
            }
        }

        public void Preload(string[] parameters)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            _log.Debug("Registering FindCatalogIndexingChangeNotificationProcessor");
            context.Container.Configure(c => c.For<IChangeProcessor>().Singleton().Add<FindCatalogIndexingChangeNotificationProcessor>());

        }
    }
}
