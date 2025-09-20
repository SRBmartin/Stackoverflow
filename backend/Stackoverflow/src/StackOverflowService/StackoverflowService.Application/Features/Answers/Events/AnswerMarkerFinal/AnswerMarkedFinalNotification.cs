using MediatR;

namespace StackoverflowService.Application.Features.Answers.Events.AnswerMarkerFinal
{
    public class AnswerMarkedFinalNotification : INotification
    {
        public string QuestionId { get; }

        public AnswerMarkedFinalNotification(string questionId)
        {
            QuestionId = questionId;
        }

    }
}
