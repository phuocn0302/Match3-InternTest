using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : ScriptableObject
{
    public int BoardSizeX = 5;

    public int BoardSizeY = 5;
    public int BottomBarSize = 5;

    public int MatchesMin = 3;

    public int LevelMoves = 16;

    public float LevelTime = 30f;
    
    public float LevelNormal = 0;
    
    public float LevelTimeAttack = 60f;

    public float TimeForHint = 5f;
}
