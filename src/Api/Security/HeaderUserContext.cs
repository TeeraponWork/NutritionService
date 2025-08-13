using Application.Abstractions;

namespace Api.Security
{
    public sealed class HeaderUserContext : IUserContext
    {
        private readonly IHttpContextAccessor _http;

        public HeaderUserContext(IHttpContextAccessor http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        public Guid UserId
        {
            get
            {
                var h = _http.HttpContext?.Request.Headers["X-User-Id"].ToString();
                if (Guid.TryParse(h, out var id)) return id;
                throw new InvalidOperationException("Missing or invalid X-User-Id header.");
            }
        }
    }
}
