/* jshint -W099 */
/* global jQuery:false */
/* global angular:false */
(function ($, productApp) {

    "use strict";

    productApp.factory("trackingService", [function () {
        return {
            trackAddToCart: function (product) {
                if (window.ga) {
                    console.log("Track add to cart", product);
                    var thisGa = window.ga;
                    thisGa('ec:addProduct', {
                        'id': product.code,
                        'name': product.name || product.code,
                        'quantity': product.quantity
                    });
                    thisGa('ec:setAction', 'add');
                    thisGa('send', 'event', 'UX', 'click', 'add to cart');
                }
            },

            trackRemoveFromCart: function (product) {
                if (window.ga) {
                    console.log("Track remove from cart", product);
                    var thisGa = window.ga;
                    thisGa('ec:addProduct', {
                        'id': product.code,
                        'name': product.name || product.code,
                        'quantity': product.quantity
                    });
                    thisGa('ec:setAction', 'remove');
                    thisGa('send', 'event', 'UX', 'click', 'remove from cart');
                }
            }
        };

    }]);

})(jQuery, window.productApp);