﻿using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using NGraphics.Test;
using System.Linq;
using System.Threading.Tasks;

namespace NGraphics.Android.Test
{
    [Activity(Label = "NGraphics.Android.Test", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            RunUnitTests();

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);

            button.Click += delegate
            {
                button.Text = string.Format("{0} clicks!", count++);
            };
        }

        async Task RunUnitTests()
        {
            var tat = typeof(NUnit.Framework.TestAttribute);
            var tfat = typeof(NUnit.Framework.TestFixtureAttribute);

            var types = typeof(DrawingTest).Assembly.GetTypes();
            var tfts = types.Where(t => t.GetCustomAttributes(tfat, false).Length > 0);

            var ngd = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            PlatformTest.ResultsDirectory = System.IO.Path.Combine(ngd, "TestResults");
            PlatformTest.Platform = Platforms.Current;
            System.IO.Directory.CreateDirectory(PlatformTest.ResultsDirectory);

            var resultDir = System.IO.Path.Combine(PlatformTest.ResultsDirectory, "Android");
            System.IO.Directory.CreateDirectory(resultDir);
            //System.Environment.CurrentDirectory = PlatformTest.ResultsDirectory;

            foreach (var t in tfts)
            {
                var test = Activator.CreateInstance(t);
                var ms = t.GetMethods().Where(m => m.GetCustomAttributes(tat, true).Length > 0);
                foreach (var m in ms)
                {

                    if (m.Name.Equals("ErulisseuiinSpaceshipPack"))
                    {
                        try
                        {
                            var r = m.Invoke(test, null);
                            var ta = r as Task;
                            if (ta != null)
                                await ta;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                  
                }
            }

            var client = new System.Net.WebClient();
            var pngs = System.IO.Directory.GetFiles(resultDir, "*.png").ToList();
            foreach (var f in pngs)
            {
                try
                {
                    var fileName = System.IO.Path.GetFileName(f);
                    client.UploadData("http://10.0.0.14:1234/" + fileName, System.IO.File.ReadAllBytes(f));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}

