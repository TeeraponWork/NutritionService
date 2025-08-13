using Microsoft.Extensions.Options;

namespace Api.Security
{
    public sealed class DevUserHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SecurityOptions _opt;

        public DevUserHeaderMiddleware(RequestDelegate next, IOptions<SecurityOptions> options)
        {
            _next = next;
            _opt = options.Value;
        }

        public async Task Invoke(HttpContext ctx)
        {
            // ข้าม swagger/health ได้ด้วย หากต้องการ:
            // if (ctx.Request.Path.StartsWithSegments("/swagger")) { await _next(ctx); return; }

            if (!ctx.Request.Headers.ContainsKey("X-User-Id"))
            {
                var userId = _opt.DevUserId ?? "8a9defb6-e562-405b-9ac1-672e238bd20f";
                ctx.Request.Headers.Append("X-User-Id", userId);
            }
            await _next(ctx);
        }
    }
}
