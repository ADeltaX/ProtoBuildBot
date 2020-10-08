using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ProtoBuildBot.Attributes
{
    /// <summary>
    /// Let's make a resource accessible only on specific subdomains
    /// Default ctor or empty allowed subdomains == nothing is accessible.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SubdomainFilterAttribute : Attribute, IResourceFilter
    {
        private readonly string[] _allowedSubdomains = Array.Empty<string>();
        private readonly string _domain = "";

        public SubdomainFilterAttribute() : this(SecretKeys.BaseHost)
        { }

        public SubdomainFilterAttribute(string domain, params string[] allowedSubdomains)
        {
            _domain = domain;
            _allowedSubdomains = allowedSubdomains ?? throw new ArgumentNullException(nameof(allowedSubdomains));
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            string url = context.HttpContext.Request.Host.Value;
            string scheme = context.HttpContext.Request.Scheme;
            if (Uri.TryCreate(scheme + @"://" + url, UriKind.Absolute, out Uri uri))
            {
                string subDomain = GetSubdomain(uri.DnsSafeHost, _domain);
                if (!_allowedSubdomains.Contains(subDomain, StringComparer.InvariantCultureIgnoreCase))
                    context.Result = new NotFoundResult();
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        /// <summary>Gets the subdomain portion of a url, given a known "root" domain</summary>
        public static string GetSubdomain(string url, string domain = null)
        {
            var subdomain = url;
            if (subdomain != null)
            {
                if (domain == null)
                {
                    // Since we were not provided with a known domain, assume that second-to-last period divides the subdomain from the domain.
                    var nodes = url.Split('.');
                    var lastNodeIndex = nodes.Length - 1;
                    if (lastNodeIndex > 0)
                        domain = nodes[lastNodeIndex - 1] + "." + nodes[lastNodeIndex];
                }

                // Verify that what we think is the domain is truly the ending of the hostname... otherwise we're hooped.
                if (!subdomain.EndsWith(domain, StringComparison.InvariantCultureIgnoreCase))
                    return subdomain;

                // Quash the domain portion, which should leave us with the subdomain and a trailing dot IF there is a subdomain.
                subdomain = subdomain.Replace(domain, "", StringComparison.InvariantCultureIgnoreCase);
                // Check if we have anything left.  If we don't, there was no subdomain, the request was directly to the root domain:
                if (string.IsNullOrWhiteSpace(subdomain))
                    return null;

                // Quash any trailing periods
                subdomain = subdomain.TrimEnd(new[] { '.' });
            }

            return subdomain;
        }
    }
}
