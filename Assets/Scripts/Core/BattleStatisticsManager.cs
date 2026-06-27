using UnityEngine;
using System.Collections.Generic;

public class BattleStatisticsManager : MonoBehaviour
{
    public static BattleStatisticsManager instance;
    
    public Dictionary<string, PlayerStatistics> playerStats;
    public Dictionary<string, TroopUsageStatistics> troopStats;
    public BattleSessionStatistics currentSession;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            playerStats = new Dictionary<string, PlayerStatistics>();
            troopStats = new Dictionary<string, TroopUsageStatistics>();
            currentSession = new BattleSessionStatistics();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void StartNewSession(int level)
    {
        currentSession = new BattleSessionStatistics
        {
            level = level,
            startTime = Time.time
        };
    }
    
    public void RecordUnitKilled(string troopId, UnitSide side)
    {
        if (side == UnitSide.PLAYER)
        {
            currentSession.playerUnitsLost++;
            currentSession.totalPlayerDamageTaken += 100; // 估算伤害
        }
        else
        {
            currentSession.enemyUnitsKilled++;
            currentSession.totalPlayerDamageDealt += 100; // 估算伤害
            
            UpdateTroopStats(troopId, TroopStatType.KILLS, 1);
        }
    }
    
    public void RecordDamage(int damage, UnitSide attackerSide)
    {
        if (attackerSide == UnitSide.PLAYER)
        {
            currentSession.totalPlayerDamageDealt += damage;
        }
        else
        {
            currentSession.totalPlayerDamageTaken += damage;
        }
    }
    
    public void RecordSkillUse(string troopId, string skillName)
    {
        currentSession.skillsUsed++;
        UpdateTroopStats(troopId, TroopStatType.SKILLS_USED, 1);
    }
    
    public void RecordCriticalHit(string troopId)
    {
        currentSession.criticalHits++;
        UpdateTroopStats(troopId, TroopStatType.CRITICAL_HITS, 1);
    }
    
    public void EndSession(bool victory)
    {
        currentSession.victory = victory;
        currentSession.endTime = Time.time;
        currentSession.duration = currentSession.endTime - currentSession.startTime;
        
        UpdatePlayerStats(currentSession);
        
        GameManager.instance.SaveGame();
    }
    
    void UpdateTroopStats(string troopId, TroopStatType statType, int value)
    {
        if (!troopStats.ContainsKey(troopId))
        {
            troopStats[troopId] = new TroopUsageStatistics
            {
                troopId = troopId,
                name = GameManager.instance.GetTroopData(troopId)?.name ?? troopId
            };
        }
        
        switch (statType)
        {
            case TroopStatType.KILLS:
                troopStats[troopId].kills += value;
                break;
            case TroopStatType.SKILLS_USED:
                troopStats[troopId].skillsUsed += value;
                break;
            case TroopStatType.CRITICAL_HITS:
                troopStats[troopId].criticalHits += value;
                break;
            case TroopStatType.DAMAGE_DEALT:
                troopStats[troopId].damageDealt += value;
                break;
        }
        
        troopStats[troopId].totalUses++;
    }
    
    void UpdatePlayerStats(BattleSessionStatistics session)
    {
        string playerName = GameManager.instance.currentSave.playerName;
        
        if (!playerStats.ContainsKey(playerName))
        {
            playerStats[playerName] = new PlayerStatistics
            {
                playerName = playerName
            };
        }
        
        PlayerStatistics stats = playerStats[playerName];
        stats.totalBattles++;
        stats.totalDamageDealt += session.totalPlayerDamageDealt;
        stats.totalDamageTaken += session.totalPlayerDamageTaken;
        stats.totalUnitsKilled += session.enemyUnitsKilled;
        stats.totalUnitsLost += session.playerUnitsLost;
        stats.totalSkillsUsed += session.skillsUsed;
        
        if (session.victory)
        {
            stats.totalVictories++;
            stats.highestLevel = Mathf.Max(stats.highestLevel, session.level);
        }
        else
        {
            stats.totalDefeats++;
        }
    }
    
    public PlayerStatistics GetPlayerStats()
    {
        string playerName = GameManager.instance.currentSave.playerName;
        if (playerStats.ContainsKey(playerName))
            return playerStats[playerName];
        return new PlayerStatistics();
    }
    
    public List<TroopUsageStatistics> GetTopTroopsByKills(int count)
    {
        List<TroopUsageStatistics> sorted = new List<TroopUsageStatistics>(troopStats.Values);
        sorted.Sort((a, b) => b.kills.CompareTo(a.kills));
        return sorted.GetRange(0, Mathf.Min(count, sorted.Count));
    }
}

public enum TroopStatType
{
    KILLS,
    SKILLS_USED,
    CRITICAL_HITS,
    DAMAGE_DEALT
}

public class PlayerStatistics
{
    public string playerName;
    public int totalBattles;
    public int totalVictories;
    public int totalDefeats;
    public int highestLevel;
    public int totalDamageDealt;
    public int totalDamageTaken;
    public int totalUnitsKilled;
    public int totalUnitsLost;
    public int totalSkillsUsed;
    
    public float winRate => totalBattles > 0 ? (float)totalVictories / totalBattles : 0;
}

public class TroopUsageStatistics
{
    public string troopId;
    public string name;
    public int totalUses;
    public int kills;
    public int skillsUsed;
    public int criticalHits;
    public int damageDealt;
    
    public float averageDamage => totalUses > 0 ? (float)damageDealt / totalUses : 0;
}

public class BattleSessionStatistics
{
    public int level;
    public float startTime;
    public float endTime;
    public float duration;
    public bool victory;
    public int playerUnitsLost;
    public int enemyUnitsKilled;
    public int totalPlayerDamageDealt;
    public int totalPlayerDamageTaken;
    public int skillsUsed;
    public int criticalHits;
    public int effectiveRound;
}