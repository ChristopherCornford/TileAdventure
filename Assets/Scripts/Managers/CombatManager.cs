using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Old Combat System Total Line Count: 1045
/// 
/// New Combat System Total Line Count: 
/// </summary>

public class CombatManager : MonoBehaviour
{
    public bool automatedCombat = false;
    public CombatStyle combatStyle = CombatStyle.TurnBasedAction;

    public GameManager gameManager;
    public PartyBehaviour partyBehaviour;


    public List<Character> CharactersInCombat;
    public List<CombatTolken> CombatTolkenList = new List<CombatTolken>();
    public int InitativeCount = 0;



    public List<HeroClass> heroParty;
    public List<Enemy> enemyParty;

    public List<Character> combatOrder;

    public float RTA_CooldownTime;

    public int goldReward;
    public int xpReward;

    public TextMeshProUGUI gameLog;

    public List<SpriteRenderer> heroMarkers = new List<SpriteRenderer>();
    public List<SpriteRenderer> enemyMarkers = new List<SpriteRenderer>();
    
    public Sprite baseSprite;
    public Color baseColor;

    public Sprite selectionSprite;
    public Color selectionColor;
    public Color specialReadyColor;

    public Color targetColor;

    public GameObject selectionMarker;
    
    public bool targetNeeded;
    public Character target;
    
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

        heroMarkers.Clear();
        enemyMarkers.Clear();

        if(combatStyle == CombatStyle.TurnBasedAction)
        {
            SetInitativeList();
        }
        /*else
        {
            List<HeroClass> heroClasses = MakeHeroCopies(heroParty);
            List<Enemy> enemies = MakeEnemyCopies(enemyParty);
            //StartCoroutine(BeginRealTimeCombat(enemies, heroClasses));
        }
        */
    }

    private void SetCharacterPositions()
    {
        GameObject heroParty = GameObject.FindGameObjectWithTag("Player");
        GameObject enemyParty = GameObject.FindGameObjectWithTag("Enemy");

        List<GameObject> heroes = new List<GameObject>();
        List<GameObject> enemies = new List<GameObject>();

        GameObject sideOne = GameObject.FindGameObjectWithTag("SideOne");
        GameObject sideTwo = GameObject.FindGameObjectWithTag("SideTwo");

        Vector3 posMod = new Vector3(0.0f, .25f, 0.0f);

        for (int i = 0; i < heroParty.transform.childCount; i++)
        {
            if (heroParty.transform.GetChild(i) != null)
            {
                heroes.Add(heroParty.transform.GetChild(i).gameObject);
            }

            if (heroes[i] != null)
            {
                heroes[i].transform.position = sideOne.transform.GetChild(i).transform.position;
            }

            GameObject marker = null;

            if (heroes[i].transform.childCount == 0)
            {
                marker = Instantiate(selectionMarker, heroes[i].transform);
            }

            marker.transform.position += posMod;

            heroes[i].GetComponent<HeroClass>().selectionMarker = marker;

            heroMarkers.Add(marker.transform.GetChild(1).GetComponent<SpriteRenderer>());
        }

        for (int i = 0; i < enemyParty.transform.childCount; i++)
        {
            if (enemyParty.transform.GetChild(i) != null)
            {
                enemies.Add(enemyParty.transform.GetChild(i).gameObject);
            }

            if (enemies[i] != null)
            {
                enemies[i].transform.position = sideTwo.transform.GetChild(i).transform.position;
            }

            GameObject marker = Instantiate(selectionMarker, enemies[i].transform);

            marker.transform.position += posMod;
            
            enemies[i].GetComponent<Enemy>().selectionMarker = marker;

            enemyMarkers.Add(marker.transform.GetChild(1).GetComponent<SpriteRenderer>());
        }


        Camera.main.transform.position = sideOne.transform.parent.transform.position;
        Camera.main.GetComponent<PlayerBehaviour>().zoomSlider.value = 0.75f;
    }
    /*
    private List<HeroClass> MakeHeroCopies(List<HeroClass> heroParty)
    {
        List<HeroClass> combatHeroCopies = new List<HeroClass>();
        int index = 0;

        foreach (HeroClass hero in heroParty)
        {
            GameObject nextHero = new GameObject(hero.name, typeof(HeroClass));
            nextHero.transform.SetParent(this.transform);

            HeroClass heroClass = nextHero.GetComponent<HeroClass>();

            heroClass.gameLog = gameLog;

            hero.CopyHero(heroClass, hero);
            combatHeroCopies.Add(heroClass);

            heroClass.combatIndex = index;

            index++;
        }

        return combatHeroCopies;
    }

    private List<Enemy> MakeEnemyCopies(List<Enemy> EnemyParty)
    {
        List<Enemy> combatEnemyCopies = new List<Enemy>();

        int index = 0;

        foreach (Enemy enemy in EnemyParty)
        {
            GameObject nextEnemy = new GameObject(enemy.name, typeof(Enemy));
            nextEnemy.transform.SetParent(this.transform);

            Enemy newEnemy = nextEnemy.GetComponent<Enemy>();

            newEnemy.gameLog = gameLog;
            
            enemy.CopyEnemy(newEnemy, enemy);

            newEnemy.SetID();

            GameObject.FindGameObjectWithTag("Enemy").transform.GetChild(index).GetComponent<Enemy>().uniqueID = newEnemy.uniqueID;

            combatEnemyCopies.Add(newEnemy);

            newEnemy.combatIndex = index;

            index++;
        }

        return combatEnemyCopies;
    }
    */
    public void SetInitativeList()
    {
        combatOrder.Clear();

       // List<HeroClass> combatHeroCopies = MakeHeroCopies(heroParty);

       // List<Enemy> combatEnemyCopies = MakeEnemyCopies(enemyParty);

        List<int> initativeList = new List<int>();

        List<Character> characters = new List<Character>();

        foreach (HeroClass hero in heroParty)
            characters.Add(hero);
        foreach (Enemy enemy in enemyParty)
            characters.Add(enemy);

        CharactersInCombat = characters;

        int maxAttacks = new int();


        foreach (Character character in characters)
        {
            int attacks = 1 + (character.Speed / 10);

            if (attacks > maxAttacks)
            {
                maxAttacks = attacks;
            }
        }


        foreach (HeroClass hero in heroParty)
        {
            int numOfAttacks = 1 + (hero.Speed / 10);

            for (int i = 0; i < numOfAttacks; i++)
            {
                initativeList.Add(hero.Speed - (10 * i));
            }
        }

        foreach (Enemy enemy in enemyParty)
        {
            int numOfAttacks = 1 + (enemy.Speed / 10);

            for (int i = 0; i < numOfAttacks; i++)
            {
                initativeList.Add(enemy.Speed - (10 * i));
            }
        }

        initativeList.Sort();
        initativeList.Reverse();

        InitativeCount = initativeList.Count;
        CreateCombatTolkens(characters, initativeList);

        /*
        for (int i = 0; i < initativeList.Count; i++)
        {
            bool spotTaken = new bool();

            foreach (Character character in characters)
            {
                if (spotTaken == false)
                {
                    for (int j = 0; j < maxAttacks; j++)
                    {
                        int speedMod = 10 * j;

                        if ((character.Speed - speedMod) == initativeList[i])
                        {
                            if (i != 0 && character.uniqueID != combatOrder[i - 1].uniqueID)
                            {
                                combatOrder.Insert(i, character);
                                
                                spotTaken = true;
                            }
                            else if (i == 0)
                            {
                                combatOrder.Insert(i, character);

                                spotTaken = true;
                            }
                        }
                    }
                }
            }
        }

        
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
        */

        SetCharacterPositions();

       /*
        if (automatedCombat)
            StartCoroutine(AutomatedCharacterActionPhase(combatEnemyCopies, combatHeroCopies));
        else
            StartCoroutine(ManualCharacterActionPhase(combatEnemyCopies, combatHeroCopies));
       */
    }
    
    IEnumerator WaitForPlayerInput()
    {
        while (targetNeeded)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

                RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 5f);

                if (hit.collider != null)
                {
                    Debug.Log(hit.collider.name);

                    if (hit.transform.gameObject.GetComponent<HeroClass>())
                    {
                        HeroClass hero = hit.transform.GetComponent<HeroClass>();

                        if (heroMarkers[ListIndex(heroParty, hero)].color == targetColor)
                        {
                            target = hero;
                        }
                    }
                    if (hit.transform.gameObject.GetComponent<Enemy>())
                    {
                        Enemy enemy = hit.transform.GetComponent<Enemy>();

                        if (enemyMarkers[EnemyListIndex(enemyParty, enemy)].color == targetColor)
                        {
                            target = enemy;
                        }
                    }
                }
            }

            if (target != null)
            {
                targetNeeded = false;
            }
            else
            {
                targetNeeded = true;
            }
            yield return null;
        }
    }

    public IEnumerator AutomatedCharacterActionPhase(List<Enemy> combatEnemyCopies, List<HeroClass> combatHeroCopies)
    {
        if (gameManager.currentGamePhase == GameManager.GamePhase.Combat)
        {
            for (int i = 0; i < combatOrder.Count; i++)
            {
                PopulateCombatPanel(combatHeroCopies, combatEnemyCopies);

                Character currentCharacter = combatOrder[i];

                if (gameManager.currentGamePhase == GameManager.GamePhase.Combat)
                {
                    combatOrder[i].ResolveStatusEffect();

                    if (combatOrder[i].isStunned == false)
                    {
                        bool isHealer = new bool();

                        if (combatOrder[i].GetType() == typeof(HeroClass))
                        {

                            yield return new WaitForSeconds(1.0f);

                            HeroClass currentHero = combatOrder[i] as HeroClass;

                            heroMarkers[ListIndex(heroParty, currentHero)].color = selectionColor;

                            yield return new WaitForSeconds(0.5f);

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
                            yield return new WaitForSeconds(1.0f);

                            Enemy currentEnemy = combatOrder[i] as Enemy;

                            enemyMarkers[EnemyListIndex(enemyParty, currentEnemy)].color = selectionColor;

                            yield return new WaitForSeconds(0.5f);

                            currentEnemy.BasicAttack(combatHeroCopies[Random.Range(0, combatHeroCopies.Count)], this, combatHeroCopies);
                        }

                        if (combatOrder[i] != currentCharacter)
                        {
                            i--;
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
                    else
                    {
                        gameLog.text = combatOrder[i].name + " is Stunned and cannot move.";
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

                currentCharacter.ResolveStatusEffect();

                if (currentCharacter.isStunned == false)
                {
                    if (currentCharacter.GetType() == typeof(HeroClass))
                    {
                        HeroClass currentHero = currentCharacter as HeroClass;
                        

                        if (currentHero.isSpecialReady)
                        {
                            heroMarkers[ListIndex(combatHeroCopies, currentHero)].color = specialReadyColor;
                        }
                        else
                        {
                            heroMarkers[ListIndex(combatHeroCopies, currentHero)].color = selectionColor;
                        }
                        
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
                                        hero.needsHealing = false;
                                    }
                                    else
                                    {
                                        isHealing = true; //This hero does need healing
                                        hero.needsHealing = true;
                                    }
                                }
                            }
                        }


                        //Hero Action, pauses for user input
                        switch (isHealing)
                        {
                            case true:
                                HeroClass heroToHeal = null;

                                if (currentHero.isSpecialReady)
                                {
                                    currentHero.UseSpecialAbility(null, this, null, combatHeroCopies.ToArray());
                                }

                                foreach(HeroClass hero in combatHeroCopies)
                                {
                                    if(hero.currentHealth < hero.Health)
                                    {
                                        heroMarkers[ListIndex(combatHeroCopies, hero)].color = targetColor;
                                    }
                                }

                                targetNeeded = true;

                                do { yield return null; } while (targetNeeded == true);

                                heroToHeal = combatHeroCopies[ListIndex(combatHeroCopies, target as HeroClass)];

                                currentHero.BasicHeal(heroToHeal);

                                targetNeeded = false;
                                target = null;

                                break;

                            case false:
                                Enemy enemyToAttack = null;

                                targetNeeded = true;

                                foreach(Enemy enemy in combatEnemyCopies)
                                {
                                    enemyMarkers[EnemyListIndex(combatEnemyCopies, enemy)].color = targetColor;
                                }

                                do { yield return null; } while (targetNeeded == true);

                                enemyToAttack = combatEnemyCopies[EnemyListIndex(combatEnemyCopies, target as Enemy)];

                                if (currentHero.isSpecialReady)
                                {
                                    currentHero.UseSpecialAbility(enemyToAttack, this, combatEnemyCopies, combatEnemyCopies.ToArray());
                                }
                                else
                                {
                                    currentHero.BasicAttack(enemyToAttack, this, combatEnemyCopies);
                                }

                                targetNeeded = false;
                                target = null;
                                
                                break;
                        }
                    }
                    else if (currentCharacter.GetType() == typeof(Enemy))
                    {
                        Enemy currentEnemy = currentCharacter as Enemy;

                        enemyMarkers[EnemyListIndex(combatEnemyCopies, currentEnemy)].color = selectionColor;

                        yield return new WaitForSeconds(1.5f);

                        HeroClass target = combatHeroCopies[Random.Range(0, combatHeroCopies.Count)];

                        if (target.role == HeroRole.Fencer && target.isSpecialReady)
                        {
                            target.UseSpecialAbility(currentEnemy, this, combatEnemyCopies);
                        }
                        else
                        {
                            currentEnemy.BasicAttack(target, this, combatHeroCopies);
                        }
                    }
                }
                else
                {
                    gameLog.text = combatOrder[i].name + " is Stunned and cannot move.";
                }

                if (i <= (combatOrder.Count - 1) && combatOrder[i].uniqueID != currentCharacter.uniqueID)
                {
                    i--;
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


    //RTA Combat Section
    /*
    public IEnumerator BeginRealTimeCombat(List<Enemy> combatEnemyCopies, List<HeroClass> combatHeroCopies)
    {
        StartCoroutine(HeroPartyCombat(combatHeroCopies, combatEnemyCopies));
        StartCoroutine(EnemyPartyCombat(combatHeroCopies, combatEnemyCopies));
        yield return null;
    }

    public IEnumerator HeroPartyCombat(List<HeroClass> combatHeroCopies, List<Enemy> combatEnemyCopies)
    {
        List<HeroClass> heroesReadyToAct = new List<HeroClass>();
        heroesReadyToAct = combatHeroCopies;

        List<HeroClass> heroesOnCooldown = new List<HeroClass>();

        HeroClass currentlySelectedHero = heroesReadyToAct[0];

        bool combatComplete = new bool();


        while (combatComplete == false)
        {
            PopulateCombatPanel(combatHeroCopies, combatEnemyCopies);

            foreach (GameObject buttonObj in heroButtons)
            {
                buttonObj.GetComponent<Button>().onClick.RemoveAllListeners();
            }

            for (int i = 0; i < heroButtons.Count; i++)
            {
                int indexI = i;

                if (heroesReadyToAct.Contains(combatHeroCopies[indexI]))
                {
                    Button button = heroButtons[indexI].GetComponent<Button>();

                    button.onClick.AddListener(delegate
                    {
                        currentlySelectedHero = combatHeroCopies[indexI];

                        if (currentlySelectedHero.role == HeroRole.Mender)
                        {
                            HeroClass heroToHeal = null;

                            for (int j = 0; j < heroButtons.Count; j++)
                            {
                                int indexJ = j;

                                heroButtons[indexJ].GetComponent<Button>().onClick.AddListener(delegate
                                {
                                    heroToHeal = combatHeroCopies[indexJ];

                                    currentlySelectedHero.BasicHeal(heroToHeal);
                                });
                            }
                        }

                        foreach (GameObject buttonObj in heroButtons)
                        {
                            buttonObj.GetComponent<Button>().onClick.RemoveAllListeners();

                            if (buttonObj == heroButtons[indexI])
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
                    int index = i;

                    Button button = enemyButtons[index].GetComponent<Button>();

                    button.onClick.AddListener(delegate
                    {
                        currentlySelectedHero.BasicAttack(combatEnemyCopies[index], this, combatEnemyCopies);

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

        gameLog.text += ("\n The Party Has Destroyed The Enemy!");
        yield return new WaitForSeconds(1.0f);
        EndCombat(combatHeroCopies, true);
    }

    public IEnumerator EnemyPartyCombat(List<HeroClass> combatHeroCopies, List<Enemy> combatEnemyCopies)
    {
        List<Enemy> enemiesReadyToAct = combatEnemyCopies;
        List<Enemy> enemiesOnCooldown = new List<Enemy>();

        bool combatComplete = new bool();


        while (combatComplete == false)
        {
            PopulateCombatPanel(combatHeroCopies, combatEnemyCopies);

            for (int i = 0; i < enemiesReadyToAct.Count; i++)
            {
                int index = i;

                enemiesReadyToAct[index].BasicAttack(combatHeroCopies[Random.Range(0, combatHeroCopies.Count)], this, combatHeroCopies);
                enemiesReadyToAct[index] = null;

                enemiesOnCooldown.Add(enemiesReadyToAct[index]);
                yield return new WaitForSeconds(1.0f);
            }

            int count = enemiesReadyToAct.Count;

            for (int i = 0; i < count; i++)
            {
                int index = i;

                if (enemiesReadyToAct[index] == null)
                {
                    enemiesReadyToAct.RemoveAt(index);
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

        gameLog.text += ("\n The Party Was Defeated...");
        yield return new WaitForSeconds(1.0f);
        EndCombat(combatHeroCopies);
    }
    */

    public void StopAllCombatImmeadiately()
    {
        this.StopAllCoroutines();
    }

    public void EndCombat(List<HeroClass> combatHeroParty, bool victory = false)
    {
        bool killedBoss = false;

        if (!victory)
        {
            SceneManager.LoadScene(0);

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

                Destroy(heroParty[i].transform.GetChild(0).gameObject);

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

        Destroy(GameObject.FindGameObjectWithTag("BattleFormation"));

        foreach (Enemy enemy in enemyParty)
        {
            killedBoss = (enemy.isBoss) ? true : false;

            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
        }

        partyBehaviour.EndCombat(killedBoss);
    }

    public void RemoveCharacterFromCombat(Character character, List<Enemy> currentEnemyParty = null, List<HeroClass> currentHeroParty = null)
    {
        List<Character> newCombatOrder = new List<Character>();

        foreach(Character charObj in combatOrder)
        {
            if (charObj.uniqueID != character.uniqueID)
            {
                newCombatOrder.Add(charObj);
            }
        }

        combatOrder = newCombatOrder;

        if (character.GetType() == typeof(HeroClass))
        {
            for (int i = 0; i < heroParty.Count; i++)
            {
                if (heroParty[i].uniqueID == character.uniqueID)
                {
                    heroParty[i].isDead = true;
                    heroParty[i].GetComponent<SpriteRenderer>().enabled = false;
                }
            }

            heroMarkers[ListIndex(currentHeroParty, character as HeroClass)].transform.parent.gameObject.SetActive(false);
            heroMarkers.RemoveAt(ListIndex(currentHeroParty, character as HeroClass));

            GameObject.FindGameObjectWithTag("Player").transform.GetChild(ListIndex(currentHeroParty, character as HeroClass)).gameObject.SetActive(false);
            currentHeroParty.Remove(character as HeroClass);
        }
        else if (character.GetType() == typeof(Enemy))
        {
            Enemy enemy = character as Enemy;

            goldReward += enemy.goldToReward;
            xpReward += enemy.xpReward;

            enemyMarkers[EnemyListIndex(currentEnemyParty, enemy)].transform.parent.gameObject.SetActive(false);
            enemyMarkers.RemoveAt(EnemyListIndex(currentEnemyParty, enemy));

            GameObject.FindGameObjectWithTag("Enemy").transform.GetChild(EnemyListIndex(currentEnemyParty, enemy)).gameObject.SetActive(false);
            currentEnemyParty.Remove(character as Enemy);
        }
    }

    public void PopulateCombatPanel(List<HeroClass> heroes = null, List<Enemy> enemies = null)
    {
        if (heroes != null)
        {
            for (int i = 0; i < heroMarkers.Count; i++)
            {
                heroMarkers[i].color = Color.white;
            }
        }
        
        if (enemies != null)
        {
            for (int i = 0; i < enemyMarkers.Count; i++)
            {
                enemyMarkers[i].color = Color.white;
            }
        }
    }

    private int ListIndex(List<HeroClass> heroes, HeroClass hero)
    {
        int index = new int();

        for (int i = 0; i < heroes.Count; i++)
            if (heroes[i].uniqueID == hero.uniqueID)
                index = i;

        return index;
    }

    private int EnemyListIndex(List<Enemy> enemies, Enemy enemy)
    {
        int index = new int();

        for (int i = 0; i < enemies.Count; i++)
            if (enemies[i].uniqueID == enemy.uniqueID)
                index = i;

        return index;
    }

    private int ListIndex(List<Character> characters, Character character)
    {
        int index = new int();

        for (int i = 0; i < characters.Count; i++)
            if (characters[i].uniqueID == character.uniqueID)
                index = i;

        return index;
    }

    void CreateCombatTolkens(List<Character> characters, List<int> initative)
    {
        int numOfAttacks = new int();

        List<int> initativeCopy = new List<int>();

        for (int i = 0; i < initative.Count; i++)
            initativeCopy.Add(initative[i]);

        for (int i = 0; i < characters.Count; i++)
        {
            numOfAttacks = ((1 + (characters[i].Speed / 10)) < 3) ? 1 + (characters[i].Speed / 10) : 3;
            
            List<int> AttackPositions = new List<int>(numOfAttacks);
            
            CombatTolken tolken = new CombatTolken(characters[i].uniqueID, i, AttackPositions);

            CombatTolkenList.Add(tolken);
        }

        foreach (CombatTolken ct in CombatTolkenList)
        {
            double lastID = 0;

            if (ct.CombatPositions.Count < numOfAttacks)
            {
                for (int i = 0; i < initative.Count; i++)
                {
                    if (initative[i] != -1)
                    {
                        for (int j = 0; j < numOfAttacks; j++)
                        {
                            int speedMod = 10 * j;

                            if ((characters[ct.Index].Speed - speedMod) == initative[i])
                            {
                                if (ct.CharacterID != lastID)
                                {
                                    if (j == 0)
                                        ct.CombatPositions.Add(i);
                                    else if (j != ct.CombatPositions.Count - 1 && j == 1)
                                        ct.CombatPositions.Add(i);
                                    else if (j != ct.CombatPositions.Count - 1 && j == 2)
                                        ct.CombatPositions.Add(i);

                                    initative[i] = -1;
                                }

                                lastID = ct.CharacterID;
                            }
                        }
                    }
                }
            }
        }

        StartCoroutine(CombatSequence(initativeCopy));
    }

    IEnumerator CombatSequence(List<int> initative)
    {
        // int Round = 1;

        bool combatIsOver = false;

        while (!combatIsOver)
        {
            for (int i = 0; i < InitativeCount; i++)
                foreach (CombatTolken ct in CombatTolkenList)
                    if (ct.CombatPositions.Contains(i))
                        Debug.Log(CharactersInCombat[ct.Index].name);

            combatIsOver = true;
        }

        yield return new WaitForFixedUpdate();
    }
}

[System.Serializable]
public class CombatTolken
{
    public double CharacterID;

    public int Index;

    public List<int> CombatPositions;

    public bool isDead;

    public CombatTolken(double charID, int indexOfCharacterList, List<int> comPos)
    {
        CharacterID = charID;

        Index = indexOfCharacterList;

        CombatPositions = comPos;

        isDead = false;
    }
}


public enum CombatStyle
{
    TurnBasedAction,
    RealTimeAction,
}