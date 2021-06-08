using Unity.Mathematics;
using UnityEngine;
namespace AdaptiveCamera.Algorithm
{
    [System.Serializable]
    public class PID
    {
        public float pFactor = 1, iFactor = 0, dFactor = 0;
        public float maxIntegral;

        private float3 _integral;
        private float3 _lastError;
        public bool debug;
        public PID() : this(1,0,0){}
        public PID(float pFactor, float iFactor, float dFactor)
        {
            this.pFactor = pFactor;
            this.iFactor = iFactor;
            this.dFactor = dFactor;
        }
        float derivesgn = 0;
        float oscTime = 0;
        int currIndex = 0;
        float[] oscTimes = new float[20];
        public float3 Update(float3 target, float3 current, float deltatime)
        {
            var error = target - current;
            _integral += error * deltatime;
            var derivative = (error - _lastError) / deltatime;
            _lastError = error;
            var output = error * pFactor + _integral * iFactor + derivative * dFactor;
            if (debug)
            {
                oscTime += deltatime;
                if(math.sign(derivative.x) != 0 && math.sign(derivative.x) != derivesgn){
                    oscTimes[currIndex] = oscTime;
                    currIndex++;
                    if(currIndex == oscTimes.Length) currIndex = 0;
                    oscTime = 0;
                    var ave = 0f;
                    var sqave = 0f;
                    for (var i = 0; i < oscTimes.Length; i++){
                        ave += oscTimes[i];
                        sqave += oscTimes[i] * oscTimes[i];
                    }
                    ave /= oscTimes.Length-1;
                    sqave /= oscTimes.Length-1;
                    var stdev = sqave - ave * ave;
                    Debug.Log(new float2(ave, stdev));
                }
                derivesgn = math.sign(derivative.x);
            }
            if(maxIntegral >0) LimitIntegral(maxIntegral);
            return output;
        }
        public void LimitIntegral(float value)
        {
            _integral = math.sign(_integral) * math.min(math.abs(_integral), math.abs(value));
        }
    }
}