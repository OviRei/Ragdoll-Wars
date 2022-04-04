using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    private PhotonView PV;
    private GameObject controller;
    private PlayerBones[] playerBones;

    private void Awake() 
    {
        PV = GetComponent<PhotonView>();  
    }

    private void Start()
    {
        if(PV.IsMine) CreateController();
    }

    private void CreateController()
    {
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();
        
        //Spawns in player
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", $"PlayerController"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { PV.ViewID });
        controller.name = $"PlayerController({PlayerPrefs.GetString("Username")})";

        playerBones = controller.GetComponentsInChildren<PlayerBones>();
        foreach(PlayerBones bone in playerBones)
        {
            if(bone.isJoint) continue;
            bone.SetPlayerColor(PlayerPrefs.GetString($"Loadout{PlayerPrefs.GetInt($"SelectedLoadout")}Ability"));
        }
    }

    public IEnumerator Die()
    {
        controller.GetComponent<PlayerController>().isDead = true;
        playerBones = controller.GetComponentsInChildren<PlayerBones>();
        foreach(PlayerBones bone in playerBones)
        {
            bone.BoneDiscombuggle();
        }

        yield return new WaitForSeconds(2);

        PhotonNetwork.Destroy(controller); //Kill player
        CreateController(); //Respawn player
        controller.GetComponent<PlayerController>().isDead = false;
    }
}
