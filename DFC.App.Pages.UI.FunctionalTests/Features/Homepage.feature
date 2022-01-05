Feature: Homepage


@Pages @Smoke
Scenario: Footer - Check Help link
	Given I am on the home page
	When I click the Help link in the footer
	Then I am taken to the Help page

@Pages @Homepage
Scenario: Footer - Check Information Sources link
	Given I am on the home page
	When I click the Information Sources link in the footer
	Then I am taken to the Information Sources page

@Pages @Homepage
Scenario: Footer - Check Privacy & Cookies link
	Given I am on the home page
	When I click the Privacy and cookies link in the footer
	Then I am taken to the Privacy and cookies page

@Pages @Homepage
Scenario: Footer - Check T&C link
	Given I am on the home page
	When I click the Terms and conditions link in the footer
	Then I am taken to the Terms and conditions page