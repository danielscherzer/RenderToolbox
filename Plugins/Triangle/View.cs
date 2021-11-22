using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.ComponentModel;
using System.Diagnostics;
using Zenseless.Patterns;
using Zenseless.RenderToolbox;

namespace Triangle
{
	/// Example class handling the rendering for OpenGL.
	public class View : NotifyPropertyChanged, IPlugin
	{
		public string Name => nameof(Triangle);
		[Description("Range [0..10]")]
		public float Hue { get => _hue; set => Set(ref _hue, OpenTK.Mathematics.MathHelper.Clamp(value, 0f, 10f)); }

		[Description("Range [0..1]")]
		public float Saturation { get => _saturation; set => Set(ref _saturation, value); }

		public View()
		{
			GL.Enable(EnableCap.Blend);
			//GL.Enable(EnableCap.DepthTest);
		}

		public void Render(float frameTime)
		{
			Saturation = MathF.Sin((float)_stopwatch.Elapsed.TotalSeconds) * 0.25f + 0.5f;
			var c = Color4.FromHsv(new Vector4(Hue / 10f, Saturation, 0.75f, 1));
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

		private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
		private float _hue = 7.5f;
		private float _saturation;
	}
}
