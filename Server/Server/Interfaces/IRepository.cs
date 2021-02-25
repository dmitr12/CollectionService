using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<long> AddItem(T item);
        Task<T> GetItemById(int id);
        Task UpdateItem(T item);
        Task DeleteItem(int id);
        Task<IEnumerable<T>> GetAllItems();
    }
}
