using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.Logging;
using Mediachase.BusinessFoundation.Data.Business;

namespace OxxCommerceStarterKit.Commerce.Business.EventHandlers
{
    public class BusinessFoundationGenericLoggingHandler : IPlugin
    {
        protected static ILogger _log = EPiServer.Logging.LogManager.GetLogger();

        public void Execute(BusinessContext context)
        {
            _log.Debug("Execute (IPlugin) for {0}. Metaclass: {1}. PrimaryKey: {2}. Plugin Stage: {3}",
                context.GetMethod(),
                context.GetTargetMetaClassName(),
                context.GetTargetPrimaryKeyId().HasValue ? context.GetTargetPrimaryKeyId().Value.ToString() : "null",
                context.PluginStage);
        }
    }
}