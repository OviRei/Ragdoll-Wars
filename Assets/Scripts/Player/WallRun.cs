using UnityEngine;
using Photon.Pun;

public class WallRun : MonoBehaviour
{
	[Header("References")]
	private Rigidbody rb;
	private PhotonView PV;
	[SerializeField] private Transform playerModel;
    private PlayerController playerController;

    [Header("Wall Running")]
    public bool isWallRunning;
    [SerializeField] private float wallRunSpeed = 20f;
    [SerializeField] private float wallDistance = 1f;
    [SerializeField] private float wallRunGravity = 2f;
    [SerializeField] private float wallRunJumpForce = 5f;

    [SerializeField] private RaycastHit leftWallHit;
    [SerializeField] private RaycastHit rightWallHit;
    [SerializeField] private int lastWallID;
    [SerializeField] private int leftWallID;
    [SerializeField] private int rightWallID;

    [SerializeField] private LayerMask wallRunIgnoreLayers;

    [Header("Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private float camTilt;
    [SerializeField] private float camTiltTime;
    public float tilt { get; private set; }

    public bool wallLeft = false;
    public bool wallRight = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if(!PV.IsMine) return;
         
        CheckWall();

        if(playerController.isGrounded)
        {
            leftWallID = 1;
            rightWallID = 1;
            lastWallID = 0;
        }
        
        if(!isWallRunning && !playerController.isGrounded && !wallLeft && !wallRight)
        {
            if(leftWallID != 1) lastWallID = leftWallID;
            if(rightWallID != 1) lastWallID = rightWallID;
            leftWallID = 1;
            rightWallID = 1;
        }

        if(wallLeft) leftWallID = leftWallHit.transform.GetInstanceID();
        else if(wallRight) rightWallID = rightWallHit.transform.GetInstanceID();

        if(!playerController.isGrounded)
        {
            if(lastWallID == leftWallID || lastWallID == rightWallID) StopWallRun();
            else
            {
                if(wallLeft || wallRight) StartWallRun();
                else StopWallRun();
            }
        }
        else StopWallRun();
    }

    private void CheckWall()
    {
        wallLeft = Physics.Raycast(transform.position, -playerModel.right, out leftWallHit, wallDistance, ~wallRunIgnoreLayers);
        wallRight = Physics.Raycast(transform.position, playerModel.right, out rightWallHit, wallDistance, ~wallRunIgnoreLayers);
    }

    private void StartWallRun()
    {
        isWallRunning = true;
        rb.useGravity = false;
        playerController.canDoubleJump = true;
        playerController.canDash = true;

        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Acceleration);

        if(wallLeft) tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime);
        else if(wallRight) tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime);

        if(wallLeft)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 wallRunJumpDirection = transform.up + leftWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
            }

        }
        else if(wallRight)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 wallRunJumpDirection = transform.up + rightWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
            }            
        }
    }

    private void StopWallRun()
    {
        rb.useGravity = true;
        isWallRunning = false;

        tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);
    }
}
