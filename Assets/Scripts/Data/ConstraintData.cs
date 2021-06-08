using Unity.Mathematics;
namespace AdaptiveCamera.Data{
    [System.Serializable]
    public struct ConstraintData{
        public ConstraintType type;
        public float3 position;
        public quaternion rotation;
        public float cost;
        public float idealFloat;
        public float scale;
    }
}