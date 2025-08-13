namespace Api.Security
{
    public sealed class SecurityOptions
    {
        public bool EnforceGatewayHeader { get; set; } = true;     // prod = true
        public string? DevUserId { get; set; }                     // สำหรับ dev/test
    }
}
