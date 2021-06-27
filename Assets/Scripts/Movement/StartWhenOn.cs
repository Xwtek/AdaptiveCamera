using UnityEngine;
using Unity.Mathematics;

[RequireComponent(typeof(LinearMovement))]
public class StartWhenOn : MonoBehaviour {
    LinearMovement movement;
    Transform player;
    public float2 size;
    private void Start() {
        movement = GetComponent<LinearMovement>();
        movement.enabled = false;
        player = NoahController.Instance.transform;
    }
    private void Update(){
        float3 pos = transform.rotation * (player.position - transform.position);
        var cond = (math.abs(pos.xz) - size)<0;
        movement.enabled = cond.x && cond.y && pos.y > 0;
    }
}