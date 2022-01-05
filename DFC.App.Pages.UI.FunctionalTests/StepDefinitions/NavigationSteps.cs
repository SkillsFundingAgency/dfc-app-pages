// <copyright file="NavigationSteps.cs" company="National Careers Service">
// Copyright (c) National Careers Service. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using DFC.App.Pages.Model;
using DFC.App.Pages.UI.FunctionalTests.Pages;
using OpenQA.Selenium;
using System;
using System.Globalization;
using TechTalk.SpecFlow;
using TestAutomation.UI.Extension;

namespace DFC.App.Pages.UI.FunctionalTests.StepDefinitions
{
    [Binding]
    internal class NavigationSteps
    {
        public NavigationSteps(ScenarioContext context)
        {
            this.Context = context;
        }

        private ScenarioContext Context { get; set; }

        [Given(@"I am on the (.*) page")]
        public void GivenIAmOnThePage(string pageName)
        {
            var pageHeadingLocator = By.ClassName("govuk-heading-xl");

            switch (pageName.ToLower(CultureInfo.CurrentCulture))
            {
                case "home":
                    var homePage = new HomePage(this.Context);
                    homePage.NavigateToHomePage();
                    this.Context.GetHelperLibrary<AppSettings>().WebDriverWaitHelper.WaitForElementToContainText(pageHeadingLocator, "National Careers Service");
                    break;

                case "careers advice":
                    var careersAdvicePage = new CareersAdvicePage(this.Context);
                    careersAdvicePage.NavigateToCareersAdvicePage();
                    this.Context.GetHelperLibrary<AppSettings>().WebDriverWaitHelper.WaitForElementToContainText(pageHeadingLocator, "Careers advice");
                    break;

                case "exam results":
                    var examResultsPage = new ExamResultsPage(this.Context);
                    examResultsPage.NavigateToExamResultsPage();
                    pageHeadingLocator = By.ClassName("govuk-heading-l");
                    this.Context.GetHelperLibrary<AppSettings>().WebDriverWaitHelper.WaitForElementToContainText(pageHeadingLocator, "Make choices after your exams");
                    break;

                default:
                    throw new OperationCanceledException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The page name provided was not recognised.");
            }
        }

        [When(@"I click the getting a job tab")]
        public void WhenIClickTheGettingAJobTab()
        {
            this.Context.GetWebDriver().FindElement(By.Id("tab_getting-a-job")).Click();
        }

        [When(@"I click the progressing your career tab")]
        public void WhenIClickTheProgressingCareer()
        {
            this.Context.GetWebDriver().FindElement(By.Id("tab_progressing-your-career")).Click();
        }
    }
}
