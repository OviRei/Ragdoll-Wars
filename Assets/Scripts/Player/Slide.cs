using UnityEngine;
using Photon.Pun;

public class Slide : MonoBehaviour
{
    [Header("References")]
    private Rigidbody rb;
    private CapsuleCollider col;
    private PhotonView PV;
    [SerializeField] private Transform playerModel;
    private PlayerController playerController;
    private WallRun wallRun;

    [Header("Sliding")]
    [SerializeField] private float maxSlideTime;
    [SerializeField] private float slideForce;
    [SerializeField] private float slideTimer;
    [SerializeField] private float colliderSlideYScale;
    private float colliderStartYScale;
    [SerializeField] private float playerModelSlideYScale;
    private float playerModelStartYScale;

    [Header("Input")]
    private KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalMovement;
    private float verticalMovement;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        playerController = GetComponent<PlayerController>();
        wallRun = GetComponent<WallRun>();
        PV = GetComponent<PhotonView>();

        colliderStartYScale = col.height;
        playerModelStartYScale = playerModel.localScale.y;
    }

    private void Update()
    {
        if(!PV.IsMine) return;

        horizontalMovement = Input.GetAxis("Horizontal");
        verticalMovement = Input.GetAxis("Vertical");

        if(playerController.idontknowwhattocallthis)
        {
            if(Input.GetKey(slideKey) && (horizontalMovement != 0 || verticalMovement != 0) && playerController.isGrounded && !wallRun.isWallRunning) StartSlide();
        }
        else
        {
            if(Input.GetKeyDown(slideKey) && (horizontalMovement != 0 || verticalMovement != 0) && playerController.isGrounded && !wallRun.isWallRunning) StartSlide();
        }
        if(Input.GetKeyUp(slideKey) && playerController.isSliding) StopSlide();
    }

    private void FixedUpdate()
    {
        if(!PV.IsMine) return;

        if(playerController.isSliding) SlideMovement();
    }

    private void StartSlide()
    {
        playerController.isSliding = true;
        playerController.idontknowwhattocallthis = false;

        col.height = colliderSlideYScale;
        playerModel.localScale = new Vector3(playerModel.localScale.x, playerModelSlideYScale, playerModel.localScale.z); //Makes player smol
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); //Stops player from floating in the sky

        slideTimer = maxSlideTime;
    }

    private void SlideMovement()
    {
        Vector3 moveDirection = playerModel.forward * verticalMovement + playerModel.right * horizontalMovement;

        if(!playerController.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(moveDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;            
        }
        else
        {
            rb.AddForce(playerController.GetSlopeMoveDirection(moveDirection) * slideForce, ForceMode.Force);
        }


        if(slideTimer <= 0) StopSlide();
    }

    private void StopSlide()
    {
        playerController.isSliding = false;

        col.height = colliderStartYScale;
        playerModel.localScale = new Vector3(playerModel.localScale.x, playerModelStartYScale, playerModel.localScale.z); //Makes player normal again
    }
}
