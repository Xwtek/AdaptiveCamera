using AdaptiveCamera.Data;
using Unity.Collections;
using Unity.Mathematics;
using AdaptiveCamera.Util;
namespace AdaptiveCamera.Algorithm{
    public static class OctreeCandidate{
        public static void SplitDefault(this OctreeNode original,
            NativeSlice<OctreeNode> afterSplit,
            float scale, float3 playerPos, int startsAt){
            for (var i = -1; i < 2; i += 2)
            {
                for (var j = -1; j < 2; j += 2)
                {
                    for (var k = -1; k < 2; k += 2)
                    {
                            var copied = original;
                            copied.current = copied.shifted;
                            var reduction = copied.size * new float3(i, j, k);
                            var index = (i + 1) / 2 + j + 1 + (k + 1) * 2;
                            copied.current += reduction / 2;
                            copied.pass++;
                            copied.size *= scale;
                            copied.shifted += reduction * (1 - scale);
                            afterSplit[index] = copied;
                    }
                }
            }
        }
    }
}