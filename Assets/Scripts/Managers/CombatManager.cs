using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    public float RTA_CooldownTime;

    public int goldReward;
    public int xpReward;

    public TextMeshProUGUI gameLog;
    public GameObject combatPanel;

    public List<GameObject> heroButtons = new List<GameObject>();
    public List<GameObject> enemyButtons = new List<GameObject>();

    public GameObject heroButton_TB;
    public GameObject heroButton_RT;
    public GameObject enemyButton_TB;
    public GameObject enemyButton_RT;

    public Sprite baseSprite;
    public Color baseColor;

    public Sprite selectionSprite;
    public Color selectionColor;
    
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
        combatPanel.gameObject.SetActive(true);

        if(combatStyle == CombatStyle.TurnBasedAction)
        {
            SetInitativeList();
        }
        else
        {
            StartCoroutine(BeginRealTimeCombat(MakeEnemyCopies(enemyParty), MakeHeroCopies(heroParty)));
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
                PopulateCombatPanel(combatHeroCopies, combatEnemyCopies);

                if (gameManager.currentGamePhase == GameManager.GamePhase.Combat)
                {
                    bool isHealer = new bool();

                    if (combatOrder[i].GetType() == typeof(HeroClass))
                    {
                        HeroClass currentHero = combatOrder[i] as HeroClass;

                        heroButtons[ListIndex(combatHeroCopies, currentHero)].GetComponent<Image>().sprite = selectionSprite;
                        heroButtons[ListIndex(combatHeroCopies, currentHero)].GetComponent<Image>().color = selectionColor;

                        yield return new WaitForSeconds(1.5f);
                        
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

                        enemyButtons[ListIndex(combatEnemyCopies, currentEnemy)].GetComponent<Image>().sprite = selectionSprite;
                        enemyButtons[ListIndex(combatEnemyCopies, currentEnemy)].GetComponent<Image>().color = selectionColor;

                        yield return new WaitForSeconds(1.5f);

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
                PopulateCombatPanel(combatHeroCopies, combatEnemyCopies);
                
                yield return new WaitForSeconds(1.5f);

                Character currentCharacter = combatOrder[i];

                if(currentCharacter.GetType() == typeof(HeroClass))
                {
                    HeroClass currentHero = currentCharacter as HeroClass;
                    
                    heroButtons[ListIndex(combatHeroCopies, currentHero)].GetComponent<Image>().sprite = selectionSprite;
                    heroButtons[ListIndex(combatHeroCopies, currentHero)].GetComponent<Image>().color = selectionColor;

                    yield return new WaitForSeconds(1.5f);

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
                            HeroClass heroToHeal = null;
                            bool heroChoosen = new bool();

                            for (int j = 0; j < heroButtons.Count; j++)
                            {
                                Button button = heroButtons[i].GetComponent<Button>();

                                button.onClick.AddListener(delegate
                                {
                                    heroToHeal = combatHeroCopies[i];
                                    heroChoosen = true;
                                });
                            }

                            do { yield return null; } while (heroChoosen == false);

                            currentHero.BasicHeal(heroToHeal); //Heals the party member with the lowest percent health
                            
                            break;

                        case false:
                            Enemy enemyToAttack = null;
                            bool enemyChoosen = new bool();

                            for (int j = 0; j < enemyButtons.Count; j ++)
                            {
                                Button button = enemyButtons[i].GetComponent<Button>();

                                button.onClick.AddListener(delegate
                                {
                                    enemyToAttack = combatEnemyCopies[i];
                                    enemyChoosen = true;
                                });
                            }

                            do { yield return null; } while (enemyChoosen == false);

                            currentHero.BasicAttack(enemyToAttack, this, combatEnemyCopies);
                            
                            break;
                    }
                }
                else if (currentCharacter.GetType() == typeof(Enemy))
                {
                    Enemy currentEnemy = currentCharacter as Enemy;

                    enemyButtons[ListIndex(combatEnemyCopies, currentEnemy)].GetComponent<Image>().sprite = selectionSprite;
                    enemyButtons[ListIndex(combatEnemyCopies, currentEnemy)].GetComponent<Image>().color = selectionColor;

                    yield return new WaitForSeconds(1.5f);

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

    public IEnumerator BeginRealTimeCombat(List<Enemy> combatEnemyCopies, List<HeroClass> combatHeroCopies)
    {
        StartCoroutine(HeroPartyCombat(combatHeroCopies, combatEnemyCopies));
        StartCoroutine(EnemyPartyCombat(combatHeroCopies, combatEnemyCopies));
        yield return null;
    }

    public IEnumerator HeroPartyCombat(List<HeroClass> combatHeroCopies, List<Enemy> combatEnemyCopies)
    {
        List<HeroClass> heroesReadyToAct = MakeHeroCopies(heroParty);
        List<HeroClass> heroesOnCooldown = new List<HeroClass>();

        HeroClass currentlySelectedHero = heroesReadyToAct[0];

        bool combatComplete = new bool();

        do
        {
            PopulateCombatPanel(combatHeroCopies, null);

            foreach (GameObject buttonObj in heroButtons)
            {
                buttonObj.GetComponent<Button>().onClick.RemoveAllListeners();
            }

            for (int i = 0; i < heroButtons.Count; i++)
            {
                if (heroesReadyToAct.Contains(combatHeroCopies[i]))
                {
                    Button button = heroButtons[i].GetComponent<Button>();

                    button.onClick.AddListener(delegate
                    {
                        currentlySelectedHero = combatHeroCopies[i];

                        if (currentlySelectedHero.role == HeroRole.Mender)
                        {
                            HeroClass heroToHeal = null;

                            for (int j = 0; j < heroButtons.Count; j++)
                            {
                                heroButtons[j].GetComponent<Button>().onClick.AddListener(delegate
                                {
                                    heroToHeal = combatHeroCopies[i];

                                    currentlySelectedHero.BasicHeal(heroToHeal);
                                });
                            }
                        }

                        foreach (GameObject buttonObj in heroButtons)
                        {
                            buttonObj.GetComponent<Button>().onClick.RemoveAllListeners();

                            if (buttonObj == heroButtons[i])
                            {
                                buttonObj.GetComponent<Image>().sprite = selectionSprite;
                                buttonObj.GetComponent<Image>().color = selectionColor;
                            }
                            else
                            {
                                buttonObj.GetComponent<Image>().sprite = baseSprite;
                                buttonObj.GetComponent<Image>().color = baseColor;
                            }
                        }
                    });
                }
            }

            if (heroesReadyToAct != null)
            {
                for (int i = 0; i < enemyButtons.Count; i++)
                {
                    Button button = enemyButtons[i].GetComponent<Button>();

                    button.onClick.AddListener(delegate
                    {
                        currentlySelectedHero.BasicAttack(combatEnemyCopies[i], this, combatEnemyCopies);

                        heroesReadyToAct.Remove(currentlySelectedHero);
                        heroesOnCooldown.Add(currentlySelectedHero);

                        currentlySelectedHero = heroesReadyToAct[0];
                    });
                }
            }
            
            if (heroesOnCooldown != null)
            {
                for (int i = 0; i < heroesOnCooldown.Count; i++)
                {
                    foreach (HeroClass hero in heroesOnCooldown)
                    {
                        hero.cooldownTimer += Time.deltaTime;
                    }
                }

                foreach (HeroClass hero in combatHeroCopies)
                {
                    if (heroesOnCooldown.Contains(hero))
                    {
                        if (hero.cooldownTimer >= (RTA_CooldownTime - (hero.Speed / 10)))
                        {
                            heroesOnCooldown.Remove(hero);
                            heroesReadyToAct.Add(hero);
                        }
                    }
                }
            }

            if(combatEnemyCopies == null)
            {
                combatComplete = true;
            }
        }
        while (combatComplete == false);

        gameLog.text += ("\n The Party Has Destroyed The Enemy!");
        yield return new WaitForSeconds(1.0f);
        EndCombat(combatHeroCopies, true);
    }

    public IEnumerator EnemyPartyCombat(List<HeroClass> combatHeroCopies, List<Enemy> combatEnemyCopies)
    {
        List<Enemy> enemiesReadyToAct = MakeEnemyCopies(enemyParty);
        List<Enemy> enemiesOnCooldown = new List<Enemy>();

        bool combatComplete = new bool();

        do
        {
            PopulateCombatPanel(null, combatEnemyCopies);

            for (int i = 0; i < enemiesReadyToAct.Count; i++)
            {
                enemiesReadyToAct[i].BasicAttack(combatHeroCopies[Random.Range(0, combatHeroCopies.Count)], this, combatHeroCopies);
                enemiesReadyToAct[i] = null;

                enemiesOnCooldown.Add(enemiesReadyToAct[i]);
                yield return new WaitForSeconds(1.0f);
            }

            int count = enemiesReadyToAct.Count;

            for (int i = 0; i < count; i++)
            {
                if (enemiesReadyToAct[i] == null)
                {
                    enemiesReadyToAct.RemoveAt(i);
                }
            }

            foreach (Enemy enemy in enemiesOnCooldown)
            {
                enemy.cooldownTimer += Time.deltaTime;
            }

            foreach (Enemy enemy in combatEnemyCopies)
            {
                if (enemiesOnCooldown.Contains(enemy))
                {
                    if (enemy.cooldownTimer >= (RTA_CooldownTime - (enemy.Speed / 10)))
                    {
                        enemiesOnCooldown.Remove(enemy);
                        enemiesReadyToAct.Add(enemy);
                    }
                }
            }

            if (combatHeroCopies == null)
            {
                combatComplete = true;
            }
        }
        while (combatComplete == false);

        gameLog.text += ("\n The Party Was Defeated...");
        yield return new WaitForSeconds(1.0f);
        EndCombat(combatHeroCopies);
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
        combatPanel.gameObject.SetActive(false);

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

    public void PopulateCombatPanel(List<HeroClass> heroes = null, List<Enemy> enemies = null)
    {
        if (heroes != null)
        {
            GameObject heroPartyPanel = GameObject.FindGameObjectWithTag("HeroPartyPanel");

            for (int i = 0; i < heroButtons.Count; i++)
            {
                Destroy(heroButtons[i].gameObject);
            }
            heroButtons.Clear();

            for (int i = 0; i < heroes.Count; i++)
            {
                HeroClass currentHero = heroes[i];
                GameObject newHeroButton = null;

                switch (combatStyle)
                {
                    case CombatStyle.TurnBasedAction:
                        newHeroButton = Instantiate(heroButton_TB, heroPartyPanel.transform);

                        heroButton_TB.GetComponent<Image>().sprite = baseSprite;
                        heroButton_TB.GetComponent<Image>().color = baseColor;

                        newHeroButton.transform.GetChild(0).GetComponent<Image>().sprite = currentHero.sprite;
                        break;

                    case CombatStyle.RealTimeAction:
                        newHeroButton = Instantiate(heroButton_RT, heroPartyPanel.transform);

                        newHeroButton.transform.GetChild(0).GetComponent<Slider>().value = (heroes[i].cooldownTimer / (RTA_CooldownTime - (heroes[i].Speed / 10)));
                        newHeroButton.transform.GetChild(1).GetComponent<Image>().sprite = currentHero.sprite;
                        break;
                }

                heroButtons.Add(newHeroButton);
            }
        }
        
        if (enemies != null)
        {
            GameObject enemyPartyPanel = GameObject.FindGameObjectWithTag("EnemyPartyPanel");
            
            for (int i = 0; i < enemyButtons.Count; i++)
            {
                Destroy(enemyButtons[i].gameObject);
            }
            enemyButtons.Clear();


            for (int i = 0; i < enemies.Count; i++)
            {
                Enemy currentEnemy = enemies[i];
                GameObject newEnemyButton = null;

                switch (combatStyle)
                {
                    case CombatStyle.TurnBasedAction:
                        newEnemyButton = Instantiate(enemyButton_TB, enemyPartyPanel.transform);

                        enemyButton_TB.GetComponent<Image>().sprite = baseSprite;
                        enemyButton_TB.GetComponent<Image>().color = baseColor;

                        newEnemyButton.transform.GetChild(0).GetComponent<Image>().sprite = currentEnemy.sprite;
                        break;

                    case CombatStyle.RealTimeAction:
                        newEnemyButton = Instantiate(enemyButton_RT, enemyPartyPanel.transform);

                        newEnemyButton.transform.GetChild(0).GetComponent<Slider>().value = enemies[i].cooldownTimer / (RTA_CooldownTime - (enemies[i].Speed / 10));

                        newEnemyButton.transform.GetChild(1).GetComponent<Image>().sprite = currentEnemy.sprite;
                        break;
                }

                enemyButtons.Add(newEnemyButton);
            }
        }
    }

    private int ListIndex(List<HeroClass> heroes, HeroClass hero)
    {
        int index = new int();

        for (int i = 0; i < heroes.Count; i++)
            if (heroes[i] == hero)
                index = i;

        return index;
    }

    private int ListIndex(List<Enemy> enemies, Enemy enemy)
    {
        int index = new int();

        for (int i = 0; i < enemies.Count; i++)
            if (enemies[i] == enemy)
                index = i;

        return index;
    }
}


public enum CombatStyle
{
    TurnBasedAction,
    RealTimeAction,
}