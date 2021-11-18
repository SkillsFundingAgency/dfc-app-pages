// <copyright file="ExamResultsPage.cs" company="National Careers Service">
// Copyright (c) National Careers Service. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using DFC.App.Pages.Model;
using System;
using TechTalk.SpecFlow;
using DFC.TestAutomation.UI.Extension;

namespace DFC.App.Pages.UI.FunctionalTests.Pages
{
    internal class ExamResultsPage
    {
        public ExamResultsPage(ScenarioContext context)
        {
            this.Context = context;

            if (this.Context == null)
            {
                throw new NullReferenceException("The scenario context is null. The action plans landing page cannot be initialised.");
            }
        }

        private ScenarioContext Context { get; set; }

        public ExamResultsPage NavigateToExamResultsPage()
        {
            this.Context.GetWebDriver().Url = this.Context.GetSettingsLibrary<AppSettings>().AppSettings.AppBaseUrl.ToString() + "exam-results";
            return this;
        }
    }
}
