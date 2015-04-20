using System.Collections.Generic;

namespace OxxCommerceStarterKit.Web.Business.FacetRegistry
{
    public interface IFacetRegistry
    {
        List<FacetDefinition> FacetDefinitions { get; set; }
    }

    public class FacetRegistry : IFacetRegistry
    {
        public List<FacetDefinition> FacetDefinitions { get; set; }
        
    }
}