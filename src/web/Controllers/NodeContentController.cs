/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Globalization;
using EPiServer.Web.Mvc;

namespace OxxCommerceStarterKit.Web.Controllers
{
    [TemplateDescriptor(Inherited = true)]
    public class NodeContentController : ContentController<NodeContent>
    {

        public ActionResult Index(NodeContent currentContent)
        {
            return View(currentContent);
        }
    }
}
