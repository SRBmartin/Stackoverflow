using StackoverflowService.Domain.Entities;
using StackoverflowService.Domain.Enums;
using StackoverflowService.Domain.ValueObjects;
using StackoverflowService.Infrastructure.Tables.Entities;

namespace StackoverflowService.Infrastructure.Mapping
{
    internal static class TableMappers
    {
        public static User ToDomain(this UserEntity e) =>
            new User(
                id: e.RowKey,
                name: e.Name,
                lastname: e.Lastname,
                email: e.Email,
                passwordHash: e.PasswordHash,
                gender: ParseGender(e.Gender),
                state: e.State, city: e.City, address: e.Address,
                photo: string.IsNullOrWhiteSpace(e.PhotoBlobName) ? null : new PhotoRef(e.PhotoBlobName, e.PhotoContainer),
                created: e.CreationDate);

        public static UserEntity ToTable(this User u) => new UserEntity
        {
            PartitionKey = "USR",
            RowKey = u.Id,
            Name = u.Name,
            Lastname = u.Lastname,
            CreationDate = u.CreationDate,
            Email = u.Email,
            Gender = ToGenderString(u.Gender),
            State = u.State,
            City = u.City,
            Address = u.Address,
            PasswordHash = u.PasswordHash,
            PhotoBlobName = u.Photo?.BlobName,
            PhotoContainer = u.Photo?.Container
        };

        public static Question ToDomain(this QuestionEntity e) =>
            new Question(
                id: e.RowKey,
                userId: e.UserId,
                title: e.Title,
                description: e.Description,
                photo: string.IsNullOrWhiteSpace(e.PhotoBlobName) ? null : new PhotoRef(e.PhotoBlobName, e.PhotoContainer),
                created: e.CreationDate,
                isClosed: e.IsClosed,
                isDeleted: e.IsDeleted);

        public static QuestionEntity ToTable(this Question q) => new QuestionEntity
        {
            PartitionKey = q.UserId,
            RowKey = q.Id,
            UserId = q.UserId,
            Title = q.Title,
            Description = q.Description,
            PhotoBlobName = q.Photo?.BlobName,
            PhotoContainer = q.Photo?.Container,
            CreationDate = q.CreationDate,
            IsClosed = q.IsClosed,
            IsDeleted = q.IsDeleted
        };

        public static Answer ToDomain(this AnswerEntity e) =>
            new Answer(id: e.RowKey, questionId: e.QuestionId, text: e.Text,
                       created: e.CreationDate, isFinal: e.IsFinal, isDeleted: e.IsDeleted);

        public static AnswerEntity ToTable(this Answer a) => new AnswerEntity
        {
            PartitionKey = a.QuestionId,
            RowKey = a.Id,
            QuestionId = a.QuestionId,
            Text = a.Text,
            CreationDate = a.CreationDate,
            IsFinal = a.IsFinal,
            IsDeleted = a.IsDeleted
        };

        public static Vote ToDomain(this VoteEntity e) =>
            new Vote(id: e.RowKey, answerId: e.AnswerId, userId: e.UserId,
                     type: e.Type == "-" ? VoteType.Down : VoteType.Up,
                     created: e.CreationDate);

        public static VoteEntity ToTable(this Vote v) => new VoteEntity
        {
            PartitionKey = v.AnswerId,
            RowKey = v.Id,
            AnswerId = v.AnswerId,
            UserId = v.UserId,
            Type = v.Type == VoteType.Down ? "-" : "+",
            CreationDate = v.CreationDate
        };

        private static Gender ParseGender(string s)
        {
            switch ((s ?? "").Trim().ToUpperInvariant())
            {
                case "M": return Gender.Male;
                case "F": return Gender.Female;
                default: return Gender.Male;
            }
        }
        private static string ToGenderString(Gender g) =>
            g == Gender.Male ? "M" : "F";
    }
}
