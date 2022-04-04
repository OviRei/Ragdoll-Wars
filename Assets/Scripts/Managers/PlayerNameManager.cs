using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInputField;

    private void Start()
    {
        if(PlayerPrefs.HasKey("Username")) usernameInputField.text = PlayerPrefs.GetString("Username");
        else 
        {
            usernameInputField.text = "Player " + Random.Range(0, 1000).ToString("0000");
            OnUsernameInputValueChanged();
        }

    }

    public void OnUsernameInputValueChanged()
    {
        PhotonNetwork.NickName = usernameInputField.text;
        PlayerPrefs.SetString("Username", usernameInputField.text);
    }
}
