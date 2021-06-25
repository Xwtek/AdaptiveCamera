using Unity.Mathematics;
using AdaptiveCamera.Data;
using System;
using Unity.Collections;
namespace AdaptiveCamera.Algorithm
{
    public struct PriorityQueue
    {
        // Priority queue of Octrees currently being searched
        internal NativeArray<OctreeNode> backingArray;
        // The number of octrees being stored in priority Queue 
        internal NativeArray<int> currentCapacity;
        // The number of octrees being given score.

        // How many octrees in the queue
        public int Count
        {
            get => currentCapacity[0];
            private set => currentCapacity[0] = value;
        }
        public int MaxCapacity => backingArray.Length;
        // The octree with lowest score
        public OctreeNode? Best => Count > 0 ? backingArray[0] as OctreeNode? : null as OctreeNode?;

        public void AddMany(NativeSlice<OctreeNode> newCubes)
        {
            if (newCubes.Length == 0) return;
            int leftParent = math.max(0, GetParentIndex(this.Count));
            int rightParent = GetParentIndex(this.Count + newCubes.Length - 1);
            while (rightParent >= 0)
            {
                for (var i = rightParent; i >= leftParent; i--)
                {
                    SiftDown(i, newCubes);
                }
                leftParent = math.max(0, GetParentIndex(leftParent));
                rightParent = GetParentIndex(rightParent);
            }
            if (Count != backingArray.Length)
            {
                var count = math.min(backingArray.Length - Count, newCubes.Length);
                for (var i = 0; i < count; i++)
                {
                    backingArray[i + Count] = newCubes[i];
                }
            }
            Count = math.min(backingArray.Length, Count + newCubes.Length);
        }
        public OctreeNode? Pop()
        {
            if (Count == 0) return null;
            var result = backingArray[0];
            backingArray[0] = backingArray[Count - 1];
            Count--;
            SiftDown();
            return result;
        }

        public PriorityQueue(int maxCapacity)
        {
            backingArray = new NativeArray<OctreeNode>(maxCapacity, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            currentCapacity = new NativeArray<int>(1, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            Count = 0;
        }
        public void Dispose()
        {
            backingArray.Dispose();
            currentCapacity.Dispose();
        }
        public void Initialize(OctreeNode original)
        {
            Count = 1;
            backingArray[0] = original;
        }

        internal void SiftDown(int position = 0, NativeSlice<OctreeNode>? extension = null)
        {
            var length = Count + (extension?.Length ?? 0);
            while (true)
            {
                var child = GetLeftChild(position);
                if (child >= length) break;
                if (child + 1 < length && GetScore(extension, child + 1) < GetScore(extension, child)) child += 1;
                if (GetScore(extension, child) >= GetScore(extension, position)) break;
                Swap(extension, child, position);
                position = child;
            }
        }

        internal void Swap(NativeSlice<OctreeNode>? extension, int firstIndex, int secondIndex)
        {
            if (firstIndex > secondIndex)
            {
                (firstIndex, secondIndex) = (secondIndex, firstIndex);
            }
            if (secondIndex < Count || !extension.HasValue)
            {
                var temp = backingArray[secondIndex];
                backingArray[secondIndex] = backingArray[firstIndex];
                backingArray[firstIndex] = temp;
            }
            else
            {
                var ex = extension.Value;
                secondIndex -= Count;
                var temp = ex[secondIndex];
                if (firstIndex < Count)
                {
                    ex[secondIndex] = backingArray[firstIndex];
                    backingArray[firstIndex] = temp;
                }
                else
                {
                    firstIndex -= Count;
                    ex[secondIndex] = ex[firstIndex];
                    ex[firstIndex] = temp;
                }
            }
        }
        internal float GetScore(NativeSlice<OctreeNode>? extension, int index)
        {
            if (index < Count)
            {
                return backingArray[index].score;
            }
            else
            {
                index -= Count;
                return extension?[index].score ?? 0;
            }
        }
        internal static int GetParentIndex(int current) => (current + 1) / 2 - 1;
        internal static int GetLeftChild(int current) => current * 2 + 1;
        #region Testing
        public bool IsAHeap()
        {
            for (int i = 0; true; i++)
            {
                var child = i * 2 + 1;
                if (child >= Count) break;
                if (backingArray[child].score < backingArray[i].score) return false;
                if (child + 1 >= Count) continue;
                if (backingArray[child + 1].score < backingArray[i].score) return false;
            }
            return true;
        }
        public bool IsAHeap(NativeArray<OctreeNode> extension, int length)
        {
            for (int i = 0; true; i++)
            {
                var child = i * 2 + 1;
                if (child >= Count + length) break;
                if (GetScore(extension, i) > GetScore(extension, child)) return false;
                if (child + 1 >= Count + length) continue;
                if (GetScore(extension, i) > GetScore(extension, child + 1)) return false;
            }
            return true;
        }
        internal float[] Scores()
        {
            var list = new float[Count];
            for (int i = 0; i < Count; i++)
            {
                list[i] = backingArray[i].score;
            }
            return list;
        }
        #endregion
    }
}