using LearningOpenTK.Graphics;
using LearningOpenTK.World;
using OpenTK.Graphics.OpenGL.Compatibility;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Simplex;

namespace LearningOpenTK;

internal sealed class Game : GameWindow {
    private const int RenderDistance = 10;
    internal static Texture Atlas = new("resources/textures/atlas.png");
    public static Noise HeatNoise = new();
    public static Noise HeightNoise = new();
    private readonly Camera _camera;
    private readonly List<Chunk> _chunks = new();
    private readonly Queue<Chunk> _chunksToUnload = new();
    private readonly ShaderProgram _shaderProgram;

    private Vector3 _currentChunk;
    private int _width, _height;

    public Game(int width, int height) : base(GameWindowSettings.Default,
        NativeWindowSettings.Default) {
        _width = width;
        _height = height;
        _shaderProgram = new ShaderProgram("resources/shaders/default.vert", "resources/shaders/default.frag");
        _camera = new Camera(_width, _height, new Vector3(0f, 44f, 3f));
        
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

        LoadChunks();

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

        var currentChunk =
            new Vector3((int)(_camera.Position.X / Chunk.Size) * Chunk.Size, 0,
                (int)(_camera.Position.Z / Chunk.Size) * Chunk.Size);

        if (currentChunk != _currentChunk) {
            _currentChunk = currentChunk;
            LoadChunks();
        }

        const int chunksToUnloadPerFrame = 2;

        for (var i = 0; i < chunksToUnloadPerFrame && _chunksToUnload.Count > 0; i++) {
            var chunk = _chunksToUnload.Dequeue();
            chunk.Dispose();
            _chunks.Remove(chunk);
        }
    }

    private void LoadChunks() {
        for (var x = -RenderDistance; x <= RenderDistance; x++)
        for (var z = -RenderDistance; z <= RenderDistance; z++) {
            var chunkPosition = new Vector3(_currentChunk.X + x * Chunk.Size, 0, _currentChunk.Z + z * Chunk.Size);

            if (!ChunkExists(chunkPosition) &&
                Vector3.Distance(chunkPosition, _currentChunk) <= RenderDistance * Chunk.Size)
                _chunks.Add(new Chunk(chunkPosition));
        }

        foreach (var chunk in _chunks)
            if (Vector3.Distance(chunk.Position, _currentChunk) > RenderDistance * Chunk.Size)
                _chunksToUnload.Enqueue(chunk);
    }

    private bool ChunkExists(Vector3 position) {
        return _chunks.Any(chunk => chunk.Position == position);
    }

    protected override void OnUnload() {
        base.OnUnload();

        _shaderProgram.Delete();
        Atlas.Delete();

        foreach (var chunk in _chunks)
            chunk.Dispose();
    }
}