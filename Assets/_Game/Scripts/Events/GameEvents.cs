using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static Action OnEnterGameplay;
    public static Action OnEndGame;
    public static Action CheckIfShapeCanPlaced;
    public static Action<ShapeSpawnType> OnNeedCreateShapes;
    public static Action<List<GoalItemType>> OnNeedCreateShapesHaveItem;
    public static Action OnStartGame;
    public static Action OnPlaceOnGrid;


    // Effect
    public static Action GoodEffect;
    public static Action GreatEffect;
    public static Action PerfectEffect;
    public static Action AmazingEffect;
    public static Action NewScoreEffect;
    public static Action AllClearEffect;
    public static Action ExcellentEffect;
    public static Action<int> ComboEfect;

    // Camera Shake
    public static Action<ShakeParam> ShakeCam;

    // BackMainMenu
    public static Action<bool> OnBackMainMenu;  // true is mainmenu, false is adventure map
    public static Action<int> OnCollectCash;    // collect cash and back to main menu will invoke

}


public enum ShapeSpawnType
{
    Normal,
    OneBeauty,
    TwoBeauty,
    ThreeBeauty,
    Perfect,
}