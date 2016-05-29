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
using Android.Graphics;

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

        public static float[] uvs;

        public FloatBuffer vertexBuffer;

        public ShortBuffer drawListBuffer;

        public FloatBuffer uvBuffer;

        private float screenWidth = 1280;

        private float screenHeight = 768;

        private Context context;

        private long lastTime;

        private RectF rect;

        //private int program;
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

        internal void ProcessTouchEvent(MotionEvent motionEvent)
        {
            // Get the half of screen value
            int screenHalfX = (int)(screenWidth / 2);
            int screenHalfY = (int)(screenHeight / 2);
            if (motionEvent.GetX() < screenHalfX)
            {
                rect.Left -= 10;
                rect.Right -= 10;
            }
            else
            {
                rect.Left += 10;
                rect.Right += 10;
            }

            if (motionEvent.GetY() < screenHalfY)
            {
                rect.Bottom += 10;
                rect.Top += 10;
            }
            else
            {
                rect.Bottom -= 10;
                rect.Top -= 10;
            }

            // Update the new data.
            TranslateSprite();
        }

        private void Render(float[] matrix)
        {
            // clear Screen and Depth Buffer, we have set the clear color as black.
            GLES20.GlClear(GLES20.GlColorBufferBit | GLES20.GlDepthBufferBit);

            // get handle to vertex shader's vPosition member
            int positionHandle = GLES20.GlGetAttribLocation(ShaderHelper.SpImage, "vPosition");

            // Enable generic vertex attribute array
            GLES20.GlEnableVertexAttribArray(positionHandle);

            // Prepare the triangle coordinate data
            GLES20.GlVertexAttribPointer(positionHandle, 3,
                                         GLES20.GlFloat, false,
                                         0, vertexBuffer);

            // Get handle to texture coordinates location
            int textureCoordinatesLocation = GLES20.GlGetAttribLocation(ShaderHelper.SpImage, "a_texCoord");

            // Enable generic vertex attribute array
            GLES20.GlEnableVertexAttribArray(textureCoordinatesLocation);

            // Prepare the texturecoordinates
            GLES20.GlVertexAttribPointer(textureCoordinatesLocation, 2, GLES20.GlFloat, false, 0, uvBuffer);

            // Get handle to shape's transformation matrix
            int matrixhandle = GLES20.GlGetUniformLocation(ShaderHelper.SpImage, "uMVPMatrix");

            // Apply the projection and view transformation
            GLES20.GlUniformMatrix4fv(matrixhandle, 1, false, matrix, 0);

            // Get handle to textures locations
            int samplerLocation = GLES20.GlGetUniformLocation(ShaderHelper.SpImage, "s_texture");

            // Set the sampler texture unit to 0, where we have saved the texture.
            GLES20.GlUniform1i(samplerLocation, 0);

            // Draw the triangle
            GLES20.GlDrawElements(GLES20.GlTriangles, indices.Length,
                    GLES20.GlUnsignedShort, drawListBuffer);

            // Disable vertex array
            GLES20.GlDisableVertexAttribArray(positionHandle);
            GLES20.GlDisableVertexAttribArray(textureCoordinatesLocation);
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
            Android.Opengl.Matrix.OrthoM(matrixProjection, 0, 0f, screenWidth, 0.0f, screenHeight, 0, 50);

            // Set the camera position (View matrix)
            Android.Opengl.Matrix.SetLookAtM(matrixView, 0, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1.0f, 0.0f);

            // Calculate the projection and view transformation
            Android.Opengl.Matrix.MultiplyMM(matrixProjectionAndView, 0, matrixProjection, 0, matrixView, 0);
        }

        public void OnSurfaceCreated(IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config)
        {
            // Create the triangle
            SetupTriangle();

            // Create the image information
            SetupImage();

            // Set the clear color to black
            GLES20.GlClearColor(0.0f, 0.0f, 0.0f, 1);

            #region Solid colors mode
            /*
            // Create the shaders, solid color
            int vertexShader = ShaderHelper.LoadShader(GLES20.GlVertexShader, ShaderHelper.VsSolidColor);
            int fragmentShader = ShaderHelper.LoadShader(GLES20.GlFragmentShader, ShaderHelper.FsSolidColor);

            ShaderHelper.SpSolidColor = GLES20.GlCreateProgram();             // create empty OpenGL ES Program
            GLES20.GlAttachShader(ShaderHelper.SpSolidColor, vertexShader);   // add the vertex shader to program
            GLES20.GlAttachShader(ShaderHelper.SpSolidColor, fragmentShader); // add the fragment shader to program
            GLES20.GlLinkProgram(ShaderHelper.SpSolidColor);                  // creates OpenGL ES program executables

            // Set our shader programm
            GLES20.GlUseProgram(ShaderHelper.SpSolidColor);
            */
            #endregion

            #region Texture mode
            // Create the shaders, images
            int vertexShader = ShaderHelper.LoadShader(GLES20.GlVertexShader, ShaderHelper.VsImage);
            int fragmentShader = ShaderHelper.LoadShader(GLES20.GlFragmentShader, ShaderHelper.FsImage);

            ShaderHelper.SpImage = GLES20.GlCreateProgram();
            GLES20.GlAttachShader(ShaderHelper.SpImage, vertexShader);
            GLES20.GlAttachShader(ShaderHelper.SpImage, fragmentShader);
            GLES20.GlLinkProgram(ShaderHelper.SpImage);

            // Set our shader programm
            GLES20.GlUseProgram(ShaderHelper.SpImage);
            #endregion
        }

        private void SetupTriangle()
        {
            rect = new RectF(10, 200, 100, 10);
            /*rect.Left = 10;
            rect.Right = 100;
            rect.Bottom = 100;
            rect.Top = 200;*/

            // We have to create the vertices.
            /*vertices = new float[]
            {
                10.0f, 200f, 0.0f,
                10.0f, 100f, 0.0f,
                100f, 100f, 0.0f,
                100f, 200f, 0.0f,
            };*/
            vertices = new float[]
            {
                rect.Left, rect.Top, 0.0f,
                rect.Left, rect.Bottom, 0.0f,
                rect.Right, rect.Bottom, 0.0f,
                rect.Right, rect.Top, 0.0f,
            };


            indices = new short[] { 0, 1, 2, 0, 2, 3 }; // The order of vertexrendering.

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

        public void TranslateSprite()
        {
            /*vertices = new float[]
            {
                rect.Left, rect.Top, 0.0f,
                rect.Left, rect.Bottom, 0.0f,
                rect.Right, rect.Bottom, 0.0f,
                rect.Right, rect.Top, 0.0f,
            };*/
            vertices[0] = rect.Left;
            vertices[1] = rect.Top;
            vertices[2] = 0.0f;

            vertices[3] = rect.Left;
            vertices[4] = rect.Bottom;
            vertices[5] = 0.0f;

            vertices[6] = rect.Right;
            vertices[7] = rect.Bottom;
            vertices[8] = 0.0f;

            vertices[9] = rect.Right;
            vertices[10] = rect.Top;
            vertices[11] = 0.0f;


            vertexBuffer.Clear();
            vertexBuffer.Put(vertices);
            vertexBuffer.Position(0);

            /*
            // The vertex buffer.
            ByteBuffer byteBuffer = ByteBuffer.AllocateDirect(vertices.Length * 4);
            byteBuffer.Order(ByteOrder.NativeOrder());
            vertexBuffer = byteBuffer.AsFloatBuffer();
            vertexBuffer.Put(vertices);
            vertexBuffer.Position(0);*/
        }

        private void SetupImage()
        {
            // Create our UV coordinates.
            uvs = new float[] {
                0.0f, 0.0f,
                0.0f, 1.0f,
                1.0f, 1.0f,
                1.0f, 0.0f
            };

            // The texture buffer
            ByteBuffer byteBuffer = ByteBuffer.AllocateDirect(uvs.Length * 4);
            byteBuffer.Order(ByteOrder.NativeOrder());
            uvBuffer = byteBuffer.AsFloatBuffer();
            uvBuffer.Put(uvs);
            uvBuffer.Position(0);

            // Generate Textures, if more needed, alter these numbers.
            int[] textureIds = new int[1];
            GLES20.GlGenTextures(1, textureIds, 0);

            // Retrieve our image from resources.
            Bitmap bitmap = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.otter);

            // Bind texture to texturename
            GLES20.GlActiveTexture(GLES20.GlTexture0);
            GLES20.GlBindTexture(GLES20.GlTexture2d, textureIds[0]);

            // Set filtering
            GLES20.GlTexParameteri(GLES20.GlTexture2d, GLES20.GlTextureMinFilter, GLES20.GlLinear);
            GLES20.GlTexParameteri(GLES20.GlTexture2d, GLES20.GlTextureMagFilter, GLES20.GlLinear);

            // Set wrapping mode
            GLES20.GlTexParameteri(GLES20.GlTexture2d, GLES20.GlTextureWrapS, GLES20.GlClampToEdge);
            GLES20.GlTexParameteri(GLES20.GlTexture2d, GLES20.GlTextureWrapT, GLES20.GlClampToEdge);

            // Load the bitmap into the bound texture.
            GLUtils.TexImage2D(GLES20.GlTexture2d, 0, bitmap, 0);

            // We are done using the bitmap so we should recycle it.
            bitmap.Recycle();
            bitmap.Dispose();
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