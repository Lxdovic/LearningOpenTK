using OpenTK.Mathematics;

namespace LearningOpenTK.World;

internal class Block {
    public Dictionary<Face, FaceData> Faces;
    public Vector3 Position;
    public BlockType Type;

    public List<Vector2> Uv = [
        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 1.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(0.0f, 0.0f)
    ];

    public Block(Vector3 position, BlockType type) {
        Position = position;
        Type = type;

        Faces = new Dictionary<Face, FaceData> {
            {
                Face.Front, new FaceData {
                    Vertices = AddTransformVertices(RawFaceData.RawVertexData[Face.Front]),
                    Uv = Uv
                }
            }, {
                Face.Back, new FaceData {
                    Vertices = AddTransformVertices(RawFaceData.RawVertexData[Face.Back]),
                    Uv = Uv
                }
            }, {
                Face.Left, new FaceData {
                    Vertices = AddTransformVertices(RawFaceData.RawVertexData[Face.Left]),
                    Uv = Uv
                }
            }, {
                Face.Right, new FaceData {
                    Vertices = AddTransformVertices(RawFaceData.RawVertexData[Face.Right]),
                    Uv = Uv
                }
            }, {
                Face.Top, new FaceData {
                    Vertices = AddTransformVertices(RawFaceData.RawVertexData[Face.Top]),
                    Uv = Uv
                }
            }, {
                Face.Bottom, new FaceData {
                    Vertices = AddTransformVertices(RawFaceData.RawVertexData[Face.Bottom]),
                    Uv = Uv
                }
            }
        };
    }

    private List<Vector3> AddTransformVertices(List<Vector3> vertices) {
        return vertices.Select(vertex => vertex + Position).ToList();
    }

    public FaceData GetFace(Face face) {
        return Faces[face];
    }
}