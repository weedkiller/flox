using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Fasterflect;

namespace S3.CoreServices.System.FastReflection
{
	public static class MethodExtensions
	{
		private const BindingFlags DefaultBindingFlags = BindingFlags.IgnoreCase 
		                                                 | BindingFlags.Public 
		                                                 | BindingFlags.Instance
		                                                 |BindingFlags.FlattenHierarchy;

		private static readonly ConcurrentDictionary<string, MethodInfo> CachedItems = new ConcurrentDictionary<string, MethodInfo>();

		public static MethodInfo GetMethodFast(this Type type, string methodName,BindingFlags bindingFlags=BindingFlags.Default)
		{
			bindingFlags = bindingFlags == BindingFlags.Default ? DefaultBindingFlags : bindingFlags;
			return CachedItems.GetOrAdd(ResolveKey(type, methodName, bindingFlags),
				(k) => type.GetMethod(methodName, bindingFlags));
		}

		private static string ResolveKey(Type type, string methodName, BindingFlags bindingFlags)
		{
			return $"{type.FullName}.{methodName}.{bindingFlags}";
		}


		private static readonly ConcurrentDictionary<string, MethodInvocationHandler> MethodMetaCache=new ConcurrentDictionary<string, MethodInvocationHandler>();
		public static MethodInvocationHandler GetFastMethodInvocationHandler(this Type type,string methodName,BindingFlags bindingFlags=BindingFlags.Default)
		{
			
			return MethodMetaCache.GetOrAdd(ResolveKey(type, methodName, bindingFlags), k =>
			{
				bindingFlags = bindingFlags == BindingFlags.Default ? DefaultBindingFlags : bindingFlags;
				var method=type.GetMethodFast(methodName,bindingFlags);

				var parameterTypes = method.Parameters().Select(x=>x.ParameterType).ToArray();
				return new MethodInvocationHandler(type.DelegateForCallMethod(methodName,bindingFlags, parameterTypes))
				{
					ArgumentTypes = parameterTypes
				};
			});
		}


	}
}