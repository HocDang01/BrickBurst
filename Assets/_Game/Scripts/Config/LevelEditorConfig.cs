using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelEditorConfig
{
    public string FolderLevelPath;
    public string FolderName;
    public string LevelName;
    public List<AdventureLevelConfig> AdventureLevelConfigs = new();
}
[Serializable]
public class AdventureLevelConfig
{
    public int MinLevel;
    public int MaxLevel;
}