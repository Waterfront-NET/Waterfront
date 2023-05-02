using Dvchevskii.Extensions.Configuration.Yaml;
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
    builder.Configuration.AddYamlFile(customConfigFilePath, CamelCaseNamingConvention.Instance, false);
}
else
{
    builder.Configuration.AddYamlFile("wf_config.yaml", CamelCaseNamingConvention.Instance, true);
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
          .ConfigureTokens(tokens => tokens.SetIssuer("http://localhost:5050").SetLifetime(120))
          .ConfigureEndpoints(opt => opt.SetTokenEndpoint("/token"))
          .WithFileSigningCertificateProvider("./certs/localhost.crt", "./certs/localhost.key")
          .WithDefaultTokenEncoder()
          .WithDefaultTokenDefinitionService()
          /*.AddStaticAuthentication(
              new StaticAclUser {
                  Username          = "root",
                  PlainTextPassword = "rootpwd",
                  Acl               = { "admin" }
              },
              new StaticAclUser {
                  Username = "anonymous",
                  Acl      = { "anonymous" }
              }
          )
          .AddStaticAuthorization(
              new StaticAclPolicy {
                  Name = "anonymous",
                  Access = {
                      new StaticAclAccessRule {
                          Type    = "repository",
                          Name    = "*#1#*",
                          Actions = { "pull" }
                      }
                  }
              },
              new StaticAclPolicy {
                  Name = "admin",
                  Access = {
                      new StaticAclAccessRule {
                          Type    = "registry",
                          Name    = "*#1#*",
                          Actions = { "*" }
                      },
                      new StaticAclAccessRule {
                          Type    = "repository",
                          Name    = "*#1#*",
                          Actions = { "*" }
                      }
                  }
              }
          )*/;

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
