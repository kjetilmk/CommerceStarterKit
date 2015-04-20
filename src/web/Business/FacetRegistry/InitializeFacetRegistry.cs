using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace OxxCommerceStarterKit.Web.Business.FacetRegistry
{
    
    [ModuleDependency(typeof(ConfigureFacetRegistryInContainer))]
    public class InitializeFacetRegistry : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            IFacetRegistry registry = ServiceLocator.Current.GetInstance<IFacetRegistry>();
            registry.FacetDefinitions.Add(new FacetStringListDefinition()
            {
                FieldName = "Country"
            });

            registry.FacetDefinitions.Add(new FacetStringListDefinition()
            {
                FieldName = "Region"
            });

            registry.FacetDefinitions.Add(new FacetRangeDefinition<decimal>()
            {
                FieldName = "Price"
            });


        }

        public void Uninitialize(InitializationEngine context)
        {
            
        }

        public void Preload(string[] parameters)
        {
            
        }
    }
}