using CoreDang;
using UnityEngine;
public class BBManager : SingletonMono<BBManager>, IObservable
{
    public static bool MilestoneViewFromMainMenu;
    public bool InMenuToGameplay;
    public bool ContinueProgress = false;
    public static bool EnableCheat = true;
    public static bool NewAdventure;

    protected override void Awake()
    {
        base.Awake();
        MilestoneViewFromMainMenu = false;
        InMenuToGameplay = false;
        ContinueProgress = false;
        // --------- Dev ---------
        EnableCheat = GameConfig.Ins.EnableCheat;

        NewAdventure = false;
    }
}
