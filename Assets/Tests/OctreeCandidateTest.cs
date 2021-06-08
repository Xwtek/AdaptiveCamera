using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Collections;
using Unity.Mathematics;
using AdaptiveCamera.Data;
using AdaptiveCamera.Algorithm;
using AdaptiveCamera.Util;
using System;
using System.Linq;

public class OctreeCandidateTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void SplitDefault()
    {
        var arr = new NativeArray<OctreeNode>(8, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        new OctreeNode(){size = 1f, shifted = new float3(0,2,1), pass=1}.SplitDefault(arr, 0.75f, new float3(0,0,0), 0);
        var resarr = arr.ToArray();
        for (int i = 0; i < 8; i++)
        {
            var item = resarr[i];
            if (item.pass != 2) Assert.Fail("Pass count doesn't get incremented");
            var sizeError = item.size - new float3(0.75f, 0.75f, 0.75f);
            if (math.max(sizeError.x, math.max(sizeError.y, sizeError.z)) > 0.01)
            {
                Assert.Fail("Size is invalid");
            }
            var shiftedCorner = GetCorners(item.shifted, item.size).ToList();
            var currentCorner = GetCorners(item.current, new float3(0.5f, 0.5f, 0.5f)).ToList();
            var originalCorner = GetCorners(new float3(0, 2, 1), new float3(0.5f, 0.5f, 0.5f)).ToList();
            if (!Meets(shiftedCorner, originalCorner, 0.01f)) Assert.Fail("The ShiftedCenter is incorrect");
            if (!Meets(currentCorner, Enumerable.Repeat(new float3(0, 2, 1), 1), 0.01f)) Assert.Fail("The Center is incorrect");
            var dir1 = math.normalize(item.shifted - new float3(0, 2, 1));
            var dir2 = math.normalize(item.current - new float3(0, 2, 1));
            if (!Mathf.Approximately(1, math.dot(dir1, dir2))) Assert.Fail("The centers move at different direction");
        }
    }/* [Test]
    public void SplitDefaultAngleCorrect()
    {
        var arr = new NativeArray<OctreeNode>(16, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        new OctreeNode(){size = 1f, shifted = new CameraConfiguration(){center = new float3(0,2,1)}, pass=1}.SplitDefault(arr, 0.75f, new float3(0.5f,0.5f,1.5f), 0);
        var resarr = arr.ToArray();
        for (int i = 0; i < 16; i++)
        {
            if(1 - math.dot(math.forward(arr[i].current.angle), math.forward())<0.01)Assert.Pass();
            Debug.Log(math.forward(arr[i].current.angle));
        }
        Assert.Fail("Angle not set properly");
    } */
    /*[Test]
    public void SplitFocused()
    {
        var arr = new NativeArray<OctreeNode>(4, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        new OctreeNode(){size = new float3(1,1,0), shifted = new CameraConfiguration(){center = new float3(0,2,1)}, pass=1}.SplitFocused(arr, 0.75f, new float3(0,0,0), new float3(1,1,0), 0);
        var resarr = arr.ToArray();
        foreach( var item in resarr){
            if(item.pass != 2) Assert.Fail("Pass count doesn't get incremented");
            var sizeError = item.size - new float3(0.75f, 0.75f, 0.75f);
            if(math.max(sizeError.x, sizeError.y) > 0.01){
                Assert.Fail("Size is invalid");
            }
            var searchSize = item.size;
            searchSize.z = 0;
            var shiftedCorner = GetCorners(item.shifted.center, searchSize).ToList();
            var currentCorner = GetCorners(item.current.center, new float3(0.5f, 0.5f, 0f)).ToList();
            var newCenter = MathUtil.PointClosestToALine(new float2(0,0), new float2(1,0), new float2(0,1)).xyy;
            newCenter.y = 2;
            var originalCorner = GetCorners(newCenter, new float3(0.5f, 0.5f, 0f)).ToList();
            if(!Meets(shiftedCorner, originalCorner, 0.01f)) Assert.Fail("The ShiftedCenter is incorrect");
            if(!Meets(currentCorner, Enumerable.Repeat(newCenter,1), 0.01f)) Assert.Fail("The Center is incorrect");
            var dir1 = math.normalize(item.shifted.center - new float3(0,2,0));
            var dir2 = math.normalize(item.current.center - new float3(0,2,0));
            if(!Mathf.Approximately(1, math.dot(dir1, dir2))) Assert.Fail("The centers move at different direction");
            if(!Mathf.Approximately(0, item.shifted.center.z)) Assert.Fail("The camera moves sideways");
            if(!Mathf.Approximately(0, item.current.center.z)) Assert.Fail("The camera moves sideways");
        }
    }
    */
    IEnumerable<float3> GetCorners(float3 center, float3 size){
        for (var i = -1; i < 2; i+=2){
            for (var j = -1; j < 2; j+=2){
                for (var k = -1; k < 2; k+=2){
                    yield return center + new float3(i, j, k) * size;
                }
            }
        }
    }
    float MaxDifference(float3 a, float3 b){
        var diff = math.abs(a - b);
        return math.max(diff.x, math.max(diff.y, diff.z));
    }
    bool Meets(IEnumerable<float3> cornersA, IEnumerable<float3> cornersB, float delta){
        foreach(var cornerA in cornersA){
            foreach(var cornerB in cornersB){
                if(MaxDifference(cornerA, cornerB) < delta) return true;
            }
        }
        return false;
    }
/*
    [Test]
    public void SplitFocused2()
    {
        var arr = new NativeArray<OctreeNode>(4, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        var center = new float3(-7.141448f, 3.687081f, 10.81654f);
        var playerPos = new float3(-4.0928483f, 1.13856959f, 11.7892685f);
        var targetPos = new float3(-1.28383017f, 3.32999992f, 12.6855545f);
        new OctreeNode(){size = new float3(5,5,0), shifted = new CameraConfiguration(){center = center}, pass=1}.SplitFocused(arr, 0.5f, playerPos, targetPos, 0);
        var resarr = arr.ToArray();
        foreach( var item in resarr){
            if(item.pass != 2) Assert.Fail("Pass count doesn't get incremented");
            var sizeError = item.size - new float3(2.5f, 2.5f, 0);
            if(math.max(sizeError.x, sizeError.y) > 0.01){
                Assert.Fail("Size is invalid");
            }
            var searchSize = item.size;
            searchSize.z = 0;
            var dir1 = math.normalize(item.shifted.center.xz);
            var dir2 = math.normalize(item.current.center.xz);
            var a = playerPos.xz - item.current.center.xz;
            var b = targetPos.xz - item.current.center.xz;
            var angle = math.dot(a, b) / (math.length(a) * math.length(b));
            if(!Mathf.Approximately(1, angle))Assert.Fail("The player position does not form a line");
        }
    }
    */
}
