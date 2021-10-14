using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace HttpEnricher
{
    public static class HttpContextExtensions
    {
        public static string GetOrAddHeader(this HttpContext context, string header)
        {
            string sessionId;
            if (context.Request.Headers.TryGetValue(header, out var s))
            {
                sessionId = s.First();
                context.Response.Headers.TryAdd(header, sessionId);
                return sessionId;
            }

            sessionId = Guid.NewGuid().ToString();
            var stringValues = new StringValues(sessionId);
            context.Request.Headers.Add(header, stringValues);
            context.Response.Headers.Add(header, stringValues);
            return sessionId;
        }
    }
}