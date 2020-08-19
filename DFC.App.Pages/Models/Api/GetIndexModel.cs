using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DFC.App.Pages.Models.Api
{
    public class GetIndexModel
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public IList<string>? Locations { get; set; }
    }
}
