global using Chunk = (int x, int z, uint[] blocks);
using LearningOpenTK.Graphics;
using LearningOpenTK.Worldd;
using OpenTK.Graphics.OpenGL.Compatibility;
using OpenTK.Mathematics;
using Simplex;

namespace LearningOpenTK;

internal sealed class World {
    internal const int RenderDistance = 10;
    internal const int ChunkSize = 16;
    internal const int ChunkHeight = 64;
    private static readonly Noise HeatNoise = new();
    private static readonly Noise HeightNoise = new();
    private readonly List<uint> _chunkIndices = [];
    private readonly List<Vector2> _chunkUvs = [];
    private readonly List<Vector3> _chunkVertices = [];

    private readonly Ibo _ibo;
    private readonly Vbo _uvVbo;
    private readonly Vao _vao;
    private readonly Vbo _vertexVbo;
    private uint _indexCount;

    public World() {
        for (var x = -RenderDistance; x < RenderDistance; x++)
        for (var z = -RenderDistance; z < RenderDistance; z++)
            Chunks.Add((x, z, new uint[ChunkSize * ChunkHeight * ChunkSize]));

        for (var chunkIndex = 0; chunkIndex < Chunks.Count; chunkIndex++) {
            var chunk = Chunks[chunkIndex];
            var chunkPosition = new Vector3(chunk.x * ChunkSize, 0, chunk.z * ChunkSize);
            var heightMap = new float[ChunkSize, ChunkSize];
            var heatMap = new float[ChunkSize, ChunkSize];

            for (var x = 0; x < ChunkSize; x++)
            for (var z = 0; z < ChunkSize; z++) {
                heightMap[x, z] =
                    HeightNoise.CalcPixel2D((int)(chunkPosition.X + x), (int)(chunkPosition.Z + z), 0.005f) / 255f;
                heatMap[x, z] = (
                    HeatNoise.CalcPixel2D((int)(chunkPosition.X + x), (int)(chunkPosition.Z + z), 0.001f) / 255f * 2 +
                    HeatNoise.CalcPixel2D((int)(chunkPosition.X + x), (int)(chunkPosition.Z + z), 0.01f) / 255f / 2
                ) / 3f;
            }

            for (var x = 0; x < ChunkSize; x++)
            for (var z = 0; z < ChunkSize; z++) {
                var columnHeight = (int)(heightMap[x, z] * 32);
                var heat = heatMap[x, z];

                for (var y = 0; y < ChunkHeight; y++) {
                    var index = x + y * ChunkSize + z * ChunkSize * ChunkHeight;
                    if (heat < 0.6) {
                        var type = columnHeight switch {
                            _ when y <= columnHeight - 4 => BlockType.Stone,
                            _ when y <= columnHeight - 2 && y > columnHeight - 4 => BlockType.Dirt,
                            _ when y <= columnHeight - 1 => BlockType.Grass,
                            _ => BlockType.Air
                        };

                        chunk.blocks[index] = BlockHelper.Encode(type, x, y, z);
                    }

                    if (heat >= 0.6) {
                        var type = columnHeight switch {
                            _ when y <= columnHeight - 4 => BlockType.SandStone,
                            _ when y <= columnHeight - 1 && y > columnHeight - 4 => BlockType.Sand,
                            _ => BlockType.Air
                        };

                        chunk.blocks[index] = BlockHelper.Encode(type, x, y, z);
                    }
                }
            }

            for (var x = 0; x < ChunkSize; x++)
            for (var z = 0; z < ChunkSize; z++)
            for (var y = 0; y < ChunkHeight; y++) {
                var index = x + y * ChunkSize + z * ChunkSize * ChunkHeight;
                var block = BlockHelper.Decode(chunk.blocks[index]);

                if (block.type == (int)BlockType.Air) continue;

                if (block.y == 0) AddFace(chunkPosition, block, Face.Bottom);
                if (block.x == 0) AddFace(chunkPosition, block, Face.Left);
                if (block.z == 0) AddFace(chunkPosition, block, Face.Back);
                if (block.y == ChunkHeight - 1) AddFace(chunkPosition, block, Face.Top);
                if (block.x == ChunkSize - 1) AddFace(chunkPosition, block, Face.Right);
                if (block.z == ChunkSize - 1) AddFace(chunkPosition, block, Face.Front);
                if (block.y > 0 && BottomBlockData(chunk, index).type == (int)BlockType.Air)
                    AddFace(chunkPosition, block, Face.Bottom);
                if (block.x > 0 && LeftBlockData(chunk, index).type == (int)BlockType.Air)
                    AddFace(chunkPosition, block, Face.Left);
                if (block.z > 0 && BackBlockData(chunk, index).type == (int)BlockType.Air)
                    AddFace(chunkPosition, block, Face.Back);
                if (block.y < ChunkHeight - 1 && TopBlockData(chunk, index).type == (int)BlockType.Air)
                    AddFace(chunkPosition, block, Face.Top);
                if (block.x < ChunkSize - 1 && RightBlockData(chunk, index).type == (int)BlockType.Air)
                    AddFace(chunkPosition, block, Face.Right);
                if (block.z < ChunkSize - 1 && FrontBlockData(chunk, index).type == (int)BlockType.Air)
                    AddFace(chunkPosition, block, Face.Front);
               
            }
        }
        
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

    internal List<Chunk> Chunks { get; } = [];

    public void Render(ShaderProgram shaderProgram) {
        shaderProgram.Bind();
        _vao.Bind();
        _ibo.Bind();
        Game.Atlas.Bind();

        GL.DrawElements(PrimitiveType.Triangles, _chunkIndices.Count, DrawElementsType.UnsignedInt, 0);

        _vao.UnBind();
        _ibo.UnBind();
    }

    private void AddFace(Vector3 chunkPosition, Block block, Face face) {
        var faceData = RawFaceData.RawVertexData[face];
        var uvData = TextureData.BlockTypeUvs[(BlockType)block.type];

        foreach (var vertex in faceData)
            _chunkVertices.Add(new Vector3(chunkPosition.X + vertex.X + block.x, chunkPosition.Y + vertex.Y + block.y,
                chunkPosition.Z + vertex.Z + block.z));

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

    private Block LeftBlockData(Chunk chunk, int blockIndex) {
        return BlockHelper.Decode(chunk.blocks[blockIndex - 1]);
    }

    private Block RightBlockData(Chunk chunk, int blockIndex) {
        return BlockHelper.Decode(chunk.blocks[blockIndex + 1]);
    }

    private Block BottomBlockData(Chunk chunk, int blockIndex) {
        return BlockHelper.Decode(chunk.blocks[blockIndex - ChunkSize]);
    }

    private Block TopBlockData(Chunk chunk, int blockIndex) {
        return BlockHelper.Decode(chunk.blocks[blockIndex + ChunkSize]);
    }

    private Block BackBlockData(Chunk chunk, int blockIndex) {
        return BlockHelper.Decode(chunk.blocks[blockIndex - ChunkSize * ChunkHeight]);
    }

    private Block FrontBlockData(Chunk chunk, int blockIndex) {
        return BlockHelper.Decode(chunk.blocks[blockIndex + ChunkSize * ChunkHeight]);
    }

    public void Render() {
        foreach (var chunk in Chunks) {
            // Render chunk
        }
    }
}