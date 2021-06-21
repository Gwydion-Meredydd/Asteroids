using UnityEngine;

#region SummarySection
/// <summary>
/// Singleton class that is used to enable and disable ui elemenets 
///  </summary>
/// <param name="UserInterfaceManager"></param>

#endregion
public class UserInterfaceManager : MonoBehaviour
{
    public static UserInterfaceManager Instance { get; private set; }
    public GameObject[] UpgradeUI;

    private void Awake()
    {
        Instance = this;
    }

    //Called to enable the Upgrade UI
    public void EnableUpgradeUI() 
    {
        GameManager.Instance.InMenu = true;
        Time.timeScale = 0;
        if (UpgradeManager.Instance.UpgradePath.x == 0) 
        {
            UpgradeUI[0].SetActive(true);
        }
        else if (UpgradeManager.Instance.UpgradePath.y == 0) 
        {
            UpgradeUI[1].SetActive(true);
        }
        else if (UpgradeManager.Instance.UpgradePath.z == 0)
        {
            UpgradeUI[2].SetActive(true);
        }
        else if (UpgradeManager.Instance.UpgradePath.w == 0)
        {
            UpgradeUI[3].SetActive(true);
        }
    }

    //Allows the player to move again and disables the Upgrade UI
    public void DisableUpgradeUI() 
    {
        foreach (GameObject ugradeUi in UpgradeUI)
        {
            ugradeUi.SetActive(false);
        }
        GameManager.Instance.InMenu = false;
        Time.timeScale = 1;
    }
}
