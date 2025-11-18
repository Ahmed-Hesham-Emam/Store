using Domain.Contracts;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Presistence.Repositories
    {
    public class CacheRepository(IConnectionMultiplexer connection) : ICacheRepository
        {
        private readonly IDatabase _database = connection.GetDatabase();
        private bool IsConnected => connection.IsConnected;
        public async Task<string?> GetAsync(string key)
            {
            //var value = await _database.StringGetAsync(key);
            //return !value.IsNullOrEmpty ? value : default;

            if ( !IsConnected ) return null; // skip immediately if Redis is down

            try
                {
                var value = await _database.StringGetAsync(key);
                return !value.IsNullOrEmpty ? value : default;
                }
            catch
                {
                return null;
                }
            }

        public async Task SetAsync(string key, object value, TimeSpan duration)
            {
            //var redisValue = JsonSerializer.Serialize(value);
            //await _database.StringSetAsync(key, redisValue, duration);

            if ( !IsConnected ) return; // skip immediately if Redis is down

            try
                {
                var redisValue = JsonSerializer.Serialize(value);
                await _database.StringSetAsync(key, redisValue, duration);
                }
            catch
                {
                // ignore
                }
            }

        }
    }
