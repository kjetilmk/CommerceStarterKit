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
	[ContentType(GUID = "B64C58FB-5057-4E31-9C09-2F023DF9F5A2",
		DisplayName = "Personal",
		Description = "A page which shows the customers current addresses and other registered information.",
        GroupName = WebGlobal.GroupNames.Specialized,
		AvailableInEditMode = false,
		Order = 100)]
    [SiteImageUrl(thumbnail: EditorThumbnail.Social)]
	public class PersonalInformationPage : CommerceSampleModulePage
	{
	}
}
