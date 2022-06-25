targetScope = 'tenant'
 
@description('Provide the full resource ID of billing scope to use for subscription creation.')
param billingScope string
@description('The name of the main group')
param mainManagementGroupName string = 'managementgroup-parent'
@description('The display name for the main group')
param mainMangementGroupDisplayName string = 'Parent Management Group'
param managementGroups array = [
  {
    name: 'managementgroup-cheddar-non-prod'
    displayName: 'Cheddar Non-Production Management Group'
    subscriptions: [
      {
        name: 'cheddar-dev'
        workload: 'Production'
      }
      {
        name: 'cheddar-test'
        workload: 'Production'
      }
    ]
  }
  {
    name: 'managementgroup-cheddar-prod'
    displayName: 'Cheddar Production Management Group'
    subscriptions: [
      {
        name: 'cheddar-prod'
        workload: 'Production'
      }
    ]
  }
]
 
resource mainManagementGroup 'Microsoft.Management/managementGroups@2020-02-01' = {
  name: mainManagementGroupName
  scope: tenant()
  properties: {
    displayName: mainMangementGroupDisplayName
  }
}
 
module subsModule './Subscriptions.bicep' = [for group in managementGroups: {
  name: 'subscriptionDeploy-${group.name}' 
  params: {
    subscriptions: group.subscriptions
    billingScope: billingScope
  }
}]
 
module mgSubModule './ManagementGroups.bicep' = [for (group, i) in managementGroups: {
  name: 'managementGroupDeploy-${group.name}'
  scope: managementGroup(mainManagementGroupName)
  params: {
    groupName: group.name
    groupDisplayName: group.displayName
    parentId: mainManagementGroup.id
    subscriptionIds: subsModule[i].outputs.subscriptionIds
  }
  dependsOn: [
    subsModule
  ]
}]
