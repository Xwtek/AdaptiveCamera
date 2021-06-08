using Unity.Collections;
using AdaptiveCamera.Data;
using System.Collections.Generic;
public interface IConstraint
{
    List<ConstraintData> Constraints {get;}
    void UpdateConstraints();
}