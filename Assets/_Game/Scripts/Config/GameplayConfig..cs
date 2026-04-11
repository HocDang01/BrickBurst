using System;
[Serializable]
public class GameplayConfig
{
    // Level test
    public int Level = 1;
    public int MaxLevel = 2;
    public int LevelToLoop = 24;

    // Gap of each tile
    public float EverySquareOffset = 5f;

    // Count row/column start hightlight rainbow
    public int CountStartRainbow = 3;

    // Minimun Height of each pool


    // Camera Shake
    public float Duration = 0.2f;
    public float InitSeverity = 5f;
    public int ComboCountShake = 3;

}
