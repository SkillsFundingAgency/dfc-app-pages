using AutoMapper;
using DFC.App.Pages.AutoMapperProfiles.ValuerConverters;
using DFC.App.Pages.Data.Common;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Models.Api;
using DFC.App.Pages.ViewModels;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.App.Pages.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class ContentPageModelProfile : Profile
    {
        public ContentPageModelProfile()
        {
            CreateMap<ContentPageModel, BodyViewModel>()
                .ForMember(d => d.Content, opt => opt.ConvertUsing(new MarkupContentConverter(), a => a.ContentItems.Where(w => !w.ContentType!.Equals(Constants.ContentTypePageLocation, System.StringComparison.OrdinalIgnoreCase)).ToList()));

            CreateMap<ContentPageModel, DocumentViewModel>()
                .ForMember(d => d.DocumentId, s => s.MapFrom(a => a.Id))
                .ForMember(d => d.Redirects, s => s.MapFrom(a => a.RedirectLocations))
                .ForMember(d => d.HtmlHead, s => s.MapFrom(a => a))
                .ForMember(d => d.Breadcrumb, s => s.Ignore())
                .ForMember(d => d.Content, opt => opt.ConvertUsing(new MarkupContentConverter(), a => a.ContentItems.Where(w => !w.ContentType!.Equals(Constants.ContentTypePageLocation, System.StringComparison.OrdinalIgnoreCase)).ToList()))
                .ForMember(d => d.BodyViewModel, s => s.MapFrom(a => a));

            CreateMap<ContentPageModel, HtmlHeadViewModel>()
                .ForMember(d => d.CanonicalUrl, s => s.Ignore())
                .ForMember(d => d.Title, s => s.MapFrom(a => a.MetaTags != null && !string.IsNullOrWhiteSpace(a.MetaTags.Title) ? a.MetaTags.Title + " | Pages | National Careers Service" : "National Careers Service"))
                .ForMember(d => d.Description, s => s.MapFrom(a => a.MetaTags != null ? a.MetaTags.Description : null))
                .ForMember(d => d.Keywords, s => s.MapFrom(a => a.MetaTags != null ? a.MetaTags.Keywords : null));

            CreateMap<ContentPageModel, IndexDocumentViewModel>();

            CreateMap<ContentPageModel, GetIndexModel>()
                .ForMember(d => d.Locations, opt => opt.ConvertUsing(new LocationsConverter(), a => a));

            CreateMap<ContentPageModel, BreadcrumbViewModel>()
                .ForMember(d => d.Breadcrumbs, opt => opt.ConvertUsing(new BreadcrumbConverter(), a => a));

            CreateMap<BreadcrumbItemModel, BreadcrumbItemViewModel>()
                .ForMember(d => d.AddHyperlink, s => s.Ignore());

            CreateMap<PagesApiDataModel, ContentPageModel>()
                .ForMember(d => d.Id, s => s.MapFrom(a => a.ItemId))
                .ForMember(d => d.CanonicalName, opt => opt.ConvertUsing(new CanonicalNameConverter(), a => a))
                .ForMember(d => d.RedirectLocations, opt => opt.ConvertUsing(new CleanStringListConverter(), a => a.RedirectLocations))
                .ForMember(d => d.PageLocation, s => s.Ignore())
                .ForMember(d => d.Etag, s => s.Ignore())
                .ForMember(d => d.PartitionKey, s => s.Ignore())
                .ForMember(d => d.TraceId, s => s.Ignore())
                .ForMember(d => d.ParentId, s => s.Ignore())
                .ForMember(d => d.Content, s => s.Ignore())
                .ForMember(d => d.LastCached, s => s.Ignore())
                .ForMember(d => d.AllContentItemIds, s => s.Ignore())
                .ForMember(d => d.SiteMapPriority, s => s.MapFrom(a => a.SiteMapPriority / 10))
                .ForMember(d => d.SiteMapPriority, s => s.MapFrom(a => a.SiteMapPriority / 10))
                .ForMember(d => d.ContentItems, s => s.MapFrom(a => a.ContentItems != null && a.ContentItems.Any() ? a.ContentItems : null))
                .ForMember(d => d.LastReviewed, s => s.MapFrom(a => a.Published))
                .ForPath(d => d.MetaTags.Title, s => s.MapFrom(a => a.Title))
                .ForPath(d => d.MetaTags.Description, s => s.MapFrom(a => a.Description))
                .ForPath(d => d.MetaTags.Keywords, s => s.MapFrom(a => a.Keywords));

            CreateMap<PagesApiContentItemModel, ContentItemModel>()
                .ForMember(d => d.Title, s => s.MapFrom(a => !a.ContentType!.Equals(Constants.ContentTypePageLocation, System.StringComparison.OrdinalIgnoreCase) ? a.Title : null))
                .ForMember(d => d.BreadcrumbLinkSegment, s => s.MapFrom(a => a.ContentType!.Equals(Constants.ContentTypePageLocation, System.StringComparison.OrdinalIgnoreCase) ? a.Title : null))
                .ForMember(d => d.LastReviewed, s => s.MapFrom(a => a.Published))
                .ForMember(d => d.ContentItems, s => s.MapFrom(a => a.ContentItems != null && a.ContentItems.Any() ? a.ContentItems : null))
                .ForMember(d => d.LastCached, s => s.Ignore());

            CreateMap<LinkDetailModel, PagesApiContentItemModel>()
                .ForMember(d => d.Url, s => s.Ignore())
                .ForMember(d => d.ItemId, s => s.Ignore())
                .ForMember(d => d.Title, s => s.Ignore())
                .ForMember(d => d.BreadcrumbText, s => s.Ignore())
                .ForMember(d => d.Content, s => s.Ignore())
                .ForMember(d => d.Justify, s => s.Ignore())
                .ForMember(d => d.Published, s => s.Ignore())
                .ForMember(d => d.CreatedDate, s => s.Ignore())
                .ForMember(d => d.HtmlBody, s => s.Ignore())
                .ForMember(d => d.Links, s => s.Ignore())
                .ForMember(d => d.ContentLinks, s => s.Ignore())
                .ForMember(d => d.ContentItems, s => s.Ignore());
        }
    }
}
