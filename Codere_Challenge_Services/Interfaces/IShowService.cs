﻿using Codere_Challenge_Domain.Entities;

namespace Codere_Challenge_Services.Interfaces
{
    public interface IShowService
    {
        Task<IEnumerable<Show>> GetShowsAsync();
        Task<Show> GetShowByIdAsync(int id);
        Task AddShowAsync(Show show);
    }
}
