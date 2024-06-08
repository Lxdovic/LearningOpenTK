using LearningOpenTK.Graphics;
using OpenTK.Graphics.OpenGL.Compatibility;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace LearningOpenTK;

internal sealed class Game : GameWindow {
    private readonly List<uint> _indices = [
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

    private Camera _camera;
    private Ibo _ibo;
    private ShaderProgram _shaderProgram;
    private Texture _texture;
    private Vao _vao;

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

        _vao = new Vao();
        var vbo = new Vbo(_vertices);
        _vao.LinkToVao(0, 3, vbo);

        var uvVbo = new Vbo(_texCoords);
        _vao.LinkToVao(1, 2, uvVbo);

        _ibo = new Ibo(_indices);
        _shaderProgram = new ShaderProgram("resources/shaders/default.vert", "resources/shaders/default.frag");
        _texture = new Texture("resources/textures/dirt.png");

        GL.Enable(EnableCap.DepthTest);

        _camera = new Camera(_width, _height, new Vector3(0f, 0f, 3f));
        CursorState = CursorState.Grabbed;
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _shaderProgram.Bind();
        _vao.Bind();
        _ibo.Bind();
        _texture.Bind();

        var view = _camera.GetViewMatrix();
        var projection = _camera.GetProjectionMatrix();

        var model = Matrix4.CreateRotationY(_yRotation);
        model *= Matrix4.CreateTranslation(new Vector3(0f, 0f, -3f));

        var modelLocation = GL.GetUniformLocation(_shaderProgram.Id, "model");
        var viewLocation = GL.GetUniformLocation(_shaderProgram.Id, "view");
        var projectionLocation = GL.GetUniformLocation(_shaderProgram.Id, "projection");

        _yRotation += 0.0001f;

        GL.UniformMatrix4f(modelLocation, 1, true, model);
        GL.UniformMatrix4f(viewLocation, 1, true, view);
        GL.UniformMatrix4f(projectionLocation, 1, true, projection);

        GL.DrawElements(PrimitiveType.Triangles, _indices.Count, DrawElementsType.UnsignedInt, 0);

        Context.SwapBuffers();
        
        base.OnRenderFrame(args);
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        base.OnUpdateFrame(args);

        _camera.Update(KeyboardState, MouseState, args);
    }

    protected override void OnUnload() {
        base.OnUnload();

        _vao.Delete();
        _ibo.Delete();
        _shaderProgram.Delete();
        _texture.Delete();
    }
}