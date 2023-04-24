using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Waterfront.Acl.Static.Extensions.DependencyInjection;
using Waterfront.Acl.Static.Models;
using Waterfront.AspNetCore.Extensions;
using Waterfront.Extensions.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables("WF_");

string? customConfigFilePath = builder.Configuration.GetValue(
    "ConfigPath",
    builder.Configuration.GetValue("Config_Path", string.Empty)!
);

if ( !string.IsNullOrEmpty(customConfigFilePath) ) 
{
    builder.Configuration.AddYamlFile(customConfigFilePath, true);
}
else
{
    builder.Configuration.AddYamlFile("wf_config.yaml", true);
    builder.Configuration.AddYamlFile("wf_config.yml", true);
    builder.Configuration.AddYamlFile("config.yaml", true);
    builder.Configuration.AddYamlFile("config.yml", true);
}

builder.Host.UseSerilog(
    (_, config) => config.WriteTo.Console(theme: AnsiConsoleTheme.Literate).MinimumLevel.Debug()
);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddWaterfront(
    wf => wf.AddTokenMiddleware()
            .ConfigureTokens(tokens => tokens.SetIssuer("http://localhost:5050").SetLifetime(120))
            .ConfigureEndpoints(opt => opt.SetTokenEndpoint("/token"))
            .WithFileSigningCertificateProvider("./certs/localhost.crt", "./certs/localhost.key")
            .WithDefaultTokenEncoder()
            .WithDefaultTokenDefinitionService()
            .AddStaticAuthentication(
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
                        new StaticAclPolicyAccessRule {
                            Type    = "repository",
                            Name    = "**/*",
                            Actions = { "pull" }
                        }
                    }
                },
                new StaticAclPolicy {
                    Name = "admin",
                    Access = {
                        new StaticAclPolicyAccessRule {
                            Type    = "registry",
                            Name    = "**/*",
                            Actions = { "*" }
                        },
                        new StaticAclPolicyAccessRule {
                            Type    = "repository",
                            Name    = "**/*",
                            Actions = { "*" }
                        }
                    }
                }
            )
);

WebApplication app = builder.Build();

app.UseWaterfront().UseSwagger().UseSwaggerUI();

app.MapControllers();

app.Run();
