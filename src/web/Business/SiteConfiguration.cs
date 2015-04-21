/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using OxxCommerceStarterKit.Web.Models.PageTypes;

namespace OxxCommerceStarterKit.Web.Business
{
    public class SiteConfiguration : ISiteSettingsProvider
    {
        public SettingsBlock GetSettings()
        {
            HomePage homePage = GetStartPage();
            if (homePage != null)
                return homePage.Settings;

            return null;
        }

        public HomePage GetStartPage()
        {
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            // This can actually be 0 if we have a problem with our language settings
            if (ContentReference.StartPage != null && ContentReference.StartPage.ID > 0)
            {
                HomePage startPage = null;
                try
                {
                    startPage = contentLoader.Get<HomePage>(ContentReference.StartPage);
                }
                catch (Exception ex)
                {
                    return null;
                }
                return startPage;
            }
            return null;
        }

    }
}
