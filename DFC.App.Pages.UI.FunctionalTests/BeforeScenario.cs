// <copyright file="BeforeScenario.cs" company="National Careers Service">
// Copyright (c) National Careers Service. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using DFC.App.Pages.Model;
using System;
using TechTalk.SpecFlow;
using TestAutomation.UI;
using TestAutomation.UI.Extension;
using TestAutomation.UI.Helper;
using TestAutomation.UI.Settings;
using TestAutomation.UI.Support;

namespace DFC.App.Pages
{
    [Binding]
    public class BeforeScenario
    {
        public BeforeScenario(ScenarioContext context)
        {
            this.Context = context;

            if (this.Context == null)
            {
                throw new NullReferenceException($"The scenario context is null. The {this.GetType().Name} class cannot be initialised.");
            }
        }

        private ScenarioContext Context { get; set; }

        [BeforeScenario(Order = 0)]
        public void SetObjectContext(ObjectContext objectContext)
        {
            this.Context.SetObjectContext(objectContext);
        }

        [BeforeScenario(Order = 1)]
        public void SetSettingsLibrary()
        {
            this.Context.SetSettingsLibrary(new SettingsLibrary<AppSettings>());
        }

        [BeforeScenario(Order = 2)]
        public void SetApplicationUrl()
        {
            string appBaseUrl = this.Context.GetSettingsLibrary<AppSettings>().AppSettings.AppBaseUrl.ToString();

            // this.Context.GetSettingsLibrary<AppSettings>().AppSettings.AppBaseUrl = new Uri($"{appBaseUrl}home");
        }

        [BeforeScenario(Order = 3)]
        public void ConfigureBrowserStack()
        {
            this.Context.GetSettingsLibrary<AppSettings>().BrowserStackSettings.Name = this.Context.ScenarioInfo.Title;
            this.Context.GetSettingsLibrary<AppSettings>().BrowserStackSettings.Build = "Pages";
        }

        [BeforeScenario(Order = 4)]
        public void SetupWebDriver()
        {
            var settingsLibrary = this.Context.GetSettingsLibrary<AppSettings>();
            var webDriver = new WebDriverSupport<AppSettings>(settingsLibrary).Create();
            webDriver.Manage().Window.Maximize();
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(settingsLibrary.TestExecutionSettings.TimeoutSettings.PageNavigation);
            this.Context.SetWebDriver(webDriver);
        }

        [BeforeScenario(Order = 5)]
        public void SetUpHelpers()
        {
            var helperLibrary = new HelperLibrary<AppSettings>(this.Context.GetWebDriver(), this.Context.GetSettingsLibrary<AppSettings>());
            this.Context.SetHelperLibrary(helperLibrary);
        }
    }
}
