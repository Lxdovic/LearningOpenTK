using LearningOpenTK.Graphics;
using OpenTK.Graphics.OpenGL.Compatibility;
using OpenTK.Mathematics;
using SimplexNoise;

namespace LearningOpenTK.World;

using Block = (int type, int x, int y, int z);

internal class Chunk {
    internal const int Size = 16;
    private const int Height = 64;

    private readonly ushort[] _blocks = new ushort[Size * Height * Size];
    private readonly List<uint> _chunkIndices;
    private readonly List<Vector2> _chunkUvs;
    private readonly List<Vector3> _chunkVertices;
    private readonly Ibo _ibo;
    private readonly Vbo _uvVbo;
    private readonly Vao _vao;
    private readonly Vbo _vertexVbo;
    private uint _indexCount;

    public Vector3 Position;

    public Chunk(Vector3 position) {
        Position = position;

        _chunkVertices = new List<Vector3>();
        _chunkUvs = new List<Vector2>();
        _chunkIndices = new List<uint>();

        var heightMap = GenerateChunk();

        GenerateBlocks(heightMap);
        GenFaces();


        _vao = new Vao();
        _vao.Bind();

        _vertexVbo = new Vbo(_chunkVertices);
        _vertexVbo.Bind();
        _vao.LinkToVao(0, 3, _vertexVbo);

        _uvVbo = new Vbo(_chunkUvs);
        _uvVbo.Bind();
        _vao.LinkToVao(1, 2, _uvVbo);

        _ibo = new Ibo(_chunkIndices);
    }

    public float[,] GenerateChunk() {
        var heightmap = new float[Size, Size];

        for (var x = 0; x < Size; x++)
        for (var z = 0; z < Size; z++)
            heightmap[x, z] = Noise.CalcPixel2D((int)(Position.X + x), (int)(Position.Z + z), 0.005f) / 128f / 2;

        return heightmap;
    }

    public void GenerateBlocks(float[,] heightMap) {
        for (var x = 0; x < Size; x++)
        for (var z = 0; z < Size; z++) {
            var columnHeight = (int)(heightMap[x, z] * 32);

            for (var y = 0; y < Height; y++) {
                var type = columnHeight switch {
                    _ when y < columnHeight - 1 => BlockType.Dirt,
                    _ when y == columnHeight - 1 => BlockType.Grass,
                    _ => BlockType.Air
                };

                _blocks[x + y * Size + z * Size * Height] = (ushort)(((int)type << 14) | (x << 10) | (y << 4) | z);
            }
        }
    }

    public Block GetBlockData(ushort block) {
        var type = block >> 14;
        var x = (block >> 10) & 0b1111;
        var y = (block >> 4) & 0b111111;
        var z = block & 0b1111;

        return (type, x, y, z);
    }

    private Block LeftBlockData(Block block) {
        return GetBlockData(_blocks[block.x - 1 + block.y * Size + block.z * Size * Height]);
    }

    private Block RightBlockData(Block block) {
        return GetBlockData(_blocks[block.x + 1 + block.y * Size + block.z * Size * Height]);
    }

    private Block BottomBlockData(Block block) {
        return GetBlockData(_blocks[block.x + (block.y - 1) * Size + block.z * Size * Height]);
    }

    private Block TopBlockData(Block block) {
        return GetBlockData(_blocks[block.x + (block.y + 1) * Size + block.z * Size * Height]);
    }

    private Block BackBlockData(Block block) {
        return GetBlockData(_blocks[block.x + block.y * Size + (block.z - 1) * Size * Height]);
    }

    private Block FrontBlockData(Block block) {
        return GetBlockData(_blocks[block.x + block.y * Size + (block.z + 1) * Size * Height]);
    }


    public void GenFaces() {
        for (var x = 0; x < Size; x++)
        for (var z = 0; z < Size; z++)
        for (var y = 0; y < Height; y++) {
            var block = GetBlockData(_blocks[x + y * Size + z * Size * Height]);

            if (block.type == (int)BlockType.Air) continue;

            if (block.y == 0) AddFace(block, Face.Bottom);
            if (block.x == 0) AddFace(block, Face.Left);
            if (block.z == 0) AddFace(block, Face.Back);
            if (block.y == Height - 1) AddFace(block, Face.Top);
            if (block.x == Size - 1) AddFace(block, Face.Right);
            if (block.z == Size - 1) AddFace(block, Face.Front);
            if (block.y > 0 && BottomBlockData(block).type == (int)BlockType.Air) AddFace(block, Face.Bottom);
            if (block.x > 0 && LeftBlockData(block).type == (int)BlockType.Air) AddFace(block, Face.Left);
            if (block.z > 0 && BackBlockData(block).type == (int)BlockType.Air) AddFace(block, Face.Back);
            if (block.y < Height - 1 && TopBlockData(block).type == (int)BlockType.Air) AddFace(block, Face.Top);
            if (block.x < Size - 1 && RightBlockData(block).type == (int)BlockType.Air) AddFace(block, Face.Right);
            if (block.z < Size - 1 && FrontBlockData(block).type == (int)BlockType.Air) AddFace(block, Face.Front);
        }
    }

    private void AddFace(Block block, Face face) {
        var faceData = RawFaceData.RawVertexData[face];
        var uvData = TextureData.BlockTypeUvs[(BlockType)block.type];

        foreach (var vertex in faceData)
            _chunkVertices.Add(new Vector3(Position.X + vertex.X + block.x, Position.Y + vertex.Y + block.y,
                Position.Z + vertex.Z + block.z));

        _chunkUvs.AddRange([
            new Vector2((uvData[(int)face].X + 1) / 16f, (uvData[(int)face].Y + 1) / 16f),
            new Vector2(uvData[(int)face].X / 16f, (uvData[(int)face].Y + 1) / 16f),
            new Vector2(uvData[(int)face].X / 16f, uvData[(int)face].Y / 16f),
            new Vector2((uvData[(int)face].X + 1) / 16f, uvData[(int)face].Y / 16f)
        ]);

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

    public void Render(ShaderProgram shaderProgram) {
        shaderProgram.Bind();
        _vao.Bind();
        _ibo.Bind();
        Game.Atlas.Bind();

        GL.DrawElements(PrimitiveType.Triangles, _chunkIndices.Count, DrawElementsType.UnsignedInt, 0);

        _vao.UnBind();
        _ibo.UnBind();
    }

    public void Dispose() {
        _vao.Delete();
        _vertexVbo.Delete();
        _uvVbo.Delete();
        _ibo.Delete();
    }
}