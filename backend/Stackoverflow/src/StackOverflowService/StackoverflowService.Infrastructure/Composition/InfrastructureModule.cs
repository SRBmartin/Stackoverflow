using Autofac;
using StackoverflowService.Application.Abstractions;
using StackoverflowService.Domain.Repositories;
using StackoverflowService.Infrastructure.Configuration;
using StackoverflowService.Infrastructure.Repositories;
using StackoverflowService.Infrastructure.Security;
using StackoverflowService.Infrastructure.Storage;
using StackoverflowService.Infrastructure.Tables.Context;
using StackoverflowService.Infrastructure.Tables.Interfaces;
using System.Configuration;
using System;
using System.Net.Http.Headers;
using System.Net.Http;
using StackoverflowService.Infrastructure.Email;
using StackoverflowService.Infrastructure.Queues.Context;
using StackoverflowService.Infrastructure.Queues.Interfaces;
using StackoverflowService.Infrastructure.Queues.Services;

namespace StackoverflowService.Infrastructure.Composition
{
    public sealed class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TableContext>()
                 .As<ITableContext>()
                 .SingleInstance()
                 .AutoActivate();

            builder.RegisterType<QueueContext>()
                .As<IQueueContext>()
                .SingleInstance()
                .AutoActivate();

            builder.RegisterType<UserPhotoStorage>()
                .As<IPhotoStorage>()
                .SingleInstance();

            builder.RegisterType<QuestionPhotoStorage>()
                .As<IQuestionPhotoStorage>()
                .SingleInstance();

            builder.RegisterType<PhotoReader>()
                .As<IPhotoReader>()
                .SingleInstance();

            builder.RegisterType<PasswordHasherPbkdf2>()
                .As<IPasswordHasher>()
                .SingleInstance();

            builder.RegisterType<IdentityService>()
                .As<IIdentityService>()
                .SingleInstance();

            builder.RegisterType<IdentityValidator>()
                .As<IIdentityValidator>()
                .SingleInstance();

            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerLifetimeScope();
            builder.RegisterType<QuestionRepository>().As<IQuestionRepository>().InstancePerLifetimeScope();
            builder.RegisterType<AnswerRepository>().As<IAnswerRepository>().InstancePerLifetimeScope();
            builder.RegisterType<VoteRepository>().As<IVoteRepository>().InstancePerLifetimeScope();

            builder.RegisterType<FinalAnswerQueue>()
                .As<IFinalAnswerQueue>()
                .SingleInstance();

            builder.Register(ctx =>
            {
                var baseUrl = ConfigurationManager.AppSettings["EmailApi:BaseAddress"];
                if (string.IsNullOrWhiteSpace(baseUrl))
                    throw new ConfigurationErrorsException("Missing appSettings key 'EmailApi:BaseAddress'.");

                var timeoutRaw = ConfigurationManager.AppSettings["EmailApi:TimeoutSeconds"];
                int timeout = 100;
                if (!string.IsNullOrWhiteSpace(timeoutRaw) && int.TryParse(timeoutRaw, out var t) && t > 0)
                    timeout = t;

                var token = ConfigurationManager.AppSettings["EmailApi:Token"] ?? string.Empty;

                return new EmailApiOptions
                {
                    BaseAddress = new Uri(baseUrl, UriKind.Absolute),
                    TimeoutSeconds = timeout,
                    Token = token
                };
            })
            .AsSelf()
            .SingleInstance();

            builder.Register(ctx =>
            {
                var opts = ctx.Resolve<EmailApiOptions>();

                var http = new HttpClient(new HttpClientHandler(), disposeHandler: true)
                {
                    BaseAddress = opts.BaseAddress,
                    Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds)
                };

                http.DefaultRequestHeaders.Accept.Clear();
                http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrWhiteSpace(opts.Token))
                {
                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", opts.Token);
                }

                return http;
            })
            .Named<HttpClient>("EmailApiClient")
            .SingleInstance();

            builder.Register(ctx =>
            {
                var http = ctx.ResolveNamed<HttpClient>("EmailApiClient");
                var questions = ctx.Resolve<IQuestionRepository>();
                var answers = ctx.Resolve<IAnswerRepository>();
                var users = ctx.Resolve<IUserRepository>();

                return new EmailClient(http, questions, answers, users);
            })
            .As<IEmailClient>()
            .SingleInstance();

        }
    }
}
