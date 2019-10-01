using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[System.Serializable]
public class HeroClass : Character
{
    public HeroRole role;

    [Header("Hero Stats")]
    public int heroLevel;
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
    public Armor armorSlot;
    public Accessory accessorySlot;

    public TextMeshProUGUI gameLog;


    public void AddStatsFromItem (Item item)
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
    }


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

    public void Die(CombatManager combatManager, List<HeroClass> characterParty)
    {
        if (combatManager != null) { }
            combatManager.RemoveCharacterFromCombat(this, null, characterParty);

        Destroy(this.gameObject);
    }



    public void CopyHero(HeroClass target, HeroClass template)
    {
        target.name = template.name;
        target.Health = template.Health;
        target.currentHealth = template.currentHealth;
        target.Attack = template.Attack;
        target.Defense = template.Defense;
        target.Speed = template.Speed;

        target.XP = template.XP;
        target.SP = template.SP;
        target.Special = template.Special;

        target.SpecialAbility = template.SpecialAbility;
        target.PassiveAbility = template.PassiveAbility;

        target.weaponSlot = template.weaponSlot;
        target.armorSlot = template.armorSlot;
        target.accessorySlot = template.accessorySlot;
    }
}


