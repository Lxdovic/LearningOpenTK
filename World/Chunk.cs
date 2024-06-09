using LearningOpenTK.Graphics;
using OpenTK.Graphics.OpenGL.Compatibility;
using OpenTK.Mathematics;

namespace LearningOpenTK.World;

internal class Chunk {
    private const int Size = 16;
    private const int Height = 32;
    private readonly List<uint> _chunkIndices;
    private readonly List<Vector2> _chunkUvs;
    private readonly List<Vector3> _chunkVertices;
    private Ibo _ibo;
    private uint _indexCount;

    private Texture _texture;
    private Vbo _uvVbo;

    private Vao _vao;
    private Vbo _vertexVbo;

    public Vector3 Position;

    public Chunk(Vector3 position) {
        Position = position;

        _chunkVertices = new List<Vector3>();
        _chunkUvs = new List<Vector2>();
        _chunkIndices = new List<uint>();

        GenerateBlocks();
        BuildChunk();
    }

    public void GenerateChunk() {
    }

    public void GenerateBlocks() {
        for (var i = 0; i < Size; i++)
        for (var j = 0; j < Height; j++)
        for (var k = 0; k < Size; k++) {
            var block = new Block(new Vector3(i, j, k));

            foreach (var face in Enum.GetValues<Face>()) {
                var faceData = block.GetFace(face);

                _chunkVertices.AddRange(faceData.Vertices);
                _chunkUvs.AddRange(faceData.Uv);
                AddIndices(1);
            }
        }
    }

    private void AddIndices(int amount) {
        for (var i = 0; i < amount; i++) {
            _chunkIndices.AddRange(new List<uint> {
                _indexCount, 1 + _indexCount, 2 + _indexCount,
                2 + _indexCount, 3 + _indexCount, _indexCount
            });

            _indexCount += 4;
        }
    }

    public void BuildChunk() {
        _vao = new Vao();
        _vao.Bind();

        _vertexVbo = new Vbo(_chunkVertices);
        _vertexVbo.Bind();
        _vao.LinkToVao(0, 3, _vertexVbo);

        _uvVbo = new Vbo(_chunkUvs);
        _uvVbo.Bind();
        _vao.LinkToVao(1, 2, _uvVbo);

        _ibo = new Ibo(_chunkIndices);

        _texture = new Texture("resources/textures/dirt.png");
    }

    public void Render(ShaderProgram shaderProgram) {
        shaderProgram.Bind();
        _vao.Bind();
        _ibo.Bind();
        _texture.Bind();

        GL.DrawElements(PrimitiveType.Triangles, _chunkIndices.Count, DrawElementsType.UnsignedInt, 0);
    }

    public void Dispose() {
        _vao.Delete();
        _vertexVbo.Delete();
        _uvVbo.Delete();
        _ibo.Delete();
        _texture.Delete();
    }
}