Feature: Careers Advice


@Pages @Smoke
Scenario: Navigate to a making career choice page
	Given I am on the careers advice page
	When I click the Post 16 options link
	Then I am taken to the Post 16 options page

@Pages @CareerAdvice
Scenario: Navigate to getting a job choice tab
	Given I am on the careers advice page
	When I click the getting a job tab
	When I click the How to find job vacancies link
	Then I am taken to the How to find job vacancies page

@Pages @CareerAdvice
Scenario: Navigate to progressing your career tab
	Given I am on the careers advice page
	When I click the progressing your career tab
	When I click the Develop your soft skills link
	Then I am taken to the Develop your soft skills page