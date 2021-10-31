namespace PluginBase
{
	public interface IPlugin
	{
		void Render(float deltaTime);
		void Resize(int width, int height);
	}
}
