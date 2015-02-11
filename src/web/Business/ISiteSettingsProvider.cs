using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxxCommerceStarterKit.Web.Models.PageTypes;

namespace OxxCommerceStarterKit.Web.Business
{
    public interface ISiteSettingsProvider
    {
        SettingsBlock GetSettings();
        HomePage GetStartPage();
    }
}
