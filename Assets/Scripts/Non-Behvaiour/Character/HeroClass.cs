using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;


[RequireComponent(typeof(SpriteRenderer))]
[System.Serializable]
public class HeroClass : Character
{
    public HeroRole role;

    public GameObject[] attackAnimations;
    public GameObject healingAnimation;

    [Header("Hero Stats")]
    [Space]
    public int XP;
    [Space]
    public int maxSP;
    [Space]
    public int currentSP;
    [Header("Special Stats")]
    public int Skill;
    [Space]
    public string SpecialAbility;
    [Space]
    public string PassiveAbility;
    [Space]
    public string PrimaryStat;
    [Space]
    public int goldCost;

    public bool isDead = false;
    private int xpNeededToLevelUp;

    [Header("Inventory")]
    public Weapon weaponSlot;
    bool wSlotFull;
    public Armor armorSlot;
    bool arSlotFull;
    public Accessory accessorySlot;
    bool acSlotFull;

    public void SetID()
    {
        this.uniqueID = Mathf.RoundToInt(Time.time);
    }

    public TextMeshProUGUI gameLog;

    
    public void BasicAttack(Enemy target, CombatManager combatManager, List<Enemy> characterParty)
    {
        int damage = 0;

        if (currentSP < maxSP)
        {
            damage = Attack - target.Defense;
        }
        else
        {
            UseSpecialAbility(target, combatManager, characterParty);
        }

        if (damage <= 0)
            damage = 1;

        target.currentHealth -= damage;

        Transform animationTransform = GameObject.FindGameObjectWithTag("Enemy").transform;
        SpawnAnimation(animationTransform);

        if (gameLog != null)
            gameLog.text = (this.name + " attacked " + target.name + " for " + damage.ToString() + " damage.");

        if (target.currentHealth <= 0)
        {
            if (combatManager != null)
                target.Die(combatManager, characterParty);

            gameLog.text += ("\n" + target.name + " has died.");
        }
        else
        {
            gameLog.text += ("\n" + "Their health is now: " + target.currentHealth);
        }

        currentSP += 1;
    }

    public void BasicHeal(HeroClass target)
    {
        target.currentHealth += Attack;

        target.currentHealth = Mathf.Clamp(target.currentHealth, 0, target.Health);

        Transform animationTransform = GameObject.FindGameObjectWithTag("Player").transform;
        SpawnAnimation(animationTransform, true);

        gameLog.text = (this.name + " healed " + target.name + " for " + Attack.ToString() + " health.");

        gameLog.text += ("\n" + "Their health is now: " + target.currentHealth);

        currentSP += 1;
    }

    private void UseSpecialAbility(Enemy target, CombatManager combatManager, List<Enemy> characterParty)
    {
        int damage = 0;

        switch (SpecialAbility)
        {
            case "Critical Strike":

                damage = (Attack * (1 + Skill)) - target.Defense;
                break;
        }

        if (damage > 0)
            target.currentHealth -= damage;

        if (gameLog != null)
            gameLog.text = (this.name + " attacked " + target.name + " for " + damage.ToString() + " damage.");

        if (target.Health <= 0)
        {
            if (combatManager != null)
                target.Die(combatManager, characterParty);

            gameLog.text += ("\n" + target.name + " has died.");
        }
        else
        {
            gameLog.text += ("\n" + "Their health is now: " + target.Health);
        }

        currentSP = 0;
    }

    public void ModifyStatsFromItems(Item item = null)
    {
        if (item != null)
        {
            if (item.GetType() == typeof(Armor))
            {
                if (arSlotFull)
                {
                    switch (armorSlot.statToBuff)
                    {
                        case "Health":
                            this.Health -= armorSlot.buffValue;
                            break;

                        case "Attack":
                            this.Attack -= armorSlot.buffValue;
                            break;

                        case "Defense":
                            this.Defense -= armorSlot.buffValue;
                            break;

                        case "Speed":
                            this.Speed += armorSlot.buffValue;
                            break;

                        case "Special":
                            this.Skill -= armorSlot.buffValue;
                            break;
                    }
                    armorSlot = null;
                    arSlotFull = false;

                    Destroy(armorSlot.gameObject);
                }

                if (!arSlotFull)
                {
                    switch (item.statToBuff)
                    {
                        case "Health":
                            this.Health += item.buffValue;
                            break;

                        case "Attack":
                            this.Attack += item.buffValue;
                            break;

                        case "Defense":
                            this.Defense += item.buffValue;
                            break;

                        case "Speed":
                            this.Speed += item.buffValue;
                            break;

                        case "Special":
                            this.Skill += item.buffValue;
                            break;
                    }

                    armorSlot = item as Armor;
                    arSlotFull = true;

                    Instantiate(armorSlot.gameObject, this.transform);
                }
            }
            else if (item.GetType() == typeof(Accessory))
            {
                if (acSlotFull)
                {
                    switch (accessorySlot.statToBuff)
                    {
                        case "Health":
                            this.Health -= accessorySlot.buffValue;
                            break;

                        case "Attack":
                            this.Attack -= accessorySlot.buffValue;
                            break;

                        case "Defense":
                            this.Defense -= accessorySlot.buffValue;
                            break;

                        case "Speed":
                            this.Speed += accessorySlot.buffValue;
                            break;

                        case "Special":
                            this.Skill -= accessorySlot.buffValue;
                            break;
                    }

                    accessorySlot = null;
                    acSlotFull = false;

                    Destroy(accessorySlot.gameObject);
                }

                if (!acSlotFull)
                {
                    switch (item.statToBuff)
                    {
                        case "Health":
                            this.Health += item.buffValue;
                            break;

                        case "Attack":
                            this.Attack += item.buffValue;
                            break;

                        case "Defense":
                            this.Defense += item.buffValue;
                            break;

                        case "Speed":
                            this.Speed += item.buffValue;
                            break;

                        case "Special":
                            this.Skill += item.buffValue;
                            break;
                    }

                    accessorySlot = item as Accessory;
                    acSlotFull = true;

                    Instantiate(accessorySlot.gameObject, this.transform);
                }
            }
            else if (item.GetType() == typeof(Weapon))
            {
                if (wSlotFull)
                {
                    switch (weaponSlot.statToBuff)
                    {
                        case "Health":
                            this.Health -= weaponSlot.buffValue;
                            break;

                        case "Attack":
                            this.Attack -= weaponSlot.buffValue;
                            break;

                        case "Defense":
                            this.Defense -= weaponSlot.buffValue;
                            break;

                        case "Speed":
                            this.Speed += weaponSlot.buffValue;
                            break;

                        case "Special":
                            this.Skill -= weaponSlot.buffValue;
                            break;
                    }

                    weaponSlot = null;
                    wSlotFull = false;

                    Destroy(weaponSlot.gameObject);
                }

                if (!wSlotFull)
                {
                    switch (item.statToBuff)
                    {
                        case "Health":
                            this.Health += item.buffValue;
                            break;

                        case "Attack":
                            this.Attack += item.buffValue;
                            break;

                        case "Defense":
                            this.Defense += item.buffValue;
                            break;

                        case "Speed":
                            this.Speed += item.buffValue;
                            break;

                        case "Special":
                            this.Skill += item.buffValue;
                            break;
                    }
                    
                    weaponSlot = item as Weapon;
                    wSlotFull = true;

                    Instantiate(weaponSlot.gameObject, this.transform);
                }
            }
        }
        else
        {
            Debug.LogError("No Item To Assign!");
        }
    }

    void SpawnAnimation(Transform target, bool isHealing = false)
    {
        GameObject newAttack = null;

        if (!isHealing)
        {
            switch (this.role)
            {
                case HeroRole.Arcanist:
                    newAttack = Instantiate(attackAnimations[0], target) as GameObject;
                    Destroy(newAttack, 1.5f);
                    break;

                case HeroRole.Fencer:
                    newAttack = Instantiate(attackAnimations[1], target) as GameObject;
                    Destroy(newAttack, 1.5f);
                    break;

                case HeroRole.Guardian:
                    newAttack = Instantiate(attackAnimations[2], target) as GameObject;
                    Destroy(newAttack, 1.5f);
                    break;

                case HeroRole.Mender:
                    newAttack = Instantiate(attackAnimations[3], target) as GameObject;
                    Destroy(newAttack, 1.5f);
                    break;

                case HeroRole.Knight:
                    newAttack = Instantiate(attackAnimations[4], target) as GameObject;
                    Destroy(newAttack, 1.5f);
                    break;
            }
        }
        else
        {
            newAttack = Instantiate(healingAnimation, target) as GameObject;
            Destroy(newAttack, 1.5f);
        }
    }

    public void Die(CombatManager combatManager, List<HeroClass> characterParty)
    {
        if (combatManager != null) { }
            combatManager.RemoveCharacterFromCombat(this, null, characterParty);

        Destroy(this.gameObject);
    }

    public void LevelUp(int newLevel)
    {
        int timesToLevel = newLevel - this.Level;

        for (int i = 0; i < timesToLevel; i++)
        {
            this.Level++;

            Health += 3;
            currentHealth += 3;

            Attack += 2;
            Defense += 2;
            Speed += 2;
            Skill += 1;

            if (this.role == HeroRole.Arcanist)
            {
                switch (this.Level)
                {
                    case 1:
                    case 2:
                        maxSP = 7;
                        break;
                    case 3:
                    case 4:
                        maxSP = 6;
                        break;
                    case 5:
                    case 6:
                        maxSP = 5;
                        break;
                    case 7:
                    case 8:
                        maxSP = 4;
                        break;
                    case 9:
                    case 10:
                        maxSP = 3;
                        break;
                }
            }

            switch (this.PrimaryStat)
            {
                case "Health":
                    Health += 1;
                    currentHealth += 1;
                    break;

                case "Attack":
                    Attack += 1;
                    break;

                case "Defence":
                    Defense += 1;
                    break;

                case "Speed":
                    Speed += 1;
                    break;

                case "Skill":
                    Skill += 1;
                    break;

                default:
                    break;
            }

            Skill = Mathf.Clamp(Skill, 1, 10);
        }

        XP = 0;
        xpNeededToLevelUp = 7 * Level;
    }

    public void CopyHero(HeroClass target, HeroClass template)
    {
        target.uniqueID = template.uniqueID;

        target.name = template.name;
        target.sprite = template.sprite;

        target.Health = template.Health;
        target.currentHealth = template.currentHealth;
        target.Attack = template.Attack;
        target.Defense = template.Defense;
        target.Speed = template.Speed;

        target.Level = template.Level;
        target.XP = template.XP;
        target.maxSP = template.maxSP;
        target.currentSP = template.currentSP;
        target.Skill = template.Skill;
        target.role = template.role;

        target.SpecialAbility = template.SpecialAbility;
        target.PassiveAbility = template.PassiveAbility;
        target.PrimaryStat = template.PrimaryStat;

        target.weaponSlot = template.weaponSlot;
        target.armorSlot = template.armorSlot;
        target.accessorySlot = template.accessorySlot;

        target.attackAnimations = template.attackAnimations;
        target.healingAnimation = template.healingAnimation;

        target.isDead = template.isDead;
    }
}


