// <copyright file="ValidationSteps.cs" company="National Careers Service">
// Copyright (c) National Careers Service. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using DFC.App.Pages.Model;
using OpenQA.Selenium;
using System.Globalization;
using TechTalk.SpecFlow;
using DFC.TestAutomation.UI.Extension;

namespace DFC.App.Pages.UI.FunctionalTests
{
    [Binding]
    internal class ValidationSteps
    {
        public ValidationSteps(ScenarioContext context)
        {
            this.Context = context;
        }

        private ScenarioContext Context { get; set; }

        [Then(@"I am taken to the (.*) page")]
        public void ThenIAmTakenToThePage(string pageName)
        {
            this.Context.GetHelperLibrary<AppSettings>().WebDriverWaitHelper.WaitForPageToLoad();

            var page = this.Context.GetWebDriver().FindElement(By.TagName("title"));

            if (page.ToString().ToLower().Contains(pageName.ToLower()))
            {
                throw new NotFoundException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The expected page is not displayed");
            }
        }

        [Then(@"I am shown the results for (.*)")]
        public void ThenIAmShownTheResultsForTheOption(string option)
        {
            var result = this.Context.GetWebDriver().FindElement(By.Id("primaryFiltersSelectedValue")).GetAttribute("innerText").ToString();

            if (result != option.ToLower())
            {
                throw new NotFoundException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The expected result is not displayed");
            }
        }

        [Then(@"I am shown a result count of (.*)")]
        public void ThenIAmShownAResultCountOf(string count)
        {
            var result = this.Context.GetWebDriver().FindElement(By.Id("totalArticles")).GetAttribute("innerText").ToString();

            if (!result.StartsWith(count))
            {
                throw new NotFoundException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The expected result count is not displayed");
            }
        }
    }
}