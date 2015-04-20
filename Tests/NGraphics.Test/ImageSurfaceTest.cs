﻿using NUnit.Framework;
using System;
using NGraphics;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NGraphics.Codes;
using NGraphics.Interfaces;
using NGraphics.Models;
using NGraphics.Models.Brushes;
using Path = NGraphics.Models.Elements.Path;

namespace NGraphics.Test
{
	public class PlatformTest
	{
		public static class Platforms
		{
			public static IPlatform Current { get { return PlatformTest.Platform; } }
		}

    public static IPlatform Platform =
#if NETFX_CORE
			new WindowsXamlPlatform ();
#else
 new NullPlatform();
#endif

		public static string ResultsDirectory = "";
		public static string GetPath (string filename)
		{
			var name = (filename.EndsWith (".png", StringComparison.Ordinal)) ?
				System.IO.Path.GetFileNameWithoutExtension (filename) :
				filename;
			var path = System.IO.Path.Combine (ResultsDirectory,Platform.Name, name + "-" + Platform.Name + ".png");
			Debug.WriteLine (path);
			return path;
		}
		public Stream OpenResource (string path)
		{
			if (string.IsNullOrEmpty (path))
				throw new ArgumentException ("path");
			var ty = typeof(PathTests);
			var ti = ty.GetTypeInfo ();
			var assembly = ti.Assembly;
			var resources = assembly.GetManifestResourceNames ();
			return assembly.GetManifestResourceStream ("NGraphics.Test.Inputs." + path);
		}
		public IImage GetResourceImage (string name)
		{
			using (var s = OpenResource (name)) {
				return Platform.LoadImage (s);
			}
		}

    public static Func<string, Stream> OpenStream = p => null;
    public static Func<Stream, string, Task> CloseStream = (s,n) => Task.FromResult<object>(null);

    public async Task SaveImage(IImage i, string name)
    {
      var path = GetPath(name);
      using (var s = OpenStream(path))
      {
        if (s == null)
        {
          i.SaveAsPng(path);
        }
        else
        {
          i.SaveAsPng(s);
          await CloseStream(s,name);
        }
      }
    }

    public async Task SaveImage(IImageCanvas canvas, string name)
    {
      await SaveImage(canvas.GetImage(), name);
    }
	}

	[TestFixture]
	public class ImageCanvasTest : PlatformTest
	{
		[Test]
		public async Task BlurImage ()
		{
			var img = Platform.CreateImage (
				          new[] { Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow },
				          2);
			var canvas = Platform.CreateImageCanvas (new Size (100), transparency: true);
			canvas.DrawImage (img, new Rect (new Size (100)));
      await SaveImage(canvas, "ImageCanvas.BlurImage");
      //canvas.GetImage ().SaveAsPng (GetPath ("ImageCanvas.BlurImage"));
		}

		[Test]
		public async Task BlurImage2 ()
		{
			var img = Platform.CreateImage (
				new[] { Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow },
				2);
			var canvas = Platform.CreateImageCanvas (new Size (200, 100), transparency: true);
			canvas.DrawImage (img, new Rect (new Size (50)));
			canvas.DrawImage (img, new Rect (new Point (0, 50), new Size (50)));
			canvas.DrawImage (img, new Rect (new Point (50, 0), new Size (150, 50)));
			canvas.DrawImage (img, new Rect (new Point (50, 50), new Size (150, 50)));
      await SaveImage(canvas, "ImageCanvas.BlurImage2");
      //canvas.GetImage().SaveAsPng(GetPath("ImageCanvas.BlurImage2"));
		}

		[Test]
		public void Cats ()
		{
			var img = GetResourceImage ("cat.png");
			var canvas = Platform.CreateImageCanvas (new Size (100, 200), transparency: true);
			canvas.DrawImage (img, new Rect (new Size (50)));
			canvas.DrawImage (img, new Rect (new Point (50, 0), new Size (50)));
			canvas.DrawImage (img, new Rect (new Point (0, 50), new Size (50, 150)));
			canvas.DrawImage (img, new Rect (new Point (50, 50), new Size (50, 150)));
			canvas.GetImage ().SaveAsPng (GetPath ("ImageCanvas.Cats"));
		}

		[Test]
		public void TriWithRadGrad ()
		{
			var canvas = Platform.CreateImageCanvas (new Size (100), transparency: true);
			var size = new Size (100);
			var b = new RadialGradientBrush (
				new Point (0.5, 1), new Size(1),
				Colors.Yellow, Colors.Blue);
			var p = new Path ();
			p.MoveTo (0, 0, false);
			p.LineTo (size.Width, 0,false);
			p.LineTo (size.Width / 2, size.Height,false);
			p.Close ();
			p.Brush = b;
			p.Draw (canvas);
			canvas.GetImage ().SaveAsPng (GetPath ("ImageCanvas.TriWithRadGrad"));
		}
		[Test]
		public void Line ()
		{
			var canvas = Platform.CreateImageCanvas (new Size (100), transparency: true);
			canvas.DrawLine (10, 20, 80, 70, Colors.DarkGray,5);
			canvas.GetImage ().SaveAsPng (GetPath ("ImageCanvas.Line"));
		}
		[Test]
		public void Ellipse ()
		{
			var p = Platform;
			var s = p.CreateImageCanvas (new Size (100), transparency: true);
			s.DrawEllipse (new Point (10, 20), new Size (30, 40), Pens.Red.WithWidth (10), Brushes.Yellow);
			var i = s.GetImage ();
			var path = GetPath ("Ellipse");
			i.SaveAsPng (path);
		}
		[Test]
		public void EllipseWithoutBackground ()
		{
			var p = Platform;
			var s = p.CreateImageCanvas (new Size (100), transparency: false);
			s.DrawEllipse (new Point (10, 20), new Size (30, 40), Pens.Red.WithWidth (10), Brushes.Yellow);
			var i = s.GetImage ();
			var path = GetPath ("ImageCanvas.EllipseWithoutBackground");
			i.SaveAsPng (path);
		}
		[Test]
		public void EllipseWithBackground ()
		{
			var p = Platform;
			var s = p.CreateImageCanvas (new Size (100), transparency: false);
			s.FillRectangle (new Rect (new Size (100)), Colors.DarkGray);
			s.DrawEllipse (new Point (10, 20), new Size (30, 40), Pens.Red.WithWidth (10), Brushes.Yellow);
			var i = s.GetImage ();
			var path = GetPath ("ImageCanvas.EllipseWithBackground");
			i.SaveAsPng (path);
		}
	}
}

