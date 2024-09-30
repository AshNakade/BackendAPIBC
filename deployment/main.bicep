metadata name ='API Integration Pattern '
metadata description ='This pattern is used to deploy an API integration pattern'
metadata type ='API Integration Pattern'
metadata category ='Integration'

param tags object = {} 
@description('Storage Account type')
@allowed([
  'Standard_LRS'
  'Standard_GRS'
  'Standard_RAGRS'
  'Standard_ZRS'
])
param storageAccountType string
// @description('Location for all resources.')
// param location string = resourceGroup().location
param functionAppName  string
param hostingPlanName string  
param appServicePlanSku object
param applicationInsightsName string  
param logAnalyticsName string
param storageAccountName string
param managedIdentityName string
param keyVaultName string
@secure()
param keyVaultsecrets object
param appServicePlanKind  string
@allowed([
  'functionapp' // function app windows os
  'functionapp,linux' // function app linux os
  'functionapp,workflowapp' // logic app workflow
  'functionapp,workflowapp,linux' // logic app docker container
  'app,linux' // linux web app
  'app' // normal web app
])
param functionAppkind string
param functionsiteConfig object
param functionAppsiteConfig object

param authSettingV2Configuration object


//Create a managed identity 
 module managedIdentity 'br/public:avm/res/managed-identity/user-assigned-identity:0.2.1'= {
    name: managedIdentityName
   params: {
     name: managedIdentityName
     tags: tags
   }
 } 
// create key vault

var KetValuetRoleAssignemt = {roleAssignments:  [

  {
    roleDefinitionIdOrName: 'Key Vault Secrets User'
    principalId: managedIdentity.outputs.principalId
    description: 'Allows the function app to access the key vault'
    principalType: 'ServicePrincipal'
  }

]}
var tempKeySecrates = union(keyVaultsecrets , KetValuetRoleAssignemt)

var mergedkeyVaultsecret ={
  secureList :  [tempKeySecrates ]

}

module keyVault 'br/public:avm/res/key-vault/vault:0.4.0' = {
  name: 'DeployKeyVault'
  params: {
    name: keyVaultName
    secrets : mergedkeyVaultsecret
    sku: 'standard'
    tags: tags
  }
}



// Create Log Analytics Workspace
module logAnalytics 'br/public:avm/res/operational-insights/workspace:0.3.4' = {
  name: 'DeployLogAnalytics'
  params: {
    name: logAnalyticsName
    tags: tags
  }
}

// Create Application Insights
module applicationInsights 'br/public:avm/res/insights/component:0.3.0' = {
  name: 'DeployAppInsights'
  params: {
    name: applicationInsightsName
    workspaceResourceId: logAnalytics.outputs.resourceId
    applicationType: 'web'
    tags: tags
  }
}


//create storage account
module storageAccount 'br/public:avm/res/storage/storage-account:0.8.2'= {
  name: 'DeployStorageAccount'
  params: {
    name: storageAccountName
    skuName: storageAccountType
    managedIdentities : {
      systemAssigned: false 
      userAssignedResourceIds: [
        managedIdentity.outputs.resourceId
      ]
    }
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }
    tags: tags
}
}

// create app service plan

module hostingPlan 'br/public:avm/res/web/serverfarm:0.1.1' = {
  name: 'DeployHostingPlan'
  params: {
    name: hostingPlanName
    sku: appServicePlanSku
    kind : appServicePlanKind
    tags: tags
  }
}

var existingAppSettings = union(functionAppsiteConfig , 
   {  'AzureAd:ClientCredentials:[0]:ClientSecret' : '@Microsoft.KeyVault(VaultName=${keyVault.outputs.name};SecretName=AzureAd-ClientSecret-BC)' 

})  

// create function app

module functionApp  'br/public:avm/res/web/site:0.3.2' = {
  name: 'DeployFunctionApp'
  params: {
    kind: functionAppkind
    name: functionAppName
    serverFarmResourceId: hostingPlan.outputs.resourceId
    siteConfig : functionsiteConfig
    storageAccountResourceId : storageAccount.outputs.resourceId
    appInsightResourceId : applicationInsights.outputs.resourceId
    keyVaultAccessIdentityResourceId : managedIdentity.outputs.resourceId
    appSettingsKeyValuePairs : existingAppSettings
    managedIdentities : {
      systemAssigned: false 
      userAssignedResourceIds: [
        managedIdentity.outputs.resourceId
      ]
    }
    // authSettingV2Configuration : {
    //    kind : functionAppkind
    //    authSettingV2Configuration : authSettingV2Configuration 
    // }
    tags: tags
  }
}


output roleAssignments object = mergedkeyVaultsecret  


