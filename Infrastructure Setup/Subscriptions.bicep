targetScope = 'tenant'
@description('Provide the full resource ID of billing scope to use for subscription creation.')
param billingScope string
param subscriptions array = []
resource subscriptionAlias 'Microsoft.Subscription/aliases@2020-09-01' = [for sub in subscriptions: {
  scope: tenant()
  name: sub.name
  properties: {
    workload: sub.workload
    displayName: sub.name
    billingScope: billingScope
  }  
}]
 output subscriptionIds array = [for (subs, i) in subscriptions: {
    id: subscriptionAlias[i].properties.subscriptionId
 }]
