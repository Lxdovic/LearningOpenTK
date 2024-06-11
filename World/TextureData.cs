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
            }, {
                BlockType.Sand, [
                    new Vector2(2f, 14f),
                    new Vector2(2f, 14f),
                    new Vector2(2f, 14f),
                    new Vector2(2f, 14f),
                    new Vector2(2f, 14f),
                    new Vector2(2f, 14f)
                ]
            }, {
                BlockType.SandStone, [
                    new Vector2(0f, 3f),
                    new Vector2(0f, 3f),
                    new Vector2(0f, 3f),
                    new Vector2(0f, 3f),
                    new Vector2(0f, 3f),
                    new Vector2(0f, 3f)
                ]
            }, {
                BlockType.Stone, [
                    new Vector2(1f, 15f),
                    new Vector2(1f, 15f),
                    new Vector2(1f, 15f),
                    new Vector2(1f, 15f),
                    new Vector2(1f, 15f),
                    new Vector2(1f, 15f)
                ]
            }
        };
}