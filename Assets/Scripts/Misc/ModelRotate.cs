using UnityEngine;

public class ModelRotate : MonoBehaviour
{
    void FixedUpdate()
    {
        transform.RotateAround(transform.position, transform.up, Time.deltaTime * 90f);
    }
}
