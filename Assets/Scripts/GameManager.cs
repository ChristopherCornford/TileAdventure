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
    public CombatPhase currentCombatPhase;
    public CharacterActionPhase currentCharacterActionPhase;
    [Space]

    [Header("Game Variables")]
    public int currentLevel;
    public int currentTurn;
    [Space]
    [Header("Tile References")]
    public List<TileBase> allBaseTiles;
    public List<VariantTiles> allVariantTiles;
    public List<GameObject> allEnemies;

    public Dictionary<int, TileBase> baseTileReference = new Dictionary<int, TileBase>();
    public Dictionary<int, VariantTiles> variantTileReference = new Dictionary<int, VariantTiles>();
    public Dictionary<int, GameObject> enemyReference = new Dictionary<int, GameObject>();
    [Space]
    [Header("Script References")]
    public GameBoardManager gameBoardManager;
    public DeckManager deckManager;
    public CanvasManager canvasManager;
    public PlayerBehaviour playerBehaviour;
    public PartyBehaviour partyBehaviour;
    public CombatManager combatManager;
    
    private void Awake()
    {
        SetScriptReferences();
        WriteToDictionaries();
       
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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ProceedToNextGamePhase();
        }

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

    public void ProceedToNextCombatPhase(int currentPhase)
    {

    }

    public void ProceedToNextCharacterActionPhase(int currentPhase)
    {
        
    }

    private void SetScriptReferences()
    {
        gameBoardManager = GameObject.FindObjectOfType(typeof(GameBoardManager)) as GameBoardManager;
        deckManager = GameObject.FindObjectOfType(typeof(DeckManager)) as DeckManager;
        canvasManager = GameObject.FindObjectOfType(typeof(CanvasManager)) as CanvasManager;
        playerBehaviour = GameObject.FindObjectOfType(typeof(PlayerBehaviour)) as PlayerBehaviour;
        partyBehaviour = GameObject.FindObjectOfType(typeof(PartyBehaviour)) as PartyBehaviour;
        combatManager = GameObject.FindObjectOfType(typeof(CombatManager)) as CombatManager;
    }

    private void WriteToDictionaries()
    {
        for (int i = 0; i < allBaseTiles.Count; i++)
        {
            baseTileReference.Add(i, allBaseTiles[i]);
        }
        for (int i = 0; i < allVariantTiles.Count; i++)
        {
            variantTileReference.Add(i, allVariantTiles[i]);
        }
        for (int i = 0; i < allEnemies.Count; i++)
        {
            enemyReference.Add(i, allEnemies[i]);
        }
    }

    #region Enums
    public enum GamePhase
    {
        TilePlacement = 1,
        Travel,
        Event,
        Combat,
        End,
    }

    public enum CombatPhase
    {
        Initative = 0,
        TurnBegin,
        CharacterAction,
        TurnEnd,
    }

    public enum CharacterActionPhase
    {
        TurnBegin = 1,
        FirstAbilities,
        SelectTarget,
        Attack,
        SecondaryAbilities,
        TurnEnd,
    }
    #endregion
}
