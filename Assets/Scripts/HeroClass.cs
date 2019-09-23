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
    public int XP;
    [Space]
    public int SP;
    [Header("Special Stats")]
    public int Special;
    [Space]
    public string SpecialAbility;
    [Space]
    public string PassiveAbility;

    public TextMeshProUGUI gameLog;

    public void BasicAttack(Enemy target, CombatManager combatManager, List<Enemy> characterParty, bool isHealing = false)
    {
        int damage = 0;
        switch (isHealing)
        {
            case true:
                target.Health += Attack;

                gameLog.text += ("\n" + this.name + " healed " + target.name + " for " + Attack.ToString() + " health.");
                break;

            case false:
                damage = Attack - target.Defense;

                if (damage > 0)
                    target.Health -= damage;
                if(gameLog != null)
                    gameLog.text += ("\n" + this.name + " attacked " + target.name + " for " + damage.ToString() + " damage.");
                break;
        }

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
    }

    public void BasicHeal(HeroClass target)
    {
        target.Health += Attack;

        gameLog.text += ("\n" + this.name + " healed " + target.name + " for " + Attack.ToString() + " health.");

        gameLog.text += ("\n" + "Their health is now: " + target.Health);

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
        target.Attack = template.Attack;
        target.Defense = template.Defense;
        target.Speed = template.Speed;

        target.XP = template.XP;
        target.SP = template.SP;
        target.Special = template.Special;

        target.SpecialAbility = template.SpecialAbility;
        target.PassiveAbility = template.PassiveAbility;
    }
}


