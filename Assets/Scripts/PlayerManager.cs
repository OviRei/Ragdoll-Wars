using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    private void Awake() 
    {
        PV = GetComponent<PhotonView>();    
    }

    private void Start()
    {
        if(PV.IsMine) CreateController();
    }

    void CreateController()
    {
        //Spawns in player
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity);
    }
}
