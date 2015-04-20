namespace OxxCommerceStarterKit.Web.Business.FacetRegistry
{
    public class FacetRangeDefinition<T> : FacetDefinition
    {
        public T From { get; set; }
        public T To { get; set; }
    }
}