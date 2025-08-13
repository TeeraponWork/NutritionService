using Api.Security;
using Application;
using Application.Abstractions;
using Infrastructure;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();

// เรียกลงทะเบียนจากแต่ละเลเยอร์
builder.Services.AddApplication();                         // MediatR + Validators
builder.Services.AddInfrastructure(builder.Configuration); // DbContext + Redis + etc.

// ของ Api เอง (ผูกกับ HttpContext)
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, HeaderUserContext>(); // อ่าน X-User-Id จาก Gateway

var app = builder.Build();

var sec = app.Services.GetRequiredService<IOptions<SecurityOptions>>().Value;

app.UseMiddleware<DevUserHeaderMiddleware>();
// เปิดอย่างใดอย่างหนึ่งตาม options
//if (sec.EnforceGatewayHeader)
//    app.UseMiddleware<UserContextMiddleware>();      // Prod
//else
//    app.UseMiddleware<DevUserHeaderMiddleware>();    // Dev/Test

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<UserContextMiddleware>();
app.MapControllers();

app.Run();
