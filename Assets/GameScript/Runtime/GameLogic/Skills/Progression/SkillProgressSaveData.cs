using System;
using System.Collections.Generic;

[Serializable]
public sealed class SkillNodeProgressData
{
    public string nodeId;
    public int rank;
}

[Serializable]
public sealed class SkillProgressSaveData
{
    public int availablePoints;
    public int totalEarnedPoints;
    public List<SkillNodeProgressData> nodes =
        new List<SkillNodeProgressData>();
}

[Serializable]
public sealed class CharacterSaveData
{
    public int version = 1;
    public SkillProgressSaveData skillProgress =
        new SkillProgressSaveData();
}
