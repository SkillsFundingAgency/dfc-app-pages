using Newtonsoft.Json;
using System;

namespace DFC.App.Pages.Data.Contracts
{
    public interface IApiDataModel
    {
        [JsonProperty("Uri")]
        Uri? Url { get; set; }
    }
}
