﻿using System;
using System.Threading.Tasks;
using Foundatio.Caching;
using Xunit;

namespace Foundatio.Tests.Caching {
    public abstract class CacheClientTestsBase {
        protected virtual ICacheClient GetCacheClient() {
            return null;
        }

        public virtual void CanSetAndGetValue() {
            var cache = GetCacheClient();
            if (cache == null)
                return;
            
            cache.FlushAll();

            cache.Set("test", 1);
            var value = cache.Get<int>("test");
            Assert.Equal(1, value);
        }

        public virtual void CanSetAndGetObject() {
            var cache = GetCacheClient();
            if (cache == null)
                return;

            cache.FlushAll();

            var dt = DateTimeOffset.Now;
            cache.Set("test", new MyData { Type = "test", Date = dt, Message = "Hello World" });
            var value = cache.Get<MyData>("test");
            Assert.NotNull(value);
            Assert.Equal(dt, value.Date);
            Assert.Equal("Hello World", value.Message);
            Assert.Equal("test", value.Type);
        }

        public virtual void CanSetExpiration() {
            var cache = GetCacheClient();
            if (cache == null)
                return;

            cache.FlushAll();

            var expiresAt = DateTime.UtcNow.AddMilliseconds(150);
            cache.Set("test", 1, expiresAt);
            Assert.Equal(1, cache.Get<int>("test"));
            Assert.Equal(expiresAt.ToString(), cache.GetExpiration("test").Value.ToString());
     
            Task.Delay(TimeSpan.FromMilliseconds(250)).Wait();
            Assert.Equal(0, cache.Get<int>("test"));
            Assert.Null(cache.GetExpiration("test"));
        }
    }

    public class MyData
    {
        public string Type { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Message { get; set; }
    }
}