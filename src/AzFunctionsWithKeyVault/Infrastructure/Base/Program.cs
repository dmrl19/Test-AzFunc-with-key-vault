using Pulumi;
using System.Threading.Tasks;

namespace Sample.Infrastructure.Base;

public class Program
{
    static Task<int> Main() => Deployment.RunAsync<BaseStack>();   
}