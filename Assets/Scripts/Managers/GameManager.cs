﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GameManager>();

                if (instance == null)
                {
                    GameObject container = new GameObject("Game Manager");
                    instance = container.AddComponent<GameManager>();
                }
            }

            return instance;
        }
    }
    

    [Header("Game Phases")]
    public GamePhase currentGamePhase;
    
    [Header("Game Variables")]
    public int currentRound;
    public bool isRoundProgressing;

    public int currentTurn;

    public Season currentSeason;

    [Header("Combat Variables")]
    public int enemyPartySizeLimit;
    public GameObject combatPositions;

    [Header("Base Tile References")]
    public List<TileBase> AllBaseTTiles;
    public List<TileBase> AllBaseLTiles;
    public List<TileBase> AllBaseXTiles;
    public List<TileBase> AllBaseBossTiles;

    
    [Header("Variant Enemy Tile References")]
    public List<VariantTiles> AllVariantTTiles_Enemy;
    public List<VariantTiles> AllVariantLTiles_Enemy;
    public List<VariantTiles> AllVariantXTiles_Enemy;

    [Header("Variant Adventure Tile References")]
    public List<VariantTiles> AllVariantTTiles_Adventure;
    public List<VariantTiles> AllVariantLTiles_Adventure;
    public List<VariantTiles> AllVariantXTiles_Adventure;

    [Header("Variant Treasure Tile References")]
    public List<VariantTiles> AllVariantTTiles_Treasure;
    public List<VariantTiles> AllVariantLTiles_Treasure;
    public List<VariantTiles> AllVariantXTiles_Treasure;

    [Header("Variant Boss Tiles References")]
    public List<VariantTiles> AllBossTiles;

    [Header("Character References")]
    public List<HeroClass> AllHeroClasses;
    public List<GameObject> AllEnemies;
    public List<GameObject> AllTier1Enemies;
    public List<GameObject> AllTier2Enemies;
    public List<GameObject> AllTier3Enemies;
    public List<GameObject> AllBosses;
    public GameObject enemyPartyTemplate;
  

    [Header("Item References")]
    public List<Armor> AllArmor;
    public List<Accessory> AllAccessories;
    public List<Weapon> AllWeapons;
    public List<Gem> allGems;

  
    [Header("Script References")]
    public GameBoardManager gameBoardManager;
    public DeckManager deckManager;
    public CanvasManager canvasManager;
    public PlayerBehaviour playerBehaviour;
    public PartyBehaviour partyBehaviour;
    public CombatManager combatManager;
    public ShopManager shopManager;

    //Combat Enemy Leveling Grid

    public readonly int[,] enemyLevelingGrid = new int[10, 4]
    {
        // Read As : {partyLevel, Tier 1 Level, Tier 2 Level, Tier 3 Level}

        {1, 1, 0, 0 },

        {2, 1, 0, 0 },

        {3, 2, 1, 0 },

        {4, 2, 1, 0 },

        {5, 3, 2, 1 },

        {6, 3, 2, 1 },

        {7, 4, 3, 2 },

        {8, 4, 3, 2 },

        {9, 5, 4, 3 },

        {10, 5, 4, 3 },
    };

    
    private void Awake()
    {
        SetScriptReferences();
    }

    public void BeginGame(HeroClass startingHero)
    {
        if (currentRound == 1 && currentTurn == 1)
        {
            gameBoardManager.StartRound();
            deckManager.StartRound();
            canvasManager.StartCoroutine("Transition", false);
            isRoundProgressing = true;
        }

        partyBehaviour.AddCharacterToParty(startingHero.name, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(currentGamePhase == GamePhase.End)
        {
            ProceedToNextGamePhase();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }

        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach(HeroClass hero in partyBehaviour.heroParty)
                hero.LevelUp(partyBehaviour.heroParty[0].Level + 1);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine("ProceedToNextRound");
        }
        */
    }
    
    public void ProceedToNextGamePhase(bool isGoingToCombat = false)
    {
        int currentPhase = (int)currentGamePhase;

        switch(currentPhase)
        {
            case 1:
                currentGamePhase = GamePhase.Travel;
                Debug.Log("Current Game Phase:" + currentGamePhase.ToString());
                break;

            case 2:
                currentGamePhase = GamePhase.Event;
                Debug.Log("Current Game Phase:" + currentGamePhase.ToString());
                break;

            case 3:

                if (isGoingToCombat)
                {
                    currentGamePhase = GamePhase.Combat;
                    Debug.Log("Current Game Phase:" + currentGamePhase.ToString());
                    combatManager.StartCombat();
                }
                else
                {
                    currentGamePhase = GamePhase.End;
                    Debug.Log("Current Game Phase:" + currentGamePhase.ToString());
                }
                   
                break;

            case 4:
                currentGamePhase = GamePhase.End;
                Debug.Log("Current Game Phase:" + currentGamePhase.ToString());

                break;

            case 5:
                
                currentTurn++;
                currentGamePhase = GamePhase.TilePlacement;


                Debug.Log("Current Turn:" + currentTurn.ToString());
                Debug.Log("Current Game Phase:" + currentGamePhase.ToString());

                break;
        }
    }

    public IEnumerator ProceedToNextRound()
    {
        isRoundProgressing = false;

        currentGamePhase = GamePhase.SwitchingRounds;

        canvasManager.StartCoroutine("Transition", true);

        yield return new WaitForSeconds(canvasManager.transitionTime);

        int nextSeasonInt = (int)currentSeason + 1;

        nextSeasonInt = (nextSeasonInt == 4) ?  0 : nextSeasonInt; 

        currentSeason = (Season)nextSeasonInt;

        gameBoardManager.StartRound(false);
        deckManager.StartRound();
        partyBehaviour.StartRound();
        playerBehaviour.StartRound();

        currentRound++;
        currentTurn = 1;

        isRoundProgressing = true;

        canvasManager.StartCoroutine("Transition", false);

        yield return new WaitForSeconds(canvasManager.transitionTime);

        currentGamePhase = GamePhase.TilePlacement;

        yield return null;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void SetScriptReferences()
    {
        gameBoardManager = FindObjectOfType(typeof(GameBoardManager)) as GameBoardManager;
        deckManager = FindObjectOfType(typeof(DeckManager)) as DeckManager;
        canvasManager = FindObjectOfType(typeof(CanvasManager)) as CanvasManager;
        playerBehaviour = FindObjectOfType(typeof(PlayerBehaviour)) as PlayerBehaviour;
        partyBehaviour = FindObjectOfType(typeof(PartyBehaviour)) as PartyBehaviour;
        combatManager = FindObjectOfType(typeof(CombatManager)) as CombatManager;
        shopManager = FindObjectOfType(typeof(ShopManager)) as ShopManager;
    }

    public enum GamePhase
    {
        TilePlacement = 1,
        Travel,
        Event,
        Combat,
        End,
        SwitchingRounds,
    }

    public enum Season
    {
        Summer,
        Fall,
        Winter,
        Spring
    }
}
