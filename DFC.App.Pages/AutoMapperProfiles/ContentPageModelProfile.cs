using AutoMapper;
using DFC.App.Pages.AutoMapperProfiles.ValuerConverters;
using DFC.App.Pages.Controllers;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Models;
using DFC.App.Pages.ViewModels;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Pages.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class ContentPageModelProfile : Profile
    {
        public ContentPageModelProfile()
        {
            CreateMap<ContentPageModel, BodyViewModel>()
                .ForMember(d => d.Content, opt => opt.ConvertUsing(new ContentItemsConverter(), a => a.ContentItems));

            CreateMap<ContentPageModel, DocumentViewModel>()
                .ForMember(d => d.DocumentId, s => s.MapFrom(a => a.Id))
                .ForMember(d => d.HtmlHead, s => s.Ignore())
                .ForMember(d => d.Breadcrumb, s => s.Ignore())
                .ForMember(d => d.Content, opt => opt.ConvertUsing(new ContentItemsConverter(), a => a.ContentItems))
                .ForMember(d => d.BodyViewModel, s => s.MapFrom(a => a));

            CreateMap<ContentPageModel, HtmlHeadViewModel>()
                .ForMember(d => d.CanonicalUrl, s => s.Ignore())
                .ForMember(d => d.Title, s => s.MapFrom(a => a.MetaTags != null ? a.MetaTags.Title + " | " + PagesController.PagesPageTitleSuffix : PagesController.PagesPageTitleSuffix))
                .ForMember(d => d.Description, s => s.MapFrom(a => a.MetaTags != null ? a.MetaTags.Description : null))
                .ForMember(d => d.Keywords, s => s.MapFrom(a => a.MetaTags != null ? a.MetaTags.Keywords : null));

            CreateMap<ContentPageModel, IndexDocumentViewModel>();

            CreateMap<ContentPageModel, BreadcrumbItemModel>();

            CreateMap<PagesApiDataModel, ContentPageModel>()
                .ForMember(d => d.Id, s => s.MapFrom(a => a.ItemId))
                .ForMember(d => d.Etag, s => s.Ignore())
                .ForMember(d => d.PartitionKey, s => s.Ignore())
                .ForMember(d => d.TraceId, s => s.Ignore())
                .ForMember(d => d.ParentId, s => s.Ignore())
                .ForMember(d => d.Content, s => s.Ignore())
                .ForMember(d => d.RedirectLocations, s => s.MapFrom(a => a.Redirects))
                .ForPath(d => d.LastReviewed, s => s.MapFrom(a => a.ModifiedDate))
                .ForPath(d => d.MetaTags.Title, s => s.MapFrom(a => a.Title))
                .ForPath(d => d.MetaTags.Description, s => s.MapFrom(a => a.Description))
                .ForPath(d => d.MetaTags.Keywords, s => s.MapFrom(a => a.Keywords));

            CreateMap<PagesApiContentItemModel, ContentItemModel>()
                .ForMember(d => d.LastReviewed, s => s.MapFrom(a => a.ModifiedDate));

            CreateMap<PagesApiContentItemModel, SharedContentItemModel>()
                .ForMember(d => d.LastReviewed, s => s.MapFrom(a => a.ModifiedDate));

        }
    }
}
