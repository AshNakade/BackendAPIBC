metadata name = 'Site Auth Settings V2 Config'
metadata description = 'This module deploys a Site Auth Settings V2 Configuration.'
metadata owner = 'Azure/module-maintainers'

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


resource app 'Microsoft.Web/sites@2022-09-01' existing = {
  name: functionAppName
}

resource  authsettingsv2    'Microsoft.Web/sites/config@2022-09-01'  = {
  name: 'authsettingsV2'
  parent: app
  kind:functionAppkind
  properties: authSettingV2Configuration
  
}
