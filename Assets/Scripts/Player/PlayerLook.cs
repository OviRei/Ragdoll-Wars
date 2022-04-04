using UnityEngine;
using Photon.Pun;

public class PlayerLook : MonoBehaviour
{
    [Header("Sensitivity")]
    public float sensX = 230f;
    public float sensY = 230f;

    [Header("Camera")]
    [SerializeField] private Transform cam;
    [SerializeField] private Transform playerModel;
    private float xRotation;
    private float yRotation;

    [Header("Misc")]
    private float mouseX;
    private float mouseY;
    private float multiplier = 0.01f;
    private PhotonView PV;

    private WallRun wallRun;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        wallRun = GetComponent<WallRun>();
        if(!PV.IsMine) return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if(!PV.IsMine) return;

        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, wallRun.tilt);
        playerModel.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
