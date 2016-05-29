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
using Android.Opengl;
using static Android.Opengl.GLSurfaceView;
using Javax.Microedition.Khronos.Egl;
using Javax.Microedition.Khronos.Opengles;
using Java.Nio;

namespace OpenGLEs2Tutorial
{
    public class CustomGLRenderer : Java.Lang.Object, IRenderer
    {
        #region Members
        private float[] matrixProjection = new float[16];

        private float[] matrixView = new float[16];

        private float[] matrixProjectionAndView = new float[16];

        public static float[] vertices;

        public static short[] indices;

        public FloatBuffer vertexBuffer;

        public ShortBuffer drawListBuffer;

        private float screenWidth = 1280;

        private float screenHeight = 768;

        private Context context;

        private long lastTime;

        private int program;
        #endregion

        #region Constructors
        public CustomGLRenderer(Context context)
        {
            this.context = context;
            lastTime = Java.Lang.JavaSystem.CurrentTimeMillis() + 100;
        }
        #endregion

        public void OnDrawFrame(IGL10 unused)
        {
            // Get the current time
            long now = Java.Lang.JavaSystem.CurrentTimeMillis();

            // We should make sure we are valid and sane
            if (lastTime > now)
            {
                return;
            }

            // Get the amount of time the last frame took.
            long timeDelta = now - lastTime;

            // Update our example

            // Render our example
            Render(matrixProjectionAndView);

            // Save the current time to see how long it took
            lastTime = now;
        }

        private void Render(float[] matrix)
        {
            // clear Screen and Depth Buffer, we have set the clear color as black.
            GLES20.GlClear(GLES20.GlColorBufferBit | GLES20.GlDepthBufferBit);

            // get handle to vertex shader's vPosition member
            int positionHandle = GLES20.GlGetAttribLocation(ShaderHelper.SpSolidColor, "vPosition");

            // Enable generic vertex attribute array
            GLES20.GlEnableVertexAttribArray(positionHandle);

            // Prepare the triangle coordinate data
            GLES20.GlVertexAttribPointer(positionHandle, 3,
                                         GLES20.GlFloat, false,
                                         0, vertexBuffer);

            // Get handle to shape's transformation matrix
            int matrixhandle = GLES20.GlGetUniformLocation(ShaderHelper.SpSolidColor, "uMVPMatrix");

            // Apply the projection and view transformation
            GLES20.GlUniformMatrix4fv(matrixhandle, 1, false, matrix, 0);

            // Draw the triangle
            GLES20.GlDrawElements(GLES20.GlTriangleStrip, indices.Length,
                    GLES20.GlUnsignedShort, drawListBuffer);

            // Disable vertex array
            GLES20.GlDisableVertexAttribArray(positionHandle);
        }

        public void OnSurfaceChanged(IGL10 unused, int width, int height)
        {
            // We need to know the current width and height.
            screenWidth = width;
            screenHeight = height;

            // Redo the Viewport, making it fullscreen.
            GLES20.GlViewport(0, 0, (int)screenWidth, (int)screenHeight);

            // Clear our matrices
            for (int i = 0; i < 16; i++)
            {
                matrixProjection[i] = 0.0f;
                matrixView[i] = 0.0f;
                matrixProjectionAndView[i] = 0.0f;
            }

            // Setup our screen width and height for normal sprite translation.
            Matrix.OrthoM(matrixProjection, 0, 0f, screenWidth, 0.0f, screenHeight, 0, 50);

            // Set the camera position (View matrix)
            Matrix.SetLookAtM(matrixView, 0, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1.0f, 0.0f);

            // Calculate the projection and view transformation
            Matrix.MultiplyMM(matrixProjectionAndView, 0, matrixProjection, 0, matrixView, 0);
        }

        public void OnSurfaceCreated(IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config)
        {
            // Create the triangle
            SetupTriangle();

            // Set the clear color to black
            GLES20.GlClearColor(0.0f, 0.0f, 0.0f, 1);

            // Create the shaders
            int vertexShader = ShaderHelper.LoadShader(GLES20.GlVertexShader, ShaderHelper.VsSolidColor);
            int fragmentShader = ShaderHelper.LoadShader(GLES20.GlFragmentShader, ShaderHelper.FsSolidColor);

            ShaderHelper.SpSolidColor = GLES20.GlCreateProgram();             // create empty OpenGL ES Program
            GLES20.GlAttachShader(ShaderHelper.SpSolidColor, vertexShader);   // add the vertex shader to program
            GLES20.GlAttachShader(ShaderHelper.SpSolidColor, fragmentShader); // add the fragment shader to program
            GLES20.GlLinkProgram(ShaderHelper.SpSolidColor);                  // creates OpenGL ES program executables

            // Set our shader programm
            GLES20.GlUseProgram(ShaderHelper.SpSolidColor);
        }

        public void SetupTriangle()
        {
            // We have to create the vertices.
            vertices = new float[]
            {
                10.0f, 100f, 0.0f,
                100f, 100f, 0.0f,
                10.0f, 200f, 0.0f,
                100f, 200f, 0.0f,
            };

            indices = new short[] { 0, 1, 2, 3 }; // The order of vertexrendering.

            // The vertex buffer.
            ByteBuffer byteBuffer = ByteBuffer.AllocateDirect(vertices.Length * 4);
            byteBuffer.Order(ByteOrder.NativeOrder());
            vertexBuffer = byteBuffer.AsFloatBuffer();
            vertexBuffer.Put(vertices);
            vertexBuffer.Position(0);

            // initialize byte buffer for the draw list
            ByteBuffer byteBuffer2 = ByteBuffer.AllocateDirect(indices.Length * 2);
            byteBuffer2.Order(ByteOrder.NativeOrder());
            drawListBuffer = byteBuffer2.AsShortBuffer();
            drawListBuffer.Put(indices);
            drawListBuffer.Position(0);

        }

        internal void OnPause()
        {
            /* Do stuff to pause the renderer */
        }

        internal void OnResume()
        {
            /* Do stuff to resume the renderer */
            lastTime = Java.Lang.JavaSystem.CurrentTimeMillis();
        }
    }
}