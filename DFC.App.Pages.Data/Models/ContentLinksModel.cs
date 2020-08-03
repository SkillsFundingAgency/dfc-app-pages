using DFC.App.Pages.Data.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.Pages.Data.Models
{
    public class ContentLinksModel
    {
        private readonly JObject jLinks;

        public ContentLinksModel(JObject jLinks)
        {
            this.jLinks = jLinks;
        }

        public List<KeyValuePair<string, List<LinkDetails>>> ContentLinks
        {
            get => LinksPrivate ??= GetLinksFromJObject();

            set => LinksPrivate = value;
        }

        public bool ExcludePageLocation { get; set; } = false;

        private List<KeyValuePair<string, List<LinkDetails>>>? LinksPrivate { get; set; }

        private static CuriesDetails? GetContentCuriesDetails(JObject links)
        {
            var curies = links["curies"]?.ToString();

            if (string.IsNullOrEmpty(curies))
            {
                return null;
            }

            var curiesList = JsonConvert.DeserializeObject<List<CuriesDetails>>(curies);

            return curiesList.FirstOrDefault();
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

            if (jLinks == null)
            {
                return contLink;
            }

            var contentCuriesDetails = GetContentCuriesDetails(jLinks);

            if (contentCuriesDetails == null)
            {
                return contLink;
            }

            foreach (var (key, jValue) in jLinks)
            {
                var value = jValue;

                if (value == null || !key.StartsWith(contentCuriesDetails.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }

                var relationShipKey = key.Replace(
                    $"{contentCuriesDetails.Name}:",
                    string.Empty,
                    StringComparison.CurrentCultureIgnoreCase);

                Enum.TryParse(typeof(ContentRelationship), relationShipKey, true, out var type);

                if (type == null || (ContentRelationship)type == ContentRelationship.Undefined || ((ContentRelationship)type == ContentRelationship.HasPageLocation && ExcludePageLocation))
                {
                    continue;
                }

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
