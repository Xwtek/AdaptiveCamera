using UnityEngine;
using System.Collections;

public class csDestroyEffect : MonoBehaviour {
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {
            Destroy(gameObject);
        }
    }
}
