using StackoverflowService.Domain.Enums;
using StackoverflowService.Domain.ValueObjects;
using System;

#nullable enable

namespace StackoverflowService.Domain.Entities
{
    public class User
    {
        public string Id { get; }
        public string Name { get; private set; }
        public string Lastname { get; private set; }
        public DateTimeOffset CreationDate { get; private set; }
        public string Email { get; private set; }
        public Gender Gender { get; private set; }
        public string State { get; private set; }
        public string City { get; private set; }
        public string Address { get; private set; }
        public string PasswordHash { get; private set; }
        public PhotoRef Photo { get; private set; }

        public User(string id, string name, string lastname, string email, string passwordHash,
                    Gender gender = Gender.Male, string state = "", string city = "", string address = "",
                    PhotoRef? photo = null, DateTimeOffset? created = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? "";
            Lastname = lastname ?? "";
            Email = email ?? "";
            PasswordHash = passwordHash ?? "";
            Gender = gender;
            State = state ?? "";
            City = city ?? "";
            Address = address ?? "";
            Photo = photo;
            CreationDate = created ?? DateTimeOffset.UtcNow;
        }

        public void SetPhoto(PhotoRef photo) => Photo = photo;

    }
}
