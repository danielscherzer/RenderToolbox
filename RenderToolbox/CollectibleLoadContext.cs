using System.Reflection;
using System.Runtime.Loader;

namespace RenderToolbox
{
	internal class CollectibleLoadContext : AssemblyLoadContext
	{
		public CollectibleLoadContext(string pluginPath) : base(name: pluginPath, isCollectible: true)
		{
		}

		protected override Assembly? Load(AssemblyName assemblyName)
		{
			//all the dependency assemblies are loaded into the default context, and the new context contains only the assemblies explicitly loaded into it.
			// https://docs.microsoft.com/en-us/dotnet/standard/assembly/unloadability
			return null;
			//no dependent assembly isolation https://stackoverflow.com/questions/58198370/assemblyloadcontext-dynamically-load-an-assembly-instantiate-an-object-and-cas
		}
	}
}
