using StackoverflowService.Domain.Enums;
using System;

namespace StackoverflowService.Domain.Entities
{
    public class Vote
    {
        public string Id { get; }
        public string AnswerId { get; }
        public string UserId { get; }
        public VoteType Type { get; private set; }
        public DateTimeOffset CreationDate { get; private set; }

        public Vote(string id, string answerId, string userId, VoteType type,
                    DateTimeOffset? created = null)
        {
            Id = id;
            AnswerId = answerId;
            UserId = userId;
            Type = type;
            CreationDate = created ?? DateTimeOffset.UtcNow;
        }

        public void Switch(VoteType newType) => Type = newType;

    }
}
