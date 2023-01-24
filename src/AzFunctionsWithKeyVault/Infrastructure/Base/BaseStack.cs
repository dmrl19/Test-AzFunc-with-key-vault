using Pulumi;
using Pulumi.AzureNative.AppConfiguration;
using Pulumi.AzureNative.Resources;
using AzureNative = Pulumi.AzureNative;

namespace Sample.Infrastructure.Base;

public class BaseStack : Stack
{
    private readonly StackConfiguration _config;

    public BaseStack()
    {
        _config = new StackConfiguration();

        var resourceGroup = new ResourceGroup($"rg-dmr-sample-{_config.Stage}");

        
        //TODO: Create AppConfgiuration
        // var appConfiguration = new ConfigurationStore("cs",new ConfigurationStoreArgs()
        // {
        //     ResourceGroupName = resourceGroup.Name,
        //     Sku = new AzureNative.AppConfiguration.Inputs.SkuArgs
        //     {
        //         Name = "Standard",
        //     },
        //     Location = resourceGroup.Location,
        //     ConfigStoreName = "AppConfigurationDmrDev"
        // });
    }
}