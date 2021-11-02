namespace PluginBase
{
	public interface IPlugin
	{
		string Name { get; }
		void Render(float deltaTime);
		void Resize(int width, int height);
	}
}
