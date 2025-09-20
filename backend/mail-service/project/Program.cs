using MediatR;
using MailService.Infrastructure.Services;
using MailService.Infrastructure.Services.Configuration;
using MailService.Application.Contracts.Services;
using MailService.Controllers.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));

var mailSettings = builder.Configuration.GetSection("MailSettings").Get<MailSettings>();

builder.Services.AddControllers();

builder.Services.AddMediatR(typeof(Program));

builder.Services.AddSingleton<IMailSenderService>(new MailSenderService(mailSettings));
builder.Services.AddScoped<BearerAuthAttribute>();

var app = builder.Build();

app.UseAuthentication();
app.MapControllers();
app.Run();