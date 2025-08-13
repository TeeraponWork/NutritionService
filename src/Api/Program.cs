using Api.Security;
using Application;
using Application.Abstractions;
using Infrastructure;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();

// ���¡ŧ����¹�ҡ�����������
builder.Services.AddApplication();                         // MediatR + Validators
builder.Services.AddInfrastructure(builder.Configuration); // DbContext + Redis + etc.

// �ͧ Api �ͧ (�١�Ѻ HttpContext)
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, HeaderUserContext>(); // ��ҹ X-User-Id �ҡ Gateway

var app = builder.Build();

var sec = app.Services.GetRequiredService<IOptions<SecurityOptions>>().Value;

app.UseMiddleware<DevUserHeaderMiddleware>();
// �Դ���ҧ����ҧ˹�觵�� options
//if (sec.EnforceGatewayHeader)
//    app.UseMiddleware<UserContextMiddleware>();      // Prod
//else
//    app.UseMiddleware<DevUserHeaderMiddleware>();    // Dev/Test

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<UserContextMiddleware>();
app.MapControllers();

app.Run();
