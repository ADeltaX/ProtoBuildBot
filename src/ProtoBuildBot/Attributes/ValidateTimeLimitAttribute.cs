using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ProtoBuildBot.DataStore;

namespace ProtoBuildBot.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidateTimeLimitAttribute : Attribute, IResourceFilter
    {
        private readonly string _valueName = "";

        public ValidateTimeLimitAttribute() : this("t")
        { }

        public ValidateTimeLimitAttribute(string valueName) => _valueName = valueName;


        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (context.HttpContext.Request.Query.TryGetValue(_valueName, out var value))
            {
                var val = value[0];

                switch (TimeLimitGeneration.ValidateHashTime(val))
                {
                    case TimeLimitGeneration.TimeValidationResult.Valid:
                        return;

                    case TimeLimitGeneration.TimeValidationResult.Expired:
                        context.Result = new ContentResult
                        {
                            Content = "<html><body>TIME LIMIT REACHED</body></html>",
                            StatusCode = (int)HttpStatusCode.Forbidden,
                            ContentType = "text/html; charset=utf-8"
                        };
                        return;

                    case TimeLimitGeneration.TimeValidationResult.Invalid:
                    default:
                        context.Result = new ContentResult
                        {
                            Content = "<html><body>INVALID</body></html>",
                            StatusCode = (int)HttpStatusCode.Forbidden,
                            ContentType = "text/html; charset=utf-8"
                        };
                        return;
                }
            }
            else
            {
                //Can't get 't' value? forbidd the access to the resource.

                context.Result = new ContentResult
                {
                    Content = "<html><body>FORBIDDEN</body></html>",
                    StatusCode = (int)HttpStatusCode.Forbidden,
                    ContentType = "text/html; charset=utf-8"
                };
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}
