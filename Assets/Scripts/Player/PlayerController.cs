using System.Net.Mime;
using System.Collections;
using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks
{
	[Header("References")]
	private Rigidbody rb;
	private PhotonView PV;
	[SerializeField] private Transform playerModel;
	[SerializeField] private Transform camHolder;
	[SerializeField] private GameObject itemHolder;
	private WallRun wallRun;
	private PlayerManager playerManager;
	[SerializeField] public Image healthBar;
	[SerializeField] private GameObject ui;

	[Header("Movement")]	
	private Vector3 moveDirection;
	private float desiredMoveSpeed;
	private float lastDesiredMoveSpeed;
	[SerializeField] private float moveSpeed = 4f;
	private float movementMultiplier = 10f;
	[SerializeField] private float airMultiplier = 0.2f;
	private float horizontalMovement;
	private float verticalMovement;
	[SerializeField] private float playerVelocity;

	[SerializeField] private float runSpeed = 14f;
	[SerializeField] private float crouchSpeed = 10f;
	[SerializeField] private float slideSpeed = 20f;
	[SerializeField] private float acceleration = 10f;

    [Header("Jumping")]
	[SerializeField] private float jumpForce = 10f;
	[SerializeField] private float doubleJumpForce = 12f;
	[SerializeField] private float dashForce = 18f;

	[Header("States")]
	public bool isDead;
  	public bool isGrounded;  
	public bool isSliding;
    public bool canDoubleJump;
	public bool canDash;
	public bool idontknowwhattocallthis;

    [Header("Drag")]
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float airDrag = 0.2f;

	[Header("Slopes")]
	private RaycastHit slopeHit;
	private Vector3 slopeMoveDirection;

	[Header("Items")]
	public Item[] items;
	private int itemIndex;
	private int previousItemIndex = -1;

	[Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

	[Header("Player")]
	[SerializeField] public float maxHealth = 200f;
	[SerializeField] public float currentHealth = 200f;

	[Header("Guns")]
	[SerializeField] private GameObject[] guns;
	[SerializeField] private GameObject AR;
	[SerializeField] private GameObject SMG;
	[SerializeField] private GameObject Shotgun;
	[SerializeField] private GameObject Sniper;
	[SerializeField] private GameObject HandCannon;


	private void Awake() 
	{
		rb = GetComponent<Rigidbody>();
		PV = GetComponent<PhotonView>();
		wallRun = GetComponent<WallRun>();
		playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
	}

	private void Start()
	{
		if(PV.IsMine)
		{
			string primaryGun = PlayerPrefs.GetString($"Loadout{PlayerPrefs.GetInt($"SelectedLoadout")}Primary").Replace(" ", "");
			string secondaryGun = PlayerPrefs.GetString($"Loadout{PlayerPrefs.GetInt($"SelectedLoadout")}Secondary").Replace(" ", "");
        	items[0] = GameObject.Find($"{gameObject.name}/CameraHolder/ItemHolder/{primaryGun}").GetComponent<Item>();
        	items[1] = GameObject.Find($"{gameObject.name}/CameraHolder/ItemHolder/{secondaryGun}").GetComponent<Item>();

			EquipItem(0);
			SetLayerRecursively(playerModel.gameObject, LayerMask.NameToLayer("Unseeable"));
			SetTagRecursively(playerModel.gameObject, "UnShootable");
		}
		else
		{
			GetComponent<CapsuleCollider>().isTrigger = true;
			Destroy(camHolder.GetComponentInChildren<Camera>().gameObject);
			Destroy(rb);
			Destroy(ui);
		}
	}

	private void Update()
	{
		if(!PV.IsMine || isDead) return;

		if(currentHealth >= maxHealth) healthBar.gameObject.SetActive(false);
        else healthBar.gameObject.SetActive(true);
		
		isGrounded = Physics.Raycast(transform.position, -transform.up, 0.3f);
		slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

		if(Input.GetKeyDown(jumpKey)) Jump();
		//if(Input.GetKeyDown(Input.GetKeyDown(KeyCode.C)) && canDash) HyperJump();
		else if(Input.GetKeyDown(dashKey) && canDash) Dash();

		MyInput();
		GunInput();
		ControlDrag();
		ControlSpeed();
		StartCoroutine(PlayerVelocity());

		for(int i = 0; i < items.Length; i++)
		{
			if(Input.GetKeyDown((i + 1).ToString())) 
			{
				EquipItem(i);
				break;
			}
		}

		if(transform.position.y < -10f) StartCoroutine(playerManager.Die());
	}

	private void FixedUpdate()
	{
		if(!PV.IsMine || isDead) return;

		MovePlayer();
	}

	private void MyInput()
	{
		horizontalMovement = Input.GetAxisRaw("Horizontal");
		verticalMovement = Input.GetAxisRaw("Vertical");

		moveDirection = playerModel.forward * verticalMovement + playerModel.right * horizontalMovement;
	}

	private void GunInput()
	{
		if(Input.GetMouseButtonDown(1)) items[itemIndex].Aim();
		if(Input.GetMouseButtonUp(1)) items[itemIndex].UnAim();

		if(Input.GetKeyDown(KeyCode.R)) items[itemIndex].ReloadGun();

		if(Input.GetMouseButton(0) && items[itemIndex].IsGunAuto()) items[itemIndex].Use();
		else if(Input.GetMouseButtonDown(0) && !items[itemIndex].IsGunAuto()) items[itemIndex].Use();
	}

	private void MovePlayer()
	{
		if(isGrounded && !OnSlope()) rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
		else if(isGrounded && OnSlope()) rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
		else if(!isGrounded) rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
	}

	private void ControlDrag()
	{
		if(isGrounded) rb.drag = groundDrag;
        else rb.drag = airDrag;
	}

	private void ControlSpeed()
	{
		if(wallRun.isWallRunning) desiredMoveSpeed = runSpeed * 1.5f; //moveSpeed = Mathf.Lerp(moveSpeed, runSpeed * 1.5f, acceleration * 2 * Time.deltaTime);
		else if(isSliding) 
		{
			if(OnSlope() && rb.velocity.y > -0.1f) desiredMoveSpeed = slideSpeed;
			else desiredMoveSpeed = runSpeed;
		}
		else moveSpeed = desiredMoveSpeed = runSpeed; //Mathf.Lerp(moveSpeed, runSpeed, acceleration * Time.deltaTime);

		if(Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 0.1f && moveSpeed != 0)
		{
			StopCoroutine(SmoothlyLerpMoveSpeed());
			StartCoroutine(SmoothlyLerpMoveSpeed());
		}
		else
		{
			moveSpeed = desiredMoveSpeed;
		}

		lastDesiredMoveSpeed = desiredMoveSpeed;
	}

	private IEnumerator SmoothlyLerpMoveSpeed()
	{
    	float time = 0;
    	float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
    	float startValue = moveSpeed;

    	while(time < difference)
    	{
    	    moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
    	   time += Time.deltaTime;
    	    yield return null;
    	}

    	moveSpeed = desiredMoveSpeed;
	}		

	private void Jump()
	{
        if(isGrounded)
        {
            canDoubleJump = true;
			canDash = true;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

			idontknowwhattocallthis = true;
        }
        else if(canDoubleJump && !(wallRun.wallLeft || wallRun.wallRight))
        {
            canDoubleJump = false;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * doubleJumpForce, ForceMode.Impulse);

			idontknowwhattocallthis = true;
        }
	}

	private void HyperJump()
	{
		rb.AddForce(transform.up * dashForce * 1.5f, ForceMode.Impulse);
		canDoubleJump = true;
		canDash = false;
	}

	private void Dash()
	{
		//rb.AddForce(moveDirection.normalized * (dashForce + 3), ForceMode.Impulse);
		canDoubleJump = true;
		canDash = false;

     	rb.velocity = new Vector3(moveDirection.normalized.x * dashForce * 3f, 0.9f, moveDirection.normalized.z * dashForce * 3f); // launch the projectile!
	}

	public bool OnSlope()
	{
		if(Physics.Raycast(transform.position, -transform.up, out slopeHit, 0.3f))
		{
			if(slopeHit.normal != Vector3.up) return true;
			else return false;
		}
		else return false;
	}

	public Vector3 GetSlopeMoveDirection(Vector3 direction)
	{
		return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
	}

	private IEnumerator PlayerVelocity()
    {
        Vector3 lastPos = gameObject.transform.position;
        yield return new WaitForFixedUpdate();
        playerVelocity = Mathf.RoundToInt(Vector3.Distance(gameObject.transform.position, lastPos) / Time.fixedDeltaTime); //KM/h
    }

	private void EquipItem(int _index)
	{
		if(_index == previousItemIndex) return;

		itemIndex = _index;

		if(items[itemIndex].gameObject == null) return; 
		
		items[itemIndex].gameObject.SetActive(true);

		if(previousItemIndex != -1)
		{
			items[previousItemIndex].gameObject.SetActive(false);
		}

		previousItemIndex = itemIndex;

		//items[itemIndex].UnAim();

		if(PV.IsMine)
		{
			Hashtable hash = new Hashtable();
			hash.Add("itemIndex", itemIndex);
			PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
		}
	}

	public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
		if(!PV.IsMine && targetPlayer == PV.Owner)
		{
			EquipItem((int)changedProps["itemIndex"]);
		}
	}

	private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if(null == obj) return;
        obj.layer = newLayer;
        foreach(Transform child in obj.transform)
        {
        	if(null == child) continue;
        	SetLayerRecursively(child.gameObject, newLayer);
        }
    }

	private void SetTagRecursively(GameObject obj, string newTag)
	{
		if(null == obj) return;
		obj.tag = newTag;
		foreach(Transform child in obj.transform)
		{
			if(null == child) continue;
			SetTagRecursively(child.gameObject, newTag);
		}
	}
}