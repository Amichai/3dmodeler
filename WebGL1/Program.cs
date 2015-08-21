using DuoCode.Dom;
using DuoCode.Runtime;
using System;

namespace WebGL {
    using GL = WebGLRenderingContext;
    using Console = System.Console;

    internal sealed class Program {
        private static readonly dynamic GLVector3 = Js.referenceAs<dynamic>("vec3"); // glMatrix functions import
        private static readonly dynamic GLMatrix3 = Js.referenceAs<dynamic>("mat3");
        private static readonly dynamic GLMatrix4 = Js.referenceAs<dynamic>("mat4");

        private readonly WebGLRenderingContext gl;
        private readonly HTMLCanvasElement canvas;

        private readonly WebGLProgram shaderProgram;
        private readonly WebGLTexture texture;

        private readonly uint aVertexPosition, aVertexNormal, aTextureCoord;
        private readonly WebGLUniformLocation uProjectionMatrix, uModelViewMatrix, uNormalMatrix, uSampler;

        private readonly WebGLBuffer bCubeVertexPositions, bCubeVertexNormals, bCubeTextureCoords, bCubeIndices;

        private readonly float[] mProjection, mModelView, mNormal;

        private readonly bool[] heldKeys = new bool[255];
        private int lastTimeInMs = 0;

        private float z = -5;
        private float rotationX = 30, rotationY = 30; // in degrees
        private const float speedX = 5, speedY = -5; // deg/sec

        public Program(HTMLCanvasElement canvas) {
            gl = Utils.CreateWebGL(canvas);

            if (!gl.IsTruthy())
                throw new Exception("could not initialize WebGL");

            Console.WriteLine("WebGL demo by DuoCode");
            Console.WriteLine(string.Format("{0}, {1}, {2}", (string)gl.getParameter(GL.VENDOR),
                                                             (string)gl.getParameter(GL.VERSION),
                                                             (string)gl.getParameter(GL.RENDERER)));
            Console.WriteLine("\r\nUse ← ↑ → ↓ for rotation and +/- for zoom");

            this.canvas = canvas;

            // init matrices
            mModelView = GLMatrix4.create();
            mProjection = GLMatrix4.create();
            mNormal = GLMatrix3.create();

            GLMatrix4.perspective(45, (double)canvas.width / canvas.height, 0.1, 100.0, mProjection);

            // init buffers
            bCubeVertexPositions = gl.createBuffer();
            this.gl.bindBuffer(GL.ARRAY_BUFFER, bCubeVertexPositions);
            this.gl.bufferData(GL.ARRAY_BUFFER, CubeData.Positions.As<ArrayBufferView>(), GL.STATIC_DRAW);

            bCubeVertexNormals = gl.createBuffer();
            this.gl.bindBuffer(GL.ARRAY_BUFFER, bCubeVertexNormals);
            this.gl.bufferData(GL.ARRAY_BUFFER, CubeData.Normals.As<ArrayBufferView>(), GL.STATIC_DRAW);

            bCubeTextureCoords = gl.createBuffer();
            this.gl.bindBuffer(GL.ARRAY_BUFFER, bCubeTextureCoords);
            this.gl.bufferData(GL.ARRAY_BUFFER, CubeData.TexCoords.As<ArrayBufferView>(), GL.STATIC_DRAW);

            bCubeIndices = gl.createBuffer();
            this.gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, bCubeIndices);
            this.gl.bufferData(GL.ELEMENT_ARRAY_BUFFER, CubeData.Indices.As<ArrayBufferView>(), GL.STATIC_DRAW);

            // init shaders
            var fragmentShader = Utils.CreateShaderFromScriptElement(gl, "shader-fs");
            var vertexShader = Utils.CreateShaderFromScriptElement(gl, "shader-vs");

            shaderProgram = Utils.CreateShaderProgram(gl, fragmentShader, vertexShader);

            gl.useProgram(shaderProgram);

            aVertexPosition = (uint)gl.getAttribLocation(shaderProgram, "aVertexPosition");
            aVertexNormal = (uint)gl.getAttribLocation(shaderProgram, "aVertexNormal");
            aTextureCoord = (uint)gl.getAttribLocation(shaderProgram, "aTextureCoord");

            uProjectionMatrix = gl.getUniformLocation(shaderProgram, "uPMatrix");
            uModelViewMatrix = gl.getUniformLocation(shaderProgram, "uMVMatrix");
            uNormalMatrix = gl.getUniformLocation(shaderProgram, "uNMatrix");
            uSampler = gl.getUniformLocation(shaderProgram, "uSampler");

            gl.enableVertexAttribArray(aVertexPosition);
            gl.enableVertexAttribArray(aVertexNormal);
            gl.enableVertexAttribArray(aTextureCoord);

            // load texture
            texture = Utils.LoadTexture(this.gl, Properties.Resources.duocode.Image);

            gl.clearColor(0f, 0f, 0f, 1f);
            gl.enable(GL.DEPTH_TEST);

            Global.document.onkeydown = OnKeyDown;
            Global.document.onkeyup = OnKeyUp;

            Render();
        }

        static void Run() // HTML body.onload event entry point, see index.html
        {
            try {
                var canvas = (HTMLCanvasElement)Global.document.getElementById("canvas");

                new Program(canvas);
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private void Render() {
            Js.de.requestAnimationFrame(new Action(() => Render()));
            HandleKeys();
            DrawScene();
            Animate();
        }

        private void HandleKeys() {
            if (heldKeys[107] || heldKeys[187]) // +
                z /= 1.02f;
            if (heldKeys[109] || heldKeys[189]) // -
                z *= 1.02f;
            if (heldKeys[37]) // left cursor key
                rotationY -= 1;
            if (heldKeys[39]) // right cursor key
                rotationY += 1f;
            if (heldKeys[38]) // up cursor key
                rotationX -= 1;
            if (heldKeys[40]) // down cursor key
                rotationX += 1;
        }

        private void DrawScene() {
            gl.viewport(0, 0, (int)canvas.width, (int)canvas.height);
            gl.clear(GL.COLOR_BUFFER_BIT | GL.DEPTH_BUFFER_BIT);

            GLMatrix4.identity(mModelView);
            GLMatrix4.translate(mModelView, new[] { 0, 0, z });
            GLMatrix4.rotate(mModelView, Utils.DegToRad(rotationX), new[] { 1f, 0, 0 });
            GLMatrix4.rotate(mModelView, Utils.DegToRad(rotationY), new[] { 0, 1f, 0 });

            gl.bindBuffer(GL.ARRAY_BUFFER, bCubeVertexPositions);
            gl.vertexAttribPointer(aVertexPosition, 3, GL.FLOAT, false, 0, 0);

            gl.bindBuffer(GL.ARRAY_BUFFER, bCubeVertexNormals);
            gl.vertexAttribPointer(aVertexNormal, 3, GL.FLOAT, false, 0, 0);

            gl.bindBuffer(GL.ARRAY_BUFFER, bCubeTextureCoords);
            gl.vertexAttribPointer(aTextureCoord, 2, GL.FLOAT, false, 0, 0);

            gl.activeTexture(GL.TEXTURE0);
            gl.bindTexture(GL.TEXTURE_2D, texture);
            gl.uniform1i(uSampler, 0);

            gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, bCubeIndices);

            gl.uniformMatrix4fv(uProjectionMatrix, false, mProjection);
            gl.uniformMatrix4fv(uModelViewMatrix, false, mModelView);

            GLMatrix4.toInverseMat3(mModelView, mNormal);
            GLMatrix3.transpose(mNormal);
            gl.uniformMatrix3fv(uNormalMatrix, false, mNormal);

            gl.drawElements(GL.TRIANGLES, CubeData.Indices.Length, GL.UNSIGNED_SHORT, 0);
        }

        private void Animate() {
            int nowInMs = Environment.TickCount;
            if (lastTimeInMs != 0) {
                int elapsed = nowInMs - lastTimeInMs;
                rotationX += (speedX * elapsed) / 1000f;
                rotationY += (speedY * elapsed) / 1000f;
            }
            lastTimeInMs = nowInMs;
        }

        private dynamic OnKeyDown(KeyboardEvent e) {
            heldKeys[e.keyCode] = true;
            return true;
        }

        private dynamic OnKeyUp(KeyboardEvent e) {
            heldKeys[e.keyCode] = false;
            return true;
        }
    }
}
