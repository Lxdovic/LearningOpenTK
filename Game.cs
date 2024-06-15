using LearningOpenTK.Graphics;
using OpenTK.Graphics.OpenGL.Compatibility;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace LearningOpenTK;

internal sealed class Game : GameWindow {
    internal static readonly Texture Atlas = new("resources/textures/atlas.png");
    private readonly Camera _camera;
    private readonly ShaderProgram _shaderProgram;
    private readonly World _world;

    private int _width, _height;

    public Game(int width, int height) : base(GameWindowSettings.Default,
        NativeWindowSettings.Default) {
        _width = width;
        _height = height;
        _shaderProgram = new ShaderProgram("resources/shaders/default.vert", "resources/shaders/default.frag");
        _camera = new Camera(_width, _height, new Vector3(0f, 44f, 3f));

        _world = new World();

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

        GL.Enable(EnableCap.DepthTest);
        GL.FrontFace(FrontFaceDirection.Cw);
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(TriangleFace.Back);

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

        _world.Render(_shaderProgram);

        Title = "MCC; FPS: " + (1f / args.Time).ToString("0.");

        Context.SwapBuffers();

        base.OnRenderFrame(args);
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        base.OnUpdateFrame(args);

        _camera.Update(KeyboardState, MouseState, args);
    }
}