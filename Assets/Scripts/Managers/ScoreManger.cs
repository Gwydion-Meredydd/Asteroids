using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManger : MonoBehaviour
{
    public static ScoreManger Instance { get; private set; }

    public int currentScore;
    public int largeAsteroidScore;
    public int mediumAsteroidScore;
    public int smallAsteroidScore;
    public int enemyShipScore;

    private void Awake()
    {
        Instance = this;
    }
}
