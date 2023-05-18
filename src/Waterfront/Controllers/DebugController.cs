using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Waterfront.Acl.Static.Authentication;
using Waterfront.Acl.Static.Authorization;
using Waterfront.Acl.Static.Configuration;
using Waterfront.Common.Authentication;
using Waterfront.Common.Authorization;
using Waterfront.Core.Configuration.Tokens;

namespace Waterfront.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class DebugController : ControllerBase
{
    private readonly ILogger<DebugController>       _logger;
    private readonly IOptions<StaticAclOptions>     _opts;
    private readonly StaticAclAuthenticationService _authnService;
    private readonly StaticAclAuthorizationService  _authzService;

    public DebugController(
        ILogger<DebugController> logger,
        IEnumerable<IAclAuthenticationService> authnServices,
        IEnumerable<IAclAuthorizationService> authzServices,
        IOptions<StaticAclOptions> opts
    )
    {
        _logger = logger;
        _opts = opts;
        _authnService = authnServices.First() as StaticAclAuthenticationService;
        _authzService = authzServices.First() as StaticAclAuthorizationService;
    }

    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        return Ok(_opts.Value.Users);
    }

    [HttpGet("acl")]
    public IActionResult GetAcl()
    {
        return Ok(_opts.Value.Acl);
    }

    [HttpGet("config")]
    public IActionResult Config([FromServices] IConfiguration config)
    {
        return Ok((config as IConfigurationRoot).GetDebugView());
    }

    [HttpGet("tokenopts")]
    public IActionResult Tokens([FromServices] IOptions<TokenOptions> options)
    {
        return Ok(options.Value);
    }
}
