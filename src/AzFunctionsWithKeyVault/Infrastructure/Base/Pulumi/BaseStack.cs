using System.Text.Json;
using Pulumi;
using Pulumi.AzureNative.AppConfiguration;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.KeyVault.Inputs;
using Pulumi.AzureNative.Resources;
using Sample.Infrastructure.Base.Pulumi.Models;
using AzureNative = Pulumi.AzureNative;

namespace Sample.Infrastructure.Base.Pulumi;

public class BaseStack : Stack
{
    [Output] public Output<string> ResourceGroupName { get; set; }
    [Output] public Output<string> AppConfigurationEndpoint { get; set; }
    
    public BaseStack()
    {
        _config = new StackConfiguration();

        var resourceGroup = new ResourceGroup($"ResourceGroup", new ResourceGroupArgs()
        {
            ResourceGroupName = $"rg-dmr-{_config.Stage}",
            Location = _config.Location,
        });
        
        ResourceGroupName = resourceGroup.Name;
        
        var secretOne = CreateKeyVaultSecret(_config.KeyVaultResourceGroupName, _config.KeyVaultName, "Super-Secret-Name-One", "Value One");
        CreateKeyVaultSecret(_config.KeyVaultResourceGroupName, _config.KeyVaultName, "Super-Secret-Name-Two", "Value Two");
        CreateKeyVaultSecret(_config.KeyVaultResourceGroupName, _config.KeyVaultName, "Super-Secret-Name-Three", "Value Three");
        CreateKeyVaultSecret(_config.KeyVaultResourceGroupName, _config.KeyVaultName, "Super-Secret-Name-Four", "Value Four");

        var appConfiguration = CreateAppConfiguration(resourceGroup);
        CreateKeyVaultReferenceInAppConfiguration(resourceGroup, appConfiguration, "KeyVaultRef", secretOne);
        CreateAppConfigurationKeyValue(resourceGroup, appConfiguration, "App-Key-One", "App-Config-One");
        CreateAppConfigurationKeyValue(resourceGroup, appConfiguration, "App-Key-Two", "App-Config-Two");
        CreateAppConfigurationKeyValue(resourceGroup, appConfiguration, "App-Key-Three", "App-Config-Three");

        AppConfigurationEndpoint = appConfiguration.Endpoint;
    }

    private readonly StackConfiguration _config;

    private AzureNative.Storage.StorageAccount CreateStorageAccount(ResourceGroup resourceGroup)
    {
        return new AzureNative.Storage.StorageAccount("storageAccount", new()
        {
            AccountName = $"strexampledmrsample{_config.Stage}",
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
            AccessTier = AzureNative.Storage.AccessTier.Hot,
            Kind = "StorageV2",
            Sku = new AzureNative.Storage.Inputs.SkuArgs()
            {
                Name = "Standard_LRS",
            }
        });
    }

    private ConfigurationStore CreateAppConfiguration(ResourceGroup resourceGroup) =>
        new("appConfiguration", new ConfigurationStoreArgs()
        {
            ResourceGroupName = resourceGroup.Name,
            Sku = new AzureNative.AppConfiguration.Inputs.SkuArgs
            {
                Name = "Standard",
            },
            Location = resourceGroup.Location,
            ConfigStoreName = $"appconfdmr{_config.Stage}"
        });

    private Secret CreateKeyVaultSecret(string rgVaultName, string vaultName, string secretName, string value)
        => new Secret($"keyVaultSecret-{secretName}", new SecretArgs()
        {
            ResourceGroupName = rgVaultName,
            VaultName = vaultName,
            SecretName = secretName,
            Properties = new SecretPropertiesArgs()
            {
                Value = value
            }
        });

    private void CreateAppConfigurationKeyValue(ResourceGroup resourceGroup, ConfigurationStore configurationStore,
        string keyName, string keyValue)
    {
        _ = new KeyValue($"AppConfigurationKeyValue-{keyName}", new()
        {
            ConfigStoreName = configurationStore.Name,
            KeyValueName = keyName,
            ResourceGroupName = resourceGroup.Name,
            Value = keyValue
        });
    }

    private void CreateKeyVaultReferenceInAppConfiguration(ResourceGroup resourceGroup,
        ConfigurationStore configurationStore, string keyName, Secret secret) //Output<string> a)
    {
        _ = new KeyValue($"AppConfigurationKeyValue-{keyName}", new()
        {
            ConfigStoreName = configurationStore.Name,
            KeyValueName = keyName,
            ResourceGroupName = resourceGroup.Name,
            ContentType = "application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8",
            
            // Needs to be a json string like '{"uri": "[SecretUri]"}'
            Value = secret.Properties.Apply(s => 
                JsonSerializer.Serialize(new KeyVaultReferenceModel(s.SecretUri)))
        });
        
    }
}