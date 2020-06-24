using AutoMapper;
using DFC.App.Pages.Data.Models;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Pages.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class EventGridSubscriptionProfile : Profile
    {
        public EventGridSubscriptionProfile()
        {
            CreateMap<EventGridSubscriptionClientOptions, EventGridSubscriptionModel>()
                .ForMember(d => d.Id, s => s.Ignore());
        }
    }
}
