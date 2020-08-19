### Digital First Careers – Pages app

## Introduction

This project provides a Pages app for use in the Composite UI (Shell application) to dynamically output markup from Pages data sources.

Details of the Composite UI application may be found here https://github.com/SkillsFundingAgency/dfc-composite-shell

This Pages app returns:

* Pages documents

The Pages app also provisions the following for consumption by the Composite UI:

* Sitemap.xml for all Pages documents
* Robots.txt

## Getting Started

This is a self-contained Visual Studio 2019 solution containing a number of projects (web application, service and repository layers, with associated unit test and integration test projects).

### Installing

Clone the project and open the solution in Visual Studio 2019.

## List of dependencies

|Item	|Purpose|
|-------|-------|
|Azure Cosmos DB | Document storage |
|DFC.Compui.Cosmos|Content page storage in Cosmos|

## Local Config Files

Once you have cloned the public repo you need to remove the -template part from the configuration file names listed below.

| Location | Filename | Rename to |
|-------|-------|-------|
| DFC.App.Pages.IntegrationTests | appsettings-template.json | appsettings.json |
| DFC.App.Pages | appsettings-template.json | appsettings.json |

## Configuring to run locally

The project contains a number of "appsettings-template.json" and "local.settings-template.json" files which contain Pages appsettings for the web app, function app and the integration test projects. To use these files, copy them to "appsettings.json" and "local.settings.json" respectively and edit and replace the configuration item values with values suitable for your environment.

By default, the appsettings include local Azure Cosmos Emulator configurations using the well known configuration values for Content Page storage and. These may be changed to suit your environment if you are not using the Azure Cosmos Emulator.

This app using the FAM API for Postcode routing. To make use of it you will require an APIM API key for that service.

## Running locally

To run this product locally, you will need to configure the list of dependencies, once configured and the configuration files updated, it should be F5 to run and debug locally. The application can be run using IIS Express or full IIS.

To run the project, start the web application. Once running, browse to the main entry point which is the "https://localhost:44327/pages". This will list all of the Pages views and pages available and from here, you can navigate to the individual Pages pages.

The Pages app is designed to be run from within the Composite UI, therefore running the Pages app outside of the Composite UI will only show simple views of the data.

## Deployments

This Pages app will be deployed as an individual deployment for consumption by the Composite UI.

## Assets

CSS, JS, images and fonts used in this site can found in the following repository https://github.com/SkillsFundingAgency/dfc-digital-assets

## Built With

* Microsoft Visual Studio 2019
* .Net Core 3.1

## References

Please refer to https://github.com/SkillsFundingAgency/dfc-digital for additional instructions on configuring individual components like Cosmos.
