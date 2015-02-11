/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.Framework.DataAnnotations;
using EPiServer.GoogleAnalytics.Helpers;
using EPiServer.Security;
using EPiServer.Web.Mvc;
using Mediachase.Commerce.Customers;
using OxxCommerceStarterKit.Web.Business.Analytics;
using OxxCommerceStarterKit.Web.Models.PageTypes;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Controllers
{
    [TemplateDescriptor()]
    public class HomePageController : PageControllerBase<HomePage>
    {
		private readonly IContentLoader _contentLoader;

        public HomePageController(IContentLoader contentLoader)
        {            
			_contentLoader = contentLoader;		    
        }

        public ViewResult Index(PageData currentPage)
        {
            var virtualPath = String.Format("~/Views/{0}/Index.cshtml", currentPage.GetOriginalType().Name);
            if (System.IO.File.Exists(Request.MapPath(virtualPath)) == false)
            {
                virtualPath = "Index";
            }

            var model = CreatePageViewModel(currentPage);
            var editHints = ViewData.GetEditHints<Chrome, HomePage>();
            editHints.AddConnection(c => c.GlobalFooterContent, p => p.GlobalFooterContent);

            return View(virtualPath, model);
        }

    }
}
