using StackoverflowService.Domain.Enums;
using System;

namespace StackoverflowService.Domain.Entities
{
    public class Vote
    {
        public string Id { get; }
        public string UserId { get; }
        public VoteType Type { get; private set; }
        public DateTimeOffset CreationDate { get; private set; }

        public VoteTarget Target { get; private set; }
        public string TargetId { get; private set; }

        public bool IsQuestionVote => Target == VoteTarget.Question;
        public bool IsAnswerVote => Target == VoteTarget.Answer;

        public Vote(string id, string targetId, VoteTarget target, string userId, VoteType type,
                    DateTimeOffset? created = null)
        {
            Id = id;
            Target = target;
            TargetId = targetId;
            UserId = userId;
            Type = type;
            CreationDate = created ?? DateTimeOffset.UtcNow;
        }

        public static Vote ForQuestion(string id, string questionId, string userId, VoteType type, DateTimeOffset? created = null)
            => new Vote(id, questionId, VoteTarget.Question, userId, type, created);

        public static Vote ForAnswer(string id, string answerId, string userId, VoteType type, DateTimeOffset? created = null)
            => new Vote(id, answerId, VoteTarget.Answer, userId, type, created);

        public void Switch(VoteType newType) => Type = newType;

    }
}
