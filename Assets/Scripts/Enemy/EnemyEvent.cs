using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class EnemyEvent : MonoBehaviour
{
    public MoveEvent MoveEvent;
    public UnityEvent OnDie;
    public UnityEvent OnRevive;
    private void Awake() {
        if(MoveEvent == null) MoveEvent = new MoveEvent();
        if(OnDie == null) OnDie = new UnityEvent();
        if(OnRevive == null) OnRevive = new UnityEvent();
    }
}

[System.Serializable] public class MoveEvent : UnityEvent<Vector3>{}