using IdentityModel.AspNetCore.OAuth2Introspection.Infrastructure;
using IdentityModel.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IdentityModel.AspNetCore.OAuth2Introspection
{
    public class PostConfigureOAuth2IntrospectionAuthenticationOptions : IPostConfigureOptions<OAuth2IntrospectionOptions>
    {
        private readonly IDistributedCache _cache;
      //  AsyncLazy<IntrospectionClient> _client;
      //  private readonly ConcurrentDictionary<string, AsyncLazy<IntrospectionResponse>> _lazyTokenIntrospections;

        public PostConfigureOAuth2IntrospectionAuthenticationOptions(IDistributedCache cache = null)
        {
            _cache = cache;

        }
        private async Task<string> GetIntrospectionEndpointFromDiscoveryDocument(OAuth2IntrospectionOptions Options )
        {
            HttpClient client;

            if (Options.DiscoveryHttpHandler != null)
            {
                client = new HttpClient(Options.DiscoveryHttpHandler);
            }
            else
            {
                client = new HttpClient();
            }

            client.Timeout = Options.DiscoveryTimeout;

            var discoEndpoint = Options.Authority.EnsureTrailingSlash() + ".well-known/openid-configuration";

            string response;
            try
            {
                response = await client.GetStringAsync(discoEndpoint).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Discovery endpoint {discoEndpoint} is unavailable: {ex.ToString()}");
            }

            try
            {
                var json = JObject.Parse(response);
                return json["introspection_endpoint"].ToString();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error parsing discovery document from {discoEndpoint}: {ex.ToString()}");
            }
        }

        private async Task<IntrospectionClient> InitializeIntrospectionClient(OAuth2IntrospectionOptions Options)
        {
            string endpoint;

            if (Options.IntrospectionEndpoint.IsPresent())
            {
                endpoint = Options.IntrospectionEndpoint;
            }
            else
            {
                endpoint = await GetIntrospectionEndpointFromDiscoveryDocument(Options).ConfigureAwait(false);
                Options.IntrospectionEndpoint = endpoint;
            }

            IntrospectionClient client;
            if (Options.IntrospectionHttpHandler != null)
            {
                client = new IntrospectionClient(
                    endpoint,
                    innerHttpMessageHandler: Options.IntrospectionHttpHandler);
            }
            else
            {
                client = new IntrospectionClient(endpoint);
            }

            client.Timeout = Options.DiscoveryTimeout;
            return client;
        }

        public void PostConfigure(string name, OAuth2IntrospectionOptions options)
        {
            if (options.Authority.IsMissing() && options.IntrospectionEndpoint.IsMissing())
            {
                throw new InvalidOperationException("You must either set Authority or IntrospectionEndpoint");
            }

            if (options.ClientId.IsMissing() && options.IntrospectionHttpHandler == null)
            {
                throw new InvalidOperationException("You must either set a ClientId or set an introspection HTTP handler");
            }

            if (options.TokenRetriever == null)
            {
                throw new ArgumentException("TokenRetriever must be set", nameof(options.TokenRetriever));
            }

            if (options.EnableCaching == true && _cache == null)
            {
                throw new ArgumentException("Caching is enabled, but no cache is found in the services collection", nameof(_cache));
            }

            options.IntrospectionClient = new AsyncLazy<IntrospectionClient>(()=>InitializeIntrospectionClient(options));
            options.LazyIntrospections =  new ConcurrentDictionary<string, AsyncLazy<IntrospectionResponse>>();


        }
    }
}
