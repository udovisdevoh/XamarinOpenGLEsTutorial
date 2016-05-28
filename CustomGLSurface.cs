using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
//using OpenTK.Platform.Android;
//using OpenTK.Graphics;
using Android.Opengl;

namespace OpenGLEs2Tutorial
{
    public class CustomGLSurface : GLSurfaceView
    {
        private readonly CustomGLRenderer renderer;

        public CustomGLSurface(Context context)
            : base(context)
        {
            // Create an OpenGL ES 2.0 context.
            SetEGLContextClientVersion(2);

            // Set the Renderer for drawing on the GLSurfaceView
            renderer = new CustomGLRenderer(context);
            SetRenderer(renderer);

            // Render the view only when there is a change in the drawing data
            RenderMode = Rendermode.Continuously;
        }

        public override void OnPause()
        {
            base.OnPause();
            renderer.OnPause();
        }

        public override void OnResume()
        {
            base.OnResume();
            renderer.OnResume();
        }
    }
}