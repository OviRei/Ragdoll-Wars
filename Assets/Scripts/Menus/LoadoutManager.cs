using UnityEngine;
using UnityEngine.EventSystems;
 using UnityEngine.UI;
using TMPro;

public class LoadoutManager : MonoBehaviour
{
    private GameObject[] loadoutButton;
    [SerializeField] private Transform loadoutButtonContainer;
    [SerializeField] private TMP_InputField loadoutNameInputField;
    [SerializeField] private TMP_Text abilityText;
    [SerializeField] private TMP_Text ordinanceText;
    [SerializeField] private TMP_Text primaryText;
    [SerializeField] private TMP_Text secondaryText;
    [SerializeField] private GameObject abilityButton;
    [SerializeField] private GameObject ordinanceButton;
    [SerializeField] private GameObject primaryButton;
    [SerializeField] private GameObject secondaryButton;
    [SerializeField] private TMP_Text abilityDescriptionText;
    [SerializeField] private TMP_Text ordinanceDescriptionText;
    [SerializeField] private TMP_Text primaryDescriptionText;
    [SerializeField] private TMP_Text secondaryDescriptionText;

    private void Start()
    {
        SetDefaultLoadoutValues();
        loadoutNameInputField.text = PlayerPrefs.GetString($"Loadout{PlayerPrefs.GetInt($"SelectedLoadout")}Name");
    }

    private void Update() 
    {
        UpdateLoadout();
        LoadoutButtonColors();
    }

    private void GetAllLoadoutButtons()
    {
        loadoutButton = new GameObject[loadoutButtonContainer.childCount];
        for(int i = 0; i < loadoutButtonContainer.childCount; i++)
        {
            loadoutButton[i] = loadoutButtonContainer.GetChild(i).gameObject;
            loadoutButton[i].GetComponent<LoadoutButtonIndentifier>().loadoutNumber = i+1;
        }
    }

    private void SetDefaultLoadoutValues()
    {
        GetAllLoadoutButtons();
        foreach(GameObject loadout in loadoutButton)
        {
            int loadoutNumber = loadout.GetComponent<LoadoutButtonIndentifier>().loadoutNumber;

            //Set default loadout when you first start game
            if(!PlayerPrefs.HasKey($"SelectedLoadout")) PlayerPrefs.SetInt($"SelectedLoadout", 1);
            if(!PlayerPrefs.HasKey($"Loadout{loadoutNumber}Name")) PlayerPrefs.SetString($"Loadout{loadoutNumber}Name", $"Loadout {loadoutNumber}"); 
            if(!PlayerPrefs.HasKey($"Loadout{loadoutNumber}Ability")) PlayerPrefs.SetString($"Loadout{loadoutNumber}Ability", "Stim"); 
            if(!PlayerPrefs.HasKey($"Loadout{loadoutNumber}Ordinance")) PlayerPrefs.SetString($"Loadout{loadoutNumber}Ordinance", "Frag"); 
            if(!PlayerPrefs.HasKey($"Loadout{loadoutNumber}Primary")) PlayerPrefs.SetString($"Loadout{loadoutNumber}Primary", "Assault Rifle");
            if(!PlayerPrefs.HasKey($"Loadout{loadoutNumber}Secondary")) PlayerPrefs.SetString($"Loadout{loadoutNumber}Secondary", "Hand Cannon");

            //Set loadout button text to the name of the loadout
            loadout.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetString($"Loadout{loadoutNumber}Name");
        }
    }

    public void OpenLoadout()
    {
        GameObject button = EventSystem.current.currentSelectedGameObject;
        int loadoutNumber = button.GetComponent<LoadoutButtonIndentifier>().loadoutNumber;

        PlayerPrefs.SetInt($"SelectedLoadout", loadoutNumber);

        loadoutNameInputField.text = PlayerPrefs.GetString($"Loadout{loadoutNumber}Name");
        button.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetString($"Loadout{loadoutNumber}Name");
    }

    private void UpdateLoadout()
    {
        GetAllLoadoutButtons();

        GameObject button = loadoutButton[PlayerPrefs.GetInt($"SelectedLoadout")-1];
        int loadoutNumber = button.GetComponent<LoadoutButtonIndentifier>().loadoutNumber;

        PlayerPrefs.SetString($"Loadout{loadoutNumber}Name", loadoutNameInputField.text);
        button.GetComponentInChildren<TextMeshProUGUI>().text = loadoutNameInputField.text;

        abilityText.text = PlayerPrefs.GetString($"Loadout{PlayerPrefs.GetInt($"SelectedLoadout")}Ability");
        abilityButton.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetString($"Loadout{PlayerPrefs.GetInt($"SelectedLoadout")}Ability");

        switch(PlayerPrefs.GetString($"Loadout{PlayerPrefs.GetInt($"SelectedLoadout")}Ability"))
        {
            case "Stim":
                abilityButton.GetComponent<Image>().color = new Color32(19, 224, 14, 255);
                break;
            case "Phase Shift":
                abilityButton.GetComponent<Image>().color = new Color32(75, 23, 149, 255);
                break;
            case "Radar":
                abilityButton.GetComponent<Image>().color = new Color32(245, 216, 15, 255);
                break;
            case "Cloak":
                abilityButton.GetComponent<Image>().color = new Color32(214, 35, 35, 255);
                break;
            case "Grapple":
                abilityButton.GetComponent<Image>().color = new Color32(0, 131, 224, 255);
                break;
            case "Amped Wall":
                abilityButton.GetComponent<Image>().color = new Color32(209, 85, 0, 255);
                break;
            default:
                abilityButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                break;
        }
        
        ordinanceText.text = PlayerPrefs.GetString($"Loadout{PlayerPrefs.GetInt($"SelectedLoadout")}Ordinance");
        ordinanceButton.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetString($"Loadout{PlayerPrefs.GetInt($"SelectedLoadout")}Ordinance");

        switch(PlayerPrefs.GetString($"Loadout{PlayerPrefs.GetInt($"SelectedLoadout")}Ordinance"))
        {
            case "Frag":
                ordinanceButton.GetComponent<Image>().color = new Color32(132, 209, 42, 255);
                break;
            case "Grav. Star":
                ordinanceButton.GetComponent<Image>().color = new Color32(1, 97, 225, 255);
                break;
            case "Firestar":
                ordinanceButton.GetComponent<Image>().color = new Color32(233, 155, 25, 255);
                break;
            case "Elec. Smoke":
                ordinanceButton.GetComponent<Image>().color = new Color32(222, 229, 44, 255);
                break;
            default:
                ordinanceButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                break;
        }

        primaryText.text = PlayerPrefs.GetString($"Loadout{PlayerPrefs.GetInt($"SelectedLoadout")}Primary");
        primaryButton.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetString($"Loadout{PlayerPrefs.GetInt($"SelectedLoadout")}Primary");

        secondaryText.text = PlayerPrefs.GetString($"Loadout{PlayerPrefs.GetInt($"SelectedLoadout")}Secondary");
        secondaryButton.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetString($"Loadout{PlayerPrefs.GetInt($"SelectedLoadout")}Secondary");
    }

    private void LoadoutButtonColors()
    {
        GetAllLoadoutButtons();

        foreach(GameObject loadout in loadoutButton)
        {
            int loadoutNumber = loadout.GetComponent<LoadoutButtonIndentifier>().loadoutNumber;

            if(loadoutNumber == PlayerPrefs.GetInt($"SelectedLoadout")) loadout.GetComponent<Image>().color = Color.green;
            else loadout.GetComponent<Image>().color = Color.white;
        }  
    }

    public void SelectPrimary()
    {
        PlayerPrefs.SetString($"Loadout{PlayerPrefs.GetInt($"SelectedLoadout")}Primary", EventSystem.current.currentSelectedGameObject.GetComponent<LoadoutButtonIndentifier>().gunName);
        MenuManager.Instance.OpenMenu("LoadoutMenu");
    }

    public void SelectSecondary()
    {
        PlayerPrefs.SetString($"Loadout{PlayerPrefs.GetInt($"SelectedLoadout")}Secondary", EventSystem.current.currentSelectedGameObject.GetComponent<LoadoutButtonIndentifier>().gunName);
        MenuManager.Instance.OpenMenu("LoadoutMenu");
    }

    public void SelectAbility()
    {
        PlayerPrefs.SetString($"Loadout{PlayerPrefs.GetInt($"SelectedLoadout")}Ability", EventSystem.current.currentSelectedGameObject.GetComponent<LoadoutButtonIndentifier>().abilityName);
        MenuManager.Instance.OpenMenu("LoadoutMenu");
    }

    public void SelectOrdinance()
    {
        PlayerPrefs.SetString($"Loadout{PlayerPrefs.GetInt($"SelectedLoadout")}Ordinance", EventSystem.current.currentSelectedGameObject.GetComponent<LoadoutButtonIndentifier>().ordinanceName);
        MenuManager.Instance.OpenMenu("LoadoutMenu");
    }

    public void GunHoverDescription(string gun)
    {
        switch(gun)
        {
            case "Assault Rifle":
                primaryDescriptionText.text = "AR go brrrr";
                secondaryDescriptionText.text = "AR go brrrr";
                break;
            case "Submachine Gun":
                primaryDescriptionText.text = "SMG go brrrrr";
                secondaryDescriptionText.text = "SMG go brrrrr";
                break;
            case "Hand Cannon":
                primaryDescriptionText.text = "Hand Cannon go pew pew pew";
                secondaryDescriptionText.text = "Hand Cannon go pew pew pew";
                break;
            case "Shotgun":
                primaryDescriptionText.text = "Shotgun go pchew pchew pchew";
                secondaryDescriptionText.text = "Shotgun go pchew pchew pchew";
                break;
            case "Sniper":
                primaryDescriptionText.text = "Sniper go kpow";
                secondaryDescriptionText.text = "Sniper go kpow";
                break;
            default:
                primaryDescriptionText.text = "";
                secondaryDescriptionText.text = "";
                break;
        }
    }

    public void AbilityHoverDescription(string ability)
    {
        switch(ability)
        {
            case "Stim":
                abilityDescriptionText.text = "Ability - Schpeed, Starts health regen instantly\n\nPassive - Regenerates health faster";
                break;
            case "Phase Shift":
                abilityDescriptionText.text = "Ability - Allows you to become invulnerable and invisible for 2 seconds at a time by entering an alternate dimension\n\nPassive - Can see other players that are in phase. Can also see them through walls";
                break;
            case "Radar":
                abilityDescriptionText.text = "Ability - Scan enemies through walls and show everyone in team\n\nPassive - Can see cloaked enemies";
                break;
            case "Cloak":
                abilityDescriptionText.text = "Ability - Hide from enemies and become invisible for 9 seconds\n\nPassive - Melee while invisible will instant kill enemies";
                break;
            case "Grapple":
                abilityDescriptionText.text = "Ability - It's a grapple mate\n\nPassive - Fun";
                break;
            case "Amped Wall":
                abilityDescriptionText.text = "Ability - Throwable shield that can be shot through one way and while behind it reloading is 50% faster\n\nPassive - Take 25% less damage but 15% slower";
                break;
            default:
                abilityDescriptionText.text = "";
                break;
        }
    }

    public void OrdinanceHoverDescription(string ordinance)
    {
        switch(ordinance)
        {
            case "Frag":
                ordinanceDescriptionText.text = "Throwable explosive.\nIt has a much larger blast radius and is much more lethal in a much wider area than other grenades.";
                break;
            case "Grav. Star":
                ordinanceDescriptionText.text = "Throwable projectile.\nAfter a short delay, the grenade generates a field around it which pulls in enemies and projectiles before exploding.";
                break;
            case "Firestar":
                ordinanceDescriptionText.text = "An incendiary throwing star that creates a patch of scorch on impact.\nIt instant kills enemies when it hits them.";
                break;
            case "Elec. Smoke":
                ordinanceDescriptionText.text = "Throwable projectile.\nExploads on impact creates a fog of smoke that damages enemies over 7 seconds.";
                break;
            default:
                ordinanceDescriptionText.text = "";
                break;
        }
    }
}