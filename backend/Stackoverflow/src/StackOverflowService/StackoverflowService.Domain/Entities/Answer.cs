using System;

namespace StackoverflowService.Domain.Entities
{
    public class Answer
    {
        public string Id { get; }
        public string QuestionId { get; }
        public string Text { get; private set; }
        public DateTimeOffset CreationDate { get; private set; }
        public bool IsFinal { get; private set; }
        public bool IsDeleted { get; private set; }

        public Answer(string id, string questionId, string text,
                      DateTimeOffset? created = null, bool isFinal = false, bool isDeleted = false)
        {
            Id = id;
            QuestionId = questionId;
            Text = text ?? "";
            CreationDate = created ?? DateTimeOffset.UtcNow;
            IsFinal = isFinal;
            IsDeleted = isDeleted;
        }

        public void MarkFinal() => IsFinal = true;
        public void Delete() => IsDeleted = true;

    }
}
