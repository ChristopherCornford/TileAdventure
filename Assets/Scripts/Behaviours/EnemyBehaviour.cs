using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public PartyBehaviour partyBehaviour;

    public Sprite previewSprite;

    public Vector3Int difficultyChances;
    
    public List<GameObject> enemyParty = new List<GameObject>();
    public List<Enemy> enemyClasses = new List<Enemy>();

    GameManager gameManager;

    private void Awake()
    {
        this.gameObject.tag = "Enemy";

        gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
        partyBehaviour = FindObjectOfType(typeof(PartyBehaviour)) as PartyBehaviour;

        int levelAvg = new int();

        foreach (HeroClass hero in partyBehaviour.heroParty)
        {
            levelAvg += hero.Level;
        }

        levelAvg = Mathf.RoundToInt((float)levelAvg / partyBehaviour.heroParty.Count);

        GenerateEnemyParty(levelAvg);

        foreach (GameObject enemyObj in enemyParty)
        {
            enemyClasses.Add(enemyObj.GetComponent<Enemy>());

            Instantiate(enemyObj, this.transform);
        }

        foreach(Enemy enemy in enemyClasses)
        {
            enemy.SetID();
        }
    }
    
    public void GenerateEnemyParty(int partyLevel)
    {
        if (partyBehaviour.currentTile.GetType() == typeof(VariantTiles))
        {
            VariantTiles variantTile = partyBehaviour.currentTile as VariantTiles;

            if (variantTile.variantType == VariantTiles.VariantType.Boss)
            {
                GameObject bossObj = gameManager.AllBosses[Random.Range(0, gameManager.AllBosses.Count)];

                enemyParty.Add(bossObj);

                return;
            }
        }
        
        int combatDifficulty = GenerateDifficulty();

        double enemyGenerationPoints = partyBehaviour.heroParty.Count + combatDifficulty;

        GameObject startingEnemy = null;

        int currentTier = new int();

        switch (partyLevel)
        {
            case 1:
            case 2:
                currentTier = 1;
                startingEnemy = GetEnemy(currentTier);
                break;
            case 3:
            case 4:
                currentTier = 2;
                startingEnemy = GetEnemy(currentTier);
                break;
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
                currentTier = 3;
                startingEnemy = GetEnemy(currentTier);
                break;

            default:
                currentTier = 3;
                startingEnemy = GetEnemy(currentTier);

                Debug.LogError("Party's Average Level is above level max.");
                break;
        }

        enemyParty.Add(startingEnemy);
        enemyGenerationPoints -= startingEnemy.GetComponent<Enemy>().CombatSize;

        //Skip while loop if possible
        if(enemyGenerationPoints > 0 && enemyParty.Count < gameManager.enemyPartySizeLimit)
        {
            while (enemyGenerationPoints > 0 && enemyParty.Count < gameManager.enemyPartySizeLimit)
            {
                int randInt = Random.Range(0, 100);

                GameObject nextEnemy = null;

                switch (currentTier)
                {
                    case 1:
                        currentTier = 1;
                        nextEnemy = GetEnemy(1);

                        if (nextEnemy.GetComponent<Enemy>().CombatSize >= enemyGenerationPoints)
                        {
                            enemyGenerationPoints -= nextEnemy.GetComponent<Enemy>().CombatSize;
                        }
                        else
                        {
                            GetEnemy(1);
                        }
                        break;

                    case 2:
                        nextEnemy = GetEnemy(2);

                        enemyGenerationPoints -= nextEnemy.GetComponent<Enemy>().CombatSize;

                        if(enemyGenerationPoints >= 2)
                        {
                            currentTier = 2;
                        }
                        else if (enemyGenerationPoints <= 1 && enemyGenerationPoints > 0)
                        {
                            currentTier = 1;
                        }

                        break;

                    case 3:
                        nextEnemy = GetEnemy(3);

                        enemyGenerationPoints -= nextEnemy.GetComponent<Enemy>().CombatSize;

                        if (enemyGenerationPoints >= 3)
                        {
                            currentTier = 3;
                        }
                        else if (enemyGenerationPoints == 2)
                        {
                            currentTier = 2;
                        }
                        else if (enemyGenerationPoints <= 1 && enemyGenerationPoints > 0)
                        {
                            currentTier = 1;
                        }
                        break;
                }

                enemyParty.Add(nextEnemy);
                
                if (enemyParty.Count == gameManager.enemyPartySizeLimit)
                {
                    if( enemyGenerationPoints > 0)
                    {
                        LevelUpEnemyParty((int)enemyGenerationPoints, partyLevel);
                        return;
                    }
                    else
                    {
                        LevelUpEnemyParty(0, partyLevel);
                        return;
                    }
                }
            }
        }
        else if (enemyGenerationPoints == 0)
        {
            LevelUpEnemyParty(0, partyLevel);
            return;
        }
    }

    private void LevelUpEnemyParty(int extraPoints, int partyLevel)
    {
        int partyLevelIndex = partyLevel - 1;

        int[,] levelingGrid = gameManager.enemyLevelingGrid;

        foreach (GameObject enemyObj in enemyParty)
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();

            int levelToBe = levelingGrid[partyLevelIndex, enemy.combatTier];

            enemy.LevelUp(levelToBe);
        }

        for (int i = extraPoints; i > 0; i--)
        {
            int index = Random.Range(0, enemyParty.Count);
            enemyParty[index].GetComponent<Enemy>().LevelUp(enemyParty[index].GetComponent<Enemy>().Level + 1);
        }
    }

    private GameObject GetEnemy (int enemyTier)
    {
        switch (enemyTier)
        {
            case 1:
                return gameManager.AllTier1Enemies[Random.Range(0, gameManager.AllTier1Enemies.Count)];

            case 2:
                return gameManager.AllTier2Enemies[Random.Range(0, gameManager.AllTier2Enemies.Count)];

            case 3:
                return gameManager.AllTier3Enemies[Random.Range(0, gameManager.AllTier3Enemies.Count)];

            default:
                Debug.LogError("GetEnemy() variable 'enemyTier' is not in range, 1 - 3." +
                                "\n enemyTier = " + enemyTier.ToString());

                return gameManager.AllTier3Enemies[Random.Range(0, gameManager.AllTier3Enemies.Count)];
        }
    }
    /*
    private int GenerateDifficulty()
    {
        int randInt = Random.Range(0, 100);

        int[] easyRange = new int[2] 
        {
            0,
            0 + difficultyChances.x - 1
        };

        int[] moderateRange = new int[2]
        {
            0 + difficultyChances.x,
            0 + difficultyChances.x + difficultyChances.y - 1
        };

        int[] hardRange = new int[2]
        {
            0 + difficultyChances.x + difficultyChances.y,
            100
        };

        if (randInt >= easyRange[0] && randInt < easyRange[1])
        {
            return 0;
        }
        else if (randInt >= moderateRange[0] && randInt < moderateRange[1])
        {
            return 1;
        }
        else if (randInt >= hardRange[0] && randInt < hardRange[1])
        {
            return 2;
        }
        else
        {
            return 0;
        }
    }
    */
}
