using CoreDang;
using UnityEngine;

public class BBSaveData : BaseUserData
{
    public static BBSaveData Ins => LocalData.Get<BBSaveData>();
    public int Level;
    public int BestScore;

    public int BBClassicPlayCount;
    public int BBStartLevelCount;
    public int BBContinueLevelCount;
    public int BBClassicContinue;

    protected override void OnInit()
    {
        Level = 1;
        BestScore = 0;
        base.OnInit();
    }

    protected override void OnLoad()
    {
        if(Level < 1) Level = 1;
        base.OnLoad();
    }
}

