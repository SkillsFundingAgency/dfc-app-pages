using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.App.Pages.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class HttpRequestExtensions
    {
        public static Uri? GetBaseAddress(this HttpRequest request, IUrlHelper? urlHelper = null)
        {
            if (request != null)
            {
                if (request.Headers.TryGetValue("x-forwarded-proto", out var forwardedProtocol)
                    && request.Headers.TryGetValue("x-original-host", out var originalHost))
                {
                    if (forwardedProtocol.ToString().Contains(","))
                        forwardedProtocol = forwardedProtocol.ToString().Substring(0, forwardedProtocol.ToString().IndexOf(","));
                    if (originalHost.ToString().Contains(","))
                        originalHost = originalHost.ToString().Substring(0, originalHost.ToString().IndexOf(","));
                    return new Uri($"{forwardedProtocol}://{originalHost}");
                }

                return string.IsNullOrWhiteSpace(request.Scheme) ? default : new Uri($"{request.Scheme}://{request.Host}{urlHelper?.Content("~")}");
            }
            return default;
        }
    }
}