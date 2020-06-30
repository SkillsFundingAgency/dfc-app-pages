using DFC.App.Pages.Data.Models;

namespace DFC.App.Pages.Data.Mappers
{
    public static class MapToPageApiContentItemModel
    {

        public static void Map(this PagesApiContentItemModel item, LinkDetails linkDetails)
        {
            if (item == null || linkDetails == null)
            {
                return;
            }

            item.Alignment = linkDetails.Alignment;
            item.Href = linkDetails.Href;
            item.Title = linkDetails.Title;
            item.ContentType = linkDetails.ContentType;
            item.Ordinal = linkDetails.Ordinal;
            item.Size = linkDetails.Size;
        }
    }
}
