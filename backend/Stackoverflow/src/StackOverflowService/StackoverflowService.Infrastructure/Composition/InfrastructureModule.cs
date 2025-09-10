using Autofac;
using StackoverflowService.Domain.Repositories;
using StackoverflowService.Infrastructure.Repositories;
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
             .SingleInstance();

            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerLifetimeScope();
            builder.RegisterType<QuestionRepository>().As<IQuestionRepository>().InstancePerLifetimeScope();
            builder.RegisterType<AnswerRepository>().As<IAnswerRepository>().InstancePerLifetimeScope();
            builder.RegisterType<VoteRepository>().As<IVoteRepository>().InstancePerLifetimeScope();
        }
    }
}
