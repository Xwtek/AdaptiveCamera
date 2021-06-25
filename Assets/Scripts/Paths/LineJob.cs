using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Jobs;
using System;
using UnityEngine;
using AdaptiveCamera.Data;
using AdaptiveCamera.Util;
public struct LineJob : IJob
{
    [ReadOnly]
    public NativeArray<Line> lines;
    public NativeArray<Line> relevantLine;
    public NativeArray<float2> bestRotation;
    public NativeArray<bool> success;
    public NativeArray<OptimalCameraParam> cameraParam;
    public NativeArray<float> vantage;
    public float3 playerPosition;
    [ReadOnly]
    public float2 playerSpeed;
    public void Execute()
    {
        var playerDir = math.normalize(playerSpeed);
        float2? bestDir = null as float2?;
        float2? actualDir = null as float2?;
        for (var i = 0; i < lines.Length; i++)
        {
            var currLine = lines[i];
            var t = currLine.ClosestToPoint(playerPosition);
            var posT = currLine.At(t);
            var tangent = currLine.TangentAt(t);
            var currDir = math.normalizesafe(tangent.xz);
            var vantage = math.atan2(tangent.y, math.length(tangent.xz));
            if(math.distancesq(posT.xz, playerPosition.xz) > currLine.width*currLine.width) continue;
            var currDot = math.abs(math.dot(currDir, playerDir));
            if (!bestDir.HasValue) goto bestNull;
            var bestDot = math.abs(math.dot(bestDir.Value, playerDir));
            if (currDot < bestDot) return;
            bestNull:
            cameraParam[0] = currLine.cameraParam;
            this.vantage[0] = vantage;
            relevantLine[0] = currLine;
            success[0] = true;
            bestDir = currDir;
            switch(currLine.cameraPosition){
                case CameraPosition.ForwardBackward:
                    actualDir = currDir;
                    if(math.dot(currDir, playerDir)<0) actualDir *= -1;
                    break;
                case CameraPosition.Left:
                    actualDir = currDir.yx * new float2(1, -1);
                    break;
                case CameraPosition.Right:
                    actualDir = currDir.yx * new float2(-1, 1);
                    break;
            }
        }
        if (actualDir.HasValue)
        {
            bestRotation[0] = actualDir.Value;
        }
    }
}