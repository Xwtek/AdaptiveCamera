using Unity.Collections;
using System.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using System.Diagnostics;
using AdaptiveCamera.Data;
using AdaptiveCamera.Algorithm;
using UnityEngine;
namespace AdaptiveCamera.Algorithm
{


    [System.Serializable]
    public class AdaptiveCameraCoroutine
    {

        PriorityQueue priorityQueue;
        NativeArray<OctreeNode> candidates;

        public float scale;
        public int maxPasses;
        public long worktimePerFrame;
        public long maxWork;
        public int octreeCubesToBeExamined;
        public int candidatesPerRound;
        private bool cancel = false;
        [HideInInspector]
        public bool finished = true;
        public float size;
        public float accumulatedTime;
        public OctreeNode? Best { get; private set; } = null;
        public float3? PreviousBest;
        public float frameCoherenceCost;
        [System.Serializable]
        public struct Debug{
            public bool enableDebug;
            public float currentTotal;

            public float3 output;

            public float[] outputBreakdown;
            public ConstraintData[] constraintsUsed;
            public int passes;
            public int evaluatedOctree;
            public static implicit operator bool(Debug debug) => debug.enableDebug;
        }
        public Debug debug;
        private float3? newPlayerPos;
        private NativeArray<ConstraintData>? newConstraints;
        private JobHandle? jobHandle;
        public IEnumerator Run(float3 initialConfiguration, float3 playerPos, NativeSlice<ConstraintData> constraints, bool disposeAfter = false)
        {
            PreviousBest = Best?.current;
            this.finished = false;
            this.cancel = false;
            this.priorityQueue.Initialize(
                new OctreeNode
                {
                    shifted = initialConfiguration,
                    size = size,
                });
            var workWatch = new Stopwatch();
            workWatch.Start();
            var frameWatch = new Stopwatch();
            frameWatch.Start();
            Best = null;
            var delta = new float3(0, 0, 0);
            while (priorityQueue.Best.Value.pass < maxPasses && workWatch.ElapsedMilliseconds < maxWork){
                var splitCount = 8;
                var numCandidates = math.min(candidates.Length / splitCount, priorityQueue.Count) * splitCount;
                var candidateSlice = candidates.Slice(0, numCandidates);
                var allJob = new ConstraintJob();
                allJob.candidates = candidateSlice;
                allJob.constraints = constraints;
                allJob.frameCoherenceCost = frameCoherenceCost;
                allJob.playerPosition = playerPos;
                allJob.priorityQueue = priorityQueue;
                allJob.previousBest = initialConfiguration;
                allJob.scale = scale;
                allJob.splitCount = splitCount;
                jobHandle = allJob.Schedule();
                JobHandle.ScheduleBatchedJobs();
                while (!jobHandle.Value.IsCompleted) if (frameWatch.ElapsedMilliseconds > worktimePerFrame)
                    {
                        yield return null;
                        if (cancel) break;
                        frameWatch.Restart();
                    }
                jobHandle.Value.Complete();
                if (cancel) break;
                if(newPlayerPos.HasValue) {
                    delta = newPlayerPos.Value - playerPos;
                }
                if(newConstraints.HasValue){
                    constraints = newConstraints.Value;
                }
                if (Best == null || Best.Value.score > priorityQueue.Best.Value.score)
                Best = priorityQueue.Best;
                if (debug)
                {
                    debug.evaluatedOctree++;
                }
            }
            finished = true;
            if (debug)
            {
                debug.passes = Best.Value.pass;
                debug.constraintsUsed = constraints.ToArray();
                debug.outputBreakdown = new float[constraints.Length];
                debug.currentTotal = Best.Value.score;
                for (var i = 0; i < constraints.Length; i++)
                {
                    debug.outputBreakdown[i] = ConstraintJob.GetCost(constraints[i], Best.Value.current, playerPos);
                }
                debug.output = Best.Value.current;
            }
            newConstraints?.Dispose();
            newConstraints = null;
        }
        public AdaptiveCameraCoroutine(int octreeCubesToBeExamined, int candidatesPerRound, float scale, int maxPasses, long maxWork, long worktimePerFrame)
        {
            this.scale = scale;
            this.maxPasses = maxPasses;
            this.maxWork = maxWork;
            this.worktimePerFrame = worktimePerFrame;
            this.candidatesPerRound = candidatesPerRound;
            this.octreeCubesToBeExamined = octreeCubesToBeExamined;
        }
        public void Initialize()
        {
            this.priorityQueue = new PriorityQueue(this.octreeCubesToBeExamined);
            this.candidates = new NativeArray<OctreeNode>(this.candidatesPerRound, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        }
        public void Cancel(){
            cancel = true;
            if(jobHandle.HasValue)jobHandle.Value.Complete();
        }
        public void Update(float3 newPlayerPos, NativeArray<ConstraintData> newConstraints ) {
            this.newPlayerPos = newPlayerPos;
            if(this.newConstraints.HasValue){
                jobHandle?.Complete();
                this.newConstraints?.Dispose();
            }
            this.newConstraints = newConstraints;
        }
        public bool IsCancelled => cancel;
        public void Dispose()
        {
            Cancel();
            this.newConstraints?.Dispose();
            this.priorityQueue.Dispose();
            this.candidates.Dispose();
        }
    }
}