using System.Threading.Tasks;
using LogProcessor.Models;
using Microsoft.Extensions.Caching.Memory;

namespace LogProcessor.Services
{
    public interface IConfiguredMemoryCache : IMemoryCache
    {
        Task CacheError(ErrorResult error);
    }
}