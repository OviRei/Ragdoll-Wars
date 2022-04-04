using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerBones: MonoBehaviour, IDamageable
{
    public bool isJoint = false;

    private PhotonView PV;
    private PlayerManager playerManager;
    private PlayerController playerController;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        playerController = GetComponentInParent<PlayerController>();
    }

    public void SetPlayerColor(string ability)
    {
        string str = ability.Replace(" ", "");
        PV.RPC("RPC_SetPlayerColor", RpcTarget.AllBuffered, str);
    }

	[PunRPC]
	private void RPC_SetPlayerColor(string ability)
	{
        GetComponent<Renderer>().material = Resources.Load($"Materials/{ability}Mat", typeof(Material)) as Material;
	}

    public void TakeDamage(float damage)
	{
		PV.RPC("RPC_TakeDamage", RpcTarget.AllBuffered, damage);
	}

	[PunRPC]
	private void RPC_TakeDamage(float damage)
	{
		if(!PV.IsMine) return;

		playerController.currentHealth -= damage;

        playerController.healthBar.fillAmount = playerController.currentHealth / playerController.maxHealth;

		if(playerController.currentHealth <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		StartCoroutine(playerManager.Die());
	}

    public void BoneDiscombuggle()
    {
        gameObject.AddComponent<Rigidbody>();
    }
}