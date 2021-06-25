using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Jobs;
using System;
using UnityEngine;
using AdaptiveCamera.Data;
namespace AdaptiveCamera.Algorithm
{
public struct ConstraintJob : IJob{
    public PriorityQueue priorityQueue;
    public NativeSlice<OctreeNode> candidates;
    [ReadOnly]
    public NativeSlice<ConstraintData> constraints;
    public float3? previousBest;
    public float frameCoherenceCost;
    public float3 playerPosition;
    public int splitCount;
    public float scale;
    public float3 delta;
    public void Execute()
    {
        var numCandidates = 0;
        for (; numCandidates + splitCount <= candidates.Length; numCandidates += splitCount)
        {
            var currOctree = priorityQueue.Pop();
            if (!currOctree.HasValue) break;
            currOctree.Value.SplitDefault(candidates.Slice(numCandidates, splitCount), scale, playerPosition, numCandidates);
        }
        for (var idx = 0; idx < numCandidates; idx++){
            var total = 0f;
            var cameraConfig = candidates[idx].current;
            for (var i = 0; i < constraints.Length; i++)
            {
                if (constraints[i].type == ConstraintType.SKIP) continue;
                total += GetCost(constraints[i], cameraConfig+delta, playerPosition+delta);
            }
            var octree = candidates[idx];
            octree.score = total;
            if (previousBest.HasValue)
            {
                var distance = math.distance(previousBest.Value, octree.current);
                octree.score += distance * frameCoherenceCost;
            }
            candidates[idx] = octree;
        }
        priorityQueue.AddMany(candidates);
    }
        [BurstDiscard]
        public static void UncheckedType(ConstraintType type)
        {
            if (!Enum.IsDefined(typeof(ConstraintType), type))
            {
                throw new ArgumentException("Unknown constraint type: " + (int)type);
            }
            else
            {
                throw new ArgumentException("Constraint type is not implemented: " + type.ToString());
            }
        }
public static float NormalizeCost(float metric, ConstraintData constraint, float ifNan = math.INFINITY){
    var result = math.abs(metric - constraint.idealFloat);
    if(math.isnan(result)) result = ifNan;
    return math.abs(result) * constraint.cost / constraint.scale;
}
public static float NormalizeAngleCost(float metric, ConstraintData constraint, float ifNan = math.INFINITY)
{
    var result = math.abs(metric - constraint.idealFloat);
    if (math.isnan(result)) return constraint.cost;
    if (result > math.PI) result = 2 * math.PI - result;
    return math.abs(result) * constraint.cost / constraint.scale;
}
public static float GetCost(ConstraintData constraint, float3 camera, float3 playerPosition){
    switch (constraint.type)
    {
        case ConstraintType.DISTANCE:
            var realDistance = math.distance(constraint.position.xz, camera.xz);
            return NormalizeCost(realDistance, constraint);
        case ConstraintType.VANTAGE:
            {
                var height = constraint.position.y - camera.y;
                var distance = math.distance(constraint.position.xz, camera.xz);
                if (distance == 0)
                {
                    return constraint.cost;
                }
                else
                {
                    var angles = math.atan2(height, distance);
                    if (math.isnan(angles)) angles = math.PI;
                    return NormalizeAngleCost(angles, constraint);
                }
            }
        case ConstraintType.CAMERA_ANGLE:
            {
                var cameraForward = math.normalizesafe(playerPosition.xz - camera.xz);
                var objDir = math.normalize(math.forward(constraint.rotation).xz);
                var angle = math.acos(math.dot(cameraForward, objDir));
                return NormalizeAngleCost(angle, constraint);
            }
        default:
            UncheckedType(constraint.type);
            return 0; //dummy
    }
}
    }
}