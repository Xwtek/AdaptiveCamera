using Unity.Collections;
using System.Collections.Generic;
using AdaptiveCamera.Data;
public class ConstraintCollection
{
    List<IConstraint> constraints = new List<IConstraint>();
    NativeArray<ConstraintData>? constraintDatas;
    public NativeSlice<ConstraintData>? GetConstraintArray()
    {
        var count = 0;
        for (var i = 0; i < constraints.Count; i++)
        {
            constraints[i].UpdateConstraints();
            count += constraints[i].Constraints.Count;
        }
        if (count == 0) return null;
        if (count > (constraintDatas?.Length ?? 0))
        {
            constraintDatas?.Dispose();
            constraintDatas = new NativeArray<ConstraintData>(count, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        }
        var start = 0;
        var cnst = constraintDatas.Value;
        for (var i = 0; i < constraints.Count; i++)
        {
            for (var j = 0; j < constraints[i].Constraints.Count;j++){
                cnst[start+j] = constraints[i].Constraints[j];
            }
            start += constraints[i].Constraints.Count;
        }
        constraintDatas = cnst;
        return constraintDatas?.Slice(0,count);
    }
    public NativeArray<ConstraintData> GetTempConstraintArray()
    {
        var count = 0;
        for (var i = 0; i < constraints.Count; i++)
        {
            constraints[i].UpdateConstraints();
            count += constraints[i].Constraints.Count;
        }
        if (count == 0) return new NativeArray<ConstraintData>(0, Allocator.TempJob);
        var result = new NativeArray<ConstraintData>(count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        var start = 0;
        for (var i = 0; i < constraints.Count; i++)
        {
            for (var j = 0; j < constraints[i].Constraints.Count;j++){
                result[start+j] = constraints[i].Constraints[j];
            }
            start += constraints[i].Constraints.Count;
        }
        return result;
    }
    public void Register(IConstraint constraint)
    {
        if(constraints.Contains(constraint)) return;
        constraints.Add(constraint);
    }
    public void Deregister(IConstraint constraint)
    {
        if(constraints.Contains(constraint)) return;
        constraints.Remove(constraint);
    }
    public void Dispose(){
        constraintDatas?.Dispose();
    }
}