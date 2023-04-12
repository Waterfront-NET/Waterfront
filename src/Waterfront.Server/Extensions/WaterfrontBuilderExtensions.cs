using Waterfront.AspNetCore.Configuration;
using Waterfront.Server.Configuration;

namespace Waterfront.Server.Extensions;

public static class WaterfrontBuilderExtensions
{
    public static WaterfrontBuilder UseCertificateProviders(
        this WaterfrontBuilder self,
        SigningCertificateProviderOptions? options
    )
    {
        if ( options != null )
        {
            
        }

        return self;
    }
}
