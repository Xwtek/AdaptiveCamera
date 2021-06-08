using NUnit.Framework;
using AdaptiveCamera.Algorithm;
using AdaptiveCamera.Data;
using System;
using Unity.Collections;
public class PriorityQueueTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void InsertionSuccessful()
    {
        var priorityQueue = new PriorityQueue(10);
        priorityQueue.Initialize(new OctreeNode(){score = 20});
        Assert.AreEqual(priorityQueue.Best.Value.score, 20, 0.01, "Error on Initialization");
        Assert.AreEqual(priorityQueue.Count, 1, "Error on Initialization");
        DoSingleRound(priorityQueue, 20, 3, 15, new OctreeNode[]{
            new OctreeNode(){score =15},
            new OctreeNode(){score =26},
            new OctreeNode(){score =29}
        });
        DoSingleRound(priorityQueue, 15, 5, 18, new OctreeNode[]{
            new OctreeNode(){score =18},
            new OctreeNode(){score =21},
            new OctreeNode(){score =23},
        });
        DoSingleRound(priorityQueue, 18, 4, 21, new OctreeNode[0]);
        DoSingleRound(priorityQueue, 21, 3, 23, new OctreeNode[0]);
        DoSingleRound(priorityQueue, 23, 2, 26, new OctreeNode[0]);
        DoSingleRound(priorityQueue, 26, 1, 29, new OctreeNode[0]);
        DoSingleRound(priorityQueue, 29, 0,  0, new OctreeNode[0]);
        //Use the Assert class to test conditions
        priorityQueue.Dispose();
    }
    private void DoSingleRound(PriorityQueue priorityQueue, int originalScore, int expectedCount,  float expectedScore, OctreeNode[] newCubes){
        Assert.AreEqual(priorityQueue.Pop().Value.score, originalScore);
        var temp = new NativeArray<OctreeNode>(newCubes, Allocator.TempJob);
        priorityQueue.AddMany(temp);
        try
        {
            Assert.True(priorityQueue.IsAHeap(),
                "RemoveAndAddMany leaves the priorityQueue violating the heap property.\n" +
                "Current Octree : " + String.Join(", ", priorityQueue.Scores()) + "\n" +
                "Parameter : " + expectedCount + ", " + expectedScore
            );
            Assert.AreEqual(priorityQueue.Count, expectedCount,
                "RemoveAndAddMany leaves the queue with a wrong length.\n" +
                "Current Octree : " + String.Join(", ", priorityQueue.Scores()) + "\n" +
                "Parameter : " + expectedCount + ", " + expectedScore
            );
            if (expectedCount != 0) Assert.AreEqual(priorityQueue.Best.Value.score, expectedScore, 0.01,
                 "RemoveAndAddMany sets the best octree completely wrong.\n" +
                 "Current Octree : " + String.Join(", ", priorityQueue.Scores()) + "\n" +
                 "Parameter : " + expectedCount + ", " + expectedScore
             );
        }finally
        {
            temp.Dispose();
        }
    }
}
