using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Waterfront.AspNetCore.Extensions;
using Waterfront.Core.Tokens.Signing.CertificateProviders;
using Waterfront.Acl.Static;
using Waterfront.Acl.Static.Models;
using Waterfront.Server.Configuration;
using Waterfront.Server.Extensions;

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
    wf => {
        wf.ConfigureTokenOptions(builder.Configuration.GetSection("Tokens").Bind)
          .ConfigureEndPoints(builder.Configuration.GetSection("Endpoints").Bind)
          .UseCertificateProviders(
              builder.Configuration.GetSection("Tokens:Signing")
                     .Get<SigningCertificateProviderOptions>()
          )
          .WithCertificateProvider<FileSigningCertificateProvider,
              FileTokenCertificateProviderOptions>(
              opt => {
                  opt.CertificatePath = "certs/localhost.crt";
                  opt.PrivateKeyPath  = "certs/localhost.key";
              }
          )
          .WithAuthentication<StaticAclAuthenticationService, StaticAclOptions>(
              opt => {
                  opt.Users = new[] {
                      new StaticAclUser {
                          Username = "localhostUser",
                          Ip       = "127.0.0.1:*",
                          Acl      = new[] { "localhost" }
                      }
                  };
                  opt.Acl = new[] {
                      new StaticAclPolicy {
                          Name = "localhost",
                          Access = new[] {
                              new StaticAclPolicyAccessRule {
                                  Type = "repository",
                                  Name = "*",
                                  Actions =
                                  new[] { "pull", "push" }
                              }
                          }
                      }
                  };
              }
          )
          .WithAuthorization<StaticAclAuthorizationService>();

    }
);

var app = builder.Build();

app.UseWaterfront();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
