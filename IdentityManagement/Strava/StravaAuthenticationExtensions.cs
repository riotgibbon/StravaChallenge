﻿using System;
using Owin;

namespace IdentityManagement.Strava
{
    public static class StravaAuthenticationExtensions
    {
        public static IAppBuilder UseStravaAuthentication(this IAppBuilder app,
            StravaAuthenticationOptions options)
        {
            if (app == null)
                throw new ArgumentNullException("app");
            if (options == null)
                throw new ArgumentNullException("options");

            app.Use(typeof(StravaAuthenticationMiddleware), app, options);

            return app;
        }

        public static IAppBuilder UseStravaAuthentication(this IAppBuilder app)
        {
            return UseStravaAuthentication(app, new StravaAuthenticationOptions
            {
                Host = StravaConfiguration.StravaHost,
                ClientId = StravaConfiguration.ClientId,
                ClientSecret = StravaConfiguration.ClientSecret
            });
        }

        public static IAppBuilder UseStravaAuthentication(this IAppBuilder app, string host, string clientId, string clientSecret)
        {
            return UseStravaAuthentication(app, new StravaAuthenticationOptions
            {
                Host = host,
                ClientId = clientId,
                ClientSecret = clientSecret
            });
        }
    }
}