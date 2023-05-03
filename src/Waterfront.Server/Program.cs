using Dvchevskii.Extensions.Configuration.Yaml;
using Dvchevskii.Extensions.Configuration.Yaml.FileExtensions;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Waterfront.Acl.Static.Extensions.DependencyInjection;
using Waterfront.Acl.Static.Models;
using Waterfront.AspNetCore.Extensions;
using Waterfront.Extensions.DependencyInjection;
using YamlDotNet.Serialization.NamingConventions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables("WF_");

string? customConfigFilePath = builder.Configuration.GetValue(
    "ConfigPath",
    builder.Configuration.GetValue("Config_Path", string.Empty)!
);

if ( !string.IsNullOrEmpty(customConfigFilePath) )
{
    builder.Configuration.AddYamlFile(customConfigFilePath, CamelCaseNamingConvention.Instance);
}
else
{
    builder.Configuration.AddYamlFile("wf_config.yaml", PascalCaseNamingConvention.Instance, true);
    builder.Configuration.AddYamlFile("wf_config.yml", CamelCaseNamingConvention.Instance, true);
    builder.Configuration.AddYamlFile("config.yaml", CamelCaseNamingConvention.Instance, true);
    builder.Configuration.AddYamlFile("config.yml", CamelCaseNamingConvention.Instance, true);
}

builder.Host.UseSerilog(
    (_, config) => config.WriteTo.Console(theme: AnsiConsoleTheme.Literate).MinimumLevel.Debug()
);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddWaterfront(
    wf => {
        wf.AddTokenMiddleware()
          .ConfigureTokens(builder.Configuration.GetSection("Tokens").Bind)
          .ConfigureEndpoints(endpoints => endpoints.SetTokenEndpoint(builder.Configuration["Endpoints:Token"]))
          .WithDefaultTokenEncoder()
          .WithDefaultTokenDefinitionService();

        if ( builder.Configuration.GetSection("CertificateProviders:File").Exists() )
        {
            wf.WithFileSigningCertificateProvider(
                builder.Configuration.GetSection("CertificateProviders:File").Bind
            );
        }

        if ( builder.Configuration.GetSection("Users").Exists() )
        {
            wf.AddStaticAuthentication(
                builder.Configuration.GetSection("Users").Get<StaticAclUser[]>()!
            );
        }

        if ( builder.Configuration.GetSection("Acl").Exists() )
        {
            wf.AddStaticAuthorization(
                builder.Configuration.GetSection("Acl").Get<StaticAclPolicy[]>()!
            );
        }
    }
);

WebApplication app = builder.Build();

app.UseWaterfront().UseSwagger().UseSwaggerUI();

app.MapControllers();

app.Run();
