using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


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
    public int currentLevel;
    public int currentTurn;

    public Season currentSeason;

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
    public List<GameObject> AllTier1EnemyParties;
    public List<GameObject> AllTier2EnemyParties;
    public List<GameObject> AllTier3EnemyParties;
    public List<GameObject> AllTier4EnemyParties;
    public List<GameObject> AllBosses;
  

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
    
    private void Awake()
    {
        SetScriptReferences();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(currentTurn == 0)
        {
            gameBoardManager.StartRound();
            deckManager.StartRound();

            currentSeason = Season.Summer;
        }
        currentTurn = 1;

        partyBehaviour.AddCharacterToParty("Knight");
    }

    // Update is called once per frame
    void Update()
    {
        if(currentGamePhase == GamePhase.End)
        {
            ProceedToNextGamePhase();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach(HeroClass hero in partyBehaviour.heroParty)
                hero.LevelUp(partyBehaviour.heroParty[0].Level + 1);
        }
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

    public GameObject GenerateEnemyParty()
    {
        int lowestLevelInHeroParty = 1;

        foreach (HeroClass hero in partyBehaviour.heroParty)
            if (hero.Level > lowestLevelInHeroParty)
                lowestLevelInHeroParty = hero.Level;
        
        if (playerBehaviour.tile.GetType() == typeof(VariantTiles))
        {
            VariantTiles variantTile = (VariantTiles)playerBehaviour.tile;

            if (variantTile.variantType == VariantTiles.VariantType.Boss)
            {
                return AllBosses[Random.Range(0, AllBosses.Count)];
            }
            else
            {
                switch (lowestLevelInHeroParty)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        return AllTier1EnemyParties[Random.Range(0, AllTier1EnemyParties.Count)];
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                        return AllTier2EnemyParties[Random.Range(0, AllTier2EnemyParties.Count)];
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                        return AllTier3EnemyParties[Random.Range(0, AllTier3EnemyParties.Count)];
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                        return AllTier4EnemyParties[Random.Range(0, AllTier4EnemyParties.Count)];
                }

                return AllTier1EnemyParties[0];
            }
        }

        return null;
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
    }

    public enum Season
    {
        Summer,
        Fall,
        Winter,
        Spring
    }
}
