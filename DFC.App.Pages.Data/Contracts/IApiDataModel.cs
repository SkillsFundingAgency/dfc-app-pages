using System;
using Newtonsoft.Json;

namespace DFC.App.Pages.Data.Contracts
{
    public interface IApiDataModel
    {
        [JsonProperty("Uri")]
        Uri? Url { get; set; }
    }
}
