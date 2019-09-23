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


    public TextMeshProUGUI gameLog;

    //private List<HeroClass> combatHeroCopies;
    //private List<Enemy> combatEnemyCopies;

    private void Awake()
    {
        gameManager = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
        partyBehaviour = GameObject.FindObjectOfType(typeof(PartyBehaviour)) as PartyBehaviour;
    }

    public void StartCombat()
    {
        heroParty = partyBehaviour.heroParty;

        gameLog.transform.parent.gameObject.SetActive(true);

        SetInitativeList();
    }

    public void SetInitativeList()
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
                    bool isHealing = new bool();

                    if (combatOrder[i].GetType() == typeof(HeroClass))
                    {
                        HeroClass currentHero = combatOrder[i] as HeroClass;

                        isHealing = (currentHero.name == "Mender") ? true : false;

                        switch (isHealing)
                        {
                            case true:

                                currentHero.BasicHeal(combatHeroCopies[Random.Range(0, heroParty.Count)]);
                                break;

                            case false:
                                currentHero.BasicAttack(combatEnemyCopies[Random.Range(0, enemyParty.Count)], this, combatEnemyCopies);
                                break;
                        }
                    }
                    else
                    {
                        Enemy currentEnemy = combatOrder[i] as Enemy;

                        currentEnemy.BasicAttack(combatHeroCopies[Random.Range(0, heroParty.Count)], this, combatHeroCopies);
                    }

                    if (i == combatOrder.Count - 1 && (combatHeroCopies.Count > 0 || combatEnemyCopies.Count > 0))
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

                    yield return new WaitForSeconds(3f);
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
        if (!victory)
        {
            gameManager.ProceedToNextGamePhase();
        }
        else if (victory)
        {
            for (int i = 0; i < combatHeroParty.Count; i++)
            {
                partyBehaviour.heroParty[i].CopyHero(partyBehaviour.heroParty[i], combatHeroParty[i]);
                combatHeroParty[i].Die(this, combatHeroParty);
            }
            
            gameManager.ProceedToNextGamePhase();
        }

        combatOrder.Clear();

        StopCoroutine("CharacterActionPhase");
        gameLog.text = "";
        gameLog.transform.parent.gameObject.SetActive(false);
    }

    public void RemoveCharacterFromCombat(Character character, List<Enemy> currentEnemyParty = null, List<HeroClass> currentHeroParty = null)
    {
        if (character.GetType() == typeof(HeroClass))
        {
            currentHeroParty.Remove(character as HeroClass);
            
        }
        else if (character.GetType() == typeof(Enemy))
        {
            currentEnemyParty.Remove(character as Enemy);
        }
    }
}
