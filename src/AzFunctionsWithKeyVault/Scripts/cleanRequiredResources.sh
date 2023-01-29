
# Deleting key vault resources
az keyvault delete --name kv-dmr-sample-dev --resource-group rg-kv-dmr-sample-dev
az keyvault purge --name kv-dmr-sample-dev --location WestEurope
az group delete --name rg-kv-dmr-sample-dev -y
echo "Key vault resources has been deleted"

# Purge App configuration
az appconfig purge --name appconfdmrdev -y
echo "App configuration has been purged"

# deleting Pulumi State Resources
az group delete --name rg-pulumi-state -y
echo "Pulumi resources has been deleted"