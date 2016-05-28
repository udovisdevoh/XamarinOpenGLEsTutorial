using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace OpenGLEs2Tutorial
{
    [Activity(Label = "OpenGLEs2Tutorial", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        /// <summary>
        ///  Our OpenGL Surfaceview
        /// </summary>
        private CustomGLSurface glSurfaceView;

        protected override void OnCreate(Bundle bundle)
        {
            // Turn off the window's title bar
            RequestWindowFeature(WindowFeatures.NoTitle);

            // Super
            base.OnCreate(bundle);

            // Fullscreen mode
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

            // We create our Surfaceview for our OpenGL here.
            glSurfaceView = new CustomGLSurface(this);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Retrieve our Relative layout from our main layout we just set to our view.
            RelativeLayout layout = FindViewById<RelativeLayout>(Resource.Id.gameLayout);

            // Attach our surfaceview to our layout from our main layout.
            RelativeLayout.LayoutParams glParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
            layout.AddView(glSurfaceView, glParams);
        }

        protected override void OnPause()
        {
            base.OnPause();
            glSurfaceView.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            glSurfaceView.OnResume();
        }
    }
}

