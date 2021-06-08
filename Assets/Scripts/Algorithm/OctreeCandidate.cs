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
/*                             copied.current.angle = quaternion.LookRotationSafe(playerPos - copied.current.center, math.up());
                            copied.shifted.angle = quaternion.LookRotationSafe(playerPos - copied.shifted.center, math.up());
 */                            //var reductionFOV = l * copied.fovSize;
                            //copied.fovSize *= scale;
                            //copied.current.fov += reductionFOV / 2;
                            //copied.shifted.fov += reductionFOV * (1 - scale);
                            afterSplit[index] = copied;
                    }
                }
            }
        }
        /*
        public static void SplitFocused(this OctreeNode original,
            NativeSlice<OctreeNode> afterSplit,
            float scale, float3 playerPos, float3 target, int startsAt){
            var axis = target - playerPos;
            axis.y = 0;
            axis = math.normalizesafe(axis);
            var newPos = MathUtil.PointClosestToALine(target.xz, playerPos.xz, original.shifted.center.xz).xyy;
            newPos.y = original.shifted.center.y;
            for (var i = -1; i < 2; i += 2)
            {
                for (var j = -1; j < 2; j += 2)
                {
                    for (var k = -1; k < 2; k += 2)
                    {                        if(k == 1 && original.fovSize < 0.001f) break;

                        var copied = original;
                        copied.shifted.center = newPos;
                        copied.current = copied.shifted;
                        var reductionSize = copied.size.x * i * axis + copied.size.y * j * math.up();
                        var index = (i + 1) / 2 + j + 1 + (k+1)*2;
                        copied.current.center += reductionSize / 2;
                        copied.pass++;
                        copied.size *= scale;
                        copied.shifted.center += reductionSize * (1 - scale);
                        copied.current.angle = quaternion.LookRotationSafe(playerPos - copied.current.center, math.up());
                        copied.shifted.angle = quaternion.LookRotationSafe(playerPos - copied.shifted.center, math.up());
                        var reductionFOV = k * copied.fovSize;
                        copied.fovSize *= scale;
                        copied.current.fov += reductionFOV / 2;
                        copied.shifted.fov += reductionFOV * (1 - scale);
                        afterSplit[index] = copied;
                    }
                }
            }
        }*/
    }
}