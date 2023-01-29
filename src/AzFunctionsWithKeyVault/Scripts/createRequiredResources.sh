
resourceGroupStateName=rg-pulumi-state
storageAccountPulumiStateName=stpulumistatedmrdev

resourceGroupKeyVaultName=rg-kv-dmr-sample-dev
keyVaultName=kv-dmr-sample-dev

echo "Creating required resources..."

# Create pulumi state resource group
az group create -l WestEurope -n $resourceGroupStateName
echo  "Resource group:" $resourceGroupStateName  "was created"

# Create Storage account for the pulumi State
az storage account create --name $storageAccountPulumiStateName -l WestEurope --sku Standard_LRS -g $resourceGroupStateName
echo  "Storage account:" $storageAccountPulumiStateName  "was created"

az storage container create --name state --account-name $storageAccountPulumiStateName
echo "Storage container for pulumi state created successfully"

# List Access Key from pulumi state storage account
az storage account keys list -g $resourceGroupStateName --account-name $storageAccountPulumiStateName

# Create Key Vault Resource Group
az group create --location WestEurope --name $resourceGroupKeyVaultName
echo  "Resource group:" $resourceGroupKeyVaultName  "was created"

# Create Key Vault
az keyvault create --location WestEurope --name kv-dmr-sample-dev --resource-group $resourceGroupKeyVaultName
echo  "Key vault:" $keyVaultName  "was created"