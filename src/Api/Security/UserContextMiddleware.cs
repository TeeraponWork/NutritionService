namespace Api.Security
{
    public sealed class UserContextMiddleware
    {
        private readonly RequestDelegate _next;
        public UserContextMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext ctx)
        {
            // ป้องกันคนเรียกตรง: บังคับมี X-User-Id (มาจาก Gateway)
            if (!ctx.Request.Headers.ContainsKey("X-User-Id"))
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await ctx.Response.WriteAsync("Unauthorized: X-User-Id header required.");
                return;
            }
            await _next(ctx);
        }
    }
}
