using System;
using Owin;

namespace IdentityManagement.OpenAM
{
    public static class OpenAMAuthenticationExtensions
    {
        public static IAppBuilder UseOpenAMAuthentication(this IAppBuilder app,
            OpenAMAuthenticationOptions options)
        {
            if (app == null)
                throw new ArgumentNullException("app");
            if (options == null)
                throw new ArgumentNullException("options");

            app.Use(typeof(OpenAMAuthenticationMiddleware), app, options);

            return app;
        }

        public static IAppBuilder UseOpenAMAuthentication(this IAppBuilder app)
        {
            return UseOpenAMAuthentication(app, new OpenAMAuthenticationOptions
            {
                Host = OpenAMConfiguration.OpenAMHost,
                ClientId = OpenAMConfiguration.ClientId,
                ClientSecret = OpenAMConfiguration.ClientSecret
            });
        }

        public static IAppBuilder UseOpenAMAuthentication(this IAppBuilder app, string host, string clientId, string clientSecret)
        {
            return UseOpenAMAuthentication(app, new OpenAMAuthenticationOptions
            {
                Host = host,
                ClientId = clientId,
                ClientSecret = clientSecret
            });
        }
    }
}