using AutoMapper;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using Microsoft.AspNetCore.Html;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DFC.App.Pages.AutoMapperProfiles.ValuerConverters
{
    public class CmsPageConverter : IValueConverter<IList<Widget>?, HtmlString?>
    {
        private readonly Dictionary<int, string> columnWidthClasses = new Dictionary<int, string>
        {
            { 25, "govuk-grid-column-one-quarter" },
            { 33, "govuk-grid-column-one-third" },
            { 50, "govuk-grid-column-one-half" },
            { 66, "govuk-grid-column-two-thirds" },
            { 75, "govuk-grid-column-three-quarters" },
        };

        private readonly Dictionary<string, string> alignmentClasses = new Dictionary<string, string>
        {
            { "Left", "dfc-app-pages-alignment-left" },
            { "Right", "dfc-app-pages-alignment-right" },
            { "Centre", "dfc-app-pages-alignment-centre" },
            { "Justify", "dfc-app-pages-alignment-justify" },
        };

        public HtmlString? Convert(IList<Widget>? sourceMember, ResolutionContext context)
        {
            if (sourceMember == null || !sourceMember.Any())
            {
                return null;
            }

            var result = new StringBuilder();
            result.Append("<div class=\"govuk-grid-row\">");

            foreach (var contentItemModel in sourceMember.Where(ctr => ctr.HtmlBody != null || ctr.SharedContent != null || ctr.Metadata != null || ctr.FormContent != null))
            {
                var sizeClass = "govuk-grid-column-full";
                var alignmentClass = string.Empty;

                if (contentItemModel.Metadata.Size != 0 && columnWidthClasses.Keys.Contains(contentItemModel.Metadata.Size))
                {
                    sizeClass = columnWidthClasses[contentItemModel.Metadata.Size];
                }

                if (!string.IsNullOrWhiteSpace(contentItemModel.Metadata.Alignment) && alignmentClasses.Keys.Contains(contentItemModel.Metadata.Alignment))
                {
                    alignmentClass = alignmentClasses[contentItemModel.Metadata.Alignment];
                }

                result.Append($"<div class=\"{sizeClass}\">");

                if (!string.IsNullOrWhiteSpace(alignmentClass))
                {
                    result.Append($"<div class=\"{alignmentClass}\">");
                }

                result.Append(GetContentFromItem(contentItemModel));

                if (!string.IsNullOrWhiteSpace(alignmentClass))
                {
                    result.Append("</div>");
                }

                result.Append("</div>");
            }

            result.Append("</div>");

            return new HtmlString(result.ToString());
        }

        private static string GetContentFromItem(Widget model)
        {
            var content = new StringBuilder();

            if (model.HtmlBody != null)
            {
                content.Append(model.HtmlBody.Html);
            }
            else if (model.FormContent != null)
            {
                content.Append(model.FormContent);
            }
            else
            {
                foreach (var item in model.SharedContent.ContentItems)
                {
                    content.Append(item.Content.Html);
                }
            }

            return content.ToString();
        }
    }
}
