using Microsoft.Extensions.Configuration;

namespace Batch.Configuration;

public class AwsSecretsManagerConfigurationSource : IConfigurationSource
{
    public string SecretName { get; set; } = string.Empty;
    public string Region { get; set; } = "ap-southeast-1";

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new AwsSecretsManagerConfigurationProvider(this);
    }
}