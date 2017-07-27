using IdentityModel.AspNetCore.OAuth2Introspection;
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

        public static IServiceCollection AddOAuth2IntrospectionAuthentication(this IServiceCollection services) => services.AddOAuth2IntrospectionAuthentication(OAuth2IntrospectionDefaults.AuthenticationScheme);

        public static IServiceCollection AddOAuth2IntrospectionAuthentication(this IServiceCollection services, string authenticationScheme) => services.AddOAuth2IntrospectionAuthentication(authenticationScheme, configureOptions: null);

        public static IServiceCollection AddOAuth2IntrospectionAuthentication(this IServiceCollection services, Action<OAuth2IntrospectionOptions> configureOptions) =>
            services.AddOAuth2IntrospectionAuthentication(OAuth2IntrospectionDefaults.AuthenticationScheme, configureOptions);

        public static IServiceCollection AddOAuth2IntrospectionAuthentication(this IServiceCollection services, string authenticationScheme, Action<OAuth2IntrospectionOptions> configureOptions)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<OAuth2IntrospectionOptions>, PostConfigureOAuth2IntrospectionAuthenticationOptions>());
            return services.AddScheme<OAuth2IntrospectionOptions,OAuth2IntrospectionHandler>(authenticationScheme, configureOptions);
        }

    }
}
