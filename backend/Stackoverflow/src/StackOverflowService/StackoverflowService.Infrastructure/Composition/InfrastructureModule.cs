using Autofac;
using StackoverflowService.Application.Abstractions;
using StackoverflowService.Domain.Repositories;
using StackoverflowService.Infrastructure.Repositories;
using StackoverflowService.Infrastructure.Security;
using StackoverflowService.Infrastructure.Storage;
using StackoverflowService.Infrastructure.Tables.Context;
using StackoverflowService.Infrastructure.Tables.Interfaces;

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

            builder.RegisterType<UserPhotoStorage>()
                .As<IPhotoStorage>()
                .SingleInstance();

            builder.RegisterType<PasswordHasherPbkdf2>()
                .As<IPasswordHasher>()
                .SingleInstance();

            builder.RegisterType<IdentityService>()
                .As<IIdentityService>()
                .SingleInstance();

            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerLifetimeScope();
            builder.RegisterType<QuestionRepository>().As<IQuestionRepository>().InstancePerLifetimeScope();
            builder.RegisterType<AnswerRepository>().As<IAnswerRepository>().InstancePerLifetimeScope();
            builder.RegisterType<VoteRepository>().As<IVoteRepository>().InstancePerLifetimeScope();
        }
    }
}
