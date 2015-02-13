/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using AuthorizeNet.APICore;
using EPiServer;
using EPiServer.Core;
using EPiServer.Editor;
using EPiServer.Logging;
using EPiServer.Web.Routing;
using Mediachase.Commerce;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Website.Helpers;
using OxxCommerceStarterKit.Core.Extensions;
using OxxCommerceStarterKit.Core.Objects.SharedViewModels;
using OxxCommerceStarterKit.Core.PaymentProviders;
using OxxCommerceStarterKit.Core.PaymentProviders.DIBS;
using OxxCommerceStarterKit.Core.PaymentProviders.Payment;
using OxxCommerceStarterKit.Core.Repositories.Interfaces;
using OxxCommerceStarterKit.Web.Business;
using OxxCommerceStarterKit.Web.Business.Analytics;
using OxxCommerceStarterKit.Web.Business.Payment;
using OxxCommerceStarterKit.Web.Models.PageTypes;
using OxxCommerceStarterKit.Web.Models.PageTypes.Payment;
using OxxCommerceStarterKit.Web.Models.PageTypes.System;
using OxxCommerceStarterKit.Web.Models.ViewModels;
using OxxCommerceStarterKit.Web.Models.ViewModels.Payment;
using LineItem = OxxCommerceStarterKit.Core.Objects.LineItem;
using OxxCommerceStarterKit.Core.Services;

namespace OxxCommerceStarterKit.Web.Controllers
{
    public class GenericPaymentController : PaymentBaseController<GenericPaymentPage>
    {
        private readonly IContentRepository _contentRepository;
        private readonly IOrderService _orderService;
        private readonly ISiteSettingsProvider _siteConfiguration;
        private readonly IPaymentCompleteHandler _paymentCompleteHandler;
        private readonly ICurrentMarket _currentMarket;
        private readonly ILogger _logger;

        public GenericPaymentController(IContentRepository contentRepository, IOrderService orderService, IPaymentCompleteHandler paymentCompleteHandler, ISiteSettingsProvider siteConfiguration, ICurrentMarket currentMarket, ILogger logger)
        {
            _contentRepository = contentRepository;
            _orderService = orderService;
            _siteConfiguration = siteConfiguration;
            _paymentCompleteHandler = paymentCompleteHandler;
            _currentMarket = currentMarket;
            _logger = logger;
        }

        [RequireSSL]
        public ActionResult Index(GenericPaymentPage currentPage)
        {
            CartHelper ch = new CartHelper(Cart.DefaultName);

            if (ch.IsEmpty && !PageEditing.PageIsInEditMode)
            {                
                return View("Error/_EmptyCartError");
            }

            var orderInfo = new OrderInfo()
            {
                // Dibs expect the order to be without decimals
                Amount = Convert.ToInt32(ch.Cart.Total * 100),
                Currency = ch.Cart.BillingCurrency,
                OrderId = ch.Cart.GeneratePredictableOrderNumber(),
                ExpandOrderInformation = true
            };

            Guid paymentMethod = Guid.Empty; // When not set (creating a page), this is null
            if (string.IsNullOrEmpty(currentPage.PaymentMethod) == false)
                paymentMethod = new Guid(currentPage.PaymentMethod);

            GenericPaymentViewModel<GenericPaymentPage> model = new GenericPaymentViewModel<GenericPaymentPage>(paymentMethod, currentPage, orderInfo, ch.Cart);

            // Get cart and track it
            Api.CartController cartApiController = new Api.CartController();
            cartApiController.Language = currentPage.LanguageBranch;
            List<Core.Objects.LineItem> lineItems = cartApiController.GetItems(Cart.DefaultName);
            TrackBeforePayment(lineItems);

            return View(model);
        }

        [HttpPost]
        public ActionResult Index()
        {
            var receiptPage = _contentRepository.Get<ReceiptPage>(_siteConfiguration.GetSettings().ReceiptPage);

            var cartHelper = new CartHelper(Cart.DefaultName);

            if (cartHelper.IsEmpty && !PageEditing.PageIsInEditMode)
            {                
                return View("Error/_EmptyCartError");
            }

            string message = "";
            OrderViewModel orderViewModel = null;

            if (cartHelper.Cart.OrderForms.Count > 0)
            {
                var orderNumber = cartHelper.Cart.GeneratePredictableOrderNumber();

                _log.Debug("Order placed - order number: " + orderNumber);

                cartHelper.Cart.OrderNumberMethod = CartExtensions.GeneratePredictableOrderNumber;

                var results = OrderGroupWorkflowManager.RunWorkflow(cartHelper.Cart, OrderGroupWorkflowManager.CartCheckOutWorkflowName);
                message = string.Join(", ", OrderGroupWorkflowManager.GetWarningsFromWorkflowResult(results));

                if (message.Length == 0)
                {
                    cartHelper.Cart.SaveAsPurchaseOrder();
                    cartHelper.Cart.Delete();
                    cartHelper.Cart.AcceptChanges();
                }

                var order = _orderService.GetOrderByTrackingNumber(orderNumber);

                // Must be run after order is complete, 
                // This will release the order for shipment and 
                // send the order receipt by email
                _paymentCompleteHandler.ProcessCompletedPayment(order, User.Identity);

                orderViewModel = new OrderViewModel(_currentMarket.GetCurrentMarket().DefaultCurrency.Format, order);
            }

            ReceiptViewModel model = new ReceiptViewModel(receiptPage);
            model.CheckoutMessage = message;
            model.Order = orderViewModel;

            // Track successfull order in Google Analytics
            TrackAfterPayment(model);

            return View("ReceiptPage", model);
        }

        private void TrackBeforePayment(IEnumerable<LineItem> lineItems)
        {
            // Track Analytics. 
            GoogleAnalyticsTracking tracking = new GoogleAnalyticsTracking(ControllerContext.HttpContext);

            // Add the products
            int i = 1;
            foreach (LineItem lineItem in lineItems)
            {
                tracking.ProductAdd(code: lineItem.Code,
                    name: lineItem.Name,
                    quantity: (int)lineItem.Quantity,
                    price: (double)lineItem.PlacedPrice,
                    position: i
                    );
                i++;
            }

            // Step 3 is to pay
            tracking.Action("checkout", "{\"step\":3}");

            // Send it off
            tracking.Custom("ga('send', 'pageview');");
        }

        /// <summary>
        /// Processes the payment after we get back from the
        /// payment provider
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="result">The result posted from the provider.</param>
        [HttpPost]
        [RequireSSL]
        public ActionResult ProcessPayment(DibsPaymentPage currentPage, DibsPaymentResult result)
        {
            ReceiptPage receiptPage = _contentRepository.Get<ReceiptPage>(_siteConfiguration.GetSettings().ReceiptPage);

            if (_log.IsDebugEnabled())
                _log.Debug("Payment processed: {0}", result);

            CartHelper cartHelper = new CartHelper(Cart.DefaultName);
            string message = "";
            OrderViewModel orderViewModel = null;

            if (cartHelper.Cart.OrderForms.Count > 0)
            {
                var payment = cartHelper.Cart.OrderForms[0].Payments[0] as DibsPayment;

                if (payment != null)
                {
                    payment.CardNumberMasked = result.CardNumberMasked;
                    payment.CartTypeName = result.CardTypeName;
                    payment.TransactionID = result.Transaction;
                    payment.TransactionType = TransactionType.Authorization.ToString();
                    payment.Status = result.Status;
                    cartHelper.Cart.Status = DIBSPaymentGateway.PaymentCompleted;
                }
                else
                {
                    throw new Exception("Not a DIBS Payment");
                }

                var orderNumber = cartHelper.Cart.GeneratePredictableOrderNumber();

                _log.Debug("Order placed - order number: " + orderNumber);

                cartHelper.Cart.OrderNumberMethod = CartExtensions.GeneratePredictableOrderNumber;

                var results = OrderGroupWorkflowManager.RunWorkflow(cartHelper.Cart, OrderGroupWorkflowManager.CartCheckOutWorkflowName);
                message = string.Join(", ", OrderGroupWorkflowManager.GetWarningsFromWorkflowResult(results));

                if (message.Length == 0)
                {
                    cartHelper.Cart.SaveAsPurchaseOrder();
                    cartHelper.Cart.Delete();
                    cartHelper.Cart.AcceptChanges();
                }

                var order = _orderService.GetOrderByTrackingNumber(orderNumber);

                // Must be run after order is complete, 
                // This will release the order for shipment and 
                // send the order receipt by email
                _paymentCompleteHandler.ProcessCompletedPayment(order, User.Identity);

                orderViewModel = new OrderViewModel(_currentMarket.GetCurrentMarket().DefaultCurrency.Format, order);
            }

            ReceiptViewModel model = new ReceiptViewModel(receiptPage);
            model.CheckoutMessage = message;
            model.Order = orderViewModel;

            // Track successfull order in Google Analytics
            TrackAfterPayment(model);

            return View("ReceiptPage", model);
        }

        private void TrackAfterPayment(ReceiptViewModel model)
        {
            // Track Analytics 
            GoogleAnalyticsTracking tracking = new GoogleAnalyticsTracking(ControllerContext.HttpContext);

            // Add the products
            int i = 1;
            foreach (OrderLineViewModel orderLine in model.Order.OrderLines)
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
    }
}
