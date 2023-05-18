using YamlDotNet.Serialization.NamingConventions;
using Dvchevskii.Extensions.Configuration.Yaml.FileExtensions;

namespace Waterfront.Extensions;

public static class ConfigurationManagerExtensions
{
    public static ConfigurationManager AddWaterfrontConfiguration(this ConfigurationManager self)
    {
        string? customConfigFilePath = self.GetValue<string?>("ConfigPath", null);

        if (!string.IsNullOrEmpty(customConfigFilePath))
        {
            self.AddYamlFile(
                Path.GetFullPath(customConfigFilePath),
                CamelCaseNamingConvention.Instance
            );
        }
        else
        {
            self.AddYamlFile(
                Path.GetFullPath("wf_config.yaml"),
                PascalCaseNamingConvention.Instance,
                true
            );
            self.AddYamlFile(
                Path.GetFullPath("wf_config.yml"),
                CamelCaseNamingConvention.Instance,
                true
            );
            self.AddYamlFile(
                Path.GetFullPath("config.yaml"),
                CamelCaseNamingConvention.Instance,
                true
            );
            self.AddYamlFile(
                Path.GetFullPath("config.yml"),
                CamelCaseNamingConvention.Instance,
                true
            );
        }

        return self;
    }
}
