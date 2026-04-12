using CoreDang;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig")]
public class GameConfig : SingletonScriptableObject<GameConfig>
{
    public bool EnableCheat;
    public TileColorConfig TileColorConfig;
    public ShapeSpawnerConfig ShapeSpawnerConfig;
    public GameplayConfig GameplayConfig;
    public ScoreConfig ScoreConfig;
    public EffectConfig EffectConfig;
    public LevelEditorConfig LevelEditorConfig;

}