using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(x =>
    {
        x.AddAzureAppConfiguration(opt =>
        {
            var appConfigConectionString = Environment.GetEnvironmentVariable("AppConfigurationConnectionString")!;

            if (string.IsNullOrEmpty(appConfigConectionString))
                throw new ArgumentNullException(nameof(appConfigConectionString));
        
            opt.Connect(appConfigConectionString);
            opt.ConfigureKeyVault(kv =>
            {
                //Connect as my self when doing `az login`
                kv.SetCredential(new AzureCliCredential());
            });
        });
    })
    .Build();

host.Run();