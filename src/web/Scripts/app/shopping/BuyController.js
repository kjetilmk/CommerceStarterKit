/* jshint -W099 */
/* global jQuery:false */
/* global angular:false */
(function($, productApp, commercestarterkit) {

	"use strict";

	productApp.controller("BuyController", ['$scope', 'handleCartService', 'trackingService', function ($scope, handleCartService, trackingService) {

	    $scope.animationImageId = "";
	    $scope.showPopupDialog = false;
        // Used to format numbers

        $scope.initLanguage = function (language) {
	        $scope.language = language;
	    }

        $scope.initWithProduct = function (product) {
            // console.log("Init buy controller with product");
            // console.log(product);
            $scope.initLanguage(product.Language);
            $scope.product = $scope.getProductModel(product);
            if (product.Variants) {
                $scope.showPopupDialog = true;
            }
        };

	    $scope.getProductModel = function(fullProductModel) {
	        var product = {
	            code: fullProductModel.Code,
	            name: fullProductModel.Name,
	            quantity: 1,
	            color: '',
	            size: ''
	        };

	        // Defaults
	        if (fullProductModel.Color && fullProductModel.Color.length > 0) {
	            product.color = fullProductModel.Color[0];
	        }
	        if (fullProductModel.Sizes && fullProductModel.Sizes.length > 0) {
	            product.size = fullProductModel.Sizes[0];
	        }
	        return product;
	    };

	    $scope.init = function (language, code, name, quantity, color, size, animationImageId, contentType) {
            $scope.initLanguage(language);
			$scope.product = {
				code: code,
				name: name,
				quantity: quantity,
				color: color,
				size: size
			};
			$scope.animationImageId = animationImageId;
	        // Since we don't have Variants for the simple init, we check the content type
			if (contentType && contentType.lastIndexOf("Fashion", 0) === 0) {
			    $scope.showPopupDialog = true;
			}
        };

		$scope.sanityCheckQuantity = function(quantity) {
			quantity = parseInt(quantity);
			if(quantity < 1 || quantity > 1000) {
				quantity = 1;
			}
			return quantity;
		};

		
		$scope.addToCartDefault = function () {
		    if ($scope.showPopupDialog === true) {
		        commercestarterkit.openProductDialog($scope.product.code);
		    } else {
		        $scope.addToCart($scope.product);
		    }
		};

	    $scope.addToCart = function(product) {
			product.quantity = $scope.sanityCheckQuantity(product.quantity);
	        if ($scope.showPopupDialog === true) {
	            commercestarterkit.openProductDialog($scope.product.code);
	        } else {

	            // var cart = $('.menu-top-right ul li').first();
	            var cart = $('.cart-counter').parent();
	            if (cart) {
	                animateAddToCart(cart, $scope.animationImageId, $.proxy($scope._addToCartAnimateComplete, $scope, product));
	            } else {
	                $scope._addToCartAnimateComplete(product);
	            }
	        }
	        // Track in Analytics
		    trackingService.trackAddToCart(product);
		};

		$scope._addToCartAnimateComplete = function(product) {
		    handleCartService.addToCart($scope.language, product);
			$scope.addedToCartMessageVisible = true;
			commercestarterkit.updateCartCounter(commercestarterkit.getCartCounter() + parseInt(product.quantity));
		};


		function animateAddToCart(cart, animationImageId, callback) {
		    var imgtodrag = $('#' + animationImageId).first();

			if (imgtodrag && imgtodrag.length > 0) {
				var imgclone = imgtodrag.clone()
					.offset({
						top: imgtodrag.offset().top,
						left: imgtodrag.offset().left
					})
					.css({
						'opacity': '0.5',
						'position': 'absolute',
						'height': '150px',
						'width': '150px',
						'z-index': '100'
					})
					.appendTo($('body'))
					.animate({
						'top': cart.offset().top,
						'left': cart.offset().left + 30,
						'width': 75,
						'height': 75
					}, 700, 'easeInOutExpo', callback);

				imgclone.animate({
					'width': 0,
					'height': 0
				}, function() {
					imgclone.remove();
				});
			} else {
				callback();
			}
		}
	}]);

})(jQuery, window.productApp, window.commercestarterkit);
