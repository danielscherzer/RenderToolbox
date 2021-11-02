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
		public string Name => nameof(Triangle);
		public float Hue { get; set; } = 7.5f;

		public View()
		{
			GL.Enable(EnableCap.Blend);
			//GL.Enable(EnableCap.DepthTest);
		}

		public void Render(float frameTime)
		{
			var saturation = MathF.Sin((float)_stopwatch.Elapsed.TotalSeconds) * 0.5f + 0.5f;
			var c = Color4.FromHsv(new Vector4(Hue / 10f, saturation, 0.75f, 1));
			GL.ClearColor(c);
			GL.Clear(ClearBufferMask.ColorBufferBit);
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
