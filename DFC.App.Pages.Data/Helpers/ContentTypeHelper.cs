using DFC.App.Pages.Data.Enums;
using System;
using System.IO;

namespace DFC.App.Pages.Data.Helpers
{
    public static class ContentTypeHelper
    {
        public static EventContentType GetContentTypeFromUrl(this Uri url)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            var urlAsString = url.ToString();

            if (urlAsString.Contains("pagelocation", StringComparison.OrdinalIgnoreCase))
            {
                return EventContentType.PageLocation;
            }

            if (urlAsString.Contains("page", StringComparison.OrdinalIgnoreCase))
            {
                return EventContentType.Page;
            }

            if (urlAsString.Contains("sharedcontent", StringComparison.OrdinalIgnoreCase))
            {
                return EventContentType.SharedContent;
            }

            throw new InvalidDataException($"URL: {url} doesn't contain a supported {nameof(EventContentType)}");
        }
    }
}
