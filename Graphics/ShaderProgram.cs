using OpenTK.Graphics.OpenGL.Compatibility;

namespace LearningOpenTK.Graphics;

internal sealed class ShaderProgram {
    internal readonly int Id;

    public ShaderProgram(string vertexShaderFilePath, string fragmentShaderFilePath) {
        Id = GL.CreateProgram();

        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        var vertexShaderSource =
            LoadShaderSource(Path.Combine(Environment.CurrentDirectory, vertexShaderFilePath));
        var fragmentShaderSource =
            LoadShaderSource(Path.Combine(Environment.CurrentDirectory, fragmentShaderFilePath));

        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);

        GL.AttachShader(Id, vertexShader);
        GL.AttachShader(Id, fragmentShader);
        GL.LinkProgram(Id);

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    public void Bind() {
        GL.UseProgram(Id);
    }

    public void UnBind() {
        GL.UseProgram(0);
    }

    public void Delete() {
        GL.DeleteProgram(Id);
    }

    private static string LoadShaderSource(string path) {
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