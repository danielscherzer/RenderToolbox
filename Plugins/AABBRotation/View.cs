using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using PluginBase;
using System;
using System.Diagnostics;
using System.Drawing;

namespace Triangle
{
	using Line = Tuple<Vector2, Vector2>;

	/// Example class handling the rendering for OpenGL.
	public class View : IDisposable, IPlugin
	{
		public View()
		{
			GL.Enable(EnableCap.Blend);
			GL.Disable(EnableCap.DepthTest);
			GL.Enable(EnableCap.LineSmooth);
			GL.ClearColor(Color.White);
			GL.LineWidth(5f);
		}

		public float Angle { get; set; } = 0f;

		public void Render(float frameTime)
		{
			Angle += 15f * frameTime;
			var newStick = RotateLine(_stick, Angle);


			GL.Clear(ClearBufferMask.ColorBufferBit);
			GL.LoadIdentity();
			GL.Color3(Color.CornflowerBlue);
			DrawLine(newStick);

			GL.Color3(Color.YellowGreen);
			DrawAABB(new Box2(newStick.Item1, newStick.Item2));

			GL.Color3(Color.Black);
			DrawAABB(new Box2(-0.01f, -0.01f, 0.01f, 0.01f));
		}

		public void Resize(int width, int height)
		{

		}

		public void Dispose()
		{
		}

		private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

		public string Name => "AABB Rotation";

		private const float size = 0.7f;
		private readonly Line _stick = new(new Vector2(-size, -size), new Vector2(size, size));

		private static Line RotateLine(Line stick, float rotationAngleDegrees)
		{
			var mtxRotation = Matrix2.CreateRotation(MathHelper.DegreesToRadians(rotationAngleDegrees));
			var a = Vector2.TransformRow(stick.Item1, mtxRotation);
			var b = Vector2.TransformRow(stick.Item2, mtxRotation);
			return new Line(a, b);
		}

		private static void DrawLine(in Line stick)
		{
			GL.Begin(PrimitiveType.Lines);
			GL.Vertex2(stick.Item1.X, stick.Item1.Y);
			GL.Vertex2(stick.Item2.X, stick.Item2.Y);
			GL.End();
		}

		private static void DrawAABB(Box2 rect)
		{
			GL.Begin(PrimitiveType.LineLoop);
			GL.Vertex2(rect.Min);
			GL.Vertex2(rect.Max.X, rect.Min.Y);
			GL.Vertex2(rect.Max);
			GL.Vertex2(rect.Min.X, rect.Max.Y);
			GL.End();
		}
	}
}
