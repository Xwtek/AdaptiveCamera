using UnityEngine;
using RPGCharacterAnimsFREE;
[RequireComponent(typeof(EnemyEvent))]
[RequireComponent(typeof(EnemyController))]
public class PlayerKiller : MonoBehaviour {
    private void OnCollisionEnter(Collision other)
    {
        if(controller != null && controller.dying)return;
        if(other.collider.tag != "Player") return;
        GameEvents.OnDie.Invoke();
    }
    EnemyController controller;
    private void Start() {
        controller = GetComponent<EnemyController>();
    }
}