// <copyright file="FormSteps.cs" company="National Careers Service">
// Copyright (c) National Careers Service. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using DFC.App.Pages.Model;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using DFC.TestAutomation.UI.Extension;

namespace DFC.App.Pages.UI.FunctionalTests.StepDefinitions
{
    [Binding]
    internal class FormSteps
    {
        public FormSteps(ScenarioContext context)
        {
            this.Context = context;
        }

        private ScenarioContext Context { get; set; }

        [When(@"I select the radio button option (.*)")]
        public void WhenISelectTheRadioButtonOption(string radioButtonLabel)
        {
            if (!this.InteractWithRadioButtonOrCheckbox(radioButtonLabel))
            {
                throw new NotFoundException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The label could not be found.");
            }
        }

        [When(@"I click the checkbox option (.*)")]
        public void WhenIClickTheCheckboxOption(string checkboxLabel)
        {
            if (!this.InteractWithRadioButtonOrCheckbox(checkboxLabel))
            {
                throw new NotFoundException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The label could not be found.");
            }
        }

        private bool InteractWithRadioButtonOrCheckbox(string inputLabelText)
        {
            var allLabels = this.Context.GetWebDriver().FindElements(By.TagName("label"));
            foreach (var label in allLabels)
            {
                if (label.Text.Trim().Equals(inputLabelText, System.StringComparison.OrdinalIgnoreCase))
                {
                    var parentNode = this.Context.GetHelperLibrary<AppSettings>().JavaScriptHelper.GetParentElement(label);
                    var input = parentNode.FindElement(By.TagName("input"));
                    input.Click();
                    return true;
                }
            }

            return false;
        }
    }
}
