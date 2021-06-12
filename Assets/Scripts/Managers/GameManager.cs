using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool InGame;
    public bool Paused;

    public static GameManager _gameManager;
    [HideInInspector]
    public GameManager refGameManger;

    private void Start()
    {
        refGameManger = this;
        _gameManager = refGameManger;
    }
}
