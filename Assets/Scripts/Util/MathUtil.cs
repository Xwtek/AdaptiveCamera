using Unity.Mathematics;
using System.Numerics;
using UnityEngine;
namespace AdaptiveCamera.Util{
    public static class MathUtil{
        public static float3 ToEuler(quaternion rotation){
            var xvalue = 2*(rotation.value.w * rotation.value.x-rotation.value.z*rotation.value.y);
            var ynumerator = 2*math.dot(rotation.value.wz, rotation.value.yx);
            var ydenumerator = 1-2*math.dot(rotation.value.xy, rotation.value.xy);
            var znumerator = 2*math.dot(rotation.value.wx, rotation.value.zy);
            var zdenumerator = 1-2*math.dot(rotation.value.zx, rotation.value.zx);
            return new float3(math.asin(xvalue), math.atan2(ynumerator, ydenumerator), math.atan2(znumerator,zdenumerator));
        }
        public static float2 PointClosestToALine(float2 a, float2 b, float2 p, bool clamp = false){
            var ab = b - a;
            var ap = p - a;
            var magnitude = math.lengthsq(ab);
            var abap = math.dot(ab, ap);
            var distance = abap / magnitude;
            if(clamp) distance = math.clamp(distance, 0, 1);
            return a + distance * ab;
        }
        public static float3 PointClosestToALine(float3 a, float3 b, float3 p, bool clamp = false){
            var ab = b - a;
            var ap = p - a;
            var magnitude = math.lengthsq(ab);
            var abap = math.dot(ab, ap);
            var distance = abap / magnitude;
            if(clamp) distance = math.clamp(distance, 0, 1);
            return a + distance * ab;
        }
        public static float2 GetCameraAngle(float3 direction){
            var y = direction.xy;
            var x = direction.zx;
            x.y = math.length(x);
            return math.atan2(y, x);
        }
        public static float3 FromCameraAngle(float2 angle){
            return new float3(math.sin(angle.x) * math.cos(angle.y), math.sin(angle.y), math.cos(angle.x) * math.cos(angle.y));
        }
        public static (complex, complex, complex) SolveCubic(float a, float b, float c, float d){
            if(Mathf.Approximately(a,0)){
                //Quadratic
                if(Mathf.Approximately(b,0)){
                    //Linear
                    return (new complex(-d / c), new complex(math.NAN), new complex(math.NAN));
                }
                var Dq = c * c - 2 * b * d;
                complex vDq;
                if(Dq< 0) vDq = new complex(0, math.sqrt(-Dq));
                else vDq = new complex(math.sqrt(Dq));
                return ((new complex(c) + vDq) / (2 * a), (new complex(c) - vDq) / (2 * a), new complex(math.NAN));
            }
            var d0 = b * b - 3 * a * c;
            var d1 = 2 * b * b * b - 9 * a * b * c + 27 * a * a * d;
            if(Mathf.Approximately(d0,0) && Mathf.Approximately(d1,0)){
                var result = new complex(b / (3 * a));
                return (result, result, result);
            }
            var D = d1 * d1 - 4 * d0 * d0 * d0;
            complex vD;
            if(D==0) vD = new complex(0f);
            else if(D>0) vD = new complex(math.sqrt(D));
            else vD = new complex(0, math.sqrt(-D));
            complex C;
            if(d1>0) C = ((new complex(d1) + vD) / 2).power(1f/3f);
            else C = ((new complex(d1) - vD) / 2).power(1f/3f);
            var d0C = d0/C;
            var s1 = -(new complex(b) + C + d0C) / (3 * a);
            var s2 = -(new complex(b) + complex.unity(3,1) * C + complex.unity(3,2)*d0C) / (3 * a);
            var s3 = -(new complex(b) + complex.unity(3,2) * C + complex.unity(3,1)*d0C) / (3 * a);
            return (s1, s2, s3);
        }
    }
}