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


    [Header("Tile References")]
    public List<TileBase> AllBaseTiles;
    public List<VariantTiles> AllVariantTiles;

    [Header("Character References")]
    public List<HeroClass> AllHeroClasses;
    public List<GameObject> AllEnemies;
  

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
}
