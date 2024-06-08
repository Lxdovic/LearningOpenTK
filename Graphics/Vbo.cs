using OpenTK.Graphics.OpenGL.Compatibility;
using OpenTK.Mathematics;

namespace LearningOpenTK.Graphics;

internal sealed class Vbo {
    private readonly int _id;

    public Vbo(List<Vector3> data) {
        _id = GL.GenBuffer();

        GL.BindBuffer(BufferTarget.ArrayBuffer, _id);
        GL.BufferData(BufferTarget.ArrayBuffer, data.ToArray(), BufferUsage.StaticDraw);
    }

    public Vbo(List<Vector2> data) {
        _id = GL.GenBuffer();

        GL.BindBuffer(BufferTarget.ArrayBuffer, _id);
        GL.BufferData(BufferTarget.ArrayBuffer, data.ToArray(), BufferUsage.StaticDraw);
    }

    public void Bind() {
        GL.BindBuffer(BufferTarget.ArrayBuffer, _id);
    }

    public void UnBind() {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public void Delete() {
        GL.DeleteBuffer(_id);
    }
}