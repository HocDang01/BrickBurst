using CoreDang;
using UnityEngine;

public class BBSaveData : BaseUserData
{
    public static BoardSaveData Ins => LocalData.Get<BoardSaveData>();
    public int BestScore;
    public int BBStartLevelCount;
    public int BBContinueLevelCount;
}

