using OxxCommerceStarterKit.Web.Business.Analytics;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Business.ClientTracking
{
    public class GoogleAnalyticsTracker : IGoogleAnalyticsTracker
    {
        private readonly IHttpContextProvider _contextProvider;

        public GoogleAnalyticsTracker(IHttpContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public void TrackAfterPayment(ReceiptViewModel model)
        {
            // Track Analytics 
            var tracking = new GoogleAnalyticsTracking(_contextProvider.GetContext());

            // Add the products
            int i = 1;
            foreach (var orderLine in model.Order.OrderLines)
            {
                if (string.IsNullOrEmpty(orderLine.Code) == false)
                {
                    tracking.ProductAdd(code: orderLine.Code,
                        name: orderLine.Name,
                        quantity: orderLine.Quantity,
                        price: (double)orderLine.Price,
                        position: i
                        );
                    i++;
                }
            }

            // And the transaction itself
            tracking.Purchase(model.Order.OrderNumber,
                null, (double)model.Order.TotalAmount, (double)model.Order.Tax, (double)model.Order.Shipping);
        }

        public void TrackPromotionImpression(string id, string name, string bannerName)
        {
            var tracking = new GoogleAnalyticsTracking(_contextProvider.GetContext());
            tracking.PromotionImpression(id, name, bannerName);
        }

        public string GetPromotionClickScript(string id, string name, string bannerName)
        {
            var tracking = new GoogleAnalyticsTracking(_contextProvider.GetContext());
            return tracking.PromotionClickScript(id, name, bannerName);
        }
    }
}