Feature: Exam Results


@Pages @Smoke
Scenario: Navigate to A-Level page
	Given I am on the exam results page
	When I click the Options after A level, vocational, technical or other level 3 results link
	Then I am taken to the A level page

@Pages @ExamResults
Scenario: Navigate to GCSE page
	Given I am on the exam results page
	When I click the Options after GCSE, vocational, technical or other level 2 results link
	Then I am taken to the GCSE page
