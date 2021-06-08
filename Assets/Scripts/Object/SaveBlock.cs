using UnityEngine;
using Unity.Mathematics;
[RequireComponent(typeof(Collider))]
public class SaveBlock : MonoBehaviour {
    
    Transform player;
    public float2 size;
    private void Start() {
        player = NoahController.Instance.transform;
    }
    private void Update(){
        float3 pos = transform.rotation * (player.position - transform.position);
        var cond = (math.abs(pos.xz) - size)<0;
        if(!saved && cond.x && cond.y && pos.y > 0){
            SaveState.Save();
            saved = true;
        }
    }
    public bool saved;
}