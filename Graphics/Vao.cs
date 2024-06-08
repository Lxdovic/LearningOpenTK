using OpenTK.Graphics.OpenGL.Compatibility;

namespace LearningOpenTK.Graphics;

internal sealed class Vao {
    private readonly int _id;

    public Vao() {
        _id = GL.GenVertexArray();

        GL.BindVertexArray(_id);
    }

    public void LinkToVao(uint location, int size, Vbo vbo) {
        Bind();
        vbo.Bind();

        GL.VertexAttribPointer(location, size, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(location);

        UnBind();
    }

    public void Bind() {
        GL.BindVertexArray(_id);
    }

    public void UnBind() {
        GL.BindVertexArray(0);
    }

    public void Delete() {
        GL.DeleteVertexArray(_id);
    }
}