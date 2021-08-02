using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using AdaptiveCamera.Util;
using AdaptiveCamera.Data;
[System.Serializable]
public class Focusable : MonoBehaviour
{
    public Collider mainCollider;
    public Renderer m_Renderer;
    public bool CanBeFocused{ get; private set; }
    private bool potentialFocusable;
    //public float minDistance;
    // Start is called before the first frame update
    void Start()
    {
        AdaptiveCameraBrain.Instance.focusables.Register(this);
    }
    void Update()
    {
        var distance = transform.position - NoahController.Instance.transform.position;
        distance.y = 0;
        potentialFocusable = m_Renderer == null || m_Renderer.isVisible;
    }
    private void FixedUpdate() {
        if(!potentialFocusable) CanBeFocused = false;
        else{
            var playerLoc = AdaptiveCameraBrain.Instance.transform.position;
            var dir = transform.position - playerLoc;
            var layer = LayerMask.GetMask("Ignore Raycast", "Player");
            if(Physics.Raycast(playerLoc, dir.normalized, out var hitInfo, dir.magnitude, ~layer)){
                CanBeFocused = hitInfo.collider == mainCollider;
            }else{
                CanBeFocused = true;
            }
        }
    }
    private void OnDisable() {
        potentialFocusable = false;
        CanBeFocused = false;
    }
}
