using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Waterfront.Acl.Static.Authentication;
using Waterfront.Acl.Static.Authorization;
using Waterfront.Acl.Static.Configuration;
using Waterfront.Acl.Static.Models;
using Waterfront.AspNetCore.Extensions;
using Waterfront.Core.Tokens.Signing.CertificateProviders.Files;

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
    waterfront => {
        waterfront.ConfigureEndpoints(endpoints => endpoints.SetTokenEndpoint("/token"))
                  .ConfigureTokens(
                      tokens => tokens.SetIssuer("http://localhost:5050").SetLifetime(120)
                  );

        IConfigurationSection fileCertConfig = builder.Configuration.GetSection("Certificate_Providers:File");

        if ( fileCertConfig.Exists() )
        {
            waterfront
            .WithSigningCertificateProvider<FileSigningCertificateProvider,
                FileSigningCertificateProviderOptions>(
                opt => {
                    opt.CertificatePath = fileCertConfig.GetValue<string>("Certificate_Path")!;
                    opt.PrivateKeyPath  = fileCertConfig.GetValue<string>("Private_Key_Path")!;
                });
        }

        StaticAclUser[]? staticAclUserList = builder.Configuration.GetSection("Users").Get<StaticAclUser[]>();

        if ( staticAclUserList is { Length: not 0 } )
        {
            waterfront
            .WithAuthentication<StaticAclAuthenticationService, StaticAclAuthenticationOptions>(
                opt => opt.Users = staticAclUserList
            );
        }

        StaticAclPolicy[]? staticAclPolicyList = builder.Configuration.GetSection("Acl").Get<StaticAclPolicy[]>();

        if ( staticAclPolicyList is { Length: not 0 } )
        {
            waterfront
            .WithAuthorization<StaticAclAuthorizationService, StaticAclAuthorizationOptions>(
                opt => opt.Acl = staticAclPolicyList
            );
        }
    }
);

WebApplication app = builder.Build();

app.UseWaterfront();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
