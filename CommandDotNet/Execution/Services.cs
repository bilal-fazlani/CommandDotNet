using System;
using System.Collections.Generic;
using CommandDotNet.Extensions;

namespace CommandDotNet.Execution
{
    public class Services : IServices
    {
        private readonly Dictionary<Type, object> _servicesByType = new Dictionary<Type, object>();

        public void Add<T>(T value) where T : class
        {
            Add(typeof(T), value);
        }

        public void Add(Type type, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if(!type.IsInstanceOfType(value))
            {
                throw new ArgumentException($"{nameof(value)} '{value.GetType()}' must be an instance of {nameof(type)} '{type}'");
            }
            if (_servicesByType.ContainsKey(type))
            {
                throw new ArgumentException($"service for {nameof(type)} '{type}' already exists '{_servicesByType[type]}'");
            }

            _servicesByType.Add(type, value);
        }

        public void AddOrUpdate<T>(T value) where T : class
        {
            AddOrUpdate(typeof(T), value);
        }

        public void AddOrUpdate(Type type, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (!type.IsInstanceOfType(value))
            {
                throw new ArgumentException($"{nameof(value)} '{value.GetType()}' must be an instance of {nameof(type)} '{type}'");
            }

            _servicesByType[type] = value;
        }

        public T GetOrAdd<T>(Func<T> factory) where T : class
        {
            return (T) GetOrAdd(typeof(T), factory);
        }

        public object GetOrAdd(Type type, Func<object> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            var value = GetOrDefault(type);
            if (value == null)
            {
                value = factory();
                if (value == null)
                {
                    throw new AppRunnerException($"'{factory}' returned null");
                }
                Add(type, value);
            }

            return value;
        }

        public T? GetOrDefault<T>() where T : class
        {
            return (T?)GetOrDefault(typeof(T));
        }

        public object? GetOrDefault(Type type)
        {
            return _servicesByType.TryGetValue(type, out var value)
                ? value
                : null;
        }

        public T GetOrThrow<T>() where T : class
        {
            return (T)GetOrThrow(typeof(T));
        }

        public object GetOrThrow(Type type)
        {
            if (!_servicesByType.ContainsKey(type))
            {
                throw new ArgumentOutOfRangeException($"no service exists for type '{type}'");
            }
            return _servicesByType[type];
        }

        public ICollection<KeyValuePair<Type, object>> GetAll()
        {
            return _servicesByType.ToCollection();
        }
    }
}