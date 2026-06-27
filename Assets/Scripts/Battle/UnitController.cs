using UnityEngine;
using System.Collections.Generic;

public enum UnitState
{
    IDLE,
    MOVING,
    ATTACKING,
    SKILL_CASTING,
    DEAD
}

public enum UnitSide
{
    PLAYER,
    ENEMY
}

public enum SkillEffectType
{
    NONE,
    FIRST_STRIKE,
    CHARGE,
    SPLASH,
    BURN,
    BERSERK,
    AURA_GUARD,
    AURA_DEFENSE,
    ARMOR_IGNORE,
    ARMOR_PIERCE,
    DOUBLE_SHOT,
    MOVE_AND_SHOOT,
    DEATH_COUNTER,
    LAST_STAND,
    RIVER_BONUS,
    ELITE_STATS,
    VINE_ARMOR,
    SHIP_RAM,
    MUSKET_VOLLEY,
    CAVALRY_COUNTER,
    CHARGE_ULTIMATE,
    CHARGE_HEAVY,
    ELITE_ALL,
    ELITE_ULTIMATE,
    HAN_CAVALRY,
    SWIFT,
    VOLLEY,
    SPLASH_HEAVY,
    CAVALRY_ARCHER,
    TRAMPLE,
    PIERCE_HEAVY,
    VOLLEY_HEAVY,
    RIVER_MASTER,
    CRITICAL_STRIKE,
    LONG_RANGE,
    BALANCED,
    CHARIOT_CHARGE,
    XIAO_CHARGE,
    MONGOL_CHARGE,
    EIGHT_BANNER,
    ASSASSINATE,
    MO_DAO
}

public class UnitController : MonoBehaviour
{
    public string unitId;
    public string troopDataId;
    public UnitSide side;
    public UnitState state;
    public int currentHp;
    public int maxHp;
    public int baseAtk;
    public int atk;
    public float range;
    public float speed;
    public int baseArmor;
    public int armor;
    public int star;
    public int size;
    public bool isHorizontal;
    public Vector2Int position;
    
    public string skillName;
    public SkillEffectType skillEffectType;
    public string skillDescription;
    public float skillMultiplier;
    public int skillCooldown;
    private int currentSkillCooldown;
    private int attackCount;
    private bool hasUsedFirstStrike;
    private bool isBerserk;
    private int burnDamage;
    private int burnTurns;
    
    private float attackInterval;
    private float lastAttackTime;
    private float lastMoveTime;
    private List<Vector2Int> occupiedCells;
    private BattleGrid battleGrid;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    void Awake()
    {
        attackCount = 0;
        hasUsedFirstStrike = false;
        isBerserk = false;
        burnDamage = 0;
        burnTurns = 0;
        currentSkillCooldown = 0;
        attackInterval = 1.0f;
    }
    
    void Start()
    {
        battleGrid = FindObjectOfType<BattleGrid>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        state = UnitState.IDLE;
        occupiedCells = battleGrid.GetUnitCells(position.x, position.y, size, isHorizontal);
        ApplyStarBonus();
    }

    void ApplyStarBonus()
    {
        float starBonus = GameManager.instance.GetStarBonus(star);
        maxHp = Mathf.RoundToInt(maxHp * starBonus);
        currentHp = maxHp;
        baseAtk = atk;
        atk = Mathf.RoundToInt(atk * starBonus);
        baseArmor = armor;
        armor = Mathf.RoundToInt(armor * (1.0f + 0.1f * (star - 1)));
        
        ApplyPassiveSkillBonuses();
    }
    
    void ApplyPassiveSkillBonuses()
    {
        switch (skillEffectType)
        {
            case SkillEffectType.ELITE_STATS:
                maxHp = Mathf.RoundToInt(maxHp * 1.15f);
                currentHp = maxHp;
                atk = Mathf.RoundToInt(atk * 1.15f);
                baseAtk = atk;
                armor = Mathf.RoundToInt(armor * 1.1f);
                baseArmor = armor;
                break;
            case SkillEffectType.ELITE_ULTIMATE:
                maxHp = Mathf.RoundToInt(maxHp * 1.3f);
                currentHp = maxHp;
                atk = Mathf.RoundToInt(atk * 1.3f);
                baseAtk = atk;
                armor = Mathf.RoundToInt(armor * 1.2f);
                baseArmor = armor;
                break;
            case SkillEffectType.EIGHT_BANNER:
                maxHp = Mathf.RoundToInt(maxHp * 1.1f);
                currentHp = maxHp;
                atk = Mathf.RoundToInt(atk * 1.1f);
                baseAtk = atk;
                armor = Mathf.RoundToInt(armor * 1.1f);
                baseArmor = armor;
                speed *= 1.1f;
                break;
            case SkillEffectType.SWIFT:
                speed *= 1.2f;
                break;
            case SkillEffectType.LONG_RANGE:
                range *= 1.2f;
                break;
            case SkillEffectType.BALANCED:
                maxHp = Mathf.RoundToInt(maxHp * 1.05f);
                currentHp = maxHp;
                atk = Mathf.RoundToInt(atk * 1.05f);
                baseAtk = atk;
                armor = Mathf.RoundToInt(armor * 1.05f);
                baseArmor = armor;
                break;
        }
    }

    void Update()
    {
        if (state == UnitState.DEAD)
            return;

        UpdateStatusEffects();
        UpdateBerserk();
        UpdateAuraEffects();

        if (Time.time - lastAttackTime >= attackInterval)
        {
            UnitController target = FindTarget();
            if (target != null)
            {
                float distance = battleGrid.GetDistance(GetCenterPosition(), target.GetCenterPosition());
                if (distance <= range)
                {
                    Attack(target);
                }
                else
                {
                    MoveTowards(target);
                }
            }
            else
            {
                state = UnitState.IDLE;
            }
        }
    }

    void UpdateStatusEffects()
    {
        if (burnTurns > 0)
        {
            burnTurns--;
            TakeDamage(burnDamage, true);
        }
    }

    void UpdateBerserk()
    {
        if (skillEffectType == SkillEffectType.BERSERK)
        {
            float hpPercent = (float)currentHp / maxHp;
            if (hpPercent <= 0.3f && !isBerserk)
            {
                isBerserk = true;
                atk = Mathf.RoundToInt(baseAtk * 1.5f);
            }
            else if (hpPercent > 0.3f && isBerserk)
            {
                isBerserk = false;
                atk = baseAtk;
            }
        }
    }

    void UpdateAuraEffects()
    {
        if (skillEffectType == SkillEffectType.AURA_GUARD || 
            skillEffectType == SkillEffectType.AURA_DEFENSE)
        {
            List<UnitController> allies = GetAllies();
            foreach (UnitController ally in allies)
            {
                float dist = battleGrid.GetDistance(GetCenterPosition(), ally.GetCenterPosition());
                if (dist <= 2)
                {
                    float reduction = skillEffectType == SkillEffectType.AURA_GUARD ? 0.15f : 0.1f;
                }
            }
        }
    }

    public float GetDamageReductionFromAuras()
    {
        float reduction = 0f;
        List<UnitController> allies = GetAllies();
        
        foreach (UnitController ally in allies)
        {
            if (ally == this) continue;
            float dist = battleGrid.GetDistance(GetCenterPosition(), ally.GetCenterPosition());
            if (dist <= 2)
            {
                if (ally.skillEffectType == SkillEffectType.AURA_GUARD)
                    reduction += 0.15f;
                if (ally.skillEffectType == SkillEffectType.AURA_DEFENSE)
                    reduction += 0.1f;
            }
        }
        
        return Mathf.Min(reduction, 0.5f);
    }

    List<UnitController> GetAllies()
    {
        List<UnitController> allies = new List<UnitController>();
        UnitController[] allUnits = FindObjectsOfType<UnitController>();
        
        foreach (UnitController unit in allUnits)
        {
            if (unit.side == side && unit.state != UnitState.DEAD)
            {
                allies.Add(unit);
            }
        }
        
        return allies;
    }

    Vector2Int GetCenterPosition()
    {
        int centerX = position.x + (isHorizontal ? size / 2 : 0);
        int centerY = position.y + (isHorizontal ? 0 : size / 2);
        return new Vector2Int(centerX, centerY);
    }

    UnitController FindTarget()
    {
        UnitController[] allUnits = FindObjectsOfType<UnitController>();
        UnitController nearestTarget = null;
        float nearestDistance = float.MaxValue;

        foreach (UnitController unit in allUnits)
        {
            if (unit.side == side || unit.state == UnitState.DEAD)
                continue;

            float distance = battleGrid.GetDistance(GetCenterPosition(), unit.GetCenterPosition());
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = unit;
            }
        }

        return nearestTarget;
    }

    void MoveTowards(UnitController target)
    {
        if (Time.time - lastMoveTime < 1.0f / speed)
            return;
        
        state = UnitState.MOVING;
        lastMoveTime = Time.time;
        
        Vector2Int targetPos = target.GetCenterPosition();
        Vector2Int currentPos = GetCenterPosition();
        
        Vector2Int ferry = GetNearestFerry();
        bool needsCrossing = !IsSameSide(target);
        
        Vector2Int goalPos = needsCrossing ? 
            new Vector2Int(ferry.x, side == UnitSide.PLAYER ? battleGrid.riverRow - 1 : battleGrid.riverRow + 1) : 
            targetPos;

        Vector2Int nextMove = GetNextMove(currentPos, goalPos);
        
        if (nextMove != currentPos && battleGrid.CanMoveTo(nextMove.x, nextMove.y))
        {
            MoveUnit(nextMove);
        }
    }

    bool IsSameSide(UnitController other)
    {
        bool mySide = GetCenterPosition().y > battleGrid.riverRow;
        bool otherSide = other.GetCenterPosition().y > battleGrid.riverRow;
        return mySide == otherSide;
    }

    Vector2Int GetNearestFerry()
    {
        Vector2Int leftFerry = battleGrid.GetFerryPosition(true);
        Vector2Int rightFerry = battleGrid.GetFerryPosition(false);
        Vector2Int current = GetCenterPosition();
        
        int leftDist = battleGrid.GetDistance(current, leftFerry);
        int rightDist = battleGrid.GetDistance(current, rightFerry);
        
        return leftDist <= rightDist ? leftFerry : rightFerry;
    }

    Vector2Int GetNextMove(Vector2Int from, Vector2Int to)
    {
        int dx = Mathf.Clamp(to.x - from.x, -1, 1);
        int dy = Mathf.Clamp(to.y - from.y, -1, 1);
        
        if (dx != 0 && dy != 0)
        {
            if (Random.value > 0.5f)
                return new Vector2Int(from.x + dx, from.y);
            else
                return new Vector2Int(from.x, from.y + dy);
        }
        
        return new Vector2Int(from.x + dx, from.y + dy);
    }

    void MoveUnit(Vector2Int newPos)
    {
        battleGrid.RemoveUnit(unitId);
        
        position = newPos;
        occupiedCells = battleGrid.GetUnitCells(position.x, position.y, size, isHorizontal);
        
        battleGrid.PlaceUnit(unitId, position.x, position.y, size, isHorizontal, side == UnitSide.PLAYER);
        
        transform.position = new Vector3(position.x * 1.0f, position.y * 1.0f, 0);
        
        if (animator != null)
        {
            animator.SetTrigger("Move");
        }
        
        if (skillEffectType == SkillEffectType.MOVE_AND_SHOOT ||
            skillEffectType == SkillEffectType.CAVALRY_ARCHER)
        {
            UnitController target = FindTarget();
            if (target != null && battleGrid.GetDistance(GetCenterPosition(), target.GetCenterPosition()) <= range)
            {
                Attack(target);
            }
        }
    }

    void Attack(UnitController target)
    {
        state = UnitState.ATTACKING;
        lastAttackTime = Time.time;
        attackCount++;
        
        float counterMultiplier = CalculateCounterMultiplier(target);
        float skillBonus = CalculateSkillBonus(target);
        float starBonus = GameManager.instance.GetStarBonus(star);
        float randomFactor = Random.Range(0.9f, 1.1f);
        float riverBonus = GetRiverBonus();
        float formationBonus = GetFormationBonus();
        float eliteBonus = GetEliteBonus();
        
        int effectiveArmor = target.CalculateEffectiveArmor(this);
        float damageReduction = target.GetDamageReductionFromAuras();
        
        float baseDamage = atk * counterMultiplier * skillBonus * starBonus * randomFactor * riverBonus * formationBonus * eliteBonus;
        int damage = Mathf.Max(1, Mathf.RoundToInt((baseDamage - effectiveArmor) * (1f - damageReduction)));
        
        target.TakeDamage(damage);
        
        ApplyOnHitEffects(target);
        
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        
        if (skillEffectType == SkillEffectType.SPLASH || 
            skillEffectType == SkillEffectType.SHIP_RAM)
        {
            ApplySplashDamage(target, damage * 0.5f);
        }
        
        if (skillEffectType == SkillEffectType.SPLASH_HEAVY ||
            skillEffectType == SkillEffectType.TRAMPLE)
        {
            ApplySplashDamage(target, damage * 0.6f);
        }
        
        if (skillEffectType == SkillEffectType.BURN)
        {
            target.ApplyBurn(Mathf.RoundToInt(atk * 0.2f), 3);
        }
        
        if (skillEffectType == SkillEffectType.DOUBLE_SHOT)
        {
            if (Random.value > 0.5f)
            {
                target.TakeDamage(Mathf.RoundToInt(damage * 0.7f));
            }
        }
    }

    float CalculateCounterMultiplier(UnitController target)
    {
        TroopData myData = GameManager.instance.GetTroopData(troopDataId);
        TroopData targetData = GameManager.instance.GetTroopData(target.troopDataId);
        
        if (myData == null || targetData == null)
            return 1.0f;
        
        foreach (string counter in myData.counterBonus)
        {
            if (target.troopDataId.Contains(counter))
            {
                return myData.counterMultiplier;
            }
        }
        
        return 1.0f;
    }

    float CalculateSkillBonus(UnitController target)
    {
        float bonus = 1.0f;
        
        switch (skillEffectType)
        {
            case SkillEffectType.FIRST_STRIKE:
                if (!hasUsedFirstStrike)
                {
                    hasUsedFirstStrike = true;
                    bonus = 1.5f;
                }
                break;
            case SkillEffectType.CHARGE:
                if (!hasUsedFirstStrike)
                {
                    hasUsedFirstStrike = true;
                    bonus = 1.5f;
                }
                break;
            case SkillEffectType.CHARGE_HEAVY:
                if (!hasUsedFirstStrike)
                {
                    hasUsedFirstStrike = true;
                    bonus = 2.0f;
                }
                break;
            case SkillEffectType.CHARGE_ULTIMATE:
                if (!hasUsedFirstStrike)
                {
                    hasUsedFirstStrike = true;
                    bonus = 2.0f;
                }
                break;
            case SkillEffectType.CHARIOT_CHARGE:
                if (!hasUsedFirstStrike)
                {
                    hasUsedFirstStrike = true;
                    bonus = 1.8f;
                }
                break;
            case SkillEffectType.XIAO_CHARGE:
                if (!hasUsedFirstStrike)
                {
                    hasUsedFirstStrike = true;
                    bonus = 1.8f;
                }
                break;
            case SkillEffectType.MONGOL_CHARGE:
                if (!hasUsedFirstStrike)
                {
                    hasUsedFirstStrike = true;
                    bonus = 2.0f;
                }
                break;
            case SkillEffectType.ARMOR_PIERCE:
                if (attackCount % 3 == 0)
                {
                    bonus = 1.5f;
                }
                break;
            case SkillEffectType.ARMOR_IGNORE:
                bonus = 1.2f;
                break;
            case SkillEffectType.MUSKET_VOLLEY:
                if (attackCount % 3 == 0)
                {
                    bonus = 1.8f;
                }
                break;
            case SkillEffectType.VOLLEY:
                if (attackCount % 3 == 0)
                {
                    bonus = 1.8f;
                }
                break;
            case SkillEffectType.VOLLEY_HEAVY:
                if (attackCount % 2 == 0)
                {
                    bonus = 1.5f;
                }
                break;
            case SkillEffectType.CAVALRY_COUNTER:
                if (target.IsCavalry())
                {
                    bonus = 1.4f;
                }
                break;
            case SkillEffectType.MO_DAO:
                if (target.IsCavalry())
                {
                    bonus = 1.35f;
                }
                break;
            case SkillEffectType.HAN_CAVALRY:
                if (target.troopDataId.Contains("light_cavalry"))
                {
                    bonus = 1.25f;
                }
                break;
            case SkillEffectType.CRITICAL_STRIKE:
                if (Random.value < 0.3f)
                {
                    bonus = 2.0f;
                }
                break;
            case SkillEffectType.ASSASSINATE:
                if (target.IsRanged() || target.troopDataId.Contains("scholar"))
                {
                    bonus = 1.35f;
                }
                if (Random.value < 0.25f)
                {
                    bonus *= 1.5f;
                }
                break;
            case SkillEffectType.PIERCE_HEAVY:
                bonus = 1.2f;
                break;
            case SkillEffectType.TRAMPLE:
                bonus = 1.15f;
                break;
        }
        
        return bonus;
    }

    public int CalculateEffectiveArmor(UnitController attacker)
    {
        int effectiveArmor = baseArmor;
        
        if (attacker.skillEffectType == SkillEffectType.ARMOR_IGNORE)
        {
            effectiveArmor = Mathf.RoundToInt(effectiveArmor * 0.7f);
        }
        
        if (attacker.skillEffectType == SkillEffectType.ARMOR_PIERCE && attacker.attackCount % 3 == 0)
        {
            effectiveArmor = Mathf.RoundToInt(effectiveArmor * 0.5f);
        }
        
        if (attacker.skillEffectType == SkillEffectType.PIERCE_HEAVY)
        {
            effectiveArmor = Mathf.RoundToInt(effectiveArmor * 0.6f);
        }
        
        if (skillEffectType == SkillEffectType.VINE_ARMOR)
        {
            if (attacker.skillEffectType == SkillEffectType.BURN || attacker.troopDataId.Contains("fire"))
            {
                effectiveArmor = 0;
            }
        }
        
        if (attacker.skillEffectType == SkillEffectType.MONGOL_CHARGE && !attacker.hasUsedFirstStrike)
        {
            effectiveArmor = Mathf.RoundToInt(effectiveArmor * 0.8f);
        }
        
        if (attacker.skillEffectType == SkillEffectType.MO_DAO)
        {
            effectiveArmor = Mathf.RoundToInt(effectiveArmor * 0.8f);
        }
        
        return effectiveArmor;
    }

    float GetRiverBonus()
    {
        if (skillEffectType == SkillEffectType.RIVER_BONUS)
        {
            int distToRiver = Mathf.Abs(GetCenterPosition().y - battleGrid.riverRow);
            if (distToRiver <= 2)
            {
                return 1.25f;
            }
        }
        if (skillEffectType == SkillEffectType.RIVER_MASTER)
        {
            int distToRiver = Mathf.Abs(GetCenterPosition().y - battleGrid.riverRow);
            if (distToRiver <= 2)
            {
                return 1.3f;
            }
        }
        return 1.0f;
    }

    float GetFormationBonus()
    {
        return 1.0f;
    }

    float GetEliteBonus()
    {
        float bonus = 1.0f;
        List<UnitController> allies = GetAllies();
        
        foreach (UnitController ally in allies)
        {
            if (ally.skillEffectType == SkillEffectType.ELITE_ALL)
            {
                bonus *= 1.2f;
            }
            if (ally.skillEffectType == SkillEffectType.ELITE_ULTIMATE && ally == this)
            {
                bonus *= 1.3f;
            }
        }
        
        return bonus;
    }

    void ApplyOnHitEffects(UnitController target)
    {
        if (skillEffectType == SkillEffectType.CHARGE_HEAVY ||
            skillEffectType == SkillEffectType.CHARGE_ULTIMATE)
        {
        }
    }

    void ApplySplashDamage(UnitController primaryTarget, float splashDamage)
    {
        UnitController[] allUnits = FindObjectsOfType<UnitController>();
        
        foreach (UnitController unit in allUnits)
        {
            if (unit.side == side || unit.state == UnitState.DEAD || unit == primaryTarget)
                continue;
            
            float dist = battleGrid.GetDistance(primaryTarget.GetCenterPosition(), unit.GetCenterPosition());
            if (dist <= 1.5f)
            {
                unit.TakeDamage(Mathf.RoundToInt(splashDamage));
            }
        }
    }

    public void ApplyBurn(int damage, int turns)
    {
        burnDamage = Mathf.Max(burnDamage, damage);
        burnTurns = Mathf.Max(burnTurns, turns);
    }

    public void TakeDamage(int damage, bool isTrueDamage = false)
    {
        if (state == UnitState.DEAD)
            return;
        
        int actualDamage = damage;
        
        // 藤甲兵对远程攻击有减伤效果
        if (skillEffectType == SkillEffectType.VINE_ARMOR && !isTrueDamage)
        {
            // 假设攻击来自远程单位时，伤害减半
            actualDamage = Mathf.RoundToInt(damage * 0.5f);
        }
        
        // 玄甲冲锋时自身有减伤
        if (skillEffectType == SkillEffectType.CHARGE_ULTIMATE && !hasUsedFirstStrike)
        {
            actualDamage = Mathf.RoundToInt(actualDamage * 0.8f);
        }
        
        currentHp -= actualDamage;
        
        if (currentHp <= 0)
        {
            Die();
        }
        else
        {
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }
        }
    }

    void Die()
    {
        state = UnitState.DEAD;
        battleGrid.RemoveUnit(unitId);
        
        if (skillEffectType == SkillEffectType.DEATH_COUNTER)
        {
            UnitController target = FindTarget();
            if (target != null)
            {
                target.TakeDamage(Mathf.RoundToInt(atk * 0.5f));
            }
        }
        
        if (skillEffectType == SkillEffectType.LAST_STAND)
        {
        }
        
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayDamageSound();
        }
        
        Destroy(gameObject, 1.0f);
        
        if (BattleManager.instance != null)
        {
            BattleManager.instance.OnUnitDeath(this);
        }
    }

    public float GetHealthPercentage()
    {
        return (float)currentHp / maxHp;
    }

    public bool IsRanged()
    {
        return range > 1.5f;
    }

    public bool IsCavalry()
    {
        return troopDataId.Contains("cavalry") || troopDataId.Contains("xuanjia") || 
               troopDataId.Contains("mongol") || troopDataId.Contains("xiliang") ||
               troopDataId.Contains("bai_mayi") || troopDataId.Contains("sui") ||
               troopDataId.Contains("yuan_heavy");
    }

    public bool IsArtillery()
    {
        return troopDataId.Contains("catapult") || troopDataId.Contains("cannon") || 
               troopDataId.Contains("tiger") || troopDataId.Contains("shenji");
    }

    public bool IsHeavyArmor()
    {
        return armor >= 15;
    }
    
    public bool IsLargeUnit()
    {
        return size >= 2;
    }
}
