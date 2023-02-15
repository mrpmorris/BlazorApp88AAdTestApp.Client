namespace BlazorApp88AAdTestApp.Server;

public class ContentSecurityPolicyMiddleware : IMiddleware
{
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		context.Response.Headers.Add("Content-Security-Policy", "default-src 'self' 'unsafe-eval' *;");
		await next(context);
	}
}
