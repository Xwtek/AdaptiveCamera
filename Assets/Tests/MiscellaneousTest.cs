using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Collections;
using Unity.Mathematics;
using AdaptiveCamera.Util;

public class NativeArrayTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void NativeArrayTestSimplePasses()
    {
        var arr = new NativeArray<int>(1, Allocator.Temp);
        Set(arr);
        Assert.AreEqual(arr[0], 1);
    }
    public void Set(NativeArray<int> data){
        data[0] =1;
    }
    [Test]
    public void AtanIsNull(){
        //Assert.IsNaN(math.atan());
        Assert.AreEqual(math.atan2(math.sqrt(3), 1), math.PI / 3);
        Assert.AreEqual(math.atan2(math.sqrt(3), -1), 2* math.PI / 3);
        Assert.AreEqual(math.atan2(-math.sqrt(3), -1), -2* math.PI / 3);
        Assert.AreEqual(math.atan2(-math.sqrt(3), 1), -math.PI / 3);
    }
    [Test]
    public void CameraAngleReversible(){
        var point = math.normalize(new float3(2, 1, 3));
        var error = point- MathUtil.FromCameraAngle(MathUtil.GetCameraAngle(point));
        var maxError = math.max(error.x, math.max(error.y, error.z));
        Assert.Zero(maxError);
    }
    [Test]
    public void CubicTest(){
        var (s1,s2,s3)=MathUtil.SolveCubic(1,-6,11,-6);
        TestSolution(s1);
        TestSolution(s2);
        TestSolution(s3);
    }
    private void TestSolution(complex s){
        Assert.Less(s.value.y,0.001, "The solution are all real, got"+s);
        for(var i = 1; i<4;i++) if(Mathf.Approximately(s.value.x,i)) return;
        Assert.Fail("The solution is 1, 2, or 3, got "+s);
    }
}
