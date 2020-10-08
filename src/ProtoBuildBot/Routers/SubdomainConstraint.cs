using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProtoBuildBot.Routers
{
    public class SubdomainConstraint : IRouteConstraint
    {
        private readonly string[] _subdomains;

        public SubdomainConstraint(params string[] subdomains)
        {
            _subdomains = subdomains ?? throw new ArgumentNullException(nameof(subdomains));
        }

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            foreach (var subdomain in _subdomains)
                if (httpContext.Request.Host.Host.Contains(subdomain, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }
    }
}
