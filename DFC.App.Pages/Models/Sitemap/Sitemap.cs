using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace DFC.App.Pages.Models
{
    [XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class Sitemap
    {
        private readonly ArrayList map;

        public Sitemap()
        {
            map = new ArrayList();
        }

        [XmlIgnore]
        public IEnumerable<SitemapLocation>? Mappings => map as IEnumerable<SitemapLocation>;

        [XmlElement("url")]
        public SitemapLocation[] Locations
        {
            get
            {
                SitemapLocation[] items = new SitemapLocation[map.Count];
                map.CopyTo(items);

                return items;
            }

            set
            {
                if (value == null)
                {
                    return;
                }

                SitemapLocation[] items = value;
                map.Clear();

                foreach (SitemapLocation item in items)
                {
                    map.Add(item);
                }
            }
        }

        public int Add(SitemapLocation item)
        {
            return map.Add(item);
        }

        public void AddRange(IEnumerable<SitemapLocation> locs)
        {
            if (locs == null)
            {
                throw new ArgumentNullException(nameof(locs));
            }

            foreach (var i in locs)
            {
                map.Add(i);
            }
        }

        public void WriteSitemapToFile(string path)
        {
            using var fs = new FileStream(path, FileMode.Create);
            var ns = new XmlSerializerNamespaces();
            ns.Add("image", "http://www.google.com/schemas/sitemap-image/1.1");

            var xs = new XmlSerializer(typeof(Sitemap));
            xs.Serialize(fs, this, ns);
        }

        public string WriteSitemapToString()
        {
            using var sw = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("image", "http://www.google.com/schemas/sitemap-image/1.1");

            var xs = new XmlSerializer(typeof(Sitemap));
            xs.Serialize(sw, this, ns);
            return sw.GetStringBuilder().ToString();
        }
    }
}
