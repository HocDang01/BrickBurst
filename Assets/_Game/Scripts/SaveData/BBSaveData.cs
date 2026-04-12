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
}

