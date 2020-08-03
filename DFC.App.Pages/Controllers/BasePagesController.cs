using DFC.App.Pages.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.Pages.Controllers
{
    public abstract class BasePagesController<TController> : Controller
        where TController : Controller
    {
        public const string RegistrationPath = "pages";
        public const string LocalPath = "pages";

        protected BasePagesController(ILogger<TController> logger)
        {
            Logger = logger;
        }

        protected ILogger<TController> Logger { get; private set; }
    }
}