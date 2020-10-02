using AutoMapper;
using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using Microsoft.AspNetCore.Html;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DFC.App.Pages.AutoMapperProfiles.ValuerConverters
{
    public class MarkupContentConverter : IValueConverter<IList<ContentItemModel>?, HtmlString?>
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

        public HtmlString? Convert(IList<ContentItemModel>? sourceMember, ResolutionContext context)
        {
            if (sourceMember == null || !sourceMember.Any())
            {
                return null;
            }

            var result = new StringBuilder();
            foreach (var contentItemModel in sourceMember.OrderBy(o => o.Ordinal))
            {
                var sizeClass = "govuk-grid-column-full";
                var alignmentClass = string.Empty;

                if (contentItemModel.Size.HasValue && columnWidthClasses.Keys.Contains(contentItemModel.Size.Value))
                {
                    sizeClass = columnWidthClasses[contentItemModel.Size.Value];
                }

                if (!string.IsNullOrWhiteSpace(contentItemModel.Alignment) && alignmentClasses.Keys.Contains(contentItemModel.Alignment))
                {
                    alignmentClass = alignmentClasses[contentItemModel.Alignment];
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

            return new HtmlString(result.ToString());
        }

        private static string GetContentFromItem(ContentItemModel model)
        {
            var content = new StringBuilder();
            content.Append(string.IsNullOrEmpty(model.Content) ? model.HtmlBody : model.Content);

            if (model.ContentItems != null && model.ContentItems.Any())
            {
                foreach (var item in model.ContentItems.OrderBy(x => x.Ordinal))
                {
                    content.Append(item.Content);
                }
            }

            return content.ToString();
        }
    }
}
