# Project Configuration - Environment variables

This project is configured  with Azure Functions and integrations with Business Central API and Microsoft Identity Platform.

## Configuration Overview

The Azure Function  Environment variables is used to configure Azure Functions, including storage, authentication, and Business Central API settings. Below is an explanation of the key configurations.

### Azure  Authentication for Business Central Environment 

These settings are used for authentication with Azure Active Directory.

- **AzureAd:Instance**: 
  - The instance for Azure AD login: `https://login.microsoftonline.com/`.
  
- **AzureAd:TenantId**: 
  - Tenant ID for the Azure Active Directory instance: (to be filled).
  
- **AzureAd:ClientId**: 
  - The Client ID for your Azure AD app (to be filled).
  
- **AzureAd:ClientCredentials[0]:SourceType**: 
  - The type of client credential used, set to `ClientSecret`.
  
- **AzureAd:ClientCredentials[0]:ClientSecret**: 
  - The Client Secret for authentication (to be filled).

### Business Central API (BCAPI) Configuration

These settings define the integration with the Business Central API.

- **BCAPI:BaseUrl**: 
  - Base URL for the Business Central API: `https://api.businesscentral.dynamics.com`.
  
- **BCAPI:Scopes[0]**: 
  - Scope required for accessing the API: `https://api.businesscentral.dynamics.com/.default`.
  
- **BCAPI:RelativePath**: 
  - Relative path for custom BigQuery API: `/v2.0/UAT-2024-05-30/api/fusion5/bigQuery/v1.0`.
  
- **BCAPI:StdRelativePath**: 
  - Standard API relative path: `/v2.0/UAT-2024-05-30/api/v2.0`.

### OpenAPI Documentation

The project uses OpenAPI for API documentation. These settings define the OpenAPI specifications.

- **OpenApi__Version**: 
  - Version of OpenAPI specification: `v3`.
  
- **OpenApi__DocVersion**: 
  - Documentation version: `1.0.0`.
  
- **OpenApi__DocTitle**: 
  - API documentation title: `Swagger Petstore`.
  
- **OpenApi__DocDescription**: 
  - Description for the API documentation: `This is a sample server Petstore API designed by [http://swagger.io](http://swagger.io).`.
  
- **OpenApi__ApiKey**: 
  - Example API key for the Petstore sample API: `123`.

### BigQuery API Endpoints

Custom API paths for accessing Business Central data using BigQuery.

- **BigQueryCustomerRelationships**: 
  - Endpoint to get customer relationships: `/v2.0/UAT-2024-05-30/api/fusion5/bigQuery/v1.0/companies({companyId})/customerRelationships`.
  
- **BigQueryCustomers**: 
  - Endpoint to get customer data: `/v2.0/UAT-2024-05-30/api/fusion5/bigQuery/v2.0/companies({companyId})/customers`.
  
- **BigQueryCPInvoices**: 
  - Endpoint to get sales invoice data: `/v2.0/UAT-2024-05-30/api/fusion5/bigQuery/v1.0/companies({companyId})/salesInvoices`.


### General Settings for Local development 

- **AzureWebJobsStorage**: 
  - `UseDevelopmentStorage=true`: Indicates the project is using local development storage (Azurite or equivalent) for testing.
  
- **FUNCTIONS_WORKER_RUNTIME**: 
  - `dotnet-isolated`: Spec

## Notes

- Be sure to fill in the **ClientId** and **ClientSecret** for  `AzureAd`  before deploying to production.
- The values provided here are for local development only. Replace the necessary values for production environments.






## Project Deployment 

THERE IS NO CICD PIPELINE FOR THE PROJECT AS IT WAS NEVER ESTIMATE. PROJECT NEEDS TO BE DEPLOYED MANUALLY. 


