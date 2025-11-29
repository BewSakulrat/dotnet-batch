using Microsoft.Extensions.Configuration;

namespace Batch.Configuration;

public static class ConfigurationExtensions
{
    public static IConfigurationBuilder AddAwsSecretsManager(
        this IConfigurationBuilder builder,
        string secretName,
        string region = "us-east-1")
    {
        return builder.Add(new AwsSecretsManagerConfigurationSource
        {
            SecretName = secretName,
            Region = region
        });
    }
}