using UnityEngine;

public class SaveArea : MonoBehaviour {   
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player" && !saved){
            SaveState.Save();
        }
    }
    public bool saved;
}