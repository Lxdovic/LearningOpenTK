using OpenTK.Graphics.OpenGL.Compatibility;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace LearningOpenTK;

public class Game : GameWindow {
    private readonly uint[] _indices = [
        0, 1, 2,
        2, 3, 0
    ];

    private readonly float[] _vertices = [
        -.5f, .5f, 0f,
        .5f, .5f, 0f,
        .5f, -.5f, 0f,
        -.5f, -.5f, 0f
    ];

    private int _shaderProgram;
    private int _vao, _vbo, _ebo;
    private int _width, _height;

    public Game(int width, int height) : base(GameWindowSettings.Default,
        NativeWindowSettings.Default) {
        _width = width;
        _height = height;

        CenterWindow(new Vector2i(width, height));
    }

    protected override void OnResize(ResizeEventArgs e) {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);

        _width = e.Width;
        _height = e.Height;
    }

    protected override void OnLoad() {
        base.OnLoad();

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices, BufferUsage.StaticDraw);
        GL.BindVertexArray(_vao);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexArrayAttrib(_vao, 0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        _ebo = GL.GenBuffer();

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices, BufferUsage.StaticDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

        _shaderProgram = GL.CreateProgram();

        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        var vertexShaderSource =
            LoadShaderSource(Path.Combine(Environment.CurrentDirectory, "resources/shaders/default.vert"));
        var fragmentShaderSource =
            LoadShaderSource(Path.Combine(Environment.CurrentDirectory, "resources/shaders/default.frag"));

        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);

        GL.AttachShader(_shaderProgram, vertexShader);
        GL.AttachShader(_shaderProgram, fragmentShader);
        GL.LinkProgram(_shaderProgram);

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    protected override void OnUnload() {
        base.OnUnload();

        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_vbo);
        GL.DeleteBuffer(_ebo);
        GL.DeleteProgram(_shaderProgram);
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.UseProgram(_shaderProgram);
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

        Context.SwapBuffers();

        base.OnRenderFrame(args);
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        base.OnUpdateFrame(args);
    }

    public static string LoadShaderSource(string path) {
        var shaderSource = "";

        try {
            using var streamReader = new StreamReader(path);

            shaderSource = streamReader.ReadToEnd();
        }

        catch (Exception e) {
            Console.WriteLine($"The file could not be read: {e.Message}");
        }

        return shaderSource;
    }
}