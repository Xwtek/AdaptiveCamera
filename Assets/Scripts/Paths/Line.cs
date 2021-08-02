using Unity.Mathematics;
using UnityEngine;
using AdaptiveCamera.Util;

[System.Serializable]
public struct Line
{
    public float3 pointA;
    public float3 curvePoint;
    public float3 pointB;
    public float width;
    public CameraPosition cameraPosition;
    public OptimalCameraParam cameraParam;
    public override string ToString()
    {
        return pointA.ToString() + " to " + pointB.ToString();
    }
    public float3 At(float t) => this.pointA * (1 - t) * (1 - t) + 2* this.curvePoint * t * (1 - t) + this.pointB * t * t;
    public float3 TangentAt(float t){
        var calculated = this.pointA * 2 * (t - 1) + this.curvePoint * (2 - 4 * t) + this.pointB * 2 * t;
        if(math.lengthsq(calculated)<0.000001) return math.normalize(this.pointB-this.pointA);
        return math.normalize(calculated);
    }
    public float3? PlaneNormal {
        get
        {
            var toA = pointA - curvePoint;
            var toB = pointB - curvePoint;
            var cross = math.cross(toA, toB);
            if (math.lengthsq(cross) < 0.000001) return null;
            else return math.normalize(cross);
        }
    }
    public float3? NormalAt(float t) => PlaneNormal.HasValue ? math.cross(TangentAt(t), PlaneNormal.Value) : null as float3?;
    
    private (float, float, float, float) GetCharacteristicCubicPolynom(float3 point){
        var a = this.pointA + this.pointB - 2 * this.curvePoint;
        var b = 2 * (this.curvePoint - this.pointA);
        var c = this.pointA - point;
        var p = 4 * math.csum(a * a);
        var q = 3 * math.csum(2 * a * b);
        var r = 2 * math.csum(b * b + 2 * a * c);
        var s = math.csum(2 * b * c);
        return (p, q, r, s);
    }
    public float ClosestToPoint(float3 point){
        var (p, q, r, s) = GetCharacteristicCubicPolynom(point);
        float? possT = null;
        float? distanceT = null;
        var (t1, t2, t3) = MathUtil.SolveCubic(p, q, r, s);
        DoIteration(t1, point, ref possT, ref distanceT);
        DoIteration(t2, point, ref possT, ref distanceT);
        DoIteration(t3, point, ref possT, ref distanceT);
        return ClosestToPointNewton(point, possT??0.5f);
    }
    public float ClosestToPointNewton(float3 point, float t = 0.5f)
    {
        if (t < 0) t = 0;
        if (t > 1) t = 1;
        if (math.isnan(t)) t = 0.5f;
        var (p, q, r, s) = GetCharacteristicCubicPolynom(point);
        var p1 = 3 * p;
        var q1 = 2 * q;
        var r1 = r;
        var oldT = t;
        for (var i = 0; i < 10; i++)
        {
            var newT = math.clamp(t - (p * t * t * t + q * t * t + r * t + s) / (p1 * t * t + q1 * t + r), 0, 1);
            if (Mathf.Approximately(t - newT, 0)) break;
            t = newT;
        }
        if (math.isnan(t)) return oldT;
        return math.clamp(t, 0, 1);
    }
    private void DoIteration(complex r, float3 point, ref float? possT, ref float? distanceT){
        float t;
        if(math.isnan(r.value.x)) return;
        if(math.abs(r.value.y)>0.01)return;
        else if(math.abs(r.value.x)<0)t = 0;
        else if(math.abs(r.value.x)>1)t = 1;
        else t = r.value.x;
        var distance = math.distancesq(At(t), point);
        if (!possT.HasValue || distanceT.Value > distance)
        {
            possT = t;
            distanceT = distance;
        }
    }
}
public enum CameraPosition : int
{
    ForwardBackward = 0,
    Left = 1,
    Right = 2,
}