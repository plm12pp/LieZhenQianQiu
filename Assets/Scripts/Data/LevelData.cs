using System;
using System.Collections.Generic;

[Serializable]
public class LevelSettings
{
    public int baseEnemyCount;
    public int maxEnemyCount;
    public float difficultyMultiplier;
    public int resetInterval;
    public float resetFactor;
    public int maxLives;
}

[Serializable]
public class LevelRewards
{
    public int baseCoinReward;
    public int coinIncreasePerLevel;
    public float tokenRewardChance;
    public int tokenRewardAmount;
    public float jadeRewardChance;
    public int jadeRewardAmount;
}

[Serializable]
public class MapDimensions
{
    public int columns;
    public int rows;
    public int playerRows;
    public int enemyRows;
    public int riverRow;
    public int totalCells;
}

[Serializable]
public class FerryPosition
{
    public int column;
    public int row;
}

[Serializable]
public class LevelData
{
    public LevelSettings levelSettings;
    public Dictionary<string, int> enemyCountByLevel;
    public LevelRewards levelRewards;
    public MapDimensions mapDimensions;
    public Dictionary<string, FerryPosition> ferryPositions;
}
