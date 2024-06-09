using OpenTK.Mathematics;

namespace LearningOpenTK.World;

internal class Block {
    public Dictionary<Face, FaceData> Faces;
    public Vector3 Position;
    public BlockType Type;

    public Dictionary<Face, List<Vector2>> Uvs = new() {
        { Face.Front, new List<Vector2>() },
        { Face.Back, new List<Vector2>() },
        { Face.Left, new List<Vector2>() },
        { Face.Right, new List<Vector2>() },
        { Face.Top, new List<Vector2>() },
        { Face.Bottom, new List<Vector2>() }
    };

    public Block(Vector3 position, BlockType type) {
        Position = position;
        Type = type;

        if (type != BlockType.Air) Uvs = GetUVsFromCoordinates(TextureData.BlockTypeUvs[type]);

        Faces = new Dictionary<Face, FaceData> {
            {
                Face.Front, new FaceData {
                    Vertices = AddTransformVertices(RawFaceData.RawVertexData[Face.Front]),
                    Uv = Uvs[Face.Front]
                }
            }, {
                Face.Back, new FaceData {
                    Vertices = AddTransformVertices(RawFaceData.RawVertexData[Face.Back]),
                    Uv = Uvs[Face.Back]
                }
            }, {
                Face.Left, new FaceData {
                    Vertices = AddTransformVertices(RawFaceData.RawVertexData[Face.Left]),
                    Uv = Uvs[Face.Left]
                }
            }, {
                Face.Right, new FaceData {
                    Vertices = AddTransformVertices(RawFaceData.RawVertexData[Face.Right]),
                    Uv = Uvs[Face.Right]
                }
            }, {
                Face.Top, new FaceData {
                    Vertices = AddTransformVertices(RawFaceData.RawVertexData[Face.Top]),
                    Uv = Uvs[Face.Top]
                }
            }, {
                Face.Bottom, new FaceData {
                    Vertices = AddTransformVertices(RawFaceData.RawVertexData[Face.Bottom]),
                    Uv = Uvs[Face.Bottom]
                }
            }
        };
    }

    public Dictionary<Face, List<Vector2>> GetUVsFromCoordinates(Dictionary<Face, Vector2> coords) {
        var faceData = new Dictionary<Face, List<Vector2>>();

        foreach (var face in coords)
            faceData[face.Key] = [
                new Vector2((face.Value.X + 1f) / 16f, (face.Value.Y + 1f) / 16f),
                new Vector2(face.Value.X / 16f, (face.Value.Y + 1f) / 16f),
                new Vector2(face.Value.X / 16f, face.Value.Y / 16f),
                new Vector2((face.Value.X + 1f) / 16f, face.Value.Y / 16f)
            ];

        return faceData;
    }

    private List<Vector3> AddTransformVertices(List<Vector3> vertices) {
        return vertices.Select(vertex => vertex + Position).ToList();
    }

    public FaceData GetFace(Face face) {
        return Faces[face];
    }
}