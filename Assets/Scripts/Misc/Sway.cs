using UnityEngine;

public class Sway : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private bool UI;
    [SerializeField] private float smooth;
    [SerializeField] private float xSwayMultiplier;
    [SerializeField] private float ySwayMultiplier;

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * xSwayMultiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * ySwayMultiplier;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        if(UI)
        {
            RectTransform rectTransform = this.GetComponent<RectTransform>();
            rectTransform.rotation = Quaternion.Lerp(rectTransform.rotation, targetRotation, smooth * Time.deltaTime);        
        }
        else transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
}
