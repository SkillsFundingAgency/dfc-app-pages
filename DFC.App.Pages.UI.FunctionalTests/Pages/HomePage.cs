// <copyright file="HomePage.cs" company="National Careers Service">
// Copyright (c) National Careers Service. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using DFC.App.Pages.Model;
using System;
using TechTalk.SpecFlow;
using TestAutomation.UI.Extension;

namespace DFC.App.Pages.UI.FunctionalTests.Pages
{
    internal class HomePage
    {
        public HomePage(ScenarioContext context)
        {
            this.Context = context;

            if (this.Context == null)
            {
                throw new NullReferenceException("The scenario context is null. The action plans landing page cannot be initialised.");
            }
        }

        private ScenarioContext Context { get; set; }

        public HomePage NavigateToHomePage()
        {
            this.Context.GetWebDriver().Url = this.Context.GetSettingsLibrary<AppSettings>().AppSettings.AppBaseUrl.ToString();
            return this;
        }
    }
}
