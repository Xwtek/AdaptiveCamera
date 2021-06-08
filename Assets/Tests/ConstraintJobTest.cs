using UnityEngine.TestTools;
using AdaptiveCamera.Algorithm;
using Unity.Collections;
using AdaptiveCamera.Data;
using Unity.Mathematics;
using Unity.Jobs;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using AdaptiveCamera.Util;
public class ConstraintJobTest
{
    // A Test behaves as an ordinary method
    [UnityTest]
    public IEnumerator ConstraintJobNoError()
    {
        var candidates = MakeArray(new OctreeNode
        {
            current =  new float3(0, 0, 0)
        });
        var constraints = MakeArray(
            ConstraintUtil.MakeDistanceConstraint(0.5f, 8f, 0.4f, new float3(1f, 1f, 2f), quaternion.identity),
            ConstraintUtil.MakeViewpointConstraint(0.5f, 8f, new float3(1f, 1f, 2f), quaternion.identity)
        );
        var job = new ConstraintJob
        {
            constraints = constraints,
            candidates = candidates
        };
        var handle = job.Schedule(default);
        yield return null;
        handle.Complete();
        candidates.Dispose();
        constraints.Dispose();
    }
    [UnityTest]
    public IEnumerator ConstraintJobNoChange()
    {
        var candidates = MakeArray(
            new OctreeNode{ current =  new float3(0, 0, 0)} 
        );
        var constraints = MakeArray(
            ConstraintUtil.MakeDistanceConstraint(5f, 8f, 0.4f, new float3(3f, 0f, 4f), quaternion.identity)
        );
        var job = new ConstraintJob
        {
            candidates = candidates,
            constraints = constraints
        };
        var handle = job.Schedule(default);
        yield return null;
        handle.Complete();
        Assert.IsTrue(Mathf.Approximately(candidates[0].score, 0));
    }
    public NativeArray<T> MakeArray<T>(params T[] items) where T: struct{
        return new NativeArray<T>(items, Allocator.TempJob);
    }
    [UnityTest]
    public IEnumerator ConstraintJobCameraAngle()
    {
        return ChainEnumerator(
        TestAngle(math.forward().xz, 0),
        TestAngle(math.right().xz, 4),
        TestAngle(math.back().xz, 8),
        TestAngle(math.left().xz, 4));
    }
    public IEnumerator TestAngle(float2 direction, float expected)
    {
        var candidates = new NativeArray<OctreeNode>(new OctreeNode[]{
                new OctreeNode{current= new float3(0,0,0)}},
                Allocator.TempJob);
        var constraints = new NativeArray<ConstraintData>(new ConstraintData[]{
                ConstraintUtil.MakeCameraAngleConstraint(8f, direction, math.PI, math.forward().xz)
    }, Allocator.TempJob);
        var job = new ConstraintJob
        {
            constraints = constraints,
            candidates = candidates,
            frameCoherenceCost = 0,
            playerPosition = math.forward(),
        };
        var handle = job.Schedule(default);
        yield return null;
        handle.Complete();
        Assert.IsTrue(Mathf.Approximately(candidates[0].score, expected), "Job failed. Expected: "+expected+", actual: "+candidates[0].score, "direction: "+direction);
        candidates.Dispose();
        constraints.Dispose();
    }
    [UnityTest]
    public IEnumerator ConstraintCoroutineSampleRun()
    {
        var coro = new AdaptiveCameraCoroutine(30000, 16, 0.75f, 48, 100, 100);
        coro.size = 1;
        coro.Initialize();
        coro.worktimePerFrame = 30;
        var constraints = new NativeArray<ConstraintData>(new ConstraintData[]{
                ConstraintUtil.MakeDistanceConstraint(1f, 1f, 1f, new float3(3f, 0f, 4f), quaternion.identity),
                ConstraintUtil.MakeCameraAngleConstraint(8f, math.right().xz, math.PI, math.forward().xz)
            }
            , Allocator.Persistent);
        /*
        coro.BeginRun(
            new CameraConfiguration
            {
                center = new float3(2.4f, 0.6f, 3.3f),
                angle = quaternion.LookRotation(math.forward(), math.up())
            },
            new float3(3f, 0f, 4f),
            constraints
        );
        while(!coro.finished){
            coro.BeginRound();
            yield return null;
        }
        /*/
        var exec = coro.Run(new float3(2.4f, 0.6f, 3.3f),
            new float3(3f, 0f, 4f),
            constraints);
        while(exec.MoveNext()) yield return exec.Current;
        //*/
        Debug.Log(coro.Best.Value.current);
        Assert.Less(math.length(coro.Best.Value.current.xz - new float2(3, 4)), 1.01f);
    }
    public IEnumerator ChainEnumerator(params IEnumerator[] enumerators)
    {
        foreach(var enumerator in enumerators){
            while(enumerator.MoveNext()) yield return enumerator.Current;
        }
    }
    [Test]
    public void MakeCameraAngleConstraint(){
        var constraint = ConstraintUtil.MakeCameraAngleConstraint(0, math.forward().xz, math.radians(60), math.right().xz);
        var angle = math.degrees(math.acos(math.dot(math.forward(constraint.rotation), math.forward())));
        Debug.Log(angle);
        Assert.IsTrue(Mathf.Approximately(angle, 30));
    }
}