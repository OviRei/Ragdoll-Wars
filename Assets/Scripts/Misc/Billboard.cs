using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera cam;

    private void Update() 
    {
        if(cam == null) cam = FindObjectOfType<Camera>();
        if(cam == null) return;
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }
}
