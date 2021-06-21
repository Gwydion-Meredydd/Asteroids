using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region SummarySection
/// <summary>
/// Singleton class that is used to track the game states 
///  </summary>
/// <param name="GameManager"></param>

#endregion
public class GameManager : MonoBehaviour
{
    public bool InGame;
    public bool Paused;
    public bool InMenu;

    public static GameManager Instance { get; private set;}

    private void Awake()
    {
        Instance = this;
    }
}
