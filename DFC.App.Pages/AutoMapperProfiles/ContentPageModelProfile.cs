using AutoMapper;
using DFC.App.Pages.AutoMapperProfiles.ValuerConverters;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Models.Api;
using DFC.App.Pages.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using Microsoft.AspNetCore.Html;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.App.Pages.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class ContentPageModelProfile : Profile
    {
        private const string NcsPageTitle = "National Careers Service";

        public ContentPageModelProfile()
        {
            CreateMap<BreadcrumbItemModel, BreadcrumbItemViewModel>()
                .ForMember(d => d.AddHyperlink, s => s.Ignore());

            CreateMap<PageApi, GetIndexModel>()
                .ForMember(d => d.Id, s => s.MapFrom(a => a.GraphSync.NodeId.Substring(a.GraphSync.NodeId.LastIndexOf('/') + 1)))
                .ForMember(d => d.Locations, s => s.MapFrom(a => a.PageLocation.FullUrl.Split(
                    ',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList()));

            CreateMap<Page, HeadViewModel>()
                .ForMember(d => d.CanonicalUrl, s => s.Ignore())
                .ForMember(d => d.Title, s => s.MapFrom(a => a.DisplayText != null && !string.IsNullOrWhiteSpace(a.DisplayText) ? a.DisplayText + " | " + NcsPageTitle : NcsPageTitle))
                .ForMember(d => d.Description, s => s.MapFrom(a => a.Description != null ? a.Description : null))
                .ForMember(d => d.Keywords, s => s.Ignore());

            CreateMap<Page, HeroBannerViewModel>()
                 .ForMember(d => d.Content, s => s.MapFrom(a => new HtmlString(a.Herobanner.Html)));

            CreateMap<Page, BodyViewModel>()
               .ForMember(d => d.Content, opt => opt.ConvertUsing(new CmsPageConverter(), a => a.Flow.Widgets));

            CreateMap<PageUrl, IndexDocumentViewModel>()
                .ForMember(d => d.CanonicalName, s => s.MapFrom(a => a.PageLocation.UrlName ?? string.Empty))
                .ForMember(d => d.PageLocation, s => s.MapFrom(a => (a.Breadcrumb.TermContentItems.FirstOrDefault().DisplayText == "/" ? "/" : $"/{a.Breadcrumb.TermContentItems.FirstOrDefault().DisplayText}")))
                .ForMember(d => d.IsDefaultForPageLocation, s => s.MapFrom(a => a.PageLocation.DefaultPageForLocation));

            CreateMap<Page, DocumentViewModel>()
                .ForMember(d => d.DocumentId, s => s.Ignore())
                .ForMember(d => d.Redirects, s => s.MapFrom(a => a.PageLocation.RedirectLocations))
                .ForMember(d => d.Head, s => s.MapFrom(a => a))
                .ForMember(d => d.Breadcrumb, s => s.Ignore())
                .ForMember(d => d.Content, opt => opt.ConvertUsing(new CmsPageConverter(), a => a.Flow.Widgets))
                .ForMember(d => d.BodyViewModel, s => s.MapFrom(a => a))
                .ForMember(d => d.HeroBannerViewModel, s => s.MapFrom(a => a))
                .ForMember(d => d.CanonicalName, s => s.Ignore())
                .ForMember(d => d.PartitionKey, s => s.Ignore())
                .ForMember(d => d.Version, s => s.Ignore())
                .ForMember(d => d.IsDefaultForPageLocation, s => s.Ignore())
                .ForMember(d => d.IncludeInSitemap, s => s.Ignore())
                .ForMember(d => d.SiteMapPriority, s => s.Ignore())
                .ForMember(d => d.SiteMapChangeFrequency, s => s.Ignore())
                .ForMember(d => d.Url, s => s.Ignore())
                .ForMember(d => d.LastReviewed, s => s.Ignore())
                .ForMember(d => d.CreatedDate, s => s.Ignore())
                .ForMember(d => d.LastCached, s => s.Ignore());
        }
    }
}
