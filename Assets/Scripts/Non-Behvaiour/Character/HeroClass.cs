using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[System.Serializable]
public class HeroClass : Character
{
    public HeroRole role;

    public GameObject[] attackAnimations;

    [Header("Hero Stats")]
    [Space]
    public int XP;
    [Space]
    public int SP;
    [Space]
    public int spChargeRate = 1;
    [Header("Special Stats")]
    public int Special;
    [Space]
    public string SpecialAbility;
    [Space]
    public string PassiveAbility;
    [Space]
    public string PrimaryStat;
    [Space]
    public int goldCost;

    [Header("Inventory")]
    public Weapon weaponSlot;
    bool wSlotFull;
    public Armor armorSlot;
    bool arSlotFull;
    public Accessory accessorySlot;
    bool acSlotFull;

    public TextMeshProUGUI gameLog;

    
    public void BasicAttack(Enemy target, CombatManager combatManager, List<Enemy> characterParty)
    {
        int damage = 0;

        if (SP < 20)
        {
            damage = Attack - target.Defense;
        }
        else
        {
            UseSpecialAbility(target, combatManager, characterParty);
        }

        if (damage < 0)
            damage = 1;

        target.currentHealth -= damage;

        Transform animationTransform = GameObject.FindGameObjectWithTag("Enemy").transform;
        SpawnAnimation(animationTransform);

        if (gameLog != null)
            gameLog.text += ("\n" + this.name + " attacked " + target.name + " for " + damage.ToString() + " damage.");

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

        SP += spChargeRate;
    }

    public void BasicHeal(HeroClass target)
    {
        target.currentHealth += Attack;

        gameLog.text += ("\n" + this.name + " healed " + target.name + " for " + Attack.ToString() + " health.");

        gameLog.text += ("\n" + "Their health is now: " + target.currentHealth);

        SP += spChargeRate;
    }

    private void UseSpecialAbility(Enemy target, CombatManager combatManager, List<Enemy> characterParty)
    {
        int damage = 0;

        switch (SpecialAbility)
        {
            case "Critical Strike":
                
                damage = (Attack * (1 + SP)) - target.Defense;
                break;
        }

        if (damage > 0)
            target.currentHealth -= damage;

        if (gameLog != null)
            gameLog.text += ("\n" + this.name + " attacked " + target.name + " for " + damage.ToString() + " damage.");

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

        SP = 0;
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
                            this.Special -= armorSlot.buffValue;
                            break;

                        case "SP Charge Rate":
                            this.spChargeRate -= armorSlot.buffValue;
                            break;
                    }
                    armorSlot = null;
                    arSlotFull = false;
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
                            this.Special += item.buffValue;
                            break;

                        case "SP Charge Rate":
                            this.spChargeRate += item.buffValue;
                            break;
                    }

                    armorSlot = item as Armor;
                    arSlotFull = true;
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
                            this.Special -= accessorySlot.buffValue;
                            break;

                        case "SP Charge Rate":
                            this.spChargeRate -= accessorySlot.buffValue;
                            break;
                    }

                    accessorySlot = null;
                    acSlotFull = false;
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
                            this.Special += item.buffValue;
                            break;

                        case "SP Charge Rate":
                            this.spChargeRate += item.buffValue;
                            break;
                    }

                    accessorySlot = item as Accessory;
                    acSlotFull = true;
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
                            this.Special -= weaponSlot.buffValue;
                            break;

                        case "SP Charge Rate":
                            this.spChargeRate -= weaponSlot.buffValue;
                            break;
                    }

                    weaponSlot = null;
                    wSlotFull = false;
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
                            this.Special += item.buffValue;
                            break;

                        case "SP Charge Rate":
                            this.spChargeRate += item.buffValue;
                            break;
                    }
                    
                    weaponSlot = item as Weapon;
                    wSlotFull = true;
                }
            }
        }
        else
        {
            Debug.LogError("No Item To Assign!");
        }
    }

    void SpawnAnimation(Transform target)
    {
        GameObject newAttack = null;

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

            switch (this.Level)
            {
                case 2:
                case 6:
                case 10:
                case 14:
                case 18:
                    Health++;
                    Attack++;

                    if(this.PrimaryStat == "Attack")
                        Attack++;
                    break;

                case 3:
                case 7:
                case 11:
                case 15:
                case 19:
                    Health++;
                    Defense++;

                    if (this.PrimaryStat == "Defense")
                        Defense++;
                    break;

                case 4:
                case 8:
                case 12:
                case 16:
                case 20:
                    Health++;
                    Speed++;

                    if (this.PrimaryStat == "Speed")
                        Speed++;
                    break;

                case 5:
                case 9:
                case 13:
                case 17:
                    Health++;
                    Special++;

                    if (this.PrimaryStat == "Special")
                        Special++;
                    break;
            }
        }

        if(this.role == HeroRole.Arcanist)
        {
            this.spChargeRate = Special;
        }
    }

    public void CopyHero(HeroClass target, HeroClass template)
    {
        target.name = template.name;
        target.Health = template.Health;
        target.currentHealth = template.currentHealth;
        target.Attack = template.Attack;
        target.Defense = template.Defense;
        target.Speed = template.Speed;

        target.Level = template.Level;
        target.XP = template.XP;
        target.SP = template.SP;
        target.Special = template.Special;
        target.role = template.role;

        target.SpecialAbility = template.SpecialAbility;
        target.PassiveAbility = template.PassiveAbility;
        target.PrimaryStat = template.PrimaryStat;

        target.weaponSlot = template.weaponSlot;
        target.armorSlot = template.armorSlot;
        target.accessorySlot = template.accessorySlot;

        target.attackAnimations = template.attackAnimations;
    }
}


