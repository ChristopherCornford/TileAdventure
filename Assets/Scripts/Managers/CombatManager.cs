using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatManager : MonoBehaviour
{
    public GameManager gameManager;
    public PartyBehaviour partyBehaviour;

    public List<HeroClass> heroParty;
    public List<Enemy> enemyParty;
    
    public List<Character> combatOrder;

    public int goldReward;
    public int xpReward;

    public TextMeshProUGUI gameLog;

    private void Awake()
    {
        gameManager = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
        partyBehaviour = GameObject.FindObjectOfType(typeof(PartyBehaviour)) as PartyBehaviour;
    }

    public void StartCombat()
    {
        heroParty = partyBehaviour.heroParty;
        enemyParty = GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyBehaviour>().enemyClasses;

        gameLog.transform.parent.gameObject.SetActive(true);

        SetInitativeList();
    }

    public void SetInitativeList()
    {
        combatOrder.Clear();

        List<HeroClass> combatHeroCopies = new List<HeroClass>();

        foreach (HeroClass hero in heroParty)
        {
            GameObject nextHero = new GameObject(hero.name, typeof(HeroClass));
            nextHero.transform.SetParent(this.transform);

            HeroClass heroClass = nextHero.GetComponent<HeroClass>();

            heroClass.gameLog = gameLog;

            hero.CopyHero(heroClass, hero);
            combatHeroCopies.Add(heroClass);
        }

        List<Enemy> combatEnemyCopies = new List<Enemy>();

        foreach (Enemy enemy in enemyParty)
        {
            GameObject nextEnemy = new GameObject(enemy.name, typeof(Enemy));
            nextEnemy.transform.SetParent(this.transform);

            Enemy newEnemy = nextEnemy.GetComponent<Enemy>();

            newEnemy.gameLog = gameLog;

            enemy.CopyEnemy(newEnemy, enemy);
            combatEnemyCopies.Add(newEnemy);
        }

        
        List<int> initativeList = new List<int>();

        foreach (HeroClass hero in combatHeroCopies)
            initativeList.Add(hero.Speed);

        foreach (Enemy enemy in combatEnemyCopies)
            initativeList.Add(enemy.Speed);

        initativeList.Sort();
        initativeList.Reverse();

        for(int i = 0; i < initativeList.Count; i++)
        {
            foreach (Enemy enemy in combatEnemyCopies)
                if (enemy.Speed == initativeList[i])
                    if(!combatOrder.Contains(enemy))
                        combatOrder.Insert(i, enemy);

            foreach (HeroClass heroClass in combatHeroCopies)
                if (heroClass.Speed == initativeList[i])
                    if (!combatOrder.Contains(heroClass))
                        combatOrder.Insert(i, heroClass);
        }

        
        StartCoroutine(CharacterActionPhase(combatEnemyCopies, combatHeroCopies));


    }

    public IEnumerator CharacterActionPhase(List<Enemy> combatEnemyCopies, List<HeroClass> combatHeroCopies)
    {
        if (gameManager.currentGamePhase == GameManager.GamePhase.Combat)
        {
            for (int i = 0; i < combatOrder.Count; i++)
            {
                if (gameManager.currentGamePhase == GameManager.GamePhase.Combat)
                {

                    yield return new WaitForSeconds(1.5f);

                    bool isHealer = new bool();

                    if (combatOrder[i].GetType() == typeof(HeroClass))
                    {
                        HeroClass currentHero = combatOrder[i] as HeroClass;

                        isHealer = (currentHero.name == "Mender") ? true : false;

                        bool isHealing = new bool();

                        if(isHealer)
                        {
                            if (combatHeroCopies.Count == 1 && combatHeroCopies[0] == currentHero)
                            {
                                foreach (HeroClass hero in combatHeroCopies)
                                {
                                    if(hero.currentHealth == hero.Health)
                                    {
                                        isHealing = true;
                                    }
                                    else
                                    {
                                        isHealing = false;
                                    }
                                }
                            }
                            else
                            {
                                isHealing = true;
                            }
                        }

                        switch (isHealing)
                        {
                            case true:
                                HeroClass heroToHeal = currentHero;

                                foreach (HeroClass hero in combatHeroCopies)
                                {
                                    if (hero.currentHealth < currentHero.Health)
                                        heroToHeal = hero;
                                }

                                currentHero.BasicHeal(heroToHeal);
                                break;

                            case false:
                                currentHero.BasicAttack(combatEnemyCopies[Random.Range(0, combatEnemyCopies.Count)], this, combatEnemyCopies);
                                break;
                        }
                    }
                    else
                    {
                        Enemy currentEnemy = combatOrder[i] as Enemy;

                        currentEnemy.BasicAttack(combatHeroCopies[Random.Range(0, combatHeroCopies.Count)], this, combatHeroCopies);
                    }

                    if ((i == combatOrder.Count || i == combatOrder.Count - 1) && (combatHeroCopies.Count > 0 && combatEnemyCopies.Count > 0))
                    {
                        i = -1;
                    }
                    else if (combatHeroCopies.Count <= 0 || combatEnemyCopies.Count <= 0)
                    {
                        if (combatHeroCopies.Count <= 0)
                        {
                            gameLog.text += ("\n The Party Was Defeated...");
                            yield return new WaitForSeconds(1.0f);
                            EndCombat(combatHeroCopies);
                        }

                        if (combatEnemyCopies.Count <= 0)
                        {
                            gameLog.text += ("\n The Party Has Destroyed The Enemy!");
                            yield return new WaitForSeconds(1.0f);
                            EndCombat(combatHeroCopies, true);
                        }
                    }

                    yield return new WaitForSeconds(1.5f);
                }
            }
        }
        else
        {
            StopCoroutine("CharacterActionPhase");
        }
    
    }


    public void EndCombat(List<HeroClass> combatHeroParty, bool victory = false)
    {
        bool killedBoss = false;

        if (!victory)
        {
            gameManager.ProceedToNextGamePhase();
        }
        else if (victory)
        {
            partyBehaviour.Gold += goldReward;

            goldReward = 0;
            

            for (int i = 0; i < combatHeroParty.Count; i++)
            {
                combatHeroParty[i].XP += xpReward;
                heroParty[i].CopyHero(heroParty[i], combatHeroParty[i]);

                partyBehaviour.heroParty[i].CopyHero(partyBehaviour.heroParty[i], heroParty[i]);
            }
            
            foreach(HeroClass hero in combatHeroParty)
            {
                Destroy(hero.gameObject);
            }

            gameManager.ProceedToNextGamePhase();
        }

        xpReward = 0;

        combatOrder.Clear();

        StopCoroutine("CharacterActionPhase");
        gameLog.text = "";
        gameLog.transform.parent.gameObject.SetActive(false);

        foreach(Enemy enemy in enemyParty)
        {
            killedBoss = (enemy.isBoss) ? true : false;

            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
        }

        partyBehaviour.EndCombat(killedBoss);
    }

    public void RemoveCharacterFromCombat(Character character, List<Enemy> currentEnemyParty = null, List<HeroClass> currentHeroParty = null)
    {
        combatOrder.Remove(character);

        if (character.GetType() == typeof(HeroClass))
        {
            for (int i = 0; i < heroParty.Count; i++)
            {
                if (heroParty[i].uniqueID == character.uniqueID)
                {
                    heroParty[i].isDead = true;
                }
            }
            currentHeroParty.Remove(character as HeroClass);
        }
        else if (character.GetType() == typeof(Enemy))
        {
            Enemy enemy = character as Enemy;

            goldReward += enemy.goldToReward;
            xpReward += enemy.xpReward;

            currentEnemyParty.Remove(character as Enemy);
        }
    }
}
