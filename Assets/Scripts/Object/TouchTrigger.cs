using UnityEngine;
using UnityEngine.Events;

public class TouchTrigger : MonoBehaviour {
    public UnityEvent OnTriggerIn = new UnityEvent();
    public UnityEvent OnTriggerOut = new UnityEvent();
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            OnTriggerIn.Invoke();
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.tag == "Player"){
            OnTriggerOut.Invoke();
        }
    }
}