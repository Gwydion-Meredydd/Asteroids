using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#region SummarySection
/// <summary>
/// Singleton class that is used to control all User Interface elements
///  </summary>
/// <param name="UserInterfaceManager"></param>

#endregion
public class UserInterfaceManager : MonoBehaviour
{
    public static UserInterfaceManager Instance { get; private set; }
    public GameObject GameUI;
    public GameObject MainMenuUI;
    public GameObject PauseMenuUI;
    public GameObject DeathUI;

    [Header ("in Game UI")]
    public GameObject[] UpgradeUI;
    public GameObject ScoreUI;
    public Text ScoreText;
    public GameObject[] HealthIcons;
    public Text SheildTime;

    [Header("MainMenu UI")]
    public GameObject TitleMenu;
    public GameObject HelpMenu;
    public GameObject OptionsMenu;
    public GameObject ExitMenu;
    [Space]
    private int HelpMenuIndexValue;
    public GameObject[] HelpMenus;
    [Header("Death Screen UI")]
    public Text DeathScore;
    public Text HighScore;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (GameManager.Instance.InGame) 
        {
            GameUI.SetActive(true);
            MainMenuUI.SetActive(false);
            Cursor.visible = false;
        }
        else 
        {
            GameUI.SetActive(false);
            MainMenuUI.SetActive(true);
            Cursor.visible = true;
        }
    }
    #region MainMenu 
    public void MainMenuStartButtonPressed() 
    {
        GameManager.Instance.InGame = true;
        GameUI.SetActive(true);
        MainMenuUI.SetActive(false);
        Cursor.visible = false;
    }
    #region MainMenu HelpMenu
    public void MainMenuHelpPressed() 
    {
        TitleMenu.SetActive(false);
        HelpMenu.SetActive(true);
        HelpMenuIndexValue = 0;
        foreach (GameObject HelpMenu in HelpMenus)
        {
            HelpMenu.SetActive(false);
        }
        HelpMenus[HelpMenuIndexValue].SetActive(true);
    }
    public void MainMenuHelpReturn() 
    {
        TitleMenu.SetActive(true);
        HelpMenu.SetActive(false);
        HelpMenuIndexValue = 0;
        foreach (GameObject HelpMenu in HelpMenus)
        {
            HelpMenu.SetActive(false);
        }
        HelpMenus[HelpMenuIndexValue].SetActive(true);
    }
    public void MainMenuHelpNextPressed()
    {
        foreach (GameObject HelpMenu in HelpMenus)
        {
            HelpMenu.SetActive(false);
        }
        if (HelpMenuIndexValue != 3)
        {
            HelpMenuIndexValue += 1;
            HelpMenus[HelpMenuIndexValue].SetActive(true);
        }
        else 
        {
            HelpMenuIndexValue = 0;
            HelpMenus[HelpMenuIndexValue].SetActive(true);
        }
    }
    public void MainMenuHelpPreviousPressed()
    {
        foreach (GameObject HelpMenu in HelpMenus)
        {
            HelpMenu.SetActive(false);
        }
        if (HelpMenuIndexValue != 0)
        {
            HelpMenuIndexValue -= 1;
            HelpMenus[HelpMenuIndexValue].SetActive(true);
        }
        else 
        {
            HelpMenuIndexValue = 3;
            HelpMenus[HelpMenuIndexValue].SetActive(true);
        }
    }
    #endregion

    #region MainMenu OptionsMenu
    public void MainMenuOptionsMenuPressed() 
    {
        TitleMenu.SetActive(false);
        OptionsMenu.SetActive(true);
    }
    public void MainMenuOptionsMenuReturn()
    {
        TitleMenu.SetActive(true);
        OptionsMenu.SetActive(false);
    }
    #endregion

    #region MainMenu ExitMenu
    public void MainMenuExitButtonPressed() 
    {
        TitleMenu.SetActive(false);
        ExitMenu.SetActive(true);
    }
    public void MainMenuExitReturnPressed()
    {
        TitleMenu.SetActive(true);
        ExitMenu.SetActive(false);
    }
    public void MainMenuExitYesPressed() 
    {
        Application.Quit();
    }
    #endregion
    #endregion

    #region PauseMenu
    public void PauseMenuEnablePauseMenu() 
    {
        PauseMenuUI.SetActive(true);
        GameUI.SetActive(false);
        GameManager.Instance.Paused = true;
        GameManager.Instance.InGame = false;
        Cursor.visible = true;
        Time.timeScale = 0;
    }
    public void PauseMenuDisablePauseMenu() 
    {
        PauseMenuUI.SetActive(false);
        GameUI.SetActive(true);
        GameManager.Instance.Paused = false;
        GameManager.Instance.InGame = true;
        Cursor.visible = false;
        Time.timeScale = 1;
    }
    //called from shipsystem
    public void EscapeKeyPressed() 
    {
        if (GameManager.Instance.Paused) 
        {
            PauseMenuDisablePauseMenu();
        }
        else 
        {
            PauseMenuEnablePauseMenu();
        }
    }
    //ensures all memeory is cleared and then restarts the scene
    public void PauseMenuBacktoMainMenuPressed() 
    {
        ShipManager.Instance.ClearMemeory();
        AsteroidManager.Instance.ClearMemory();
        UpgradeManager.Instance.ClearMemory();
        EnemyManager.Instance.ClearMemeory();
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    #endregion

    #region Game User Interface
    //Called to enable the Upgrade UI
    public void EnableUpgradeUI() 
    {
        Cursor.visible = true;
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

    //updates shield time ui
    public void UpdateShieldTime() 
    {
        SheildTime.text = ShipManager.Instance.CurrentShieldTime.ToString();
    }

    //Allows the player to move again and disables the Upgrade UI
    public void DisableUpgradeUI() 
    {
        Cursor.visible = false;
        foreach (GameObject ugradeUi in UpgradeUI)
        {
            ugradeUi.SetActive(false);
        }
        GameManager.Instance.InMenu = false;
        Time.timeScale = 1;
    }

    //Changes the score UI
    public void UpdateScore()
    {
        ScoreText.text =  ScoreManger.Instance.currentScore.ToString();
    }

    //responsible for Health Icons UI
    public void UpdateHealth() 
    {
        foreach (GameObject HealthIcon in HealthIcons)
        {
            HealthIcon.SetActive(true);
        }
        switch (ShipManager.Instance.Health) 
        {
            case 0:
                for (int i = 0; i < HealthIcons.Length; i++)
                {
                    HealthIcons[i].SetActive(false);
                }
                break;
            case 1:
                for (int i = 1; i < HealthIcons.Length; i++)
                {
                    HealthIcons[i].SetActive(false);
                }
                break;
            case 2:
                for (int i = 2; i < HealthIcons.Length; i++)
                {
                    HealthIcons[i].SetActive(false);
                }
                break;
            case 3:
                for (int i = 3; i < HealthIcons.Length; i++)
                {
                    HealthIcons[i].SetActive(false);
                }
                break;
            case 4:
                for (int i = 4; i < HealthIcons.Length; i++)
                {
                    HealthIcons[i].SetActive(false);
                }
                break;
            case 5:
                for (int i = 5; i < HealthIcons.Length; i++)
                {
                    HealthIcons[i].SetActive(false);
                }
                break;
        }
    }
    #endregion

    public void EnableDeathUI() 
    {
        PauseMenuUI.SetActive(false);
        GameUI.SetActive(false);
        DeathUI.SetActive(true);
        DeathScore.text = ScoreManger.Instance.currentScore.ToString();
        if (ScoreManger.Instance.GetScore("score") < ScoreManger.Instance.currentScore) 
        {
            ScoreManger.Instance.SetScore("score", ScoreManger.Instance.currentScore);
        }
        HighScore.text = ScoreManger.Instance.GetScore("score").ToString();
    }
    //called from main menu and pause menu
    #region Global Options
    public void QualitySettingsChange(int QualitySettingsValue) 
    {
        QualitySettings.SetQualityLevel(QualitySettingsValue, true);
    }
    public void ToggleScreenMode() 
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    //uses unity mixer for smoother audio sliding
    public void MusicChange(float SliderValue) 
    {
        AudioManager.Instance.Mixer.SetFloat("MUSIC", Mathf.Log10(SliderValue) * 20);
    }
    //uses unity mixer for smoother audio sliding
    public void SFXChange(float SliderValue)
    {
        AudioManager.Instance.Mixer.SetFloat("SFX", Mathf.Log10(SliderValue) * 20);
    }
    #endregion
}
