using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ShapeSpawnerConfig
{

    public float MinimunWeight = 0.1f;

    public float MinOccupiedOfferOneBeauty = 0.3f;
    public float MaxOccupiedOfferOneBeauty = 0.6f;
    public float MinOccupiedOfferTwoBeauty = 0.6f;
    public float MaxOccupiedOfferTwoBeauty = 0.8f;
    public float MinOccupiedOfferThreeBeauty = 0.8f;
    public float MaxOccupiedOfferThreeBeauty = 1f;

    public List<ScorePerStageData> ScorePerStages = new();
    public List<PoolShapeConfig> PoolShapeConfigs = new();

    public float TotalWeight => PoolShapeConfigs.Sum(c => c.Weight);

    public ShapeData GetRandomShapeByWeight()   // Use this shape
    {
        int idx = GetRandomIndexPool();
        if (idx < 0)
            return null;
        return GetRandomShapebyPool(PoolShapeConfigs[idx].ShapeRatios);
    }
    private ShapeData GetRandomShapebyPool(List<ShapeRatio> shapeRatios)
    {
        if (shapeRatios == null || shapeRatios.Count <= 0) return null;
        int idx = UnityEngine.Random.Range(0, shapeRatios.Count);
        return shapeRatios[idx].ShapeData;
    }
    private int GetRandomIndexPool()
    {
        float randomValue = UnityEngine.Random.Range(0f, TotalWeight);
        float cumulativeWeight = 0;

        for (int i = 0; i < PoolShapeConfigs.Count; i++)
        {
            cumulativeWeight += PoolShapeConfigs[i].Weight;
            if (randomValue < cumulativeWeight)
            {
                return i;
            }
        }
        return -1;
    }

}

[Serializable]
public class ScorePerStageData
{
    public int Score;
}

[Serializable]
public class PoolShapeConfig
{
    public PoolShapeConfig()
    {

    }
    public float Weight;            // Use in game
    public ShapeType ShapeType;

    public WeightPool WeightPoolAdventure;
    public WeightPool WeightPoolClassic;

    // public float BaseWeight;
    // public GrowType GrowType;
    // public float GrowWeight;
    public List<ShapeRatio> ShapeRatios = new();

    public void UpdateWeightFollowLevel(int level)
    {
        if (WeightPoolAdventure.GrowType == GrowType.Increase)
        {
            Weight = Mathf.Max(WeightPoolAdventure.BaseWeight + level * WeightPoolAdventure.GrowWeight, GameConfig.Ins.ShapeSpawnerConfig.MinimunWeight);
        }
        else if (WeightPoolAdventure.GrowType == GrowType.Decrease)
        {
            Weight = Mathf.Max(WeightPoolAdventure.BaseWeight - level * WeightPoolAdventure.GrowWeight, GameConfig.Ins.ShapeSpawnerConfig.MinimunWeight);
        }
    }

    public void UpdateWeightFollowEndless(int score)
    {
        int stage = GameConfig.Ins.ShapeSpawnerConfig.ScorePerStages.Count - 1;
        for (int i = 0; i < GameConfig.Ins.ShapeSpawnerConfig.ScorePerStages.Count; i++)
        {
            if (score < GameConfig.Ins.ShapeSpawnerConfig.ScorePerStages[i].Score)
            {
                stage = i;
                break;
            }
        }
        if (WeightPoolClassic.GrowType == GrowType.Increase)
        {
            Weight = Mathf.Max(WeightPoolClassic.BaseWeight + stage * WeightPoolClassic.GrowWeight, GameConfig.Ins.ShapeSpawnerConfig.MinimunWeight);
        }
        else if (WeightPoolClassic.GrowType == GrowType.Decrease)
        {
            Weight = Mathf.Max(WeightPoolClassic.BaseWeight - stage * WeightPoolClassic.GrowWeight, GameConfig.Ins.ShapeSpawnerConfig.MinimunWeight);
        }
    }
}

[Serializable]
public class WeightPool
{
    public float BaseWeight;
    public GrowType GrowType;
    public float GrowWeight;
}

[Serializable]
public class ShapeRatio
{
    public ShapeData ShapeData;
    // public float Weight;
}

public enum ShapeType
{
    None,
    Big,
    Small,
    Line,
    Complex,
}
public enum GrowType
{
    Increase,
    Decrease
}
