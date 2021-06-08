using Unity.Mathematics;
namespace AdaptiveCamera.Data{
    public struct OctreeNode{
        public float3 current;
        public float3 shifted;
        public float score;
        /// <summary>
        /// The number of the pass the Octree is at
        /// </summary>
        public int pass;
        /// <summary>
        /// The distance between the center and the edge of the cube
        /// </summary>
        public float size;
    }
}