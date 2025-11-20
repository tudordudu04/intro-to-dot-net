namespace ProiectIndividual.Middleware;

public class CorrelationMiddleware(RequestDelegate next, ILogger<CorrelationMiddleware> logger)
{
    private const string HeaderKey = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[HeaderKey].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = GenerateCorrelationId();
        }

        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.ContainsKey(HeaderKey))
                context.Response.Headers.Add(HeaderKey, correlationId);
            return Task.CompletedTask;
        });

        using (logger.BeginScope(new Dictionary<string, object>
               {
                   ["CorrelationId"] = correlationId,
                   ["TraceId"] = context.TraceIdentifier
               }))
        {
            await next(context);
        }
    }

    private static string GenerateCorrelationId()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 8);
    }
}