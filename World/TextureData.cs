using OpenTK.Mathematics;

namespace LearningOpenTK.World;

internal static class TextureData {
    public static Dictionary<BlockType, Vector2[]> BlockTypeUvs =
        new() {
            {
                BlockType.Dirt, [
                    new Vector2(2f, 15f),
                    new Vector2(2f, 15f),
                    new Vector2(2f, 15f),
                    new Vector2(2f, 15f),
                    new Vector2(2f, 15f),
                    new Vector2(2f, 15f)
                ]
            }, {
                BlockType.Grass, [
                    new Vector2(7f, 13f),
                    new Vector2(2f, 15f),
                    new Vector2(3f, 15f),
                    new Vector2(3f, 15f),
                    new Vector2(3f, 15f),
                    new Vector2(3f, 15f)
                ]
            }
        };
}