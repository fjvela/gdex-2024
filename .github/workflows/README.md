## Provision workflow

The `provision-env` wokflow uses the GitHub Variables and Secrets below to deploy the Azure resources for the Globoticket application:

|Name|Description|
|-|-|
|vars.AZURE_CLIENT_ID|Azure Service Principal Id with `contributor` access to the Azure resource group|
|secrets.AZURE_CLIENT_SECRET|Azure Service Principal Password|
|vars.AZURE_SUBSCRIPTION_ID|Id of the Azure Subscription|
|vars.AZURE_TENANT_ID|Azure Tenant for the Entra ID environment|