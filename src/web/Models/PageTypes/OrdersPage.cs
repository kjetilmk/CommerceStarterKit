/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using EPiServer.DataAnnotations;
using OxxCommerceStarterKit.Core.Attributes;

namespace OxxCommerceStarterKit.Web.Models.PageTypes
{
	[ContentType(GUID = "3FE3BD9F-7101-49ED-B99D-AACF40459D56",
		DisplayName = "Orders Page",
		Description = "The page shows orders.",
        GroupName = WebGlobal.GroupNames.Commerce,
		AvailableInEditMode = false,
		Order = 100)]
    [SiteImageUrl(thumbnail: EditorThumbnail.Commerce)]
	public class OrdersPage : CommerceSampleModulePage
	{
	}
}
