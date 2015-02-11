/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.ComponentModel.DataAnnotations;
using EPiServer;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using OxxCommerceStarterKit.Core.Attributes;

namespace OxxCommerceStarterKit.Web.Models.Blocks
{
	[ContentType(DisplayName = "Footer button with help link",
        GUID = "889f8753-0548-4ffa-90d0-e89286f95df4")]
    [SiteImageUrl]
	public class ButtonWithHelpLinkBlock : SiteBlockData
	{
		[Display(Order = 10, GroupName = SystemTabNames.Content, Name = "Text on the button")]
		[Searchable(false)]
		public virtual string Text { get; set; }

		[Display(Order = 20, GroupName = SystemTabNames.Content, Name = "Link for the button")]
		[Searchable(false)]
		public virtual Url Link { get; set; }

		[Display(Order = 30, GroupName = SystemTabNames.Content, Name = "Link for help button")]
		[Searchable(false)]
		public virtual Url HelpLink { get; set; }
	}
}
