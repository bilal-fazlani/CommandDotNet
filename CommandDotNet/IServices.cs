using System;
using System.Collections.Generic;

namespace CommandDotNet
{
    /// <summary>
    /// Services and context data that can be stored with
    /// an object for later use.<br/>
    /// i.e. Middleware can store settings within the
    /// AppConfig.Services during initialization for use
    /// in the middleware pipeline.
    /// </summary>
    public interface IServices
    {
        void Add<T>(T value) where T : class;
        void Add(Type type, object value);
        void AddOrUpdate<T>(T value) where T : class;
        void AddOrUpdate(Type type, object value);
        T GetOrAdd<T>(Func<T> factory) where T : class;
        object GetOrAdd(Type type, Func<object> factory);
        T? GetOrDefault<T>() where T : class;
        object? GetOrDefault(Type type);
        T GetOrThrow<T>() where T : class;
        object GetOrThrow(Type type);
        ICollection<KeyValuePair<Type, object>> GetAll();
    }
}