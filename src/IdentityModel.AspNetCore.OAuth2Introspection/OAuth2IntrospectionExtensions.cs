using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OAuth2IntrospectionExtensions
    {

        public static AuthenticationBuilder AddOAuth2IntrospectionAuthentication(this AuthenticationBuilder builder) 
            => builder.AddOAuth2IntrospectionAuthentication(OAuth2IntrospectionDefaults.AuthenticationScheme);

        public static AuthenticationBuilder AddOAuth2IntrospectionAuthentication(this AuthenticationBuilder builder, string authenticationScheme) 
            => builder.AddOAuth2IntrospectionAuthentication(authenticationScheme, configureOptions: null);

        public static AuthenticationBuilder AddOAuth2IntrospectionAuthentication(this AuthenticationBuilder builder, Action<OAuth2IntrospectionOptions> configureOptions) 
            => builder.AddOAuth2IntrospectionAuthentication(OAuth2IntrospectionDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddOAuth2IntrospectionAuthentication(this AuthenticationBuilder builder, string authenticationScheme, Action<OAuth2IntrospectionOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<OAuth2IntrospectionOptions>, PostConfigureOAuth2IntrospectionAuthenticationOptions>());
            return builder.AddScheme<OAuth2IntrospectionOptions,OAuth2IntrospectionHandler>(authenticationScheme, configureOptions);
        }

    }
}
