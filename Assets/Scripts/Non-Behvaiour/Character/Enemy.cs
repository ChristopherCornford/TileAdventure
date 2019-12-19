using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[System.Serializable]
public class Enemy : Character
{
    public int combatTier;
    public int creatureSize;

    public int CombatSize { get { return ((combatTier - 1) + creatureSize); } }

    public int xpReward;

    public int goldRewardMin;
    public int goldRewardMax;

    public int goldToReward;

    public bool isBoss;

    public GameObject hitSprite;

    public void BasicAttack(HeroClass target, CombatManager combatManager, List<HeroClass> characterParty, bool isHealing = false)
    {
        int damage = 0;
        switch (isHealing)
        {
            case true:
                target.Health += Attack;

                gameLog.text = (this.name + " healed " + target.name + " for " + Attack.ToString() + " health.");
                break;

            case false:

                if (forcedTarget != null && forcedTarget != target)
                {
                    target = forcedTarget as HeroClass;
                    forcedTarget = null;
                }
                if (forcedTarget != null && forcedTarget == target)
                {
                    forcedTarget = null;
                }
                
                damage = Attack - target.Defense;

                if (damage <= 0)
                    damage = 1;

                target.currentHealth -= damage;

                gameLog.text = (this.name + " attacked " + target.name + " for " + damage.ToString() + " damage.");

                Transform animationTransform = GameObject.FindGameObjectWithTag("Player").transform.GetChild(target.combatIndex).transform;
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

        target.GetComponent<Animator>().SetTrigger("Hurt");

        Destroy(newAttack, 1.5f);
    }

    public void Die(CombatManager combatManager, List<Enemy> characterParty)
    {
        if (combatManager != null)
        {
            this.goldToReward = Random.Range(goldRewardMin, goldRewardMax + 1);
            
            combatManager.RemoveCharacterFromCombat(this, characterParty);

            this.goldToReward = 0;
        }

        Destroy(this.gameObject);
    }

    public void LevelUp(int newLevel = 0)
    {
        int timesToLevel = newLevel - this.Level;

        if (timesToLevel <= 0) { return; }

        for (int i = 0; i < timesToLevel; i++)
        {
            Health += 2;
            currentHealth += 2;

            Attack += 1;
            Defense += 1;
            Speed += 1;

            xpReward += 1;
            goldRewardMax += 2;
        }
    }

    public void CopyEnemy(Enemy target, Enemy template)
    {
        target.name = template.name;
        target.sprite = template.sprite;

        target.uniqueID = template.uniqueID;

        target.Level = template.Level;

        target.Health = template.Health;
        target.currentHealth = template.currentHealth;
        target.Attack = template.Attack;
        target.Defense = template.Defense;
        target.Speed = template.Speed;

        target.hitSprite = template.hitSprite;

        target.isBoss = template.isBoss;

        target.xpReward = template.xpReward;

        target.goldRewardMax = template.goldRewardMax;
        target.goldRewardMin = template.goldRewardMin;
    }
}
