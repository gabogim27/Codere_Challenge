﻿namespace Codere_Challenge_Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IShowRepository Shows { get; }
        Task<int> CompleteAsync();
    }
}
