using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[System.Serializable]
public class Enemy : Character
{
    public TextMeshProUGUI gameLog;

    public int xpReward;

    public int goldRewardMin;
    public int goldRewardMax;

    public bool isBoss;

    public GameObject hitSprite;
    
    public void BasicAttack(HeroClass target, CombatManager combatManager, List<HeroClass> characterParty, bool isHealing = false)
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

                if (damage < 0)
                    damage = 1;

                target.currentHealth -= damage;

                gameLog.text += ("\n" + this.name + " attacked " + target.name + " for " + damage.ToString() + " damage.");

                Transform animationTransform = GameObject.FindGameObjectWithTag("Player").transform;
                SpawnAnimation(animationTransform);

                break;
        }

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
    }

    public void BasicHeal(Enemy target)
    {
        target.currentHealth += Attack;

        gameLog.text += ("\n" + this.name + " healed " + target.name + " for " + Attack.ToString() + " health.");

        gameLog.text += ("\n" + "Their health is now: " + target.currentHealth);

    }

    void SpawnAnimation(Transform target)
    {
        GameObject newAttack = Instantiate(hitSprite, target);
        Destroy(newAttack, 1.5f);
    }

    public void Die(CombatManager combatManager, List<Enemy> characterParty)
    {
        if (combatManager != null)
            combatManager.RemoveCharacterFromCombat(this, characterParty);

        Destroy(this.gameObject);
    }


    public void CopyEnemy(Enemy target, Enemy template)
    {
        target.name = template.name;
        target.Health = template.Health;
        target.currentHealth = template.currentHealth;
        target.Attack = template.Attack;
        target.Defense = template.Defense;
        target.Speed = template.Speed;

        target.hitSprite = template.hitSprite;

        target.isBoss = template.isBoss;
    }
}
