using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class HandTile : MonoBehaviour
{
    public Grid grid;
    public Tilemap tileMap;

    public Tile heldTile;
    public Sprite heldSprite;

    public TileBase tile;

    public int myIndex;

    DeckManager deckManager;
    GameManager gameManager;

    private void Awake()
    {
        grid = GameObject.FindObjectOfType(typeof(Grid)) as Grid;
        tileMap = GameObject.FindObjectOfType(typeof(Tilemap)) as Tilemap;
        deckManager = GameObject.FindObjectOfType(typeof(DeckManager)) as DeckManager;
        gameManager = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
    }

    private void LateUpdate()
    {
        if (gameManager.isRoundProgressing)
        {
            if (tile == null)
            {
                deckManager.RefreshHand(myIndex);
            }

            this.GetComponent<Image>().sprite = heldTile.sprite;
            heldSprite = heldTile.sprite;
        }
    }
}
