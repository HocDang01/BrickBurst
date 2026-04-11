using System;
[Serializable]
public class EffectConfig
{
    // Tile fall down
    public float MinJumpHeight = 10f;
    public float MaxJumpHeight = 20f;
    public float FallDistance = 200f;
    public float TimeJump = 0.3f;
    public float TimeFall = 0.7f;


    // Intro Board
    public float IntroColumnDelay = 0.03f;
    public float IntroRowDelay = 0.06f;
    public float IntroTileHide = 0.1f;


    public float EndColumnDelay = 0.03f;
    public float EndRowDelay = 0.06f;
    public float EndTileHide = 1f;

    // OutlineFadeEffect
    public float OutlineFadeSpeed = 7f;

}
