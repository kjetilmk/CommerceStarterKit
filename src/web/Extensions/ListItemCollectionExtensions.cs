/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using EPiServer;
using EPiServer.Core;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace OxxCommerceStarterKit.Web.Extensions
{
	public static class LinkItemCollectionExtension
	{
		/// <summary>
		/// Converts a LinkItemCollection to typed pages. Any non-pages will be filtered out. (Not compatible with PageList - Use ToPageDataList)
		/// </summary>
		/// <typeparam name="T">PageType</typeparam>
		/// <param name="linkItemCollection">The collection of links to convert</param>
		/// <returns>An enumerable of typed PageData</returns>
		public static IEnumerable<T> ToPages<T>(this LinkItemCollection linkItemCollection) where T : PageData
		{
            foreach (PageData page in ToContent<PageData>(linkItemCollection))
		    {
                if (IsPageAccessible(page))
                {
                    yield return (T)page;
                }
		    }
		}

		/// <summary>
		/// Converts a LinkItemCollection to typed pages. Any non-pages will be filtered out. (Not compatible with PageList - Use ToPageDataList)
		/// </summary>
		/// <typeparam name="T">PageType</typeparam>
		/// <param name="linkItemCollection">The collection of links to convert</param>
		/// <returns>An enumerable of typed PageData</returns>
        public static IEnumerable<T> ToMedia<T>(this LinkItemCollection linkItemCollection) where T : IContentData
		{
		    return ToContent<T>(linkItemCollection);
		}

        /// <summary>
        /// Converts a LinkItemCollection to typed content. Any non-pages will be filtered out. 
        /// </summary>
        /// <typeparam name="T">Content Type</typeparam>
        /// <param name="linkItemCollection">The collection of links to convert</param>
        /// <returns>An enumerable of typed ContentData</returns>
        public static IEnumerable<T> ToContent<T>(this LinkItemCollection linkItemCollection) where T : IContentData
        {
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();

            if (linkItemCollection != null)
            {
                foreach (var linkItem in linkItemCollection)
                {
                    var url = new UrlBuilder(linkItem.Href);
                    var content = UrlResolver.Current.Route(url);
                    if (content is T)
                    {
                        yield return (T)content;
                    }
                }
            }
        }


		/// <summary>
		/// Converts a LinkItemCollection to typed pages. Any non-pages will be filtered out. (PageList compatible)
		/// </summary>
		/// <typeparam name="T">PageType</typeparam>
		/// <param name="linkItemCollection"></param>
		/// <returns>A list of typed PageData</returns>
		public static List<T> ToPageDataList<T>(this LinkItemCollection linkItemCollection) where T : PageData
		{
			return linkItemCollection.ToPages<T>().ToList();
		}

		/// <summary>
		/// Converts a LinkItemCollection to a list of pages. Any non-pages will be filtered out. (PageList compatible)
		/// </summary>
		/// <param name="linkItemCollection"></param>
		/// <returns>A list of typed PageData</returns>
		public static List<PageData> ToPageDataList(this LinkItemCollection linkItemCollection)
		{
			return linkItemCollection.ToPages<PageData>().ToList();
		}

		private static bool IsPageAccessible(PageData page)
		{
			return (page != null &&
				!page.IsDeleted &&
				page.Status == VersionStatus.Published &&
				page.PendingPublish == false &&
				page.StartPublish < DateTime.Now &&
				page.StopPublish > DateTime.Now &&
				page.ACL.QueryDistinctAccess(AccessLevel.Read));
		}

		// TODO: Possibly a better solution w/o try catch?
		private static bool IsFileAccessible(string filePath)
		{
			try
			{
				HostingEnvironment.VirtualPathProvider.GetFile(filePath);
				return true;
			}
			catch { }
			return false;
		}
	}
}
