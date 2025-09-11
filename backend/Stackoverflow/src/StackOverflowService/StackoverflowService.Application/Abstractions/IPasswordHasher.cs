﻿namespace StackoverflowService.Application.Abstractions
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string passwordHash);
        bool NeedsRehash(string passwordHash);
    }
}
