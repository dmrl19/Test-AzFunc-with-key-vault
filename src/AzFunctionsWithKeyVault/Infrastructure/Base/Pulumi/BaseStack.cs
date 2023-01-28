using Pulumi;
using Pulumi.AzureNative.AppConfiguration;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.KeyVault.Inputs;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using AzureNative = Pulumi.AzureNative;

namespace Sample.Infrastructure.Base.Pulumi;

public class BaseStack : Stack
{
    public BaseStack()
    {
        _config = new StackConfiguration();
//TODO: Refactor
        var resourceGroup = new ResourceGroup($"ResourceGroup", new ResourceGroupArgs()
        {
            ResourceGroupName = $"rg-dmr-base-{_config.Stage}",
            Location = _config.Location,
        });

        var keyVaultResourceGroup = "rg-kv-dmr-sample-dev";
        var keyVaultName = "kv-dmr-sample-dev";
        
        // CreateAzureFunction(resourceGroup);


        var secretOne = CreateKeyVaultSecret(keyVaultResourceGroup, keyVaultName, "Super-Secret-Name-One", "Value One");
        CreateKeyVaultSecret(keyVaultResourceGroup, keyVaultName, "Super-Secret-Name-Two", "Value Two");
        CreateKeyVaultSecret(keyVaultResourceGroup, keyVaultName, "Super-Secret-Name-Three", "Value Three");
        CreateKeyVaultSecret(keyVaultResourceGroup, keyVaultName, "Super-Secret-Name-Four", "Value Four");

        Log.Info("===============");
        var a = secretOne.Properties.Apply(d =>
        {
            // Needs to be formed like "@Microsoft.KeyVault([SecretUri])"
            // https://learn.microsoft.com/en-us/azure/app-service/app-service-key-vault-references?tabs=azure-cli#reference-syntax
            return $"@Microsoft.KeyVault({d.SecretUri})";
        });Log.Info("===============");
        
        
         var appConfiguration = CreateAppConfiguration(resourceGroup);
         CreateKeyVaultReferenceInAppConfiguration(resourceGroup, appConfiguration, "KeyVaultRef", a);
         // CreateAppConfigurationKeyValue(resourceGroup, appConfiguration, "App-Key-One", "App-Config-One");
         // CreateAppConfigurationKeyValue(resourceGroup, appConfiguration, "App-Key-Two", "App-Config-Two");
         // CreateAppConfigurationKeyValue(resourceGroup, appConfiguration, "App-Key-Three", "App-Config-Three");
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

    private static AppServicePlan CreateAppServicePlan(ResourceGroup resourceGroup)
    {
        return new AppServicePlan("azureServicePlan", new AppServicePlanArgs()
        {
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
            Name = "app-plan-func-dmr",
            Sku = new AzureNative.Web.Inputs.SkuDescriptionArgs
            {
                Capacity = 1,
                Family = "B1",
                Name = "B1",
                Size = "B1",
                Tier = "Basic",
            }
        });
    }

    private void CreateAzureFunction(ResourceGroup resourceGroup)
    {
        var storageAccount = CreateStorageAccount(resourceGroup);
        var servicePlan = CreateAppServicePlan(resourceGroup);

        new WebApp("FunctionApp", new WebAppArgs()
        {
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
            Name = "az-func-dmr",
            Kind = "functionapp",
            ServerFarmId = servicePlan.Id,
            SiteConfig = new SiteConfigArgs()
            {
                AppSettings = new[]
                {
                    new NameValuePairArgs() { Name = "AzureWebJobsStorage", Value = storageAccount.Name }
                }
            }
        });
    }

    private ConfigurationStore CreateAppConfiguration(ResourceGroup resourceGroup) =>
        new ConfigurationStore("appConfiguration",new ConfigurationStoreArgs()
    {
        ResourceGroupName = resourceGroup.Name,
        Sku = new AzureNative.AppConfiguration.Inputs.SkuArgs
        {
            Name = "Standard",
        },
        Location = resourceGroup.Location,
        ConfigStoreName =  "app-conf-dmr-sample-dev"
    });

    private Secret CreateKeyVaultSecret(string rgVaultName, string vaultName, string secretName, string value)
        => new Secret($"keyvaultSecret-{secretName}", new SecretArgs()
        {
            ResourceGroupName = rgVaultName,
            VaultName = vaultName,
            SecretName = secretName,
            Properties = new SecretPropertiesArgs()
            {
                Value = value
            }
        });

    private KeyValue CreateAppConfigurationKeyValue(ResourceGroup resourceGroup, ConfigurationStore configurationStore,
        string keyName, string keyValue)
    {
        return new KeyValue($"AppConfigurationKeyValue-{keyName}", new()
        {
            ConfigStoreName = configurationStore.Name,
            KeyValueName = keyName,
            ResourceGroupName = resourceGroup.Name,
            Value = keyValue
        });
    }

    private KeyValue CreateKeyVaultReferenceInAppConfiguration(ResourceGroup resourceGroup,
        ConfigurationStore configurationStore, string keyName, Output<string> a)
    =>new KeyValue($"AppConfigurationKeyValue-{keyName}", new()
    {
        ConfigStoreName = configurationStore.Name,
        KeyValueName = keyName,
        ResourceGroupName = resourceGroup.Name,
        ContentType =  "application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8",
        Value = a
    });
    
}