using OpenTK.Graphics.OpenGL.Compatibility;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using StbImageSharp;

namespace LearningOpenTK;

public class Game : GameWindow {
    private readonly uint[] _indices = [
        0, 1, 2,
        2, 3, 0,

        4, 5, 6,
        6, 7, 4,

        8, 9, 10,
        10, 11, 8,

        12, 13, 14,
        14, 15, 12,

        16, 17, 18,
        18, 19, 16,

        20, 21, 22,
        22, 23, 20
    ];

    private readonly List<Vector2> _texCoords = [
        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 1.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(0.0f, 0.0f),

        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 1.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(0.0f, 0.0f),

        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 1.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(0.0f, 0.0f),

        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 1.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(0.0f, 0.0f),

        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 1.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(0.0f, 0.0f),

        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 1.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(0.0f, 0.0f)
    ];

    private readonly List<Vector3> _vertices = [
        // front face
        new Vector3(-0.5f, 0.5f, 0.5f), // topleft vert
        new Vector3(0.5f, 0.5f, 0.5f), // topright vert
        new Vector3(0.5f, -0.5f, 0.5f), // bottomright vert
        new Vector3(-0.5f, -0.5f, 0.5f), // bottomleft vert
        // right face
        new Vector3(0.5f, 0.5f, 0.5f), // topleft vert
        new Vector3(0.5f, 0.5f, -0.5f), // topright vert
        new Vector3(0.5f, -0.5f, -0.5f), // bottomright vert
        new Vector3(0.5f, -0.5f, 0.5f), // bottomleft vert
        // back face
        new Vector3(0.5f, 0.5f, -0.5f), // topleft vert
        new Vector3(-0.5f, 0.5f, -0.5f), // topright vert
        new Vector3(-0.5f, -0.5f, -0.5f), // bottomright vert
        new Vector3(0.5f, -0.5f, -0.5f), // bottomleft vert
        // left face
        new Vector3(-0.5f, 0.5f, -0.5f), // topleft vert
        new Vector3(-0.5f, 0.5f, 0.5f), // topright vert
        new Vector3(-0.5f, -0.5f, 0.5f), // bottomright vert
        new Vector3(-0.5f, -0.5f, -0.5f), // bottomleft vert
        // top face
        new Vector3(-0.5f, 0.5f, -0.5f), // topleft vert
        new Vector3(0.5f, 0.5f, -0.5f), // topright vert
        new Vector3(0.5f, 0.5f, 0.5f), // bottomright vert
        new Vector3(-0.5f, 0.5f, 0.5f), // bottomleft vert
        // bottom face
        new Vector3(-0.5f, -0.5f, 0.5f), // topleft vert
        new Vector3(0.5f, -0.5f, 0.5f), // topright vert
        new Vector3(0.5f, -0.5f, -0.5f), // bottomright vert
        new Vector3(-0.5f, -0.5f, -0.5f) // bottomleft vert
    ];

    private int _shaderProgram;
    private int _textureId;
    private int _vao, _verticesVbo, _textureVbo, _ebo;
    private int _width, _height;

    private float _yRotation;

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

        GL.BindVertexArray(_vao);

        _verticesVbo = GL.GenBuffer();

        GL.BindBuffer(BufferTarget.ArrayBuffer, _verticesVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.ToArray(), BufferUsage.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexArrayAttrib(_vao, 0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        _textureVbo = GL.GenBuffer();

        GL.BindBuffer(BufferTarget.ArrayBuffer, _textureVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _texCoords.ToArray(), BufferUsage.StaticDraw);

        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexArrayAttrib(_vao, 1);
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

        _textureId = GL.GenTexture();

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2d, _textureId);

        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        StbImage.stbi_set_flip_vertically_on_load(1);
        var dirtTexture =
            ImageResult.FromStream(
                File.OpenRead(Path.Combine(Environment.CurrentDirectory, "resources/textures/dirt.png")),
                ColorComponents.RedGreenBlueAlpha);

        GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, dirtTexture.Width, dirtTexture.Height, 0,
            PixelFormat.Rgba, PixelType.UnsignedByte, dirtTexture.Data);

        GL.BindTexture(TextureTarget.Texture2d, 0);
        GL.Enable(EnableCap.DepthTest);
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.UseProgram(_shaderProgram);
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);

        GL.BindTexture(TextureTarget.Texture2d, _textureId);

        var model = Matrix4.Identity;
        var view = Matrix4.Identity;
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f),
            (float)_width / _height, 0.1f, 100f);

        var modelLocation = GL.GetUniformLocation(_shaderProgram, "model");
        var viewLocation = GL.GetUniformLocation(_shaderProgram, "view");
        var projectionLocation = GL.GetUniformLocation(_shaderProgram, "projection");

        model = Matrix4.CreateRotationY(_yRotation);
        model *= Matrix4.CreateTranslation(new Vector3(0f, 0f, -3f));

        _yRotation += 0.0001f;

        GL.UniformMatrix4f(modelLocation, 1, true, model);
        GL.UniformMatrix4f(viewLocation, 1, true, view);
        GL.UniformMatrix4f(projectionLocation, 1, true, projection);

        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

        Context.SwapBuffers();

        base.OnRenderFrame(args);
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        base.OnUpdateFrame(args);
    }

    protected override void OnUnload() {
        base.OnUnload();

        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_verticesVbo);
        GL.DeleteBuffer(_textureVbo);
        GL.DeleteBuffer(_ebo);
        GL.DeleteProgram(_shaderProgram);
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