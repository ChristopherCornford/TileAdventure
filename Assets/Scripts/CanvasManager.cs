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

    private void Start()
    {
        gameManager = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;

        graphicRaycaster = this.GetComponent<GraphicRaycaster>();

        eventSystem = GetComponent<EventSystem>();
    }

    private void LateUpdate()
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

                if (results.Count != 1)
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
