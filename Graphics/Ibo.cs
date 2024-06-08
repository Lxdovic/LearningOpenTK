using OpenTK.Graphics.OpenGL.Compatibility;

namespace LearningOpenTK.Graphics;

internal sealed class Ibo {
    private readonly int _id;

    public Ibo(List<uint> data) {
        _id = GL.GenBuffer();

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _id);
        GL.BufferData(BufferTarget.ElementArrayBuffer, data.ToArray(), BufferUsage.StaticDraw);
    }

    public void Bind() {
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _id);
    }

    public void UnBind() {
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
    }

    public void Delete() {
        GL.DeleteBuffer(_id);
    }
}