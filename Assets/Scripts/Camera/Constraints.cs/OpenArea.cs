using System.Collections.Generic;
using AdaptiveCamera.Data;
using UnityEngine;
using AdaptiveCamera.Util;
using Unity.Mathematics;

public class OpenArea : MonoBehaviour, IConstraint
{
    public List<ConstraintData> Constraints { get; } = new List<ConstraintData>();
    public bool active;
    public float distance;
    public float distanceCost;
    public float cameraAngleCost;
    public float idealVantage;
    public float vantageCost;
    private Transform player;
    private float2? move;
    private void Start()
    {
        player = NoahController.Instance.reference;
        AdaptiveCameraBrain.Instance.constraints.Register(this);
    }
    public void UpdateConstraints()
    {
        if (math.lengthsq(NoahController.Instance.Move) > 0.01) move = NoahController.Instance.Move;
        Constraints.Clear();
        if (active && CameraControl.Instance.state == CameraState.Default)
        {
            Constraints.Add(player.MakeDistanceConstraint(distance, distanceCost, 1));
            Constraints.Add(player.MakeVantageConstraint(math.radians(idealVantage), vantageCost));
            if (move.HasValue)
            {
                Constraints.Add(
                    ConstraintUtil.MakeCameraAngleConstraint(
                        vantageCost, move.Value,
                        math.min(math.radians(30),
                            NoahController.Instance.MaxRotationPerFrame),
                        ((float3)AdaptiveCameraBrain.Instance.transform.forward).xz));
            }
        }
    }
    public void Activate() => active = true;
    public void Deactivate() => active = false;
    private void OnDestroy() {
        if(AdaptiveCameraBrain.Instance == null) return;
        AdaptiveCameraBrain.Instance.constraints.Deregister(this);
    }
}