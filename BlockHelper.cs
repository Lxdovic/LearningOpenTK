global using Block = (ushort type, ushort x, ushort y, ushort z);
using LearningOpenTK.Worldd;

namespace LearningOpenTK;

internal static class BlockHelper {
    internal static Block Decode(uint block) {
        var type = (ushort)((block >> 14) & 0b1111);
        var x = (ushort)((block >> 10) & 0b1111);
        var y = (ushort)((block >> 4) & 0b111111);
        var z = (ushort)(block & 0b1111);

        return (type, x, y, z);
    }

    internal static uint Encode(Block block) {
        return (uint)((block.type << 14) | (block.x << 10) | (block.y << 4) | block.z);
    }

    internal static uint Encode(BlockType type, int x, int y, int z) {
        return (uint)(((int)type << 14) | (x << 10) | (y << 4) | z);
    }
}