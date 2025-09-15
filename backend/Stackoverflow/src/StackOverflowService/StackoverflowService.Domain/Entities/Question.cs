using StackoverflowService.Domain.ValueObjects;
using System;

#nullable enable

namespace StackoverflowService.Domain.Entities
{
    public class Question
    {
        public string Id { get; }
        public string UserId { get; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public PhotoRef? Photo { get; private set; }
        public DateTimeOffset CreationDate { get; private set; }
        public bool IsClosed { get; private set; }
        public bool IsDeleted { get; private set; }

        public Question(string id, string userId, string title, string description,
                        PhotoRef? photo = null, DateTimeOffset? created = null,
                        bool isClosed = false, bool isDeleted = false)
        {
            Id = id;
            UserId = userId;
            Title = title ?? "";
            Description = description ?? "";
            Photo = photo;
            CreationDate = created ?? DateTimeOffset.UtcNow;
            IsClosed = isClosed;
            IsDeleted = isDeleted;
        }

        public void Edit(string title, string text)
        {
            Title = title;
            Description = text;
        }
        public void SetPhoto(PhotoRef photo) => Photo = photo;
        public void Close() => IsClosed = true;
        public void Delete() => IsDeleted = true;

    }
}
