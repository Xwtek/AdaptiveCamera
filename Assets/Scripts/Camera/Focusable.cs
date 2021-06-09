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
    internal float sortOrder;
    public bool CanBeFocused{ get; private set; }
    private bool potentialFocusable;
    public float minDistance;
    public bool needToBeVisible = true;
    public bool focused = false;
    // Start is called before the first frame update
    void Start()
    {
        AdaptiveCameraBrain.Instance.focusables.Register(this);
    }
    // Update is called once per frame
    void Update()
    {
        var distance = transform.position - NoahController.Instance.transform.position;
        distance.y = 0;
        if(distance.sqrMagnitude < minDistance * minDistance) potentialFocusable = false;
        else if(!this.needToBeVisible) potentialFocusable = true;
        else if(m_Renderer != null && !m_Renderer.isVisible) potentialFocusable = false;
        else potentialFocusable = true;
    }
    private void FixedUpdate() {
        if(!potentialFocusable) CanBeFocused = false;
        else{
            var playerLoc = AdaptiveCameraBrain.Instance.transform.position;
            var dir = transform.position - playerLoc;
            var layer = LayerMask.GetMask("Ignore Raycast", "Player");
            if(Physics.Raycast(playerLoc, dir.normalized, out var hitInfo, Mathf.Infinity, ~layer)){
                CanBeFocused = hitInfo.collider == mainCollider;
                if(!CanBeFocused && transform.parent.name!="Terrain")Debug.Log("Orig: " + transform.parent.name + ", Pos: " + gameObject.transform.position + ", Hits: " + hitInfo.collider.name + ", Pos: " + hitInfo.point + ", Player:" + playerLoc);
            }else{
                CanBeFocused = false;
            }
        }
    }
    private void OnDisable() {
        potentialFocusable = false;
        CanBeFocused = false;
    }
}
