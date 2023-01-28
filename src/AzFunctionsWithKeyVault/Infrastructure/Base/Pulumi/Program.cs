using Pulumi;
using System.Threading.Tasks;
using Sample.Infrastructure.Base.Pulumi;


public class Program
{
    static Task<int> Main() => Deployment.RunAsync<BaseStack>();   
}