using UnityEngine;
using System.Collections.Generic;

public class BattleLogManager : MonoBehaviour
{
    public static BattleLogManager instance;
    
    public int maxLogEntries = 50;
    private List<BattleLogEntry> battleLogs;
    private Queue<string> recentMessages;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            battleLogs = new List<BattleLogEntry>();
            recentMessages = new Queue<string>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void LogAttack(string attackerName, string targetName, int damage, bool isCounter, bool isCritical)
    {
        string message = FormatAttackMessage(attackerName, targetName, damage, isCounter, isCritical);
        AddLog(message, BattleLogType.ATTACK);
        recentMessages.Enqueue(message);
        
        if (recentMessages.Count > 10)
            recentMessages.Dequeue();
    }
    
    public void LogDeath(string unitName, UnitSide side)
    {
        string message = $"{unitName} {(side == UnitSide.PLAYER ? "阵亡！" : "被消灭！")}";
        AddLog(message, BattleLogType.DEATH);
        recentMessages.Enqueue(message);
    }
    
    public void LogSkill(string unitName, string skillName, string effect)
    {
        string message = $"{unitName} 使用 [{skillName}]！{effect}";
        AddLog(message, BattleLogType.SKILL);
        recentMessages.Enqueue(message);
    }
    
    public void LogBurn(string unitName, int damage)
    {
        string message = $"{unitName} 受到燃烧伤害 {damage}";
        AddLog(message, BattleLogType.STATUS);
    }
    
    public void LogCharge(string unitName, int bonusDamage)
    {
        string message = $"{unitName} 冲锋！伤害加成 +{bonusDamage}%";
        AddLog(message, BattleLogType.SKILL);
        recentMessages.Enqueue(message);
    }
    
    public void LogRoundStart(int round)
    {
        string message = $"=== 第 {round} 回合开始 ===";
        AddLog(message, BattleLogType.SYSTEM);
    }
    
    public void LogVictory(bool isPlayer)
    {
        string message = isPlayer ? "=== 战斗胜利！===" : "=== 战斗失败！===";
        AddLog(message, BattleLogType.SYSTEM);
    }
    
    string FormatAttackMessage(string attacker, string target, int damage, bool counter, bool critical)
    {
        string baseMsg = $"{attacker} 攻击 {target}";
        if (critical)
            baseMsg += $" 造成 {damage} 伤害(暴击！)";
        else if (counter)
            baseMsg += $" 造成 {damage} 伤害(克制)";
        else
            baseMsg += $" 造成 {damage} 伤害";
        return baseMsg;
    }
    
    void AddLog(string message, BattleLogType type)
    {
        BattleLogEntry entry = new BattleLogEntry
        {
            message = message,
            type = type,
            timestamp = Time.time,
            round = BattleManager.instance != null ? 
                Mathf.RoundToInt((Time.time - BattleManager.instance.currentLevel) / 10f) : 0
        };
        
        battleLogs.Add(entry);
        
        if (battleLogs.Count > maxLogEntries)
            battleLogs.RemoveAt(0);
    }
    
    public List<string> GetRecentMessages(int count)
    {
        List<string> result = new List<string>();
        int i = 0;
        foreach (var msg in recentMessages)
        {
            if (i >= recentMessages.Count - count)
                result.Add(msg);
            i++;
        }
        return result;
    }
    
    public List<BattleLogEntry> GetAllLogs()
    {
        return new List<BattleLogEntry>(battleLogs);
    }
    
    public void ClearLogs()
    {
        battleLogs.Clear();
        recentMessages.Clear();
    }
}

public enum BattleLogType
{
    ATTACK,
    DEATH,
    SKILL,
    STATUS,
    SYSTEM
}

public class BattleLogEntry
{
    public string message;
    public BattleLogType type;
    public float timestamp;
    public int round;
}