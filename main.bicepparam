using './main.bicep'

var nameSuffix = '-bc-integraion-auest-d01'

param tags = {
  Project: 'BC Integration'
  Environment: 'Dev'
}
param storageAccountType = 'Standard_LRS'
param functionAppName = 'func${nameSuffix}'
param hostingPlanName = 'ase${nameSuffix}'
param appServicePlanSku = {
  name: 'Y1'
  tier: 'Dynamic'
  size: 'Y1'
  family: 'Y'
  capacity: 0
}
param applicationInsightsName = 'appinsights${nameSuffix}'
param logAnalyticsName = 'log${nameSuffix}'
param storageAccountName = 'stbloomsbcintd01'
param managedIdentityName = 'id${nameSuffix}'
param keyVaultName = 'kv-bc-int-auest-d02'
param keyVaultsecrets = {}
param appServicePlanKind = ''
param functionAppkind = 'functionapp'
param functionsiteConfig = {}
param functionAppsiteConfig = {}
param authSettingV2Configuration = {}

