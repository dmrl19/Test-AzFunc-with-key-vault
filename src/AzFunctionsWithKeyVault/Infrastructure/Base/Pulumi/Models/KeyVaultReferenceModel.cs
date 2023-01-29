using System.Text.Json.Serialization;

namespace Sample.Infrastructure.Base.Pulumi.Models;

public class KeyVaultReferenceModel
{
    public KeyVaultReferenceModel(string secretUri)
    {
        Uri = secretUri;
    }
    
    [JsonPropertyName("uri")]
    public string Uri { get; set; }
}