using System.Reflection;
using System.Runtime.Loader;
using System;

namespace OpenTKPluginBrowser
{
	class PluginLoadContext : AssemblyLoadContext
	{
		private AssemblyDependencyResolver _resolver;

		public PluginLoadContext(string pluginPath) : base(isCollectible: true)
		{
			_resolver = new AssemblyDependencyResolver(pluginPath);
		}

		protected override Assembly Load(AssemblyName assemblyName)
		{
			//var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
			//if (assemblyPath != null)
			//{
			//	return LoadFromAssemblyPath(assemblyPath);
			//}
			//always returning null will use shared assemblies, no isolation https://stackoverflow.com/questions/58198370/assemblyloadcontext-dynamically-load-an-assembly-instantiate-an-object-and-cas
			return null;
		}

		protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
		{
			var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
			if (libraryPath != null)
			{
				return LoadUnmanagedDllFromPath(libraryPath);
			}

			return IntPtr.Zero;
		}
	}
}
