using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraPosition;

    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
