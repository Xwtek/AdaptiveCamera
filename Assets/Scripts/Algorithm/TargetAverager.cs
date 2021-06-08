using Unity.Mathematics;
using System.Collections;
using System;
namespace AdaptiveCamera.Algorithm{
    [System.Serializable]
    public class TargetAverager
    {
        [NonSerialized]
        private float3[] pastTargets;
        [NonSerialized]
        private int index=0;
        public int length=5;
        [NonSerialized]
        private int occupiedLength;
        public float3? currentAverage;
        public float3 Update(float3 point)
        {
            if (pastTargets == null) pastTargets = new float3[length];
            else if (length != pastTargets.Length)
            {
                var old = pastTargets;
                pastTargets = new float3[length];
                Array.Copy(old, index, pastTargets, 0, math.min(old.Length - index, length));
                if (old.Length - index < length) Array.Copy(old, 0, pastTargets, old.Length - index, math.min(index, length
                   - old.Length + index));
                occupiedLength = math.min(length, old.Length);
                index = 0;
            }
            pastTargets[index] = point;
            index++;
            if(index >= length) index = 0;
            if (occupiedLength < length) occupiedLength++;
            var sum = pastTargets[0];
            for (var i = 1; i < occupiedLength; i++)
            {
                sum += pastTargets[i];
            }
            currentAverage = sum / occupiedLength;
            return currentAverage.Value;
        }
        public void Clear(){
            index = 0;
            occupiedLength = 0;
            currentAverage = null;
        }
    }
}