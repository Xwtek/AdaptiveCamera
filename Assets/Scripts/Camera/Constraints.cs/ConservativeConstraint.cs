using UnityEngine;
using AdaptiveCamera.Data;
using Unity.Collections;
using System.Collections.Generic;
using AdaptiveCamera.Util;
class ConservativeConstraint : MonoBehaviour, IConstraint
{
    public float cost;
    public float distanceCost;
    public List<ConstraintData> Constraints{ get; private set; } = new List<ConstraintData>();
    public void UpdateConstraints()
    {
        var transformToBeUsed = AdaptiveCameraBrain.Instance.transform;
        Constraints.Clear();
        //constraintDatas.Add(transformToBeUsed.MakeFOVConstraint(60, cost));
        var viewpointConstraint = transformToBeUsed.MakeViewpointConstraint(0, cost);
        viewpointConstraint.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        viewpointConstraint.type = ConstraintType.CAMERA_ANGLE;
        //constraintDatas.Add(viewpointConstraint);
        Constraints.Add(transformToBeUsed.MakeDistanceConstraint(0, distanceCost, 1));
    }
    public void OnEnable()
    {
        AdaptiveCameraBrain.Instance?.constraints.Register(this);
    }
    public void OnDisable()
    {
        AdaptiveCameraBrain.Instance?.constraints.Deregister(this);
    }
    public void Start(){
        AdaptiveCameraBrain.Instance?.constraints.Register(this);
    }
    public void Update()
    {
    }
}