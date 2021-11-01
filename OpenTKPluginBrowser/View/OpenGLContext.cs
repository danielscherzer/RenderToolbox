using OpenTK.Graphics.Wgl;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Windows.Interop;
using OpenTK.Wpf;
using System.Windows;
using Window = System.Windows.Window;
using WindowState = OpenTK.Windowing.Common.WindowState;

namespace OpenTKPluginBrowser
{
	internal class OpenGLContext : IDisposable
	{
		private readonly IDisposable[] _sharedContextResources;

		public OpenGLContext(int majorVersion = 3, int minorVersion = 3, ContextFlags contextFlags = ContextFlags.Default, ContextProfile contextProfile = ContextProfile.Any)
		{
			var nws = NativeWindowSettings.Default;
			nws.StartFocused = false;
			nws.StartVisible = false;
			nws.NumberOfSamples = 0;
			// if we ask GLFW for 1.0, we should get the highest level context available with full compat.
			nws.APIVersion = new Version(majorVersion, minorVersion);
			nws.Flags = ContextFlags.Offscreen | contextFlags;
			// we have to ask for any compat in this case.
			nws.Profile = contextProfile;
			nws.WindowBorder = WindowBorder.Hidden;
			nws.WindowState = WindowState.Minimized;
			var glfwWindow = new NativeWindow(nws);
			var provider = new GLFWBindingsContext();
			Wgl.LoadBindings(provider);
			// we're already in a window context, so we can just cheat by creating a new dependency object here rather than passing any around.
			var depObject = new DependencyObject();
			// retrieve window handle/info
			var window = Window.GetWindow(depObject);
			var baseHandle = window is null ? IntPtr.Zero : new WindowInteropHelper(window).Handle;
			var hwndSource = new HwndSource(0, 0, 0, 0, 0, "GLWpfControl", baseHandle);

			Context = glfwWindow.Context;
			_sharedContextResources = new IDisposable[] { hwndSource, glfwWindow };
			Context.MakeCurrent();
		}

		public IGLFWGraphicsContext Context { get; }

		public void Dispose()
		{
			foreach (var resource in _sharedContextResources)
			{
				resource.Dispose();
			}
		}
	}
}
