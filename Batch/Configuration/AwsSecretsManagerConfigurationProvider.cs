using System.Text.Json;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Configuration;

namespace Batch.Configuration;

public class AwsSecretsManagerConfigurationProvider(AwsSecretsManagerConfigurationSource source) : ConfigurationProvider
{
    public override void Load()
    {
        try
        {
            var secret = GetSecretAsync().GetAwaiter().GetResult();
            
            if (!string.IsNullOrEmpty(secret))
            {
                var secretDict = JsonSerializer.Deserialize<Dictionary<string, string>>(secret);
                
                if (secretDict != null)
                {
                    // Add all individual values to configuration
                    foreach (var kvp in secretDict)
                    {
                        Data[kvp.Key] = kvp.Value;
                    }
                    
                    // Build SQL Server connection string from parts
                    if (HasRequiredDatabaseCredentials(secretDict))
                    {
                        var connectionString = BuildSqlServerConnectionString(secretDict);
                        
                        // Store both formats for flexibility
                        Data["ConnectionString"] = connectionString;
                        Data["ConnectionStrings:DefaultConnection"] = connectionString;
                        
                        Console.WriteLine("SQL Server connection string built from AWS Secrets Manager");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading secrets from AWS: {ex.Message}");
            throw;
        }
    }

    private bool HasRequiredDatabaseCredentials(Dictionary<string, string> secrets)
    {
        return secrets.ContainsKey("host") &&
               secrets.ContainsKey("database") &&
               secrets.ContainsKey("username") &&
               secrets.ContainsKey("password") &&
               secrets.ContainsKey("port");
    }

    private string BuildSqlServerConnectionString(Dictionary<string, string> secrets)
    {
        var host = secrets["host"];
        var port = secrets.GetValueOrDefault("port", "1433");
        var database = secrets["database"];
        var username = secrets["username"];
        var password = secrets["password"];
        
        // Additional optional parameters
        var connectionTimeout = secrets.GetValueOrDefault("connectionTimeout", "30");
        var encrypt = secrets.GetValueOrDefault("encrypt", "true");
        var trustServerCertificate = secrets.GetValueOrDefault("trustServerCertificate", "false");

        return $"Server={host},{port};" +
               $"Database={database};" +
               $"User Id={username};" +
               $"Password={password};" +
               $"Connection Timeout={connectionTimeout};" +
               $"Encrypt={encrypt};" +
               $"TrustServerCertificate={trustServerCertificate};";
    }

    private async Task<string> GetSecretAsync()
    {
        var region = RegionEndpoint.GetBySystemName(source.Region);
        using var client = new AmazonSecretsManagerClient(region);

        var request = new GetSecretValueRequest
        {
            SecretId = source.SecretName,
            VersionStage = "AWSCURRENT"
        };

        var response = await client.GetSecretValueAsync(request);
        return response.SecretString;
    }
}