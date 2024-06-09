using OpenTK.Graphics.OpenGL.Compatibility;
using StbImageSharp;

namespace LearningOpenTK.Graphics;

internal sealed class Texture {
    public Texture(string path) {
        Id = GL.GenTexture();

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2d, Id);

        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        StbImage.stbi_set_flip_vertically_on_load(1);
        var dirtTexture =
            ImageResult.FromStream(
                File.OpenRead(Path.Combine(Environment.CurrentDirectory, "resources/textures/atlas.png")),
                ColorComponents.RedGreenBlueAlpha);

        GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, dirtTexture.Width, dirtTexture.Height, 0,
            PixelFormat.Rgba, PixelType.UnsignedByte, dirtTexture.Data);

        GL.BindTexture(TextureTarget.Texture2d, 0);
    }

    public int Id { get; }

    public void Bind() {
        GL.BindTexture(TextureTarget.Texture2d, Id);
    }

    public void UnBind() {
        GL.BindTexture(TextureTarget.Texture2d, 0);
    }

    public void Delete() {
        GL.DeleteTexture(Id);
    }
}