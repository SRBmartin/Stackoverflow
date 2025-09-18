using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Questions;
using StackoverflowService.Domain.Repositories;
using System.Threading.Tasks;
using System.Threading;
using StackoverflowService.Application.Common;
using StackoverflowService.Application.DTOs.Users;

namespace StackoverflowService.Application.Features.Questions.DeleteQuestion
{
    public class DeleteQuestionCommandHandler : IRequestHandler<DeleteQuestionCommand, Result<bool>>
    {
        private readonly IQuestionRepository _questionRepository;

        public DeleteQuestionCommandHandler(
            IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public async Task<Result<bool>> Handle(DeleteQuestionCommand command, CancellationToken cancellationToken)
        {
            var question = await _questionRepository.GetAsync(command.UserId, command.QuestionId, cancellationToken);
            if (question is null)
            {
                return Result.Fail<bool>(Error.NotFound("Questions.NotFoundOrForbidden", "Question not found or you do not have permission to delete it."));
            }

            question.Delete();

            await _questionRepository.UpdateAsync(question, cancellationToken);

            return Result.Ok(true);
        }

    }
}
