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
    public float3? PlaneNormal {get
        {
            var toA = pointA - curvePoint;
            var toB = pointB - curvePoint;
            var cross = math.cross(toA, toB);
            if(math.lengthsq(cross)<0.000001) return null;
            else return math.normalize(cross);
        }
    }
    public float3? NormalAt(float t) => PlaneNormal.HasValue ? math.cross(TangentAt(t), PlaneNormal.Value) : null as float3?;
    public float ClosestToPoint(float3 point){
        /* Based on this proof:
        P(t)=A(1-t)^2+2Bt(1-t)+Ct^2
        P(t)=A(1-2t+t^2)+B(2t-2t^2)+Ct^2
        a = A-2B+C
        b = 2B-2A
        c = A
        minimalize (P(t)-p)^2 = (at^2+bt+c-p)^2
        u = c-p
        minimalize at^4+2abt^3+(b^2-2au)t^2+2but+u^2
        sum all vector directions
        4at^3+6abt^2+(2b^2-4au)t+2bu = 0
        */
        var a = this.pointA + this.pointB - 2* this.curvePoint;
        var b = 2* this.curvePoint - 2 * this.pointA;
        var c = this.pointA - point;
        var p = math.csum(4 * a*a);
        var q = math.csum(6 * a * b);
        var r = math.csum(2 * b * b - 4 * a * c);
        var s = math.csum(2 * b * c);
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
        // The distance equation t be minimized is:
        // D(t) = sum_i (f_i(t))^2
        // f_i(t)=A_i(1-t)^2+2B_it(1-t)+C_it^2-X_i
        // f_i(t)=A_i(1-2t+t^2)+B_i(2t-2t^2)+C_it^2
        var a = this.pointA + this.pointB - 2 * this.curvePoint;
        var b = 2 * (this.curvePoint - this.pointA);
        var c = this.pointA - point;
        // Then expand it to a quartic equation
        // f_i(t) = (ax^2+bx+c)^2
        // f_i(t) = a^2x^4+2abx^3+(b^2+2ac)x+2bcx+c^2
        // For D(t) = pt^4+qt^3+rt^2+st+ the derivative is
        // D'(t)=4pt^4+3qt^3+2*rt+s=0
        var p = 4 * math.csum(a * a);
        var q = 3 * math.csum(2 * a * b);
        var r = 2 * math.csum(b * b + 2 * a * c);
        var s = math.csum(2 * b * c);
        //For the second derivative
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