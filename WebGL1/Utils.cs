using DuoCode.Dom;
using System;

namespace WebGL {
    using GL = WebGLRenderingContext;

    internal static class Utils {
        public static WebGLRenderingContext CreateWebGL(HTMLCanvasElement canvas) {
            WebGLRenderingContext result = null;
            string[] names = { "webgl", "experimental-webgl", "webkit-3d", "moz-webgl" };
            foreach (string name in names) {
                try {
                    result = canvas.getContext(name);
                } catch { }
                if (result != null)
                    break;
            }
            return result;
        }

        public static WebGLShader CreateShaderFromScriptElement(WebGLRenderingContext gl, string scriptId) {
            var shaderScript = (HTMLScriptElement)Global.document.getElementById(scriptId);

            if (shaderScript == null)
                throw new Exception("unknown script element " + scriptId);

            string shaderSource = shaderScript.text;

            // Now figure out what type of shader script we have, based on its MIME type
            int shaderType = (shaderScript.type == "x-shader/x-fragment") ? GL.FRAGMENT_SHADER :
                             (shaderScript.type == "x-shader/x-vertex") ? GL.VERTEX_SHADER : 0;
            if (shaderType == 0)
                throw new Exception("unknown shader type");

            WebGLShader shader = gl.createShader(shaderType);
            gl.shaderSource(shader, shaderSource);

            // Compile the shader program
            gl.compileShader(shader);

            // See if it compiled successfully
            if (!gl.getShaderParameter(shader, GL.COMPILE_STATUS)) {
                // Something went wrong during compilation; get the error
                var errorInfo = gl.getShaderInfoLog(shader);
                gl.deleteShader(shader);
                throw new Exception("error compiling shader '" + shader + "': " + errorInfo);
            }
            return shader;
        }

        public static WebGLProgram CreateShaderProgram(WebGLRenderingContext gl, WebGLShader fragmentShader, WebGLShader vertexShader) {
            var shaderProgram = gl.createProgram();
            gl.attachShader(shaderProgram, vertexShader);
            gl.attachShader(shaderProgram, fragmentShader);
            gl.linkProgram(shaderProgram);

            bool linkStatus = gl.getProgramParameter(shaderProgram, GL.LINK_STATUS);
            if (!linkStatus)
                throw new Exception("failed to link shader");
            return shaderProgram;
        }

        public static WebGLTexture LoadTexture(WebGLRenderingContext gl, HTMLImageElement imageElement) {
            var result = gl.createTexture();
            imageElement.onload = new Func<Event, dynamic>((e) => {
                UploadTexture(gl, result, imageElement);
                return true;
            });

            return result;
        }

        public static void UploadTexture(WebGLRenderingContext gl, WebGLTexture texture, HTMLImageElement imageElement) {
            gl.pixelStorei(GL.UNPACK_FLIP_Y_WEBGL, GL.ONE);
            gl.bindTexture(GL.TEXTURE_2D, texture);
            gl.texImage2D(GL.TEXTURE_2D, 0, GL.RGBA, GL.RGBA, GL.UNSIGNED_BYTE, imageElement);
            gl.texParameteri(GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, GL.LINEAR);
            gl.texParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, GL.LINEAR_MIPMAP_NEAREST);
            gl.generateMipmap(GL.TEXTURE_2D);
            gl.bindTexture(GL.TEXTURE_2D, null);
        }

        public static float DegToRad(float degrees) {
            return (float)(degrees * System.Math.PI / 180);
        }
    }
}