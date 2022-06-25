targetScope = 'managementGroup'
param groupName string
param groupDisplayName string
param subscriptionIds array
param parentId string
resource mainManagementGroup 'Microsoft.Management/managementGroups@2020-02-01' = {
  name: groupName
  scope: tenant()
  properties: {
    displayName: groupDisplayName
    details: {      
      parent: {
        id: parentId
      }
    }
  }
}
resource subscriptionResources 'Microsoft.Management/managementGroups/subscriptions@2020-05-01' = [for sub in subscriptionIds: {
  parent: mainManagementGroup
  name: sub.id
  dependsOn: [
    mainManagementGroup
  ]
}]
output groupId string = mainManagementGroup.id
