using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ProtoBuildBot.Routers
{
    public class SubRouter : MvcRouteHandler, IRouter
    {
        private readonly string[] _allowedSubdomains = { "pca" };
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IActionInvokerFactory _actionInvokerFactory;
        private readonly IActionSelector _actionSelector;
        private readonly ILogger _logger;
        private readonly DiagnosticSource _diagnosticSource;


        public SubRouter(
            IActionInvokerFactory actionInvokerFactory,
            IActionSelector actionSelector,
            DiagnosticSource diagnosticSource,
            ILoggerFactory loggerFactory)
            : this(actionInvokerFactory, actionSelector, diagnosticSource, loggerFactory, actionContextAccessor: null)
        {
        }

        public SubRouter(IActionInvokerFactory actionInvokerFactory, IActionSelector actionSelector, DiagnosticSource diagnosticSource,
            ILoggerFactory loggerFactory, IActionContextAccessor actionContextAccessor)
            : base(actionInvokerFactory, actionSelector, diagnosticSource,
            loggerFactory, actionContextAccessor)
        {
            _actionContextAccessor = actionContextAccessor;
            _actionInvokerFactory = actionInvokerFactory;
            _actionSelector = actionSelector;
            _diagnosticSource = diagnosticSource;
            _logger = loggerFactory.CreateLogger<MvcRouteHandler>();
        }

        public new Task RouteAsync(RouteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            string url = context.HttpContext.Request.Host.Value;
            string scheme = context.HttpContext.Request.Scheme;
            if (Uri.TryCreate(scheme + @"://" + url, UriKind.Absolute, out Uri uri))
            {
                string subDomain = GetSubdomain(uri, SecretKeys.BaseHost);
                if (_allowedSubdomains.Contains(subDomain, StringComparer.InvariantCultureIgnoreCase))
                    context.RouteData.Values.Add("sub", subDomain);
            }

            var candidates = _actionSelector.SelectCandidates(context);
            if (candidates == null || candidates.Count == 0)
            {
                return Task.CompletedTask;
            }

            var actionDescriptor = _actionSelector.SelectBestCandidate(context, candidates);
            if (actionDescriptor == null)
            {
                return Task.CompletedTask;
            }

            context.Handler = (c) =>
            {
                var routeData = c.GetRouteData();

                var actionContext = new ActionContext(context.HttpContext, routeData, actionDescriptor);
                if (_actionContextAccessor != null)
                {
                    _actionContextAccessor.ActionContext = actionContext;
                }

                var invoker = _actionInvokerFactory.CreateInvoker(actionContext);
                if (invoker == null)
                {
                    throw new InvalidOperationException();
                }

                return invoker.InvokeAsync();
            };

            return Task.CompletedTask;
        }

        public static string GetSubdomain(Uri uri, string domain = null)
            => GetSubdomain(uri?.DnsSafeHost, domain);

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
                {
                    return subdomain;
                }

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
