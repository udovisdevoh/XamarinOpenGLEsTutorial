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

namespace OpenGLEs2Tutorial
{
    public static class ShaderHelper
    {
        // Program variables
        //public static int SpSolidColor;

        public static int SpImage;

        /// <summary>
        /// SHADER Solid
        /// This shader is for rendering a colored primitive.
        /// </summary>
        public const string VsSolidColor =
            "uniform    mat4        uMVPMatrix;" +
            "attribute  vec4        vPosition;" +
            "void main() {" +
            "  gl_Position = uMVPMatrix * vPosition;" +
            "}";

        public const string FsSolidColor =
            "precision mediump float;" +
            "void main() {" +
            "  gl_FragColor = vec4(0.5,0,0,1);" +
            "}";

        /// <summary>
        /// SHADER Image
        /// This shader is for rendering 2D images straight from a texture
        /// No additional effects.
        /// </summary>
        public const string VsImage =
            "uniform mat4 uMVPMatrix;" +
            "attribute vec4 vPosition;" +
            "attribute vec2 a_texCoord;" +
            "varying vec2 v_texCoord;" +
            "void main() {" +
            "  gl_Position = uMVPMatrix * vPosition;" +
            "  v_texCoord = a_texCoord;" +
            "}";
        public const string FsImage =
            "precision mediump float;" +
            "varying vec2 v_texCoord;" +
            "uniform sampler2D s_texture;" +
            "void main() {" +
            "  gl_FragColor = texture2D( s_texture, v_texCoord );" +
            "}";

        public static int LoadShader(int type, string shaderCode)
        {
            // create a vertex shader type (GLES20.GL_VERTEX_SHADER)
            // or a fragment shader type (GLES20.GL_FRAGMENT_SHADER)
            int shader = GLES20.GlCreateShader(type);

            // add the source code to the shader and compile it
            GLES20.GlShaderSource(shader, shaderCode);
            GLES20.GlCompileShader(shader);

            // return the shader
            return shader;
        }
    }
}