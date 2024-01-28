using System.Collections.Generic;
using System;
namespace Services
{
	public class GameServiceContainer : IServiceProvider
	{
		private Dictionary<Type, object> Container = new Dictionary<Type, object>();
		public void AddService(Type interfaceType, object service)
		{
			if (interfaceType == null || service == null)
			{
				return;
			}
			if (Container.ContainsKey(interfaceType))
			{
				return;
			}
			Container.Add(interfaceType, service);
		}
		public void AddService<T>(T provider) where T : class
		{
			if (provider == null)
			{
				return;
			}
			AddService(typeof(T), provider);
		}
		public object GetService(Type interfaceType)
		{
			if (interfaceType == null)
			{
				return null;
			}
			if (!Container.ContainsKey(interfaceType))
			{
				return null;
			}
			return Container[interfaceType];
		}
		public T GetService<T>() where T : class
		{
			if (typeof(T) == null)
			{
				return null;
			}
			if (!Container.ContainsKey(typeof(T)))
			{
				return null;
			}
			return (T)Container[typeof(T)];
		}
		public void RemoveService(Type interfaceType)
		{
			if (interfaceType == null)
			{
				return;
			}
			if (!Container.ContainsKey(interfaceType))
			{
				return;
			}
			Container.Remove(interfaceType);
		}
		public void RemoveService<T>() where T : class
		{
			if (typeof(T) == null)
			{
				return;
			}
			if (!Container.ContainsKey(typeof(T)))
			{
				return;
			}
			Container.Remove(typeof(T));
		}
	}
}
