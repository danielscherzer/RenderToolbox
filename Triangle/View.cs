using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using PluginBase;
using System;
using System.Diagnostics;

namespace Triangle
{
	/// Example class handling the rendering for OpenGL.
	public class View : IDisposable, IPlugin
	{
		public View()
		{
			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.DepthTest);
		}

		public void Render(float frameTime)
		{
			var hue = (float)_stopwatch.Elapsed.TotalSeconds * 0.15f % 1;
			var c = Color4.FromHsv(new Vector4(hue, 0.75f, 0.75f, 1));
			GL.ClearColor(c);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.LoadIdentity();
			GL.Begin(PrimitiveType.Triangles);

			GL.Color4(Color4.Red);
			GL.Vertex2(0.0f, 0.5f);

			GL.Color4(Color4.Green);
			GL.Vertex2(0.58f, -0.5f);

			GL.Color4(Color4.Blue);
			GL.Vertex2(-0.58f, -0.5f);

			GL.End();
			GL.Finish();
		}

		public void Resize(int width, int height)
		{

		}

		public void Dispose()
		{
		}

		private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
	}
}
