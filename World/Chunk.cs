using LearningOpenTK.Graphics;
using OpenTK.Graphics.OpenGL.Compatibility;
using OpenTK.Mathematics;
using SimplexNoise;

namespace LearningOpenTK.World;

internal class Chunk {
    internal const int Size = 16;
    private const int Height = 32;

    private readonly Block[,,] _blocks = new Block[Size, Height, Size];
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

        var heightMap = GenerateChunk();

        GenerateBlocks(heightMap);
        GenFaces();
        BuildChunk();
    }

    public float[,] GenerateChunk() {
        var heightmap = new float[Size, Size];


        for (var x = 0; x < Size; x++)
        for (var z = 0; z < Size; z++)
            heightmap[x, z] = Noise.CalcPixel2D((int)(Position.X + x), (int)(Position.Z + z), 0.01f) / 128f / 2;

        return heightmap;
    }

    public void GenerateBlocks(float[,] heightMap) {
        for (var x = 0; x < Size; x++)
        for (var z = 0; z < Size; z++) {
            var columnHeight = (int)(heightMap[x, z] * Height);

            for (var y = 0; y < Height; y++) {
                var position = Position + new Vector3(x, y, z);

                var type = columnHeight switch {
                    _ when y < columnHeight - 1 => BlockType.Dirt,
                    _ when y == columnHeight - 1 => BlockType.Grass,
                    _ => BlockType.Air
                };

                _blocks[x, y, z] = new Block(position, type);
            }
        }
    }

    public void GenFaces() {
        for (var x = 0; x < Size; x++)
        for (var z = 0; z < Size; z++)
        for (var y = 0; y < Height; y++) {
            var block = _blocks[x, y, z];

            if (block.Type == BlockType.Air) continue;

            if (y < Height - 1 && _blocks[x, y + 1, z].Type == BlockType.Air) AddFace(block, Face.Top);
            if (y == Height - 1) AddFace(block, Face.Top);
            if (y > 0 && _blocks[x, y - 1, z].Type == BlockType.Air) AddFace(block, Face.Bottom);
            if (y == 0) AddFace(block, Face.Bottom);
            if (x > 0 && _blocks[x - 1, y, z].Type == BlockType.Air) AddFace(block, Face.Left);
            if (x == 0) AddFace(block, Face.Left);
            if (x < Size - 1 && _blocks[x + 1, y, z].Type == BlockType.Air) AddFace(block, Face.Right);
            if (x == Size - 1) AddFace(block, Face.Right);
            if (z > 0 && _blocks[x, y, z - 1].Type == BlockType.Air) AddFace(block, Face.Back);
            if (z == 0) AddFace(block, Face.Back);
            if (z < Size - 1 && _blocks[x, y, z + 1].Type == BlockType.Air) AddFace(block, Face.Front);
            if (z == Size - 1) AddFace(block, Face.Front);
        }
    }

    private void AddFace(Block block, Face face) {
        var faceData = block.GetFace(face);

        _chunkVertices.AddRange(faceData.Vertices);
        _chunkUvs.AddRange(faceData.Uv);

        AddIndices(1);
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

        _texture = new Texture("resources/textures/atlas.png");
    }

    public void Render(ShaderProgram shaderProgram) {
        shaderProgram.Bind();
        _vao.Bind();
        _ibo.Bind();
        _texture.Bind();

        GL.DrawElements(PrimitiveType.Triangles, _chunkIndices.Count, DrawElementsType.UnsignedInt, 0);

        _vao.UnBind();
        _ibo.UnBind();
    }

    public void Dispose() {
        _vao.Delete();
        _vertexVbo.Delete();
        _uvVbo.Delete();
        _ibo.Delete();
        _texture.Delete();
    }
}