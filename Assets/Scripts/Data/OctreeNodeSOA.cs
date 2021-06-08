using Unity.Mathematics;
using Unity.Collections;
namespace AdaptiveCamera.Data
{
    public struct OctreeNodeSOA
    {
        public NativeArray<float3> current;
        public NativeArray<float3> shifted;
        public NativeArray<float> score;
        /// <summary>
        /// The number of the pass the Octree is at
        /// </summary>
        public NativeArray<int> pass;
        /// <summary>
        /// The distance between the center and the edge of the cube
        /// </summary>
        public NativeArray<float> size;
        public OctreeNodeSOA(int length, Allocator allocator, NativeArrayOptions options)
        {
            this.current = new NativeArray<float3>(length, allocator, options);
            this.shifted = new NativeArray<float3>(length, allocator, options);
            this.score = new NativeArray<float>(length, allocator, options);
            this.pass = new NativeArray<int>(length, allocator, options);
            this.size = new NativeArray<float>(length, allocator, options);
        }
        public int Length => current.Length;
        public OctreeNode this[int index]
        {
            get => new OctreeNode
            {
                current = this.current[index],
                shifted = this.shifted[index],
                score = this.score[index],
                pass = this.pass[index],
                size = this.size[index]
            };
            set
            {
                current[index] = value.current;
                shifted[index] = value.shifted;
                score[index] = value.score;
                size[index] = value.size;
            }
        }
        public void Swap(int i1, int i2)
        {
            (current[i1], current[i2]) = (current[i2], current[i1]);
            (shifted[i1], shifted[i2]) = (shifted[i2], shifted[i1]);
            (score[i1], score[i2]) = (score[i2], score[i1]);
            (pass[i1], pass[i2]) = (pass[i2], pass[i1]);
        }
        public void Dispose(){
            current.Dispose();
            shifted.Dispose();
            score.Dispose();
            pass.Dispose();
            size.Dispose();
        }
    }
}