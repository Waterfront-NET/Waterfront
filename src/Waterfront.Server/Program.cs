using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Waterfront.Acl.Static.Configuration;
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

IWaterfrontBuilder waterfront = builder.Services.AddWaterfront()
                                       .AddTokenMiddleware()
                                       .ConfigureTokens(
                                           opt => opt.SetIssuer("http://localhost:5050")
                                                     .SetLifetime(120)
                                       )
                                       .ConfigureEndpoints(opt => opt.SetTokenEndpoint("/token"))
                                       .WithFileSigningCertificateProvider(
                                           "./certs/localhost.crt",
                                           "./certs/localhost.key"
                                       )
                                       .WithDefaultTokenEncoder()
                                       .WithDefaultTokenDefinitionService()
                                       .AddStaticAuthentication(
                                           opt => {
                                               opt.Users.Add(
                                                   new StaticAclUser {
                                                       Username = "local_user",
                                                       Ip       = "127.0.0.1:*",
                                                       Acl      = { "default" }
                                                   }
                                               );
                                           }
                                       )
                                       .AddStaticAuthorization(
                                           new StaticAclPolicy {
                                               Name = "default",
                                               Access = {
                                                   new StaticAclPolicyAccessRule {
                                                       Name = "*",
                                                       Type =
                                                       "repository",
                                                       Actions = {
                                                           "pull"
                                                       }
                                                   }
                                               }
                                           }
                                       );

WebApplication app = builder.Build();

app.UseWaterfront();

app.UseSwagger().UseSwaggerUI();

app.MapControllers();

app.Run();
