using LearningOpenTK.Graphics;
using LearningOpenTK.World;
using OpenTK.Graphics.OpenGL.Compatibility;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace LearningOpenTK;

internal sealed class Game : GameWindow {
    private readonly List<Chunk> _chunks = new();
    private readonly int _renderDistance = 4;
    private Camera _camera;
    private ShaderProgram _shaderProgram;

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


        for (var x = -_renderDistance; x < _renderDistance; x++)
        for (var z = -_renderDistance; z < _renderDistance; z++)
            _chunks.Add(new Chunk(new Vector3(x * Chunk.Size, 0, z * Chunk.Size)));

        // _chunks.Add(new Chunk(Vector3.Zero));

        _shaderProgram = new ShaderProgram("resources/shaders/default.vert", "resources/shaders/default.frag");

        GL.Enable(EnableCap.DepthTest);
        GL.FrontFace(FrontFaceDirection.Cw);
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(TriangleFace.Back);

        _camera = new Camera(_width, _height, new Vector3(0f, 44f, 3f));
        CursorState = CursorState.Grabbed;
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        var model = Matrix4.Identity;
        var view = _camera.GetViewMatrix();
        var projection = _camera.GetProjectionMatrix();

        var modelLocation = GL.GetUniformLocation(_shaderProgram.Id, "model");
        var viewLocation = GL.GetUniformLocation(_shaderProgram.Id, "view");
        var projectionLocation = GL.GetUniformLocation(_shaderProgram.Id, "projection");

        GL.UniformMatrix4f(modelLocation, 1, true, model);
        GL.UniformMatrix4f(viewLocation, 1, true, view);
        GL.UniformMatrix4f(projectionLocation, 1, true, projection);


        foreach (var chunk in _chunks)
            chunk.Render(_shaderProgram);

        Context.SwapBuffers();

        base.OnRenderFrame(args);
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        base.OnUpdateFrame(args);

        _camera.Update(KeyboardState, MouseState, args);
    }

    protected override void OnUnload() {
        base.OnUnload();
    }
}