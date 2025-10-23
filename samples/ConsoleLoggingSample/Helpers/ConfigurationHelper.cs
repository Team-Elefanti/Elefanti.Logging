namespace ConsoleLoggingSample.Helpers;

/// <summary>
/// Helper class for retrieving webhook URLs from environment variables
/// </summary>
public static class ConfigurationHelper
{
    public static string GetWebhookUrl(string? variableName = null, string? fallback = null)
    {
        if (!string.IsNullOrEmpty(variableName))
        {
            var url = Environment.GetEnvironmentVariable(variableName);
            if (!string.IsNullOrEmpty(url))
                return url;
        }

        if (!string.IsNullOrEmpty(fallback))
            return fallback;

        return Environment.GetEnvironmentVariable("DISCORD_WEBHOOK_URL")
               ?? "https://discord.com/api/webhooks/YOUR_WEBHOOK_ID/YOUR_WEBHOOK_TOKEN";
    }

    public static string GetWebhookCritical()
    {
        return GetWebhookUrl("DISCORD_WEBHOOK_CRITICAL");
    }

    public static string GetWebhookErrors()
    {
        return GetWebhookUrl("DISCORD_WEBHOOK_ERRORS", GetWebhookCritical());
    }

    public static string GetWebhookWarnings()
    {
        return GetWebhookUrl("DISCORD_WEBHOOK_WARNINGS", GetWebhookCritical());
    }

    public static string GetWebhookInfo()
    {
        return GetWebhookUrl("DISCORD_WEBHOOK_INFO", GetWebhookCritical());
    }

    public static string GetWebhookDefault()
    {
        return GetWebhookUrl("DISCORD_WEBHOOK_DEFAULT", GetWebhookCritical());
    }

    public static string GetWebhookPayments()
    {
        return GetWebhookUrl("DISCORD_WEBHOOK_PAYMENTS", GetWebhookCritical());
    }

    public static string GetWebhookAuth()
    {
        return GetWebhookUrl("DISCORD_WEBHOOK_AUTH", GetWebhookDefault());
    }
}
