using Pulumi;

namespace Sample.Infrastructure.Base.Pulumi;

public  class StackConfiguration
{
    private readonly Config _stackConfig;
    
    public StackConfiguration()
    {
        _stackConfig = new Config();
    }

    public string Stage => _stackConfig.Require("stage");
    public string Location => _stackConfig.Require("location");
    public string KeyVaultResourceGroupName => _stackConfig.Require("keyVaultResourceGroupName");
    public string KeyVaultName => _stackConfig.Require("keyVaultName");
}