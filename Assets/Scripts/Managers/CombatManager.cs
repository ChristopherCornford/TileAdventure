using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatManager : MonoBehaviour
{
    public bool automatedCombat = false;
    public CombatStyle combatStyle = CombatStyle.TurnBasedAction;

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

        if(combatStyle == CombatStyle.TurnBasedAction)
        {
            SetInitativeList();
        }
    }

    private List<HeroClass> MakeHeroCopies(List<HeroClass> heroParty)
    {
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

        return combatHeroCopies;
    }

    private List<Enemy> MakeEnemyCopies(List<Enemy> enemyParty)
    {
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

        return combatEnemyCopies;
    }

    public void SetInitativeList()
    {
        combatOrder.Clear();

        List<HeroClass> combatHeroCopies = MakeHeroCopies(heroParty);

        List<Enemy> combatEnemyCopies = MakeEnemyCopies(enemyParty);

        List<int> initativeList = new List<int>();


        /*
         
         
         TODO: ADD SPEED-BASED MULTI-ATTACK TO INITATIVE LIST


        */


        foreach (HeroClass hero in combatHeroCopies)
            initativeList.Add(hero.Speed);

        foreach (Enemy enemy in combatEnemyCopies)
            initativeList.Add(enemy.Speed);

        initativeList.Sort();
        initativeList.Reverse();

        for (int i = 0; i < initativeList.Count; i++)
        {
            foreach (Enemy enemy in combatEnemyCopies)
                if (enemy.Speed == initativeList[i])
                    if (!combatOrder.Contains(enemy))
                        combatOrder.Insert(i, enemy);

            foreach (HeroClass heroClass in combatHeroCopies)
                if (heroClass.Speed == initativeList[i])
                    if (!combatOrder.Contains(heroClass))
                        combatOrder.Insert(i, heroClass);
        }

        if (automatedCombat)
            StartCoroutine(AutomatedCharacterActionPhase(combatEnemyCopies, combatHeroCopies));
        else
            StartCoroutine(ManualCharacterActionPhase(combatEnemyCopies, combatHeroCopies));
    }

    public IEnumerator AutomatedCharacterActionPhase(List<Enemy> combatEnemyCopies, List<HeroClass> combatHeroCopies)
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

                        if (isHealer)
                        {
                            if (combatHeroCopies.Count == 1 && combatHeroCopies[0] == currentHero)
                            {
                                if (currentHero.currentHealth != currentHero.Health)
                                {
                                    isHealing = true;
                                }
                                else
                                {
                                    isHealing = false;
                                }
                            }
                            else
                            {
                                foreach (HeroClass hero in combatHeroCopies)
                                {
                                    if (hero.currentHealth == hero.Health)
                                    {
                                        isHealing = false;
                                    }
                                    else
                                    {
                                        isHealing = true;
                                    }
                                }
                            }
                        }

                        switch (isHealing)
                        {
                            case true:
                                HeroClass heroToHeal = currentHero;

                                foreach (HeroClass hero in combatHeroCopies)
                                {
                                    double heroHealthPerc = (hero.currentHealth / hero.Health) * 100; //Grab % health of 'hero'

                                    double tempHealthPerc = (heroToHeal.currentHealth / heroToHeal.Health) * 100; //Grab % health of heroToHeal

                                    //If hero's Health% is less than the current heroToHeal's, make 'hero' the new heroToHeal
                                    if (heroHealthPerc < tempHealthPerc) 
                                        heroToHeal = hero;
                                }

                                currentHero.BasicHeal(heroToHeal); //Heals the party member with the lowest percent health
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
            StopAllCombatImmeadiately();
        }
    }

    public IEnumerator ManualCharacterActionPhase(List<Enemy> combatEnemyCopies, List<HeroClass> combatHeroCopies)
    {
        if (gameManager.currentGamePhase == GameManager.GamePhase.Combat)
        {
            for (int i = 0; i < combatOrder.Count; i++)
            {
                Character currentCharacter = combatOrder[i];

                if(currentCharacter.GetType() == typeof(HeroClass))
                {
                    HeroClass currentHero = currentCharacter as HeroClass;

                    bool isHealer = (currentHero.role == HeroRole.Mender) ? true : false;
                    bool isHealing = new bool();

                    if (isHealer)
                    {
                        if (combatHeroCopies.Count == 1 && combatHeroCopies[0] == currentHero)
                        {
                            if (currentHero.currentHealth != currentHero.Health)
                            {
                                isHealing = true; //Is alone, and not at full health
                            }
                            else
                            {
                                isHealing = false; //Is alone, but at full health
                            }
                        }
                        else
                        {
                            foreach (HeroClass hero in combatHeroCopies)
                            {
                                if (hero.currentHealth == hero.Health)
                                {
                                    isHealing = false; //This hero doesn't need healing;
                                }
                                else
                                {
                                    isHealing = true; //This hero does need healing
                                }
                            }
                        }
                    }

                    //Hero Action, pauses for user input
                    switch (isHealing)
                    {
                        case true:
                            // Healing Choice
                            break;

                        case false:
                            // Attack Choice
                            break;
                    }
                }
                else if (currentCharacter.GetType() == typeof(Enemy))
                {
                    Enemy currentEnemy = currentCharacter as Enemy;
                }
            }
        }


        yield return null;
    }

    public IEnumerator BeginRealTimeCombat(List<Enemy> combatEnemyCopies, List<HeroClass> combatHeroCopies)
    {
        StartCoroutine(HeroPartyCombat(combatEnemyCopies));
        StartCoroutine(EnemyPartyCombat(combatHeroCopies));
        yield return null;
    }

    public IEnumerator HeroPartyCombat(List<Enemy> combatEnemyCopies)
    {
        List<HeroClass> heroesReadyToAct = MakeHeroCopies(heroParty);
        List<HeroClass> heroesOnCooldown = new List<HeroClass>();

        //TODO: HERO PARTY COMBAT LOOP
        yield return null;
    }

    public IEnumerator EnemyPartyCombat(List<HeroClass> combatHeroCopies)
    {
        List<Enemy> enemiesReadyToAct = MakeEnemyCopies(enemyParty);
        List<Enemy> enemiesOnCooldown = new List<Enemy>();

        //TODO: ENEMY PARTY COMBAT LOOP
        yield return null;
    }

    public void StopAllCombatImmeadiately()
    {
        this.StopAllCoroutines();
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

            foreach (HeroClass hero in combatHeroParty)
            {
                Destroy(hero.gameObject);
            }

            gameManager.ProceedToNextGamePhase();
        }

        xpReward = 0;

        combatOrder.Clear();

        StopAllCombatImmeadiately();
        gameLog.text = "";
        gameLog.transform.parent.gameObject.SetActive(false);

        foreach (Enemy enemy in enemyParty)
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


public enum CombatStyle
{
    TurnBasedAction,
    RealTimeAction,
}