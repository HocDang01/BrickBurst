using System;
using System.Collections.Generic;
[Serializable]
public class ScoreConfig
{
    public int ScorePerRowCol = 10;
    public int IntervalCombo = 3;
    public List<BonusScore> BonusScores = new();
    public List<ComboScore> ComboScores = new();

    public int GetBonusScore(int combo)
    {
        if (combo < 2) return 0;
        var config = BonusScores.Find(e => e.Bonus == combo);
        int minScore = config != null ? config.MinScore : BonusScores[^1].MinScore;
        int maxScore = config != null ? config.MaxScore : BonusScores[^1].MaxScore;
        return UnityEngine.Random.Range(minScore, maxScore + 1);
    }
    public int GetComboScore(int streak)
    {
        if (streak < 2) return 0;
        var config = ComboScores.Find(e => e.Combo == streak);
        int minScore = config != null ? config.MinScore : ComboScores[^1].MinScore;
        int maxScore = config != null ? config.MaxScore : ComboScores[^1].MaxScore;
        return UnityEngine.Random.Range(minScore, maxScore + 1);
    }
}

[Serializable]
public class BonusScore
{
    public int Bonus;
    public int MinScore;
    public int MaxScore;
}

[Serializable]
public class ComboScore
{
    public int Combo;
    public int MinScore;
    public int MaxScore;
}
