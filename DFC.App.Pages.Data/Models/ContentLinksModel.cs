using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.Pages.Data.Models
{
    public class ContentLinksModel
    {
        private const string ContentLinkName = "cont";

        private readonly JObject JLinks;

        public ContentLinksModel(JObject jLinks)
        {
            JLinks = jLinks;
        }

        public List<KeyValuePair<string, List<LinkDetails>>> ContentLinks
        {
            get => LinksPrivate ??= GetLinksFromJObject();

            set => LinksPrivate = value;
        }

        private List<KeyValuePair<string, List<LinkDetails>>>? LinksPrivate { get; set; }

        private static CuriesDetails? GetContentCuriesDetails(JObject links)
        {
            var curies = links["curies"]?.ToString();

            if (string.IsNullOrEmpty(curies))
            {
                return null;
            }

            var curiesList = JsonConvert.DeserializeObject<List<CuriesDetails>>(curies);

            return curiesList.FirstOrDefault(x => x.Name == ContentLinkName);
        }


        private static KeyValuePair<string, List<LinkDetails>> GetLinkDetailsFromArray(JToken array, string relationshipKey, string baseHref)
        {
            var links = JsonConvert.DeserializeObject<List<LinkDetails>>(array.ToString());

            foreach (var link in links)
            {
                link.Uri = new Uri($"{baseHref}{link.Href}");
            }

            return new KeyValuePair<string, List<LinkDetails>>(relationshipKey, links);
        }

        private List<KeyValuePair<string, List<LinkDetails>>> GetLinksFromJObject()
        {
            var contLink = new List<KeyValuePair<string, List<LinkDetails>>>();

            if (JLinks == null)
            {
                return contLink;
            }

            var contentCuriesDetails = GetContentCuriesDetails(JLinks);

            if (contentCuriesDetails == null)
            {
                return contLink;
            }

            foreach (var (key, jValue) in JLinks)
            {
                var value = jValue;

                if (value == null || !key.StartsWith(contentCuriesDetails.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }

                var relationShipKey = key.Replace(
                    $"{ContentLinkName}:",
                    string.Empty,
                    StringComparison.CurrentCultureIgnoreCase);

                if (jValue is JArray)
                {
                    contLink.Add(GetLinkDetailsFromArray(jValue, relationShipKey, contentCuriesDetails.Href));
                }
                else
                {
                    var child = JsonConvert.DeserializeObject<LinkDetails>(value.ToString());
                    child.Uri = new Uri($"{contentCuriesDetails.Href}{child.Href}");

                    contLink.Add(new KeyValuePair<string, List<LinkDetails>>(
                        relationShipKey,
                        new List<LinkDetails>
                        {
                            child,
                        }));
                }
            }

            return contLink;
        }
    }
}
