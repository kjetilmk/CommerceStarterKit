/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Web;
using OxxCommerceStarterKit.Core.Attributes;

namespace OxxCommerceStarterKit.Web.Models.Blocks
{
    [ContentType(GUID = "064c7fb5-4cb5-4771-b0e1-b01a98cb01ec",
        DisplayName = "Banner",
        Description = "Banner block with image and overlay text and optional link",
        GroupName = WebGlobal.GroupNames.Default
        )]
    [SiteImageUrl(thumbnail: EditorThumbnail.Content)]
	public class BannerBlock : SiteBlockData
	{
		[Display(
			GroupName = SystemTabNames.Content,
			Order = 10)]
		[CultureSpecific]
		[UIHint(UIHint.MediaFile)]
		public virtual ContentReference Image { get; set; }

		[Display(
			GroupName = SystemTabNames.Content,
			Order = 20)]
		[CultureSpecific]
        [UIHint(EPiServer.Commerce.UIHint.AllContent)]
		public virtual ContentReference TargetLink { get; set; }

		[Display(
			GroupName = SystemTabNames.Content,
			Order = 30)]
		[CultureSpecific]
		public virtual string Title { get; set; }

		[Display(
			GroupName = SystemTabNames.Content,
			Order = 40)]
		[CultureSpecific]
		public virtual string Lead { get; set; }

		[Display(
			Name = "Text color",
            GroupName = SystemTabNames.Content,
			Order = 50)]
		[CultureSpecific]
		public virtual string TextColor { get; set; }

        [Display(
            Name = "Promotion ID",
            Description = "The name or code of the promotion to be used for analytics tracking. Example: 'SUMMER_SALE_2015'",
            GroupName = SystemTabNames.Content,
            Order = 100)]
        [CultureSpecific]
        public virtual string PromotionId { get; set; }

        [Display(
            Name = "Promotion Name",
            Description = "The display name of the promotion to be used for analytics tracking. Example: 'Summer Sale'",
            GroupName = SystemTabNames.Content,
            Order = 100)]
        [CultureSpecific]
        public virtual string PromotionName { get; set; }

        [Display(
            Name = "Promotion Banner Name",
            Description = "The display name of the promotion to be used for analytics tracking. Example: 'summer_banner_2",
            GroupName = SystemTabNames.Content,
            Order = 100)]
        [CultureSpecific]
        public virtual string PromotionBannerName { get; set; }


        //[Display(
        //    GroupName = SystemTabNames.Content,
        //    Order = 70)]
        //[Range(0, 100)]
        //[CultureSpecific]
        //public virtual int ImageTextBackgroundTransparency { get; set; }

	}
}
