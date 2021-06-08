using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraUpdate : MonoBehaviour
{
    private Camera mainCamera;
    public Transform center;
    public float rotationalSpeed;
    private float radius;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        radius = (transform.position - center.position).magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        var relativePos = transform.position - center.position;
        var angle = Mathf.Atan2(relativePos.z, relativePos.x);
        angle += rotationalSpeed * Time.deltaTime;
        var newPos = radius * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        transform.position = newPos + center.position;
        transform.forward = (center.position - transform.position).normalized;
    }
}
