/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Web;
using OxxCommerceStarterKit.Core.Attributes;
using OxxCommerceStarterKit.Web.EditorDescriptors;
using OxxCommerceStarterKit.Web.Models.CustomProperties;

namespace OxxCommerceStarterKit.Web.Models.PageTypes
{
	[ContentType(GUID = "351c7ccc-422d-4db7-b021-390e00c975d0",
	 DisplayName = "Article with sidebar",
	 Description = "An article page with a content area to the right",
     GroupName = WebGlobal.GroupNames.Default,
	 Order = 100)]
    [SiteImageUrl(thumbnail: EditorThumbnail.Content)]
	public class ArticleWithSidebarPage : ArticlePage
	{
		
		[Display(
		Name = "Sidebar content area",
		GroupName = SystemTabNames.Content,
		Order = 55)]
        [CultureSpecific]
		public virtual ContentArea SidebarContent { get; set; }

	}
}
