using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Waterfront.Acl.Sqlite.Configuration;
using Waterfront.Acl.Sqlite.Extensions;
using Waterfront.Acl.Static.Extensions.DependencyInjection;
using Waterfront.Acl.Static.Models;
using Waterfront.AspNetCore.Extensions;
using Waterfront.Extensions;
using Waterfront.Extensions.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables("WF_");
builder.Configuration.AddWaterfrontConfiguration();

builder.Host.UseSerilog(
    (_, config) => config.WriteTo.Console(theme: AnsiConsoleTheme.Literate).MinimumLevel.Debug()
);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddWaterfront(wf =>
{
    wf.AddTokenMiddleware()
        .ConfigureTokens(builder.Configuration.GetSection("Tokens").Bind)
        .ConfigureEndpoints(
            endpoints => endpoints.SetTokenEndpoint(builder.Configuration["Endpoints:Token"])
        )
        .WithDefaultTokenEncoder()
        .WithDefaultTokenDefinitionService();

    if (builder.Configuration.GetSection("CertificateProviders:File").Exists())
    {
        wf.WithFileSigningCertificateProvider(
            builder.Configuration.GetSection("CertificateProviders:File").Bind
        );
    }

    if (builder.Configuration.GetSection("Users").Exists())
    {
        wf.AddStaticAuthentication(
            builder.Configuration.GetSection("Users").Get<StaticAclUser[]>()!
        );
    }

    if (builder.Configuration.GetSection("Acl").Exists())
    {
        wf.AddStaticAuthorization(
            builder.Configuration.GetSection("Acl").Get<StaticAclPolicy[]>()!
        );
    }

    if (builder.Configuration.GetSection("Sqlite").Exists())
    {
        SqliteAclOptions sqliteOptions = builder.Configuration
            .GetSection("Sqlite")
            .Get<SqliteAclOptions>();

        if (sqliteOptions.SupportsAuthentication)
        {
            wf.AddSqliteAuthentication(opt =>
            {
                opt.DataSource = sqliteOptions.DataSource;
                opt.Users = sqliteOptions.Users;
            });
        }

        if (sqliteOptions.SupportsAuthorization)
        {
            wf.AddSqliteAuthorization(opt =>
            {
                opt.DataSource = sqliteOptions.DataSource;
                opt.Acl = sqliteOptions.Acl;
            });
        }
    }
});

WebApplication app = builder.Build();

app.UseWaterfront().UseSwagger().UseSwaggerUI();

app.MapControllers();

app.Run();
