﻿// Copyright (c) Dominick Baier & Brock Allen. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication;
using IdentityModel.AspNetCore.OAuth2Introspection.Infrastructure;
using IdentityModel.Client;
using System.Collections.Concurrent;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Options class for the OAuth 2.0 introspection endpoint authentication middleware
    /// </summary>
    public class OAuth2IntrospectionOptions : AuthenticationSchemeOptions
    {
        public OAuth2IntrospectionOptions()
        {
        //    AuthenticationScheme = "Bearer";
        //    AutomaticAuthenticate = true;
        }

        /// <summary>
        /// Sets the base-path of the token provider.
        /// If set, the OpenID Connect discovery document will be used to find the introspection endpoint.
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Sets the URL of the introspection endpoint.
        /// If set, Authority is ignored.
        /// </summary>
        public string IntrospectionEndpoint { get; set; }

        /// <summary>
        /// Specifies the id of the introspection client.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Specifies the secret of the introspection client.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Specifies the claim type to use for the name claim (defaults to 'name')
        /// </summary>
        public string NameClaimType { get; set; } = "name";

        /// <summary>
        /// Specifies the claim type to use for the role claim (defaults to 'role')
        /// </summary>
        public string RoleClaimType { get; set; } = "role";

        /// <summary>
        /// Specifies the timout for contacting the discovery endpoint
        /// </summary>
        public TimeSpan DiscoveryTimeout { get; set; } = TimeSpan.FromSeconds(60);

        /// <summary>
        /// Specifies the HTTP handler for the discovery endpoint
        /// </summary>
        public HttpMessageHandler DiscoveryHttpHandler { get; set; }

        /// <summary>
        /// Specifies the timeout for contacting the introspection endpoint
        /// </summary>
        public TimeSpan IntrospectionTimeout { get; set; } = TimeSpan.FromSeconds(60);

        /// <summary>
        /// Specifies the HTTP handler for the introspection endpoint
        /// </summary>
        public HttpMessageHandler IntrospectionHttpHandler { get; set; }

        /// <summary>
        /// Specifies whether tokens that contain dots (most likely a JWT) are skipped
        /// </summary>
        public bool SkipTokensWithDots { get; set; } = true;

        /// <summary>
        /// Specifies whether the token should be stored
        /// </summary>
        public bool SaveToken { get; set; } = true;

        /// <summary>
        /// Specifies whether the outcome of the toke validation should be cached. This reduces the load on the introspection endpoint at the STS
        /// </summary>
        public bool EnableCaching { get; set; } = false;

        /// <summary>
        /// Specifies for how long the outcome of the token validation should be cached.
        /// </summary>
        public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(10);

        /// <summary>
        /// Specifies the method how to retrieve the token from the HTTP request
        /// </summary>
        public Func<HttpRequest, string> TokenRetriever { get; set; } = TokenRetrieval.FromAuthorizationHeader();
        public AsyncLazy<IntrospectionClient> IntrospectionClient { get; internal set; }
        public ConcurrentDictionary<string, AsyncLazy<IntrospectionResponse>> LazyIntrospections { get; internal set; }
    }
}