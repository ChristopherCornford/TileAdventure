using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class CanvasManager : MonoBehaviour
{
    
    GraphicRaycaster graphicRaycaster;
    PointerEventData eventData;
    EventSystem eventSystem;

    public GameManager gameManager;

    public PlayerBehaviour playerBehaviour;

    [SerializeField]
    private bool canSelectNewTile;

    public CanvasGroup canvasGroup;

    public Image transitionPanel;
    public float transitionTime;
    public bool transitionComplete;

    [Header("Title Screen")]
    public Image startingHero;
    public GameObject heroOptions;
    public HeroClass startingHeroClass;

    public Button startButton;



    private void Start()
    {
        gameManager = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;

        graphicRaycaster = this.GetComponent<GraphicRaycaster>();

        eventSystem = GetComponent<EventSystem>();

        startButton.onClick.AddListener(delegate { gameManager.BeginGame(startingHeroClass); });
    }

    private void LateUpdate()
    {
        if (gameManager.isRoundProgressing)
        {
            canSelectNewTile = (gameManager.currentGamePhase == GameManager.GamePhase.TilePlacement) ? true : false;

            if (canSelectNewTile)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    eventData = new PointerEventData(eventSystem);

                    eventData.position = Input.mousePosition;

                    List<RaycastResult> results = new List<RaycastResult>();

                    graphicRaycaster.Raycast(eventData, results);

                    if (results.Count == 1)
                    {
                        foreach (RaycastResult result in results)
                        {
                            if (result.gameObject.GetComponent<HandTile>())
                            {
                                playerBehaviour.hasTile = true;
                                playerBehaviour.currentGrid = result.gameObject.GetComponent<HandTile>().grid;
                                playerBehaviour.currentTileMap = result.gameObject.GetComponent<HandTile>().tileMap;
                                playerBehaviour.tile = result.gameObject.GetComponent<HandTile>().heldTile;
                                playerBehaviour.previewSprite.sprite = result.gameObject.GetComponent<HandTile>().heldSprite;
                                playerBehaviour.currentHandChoice = result.gameObject;
                            }
                        }
                    }
                }
            }
        }
    }

    public IEnumerator Transition (bool toWhite = true)
    {
        transitionComplete = false;
        
        if (toWhite)
        {
            canvasGroup.alpha = 0;

            while (canvasGroup.alpha < 0.95)
            {
                float perc = canvasGroup.alpha;

                Color newColor = new Color(perc, perc, perc);

                transitionPanel.color = newColor;

                canvasGroup.alpha += Time.fixedDeltaTime / transitionTime;

                yield return null;
            }

            canvasGroup.alpha = 1;
        }
        else
        {
            canvasGroup.alpha = 1;

            while (canvasGroup.alpha > 0.05)
            {
                float perc = canvasGroup.alpha;

                Color newColor = new Color(perc, perc, perc);

                transitionPanel.color = newColor;
                
                canvasGroup.alpha -= Time.fixedDeltaTime / transitionTime;

                yield return null;
            }

            canvasGroup.alpha = 0;
        }

        transitionComplete = true;
    }
    
    public void SelectStartingHero(HeroClass hero)
    {
        startingHero.sprite = hero.sprite;
        startingHeroClass = hero;

        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(delegate { gameManager.BeginGame(hero); });
    }

}
