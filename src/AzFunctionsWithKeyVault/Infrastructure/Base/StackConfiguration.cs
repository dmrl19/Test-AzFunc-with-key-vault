using Pulumi;

namespace Sample.Infrastructure.Base;

public  class StackConfiguration
{
    private readonly Config _stackConfig;
    
    public StackConfiguration()
    {
        _stackConfig = new Config();
    }

    public string Stage => _stackConfig.Require("stage");
}