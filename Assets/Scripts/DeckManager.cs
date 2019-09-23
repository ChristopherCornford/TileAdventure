using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    private GameManager gameManager;

    public HandTile[] tileHand;
    public int handSize;
    public int deckSize;
    public int percentVariant;

    public List<TileBase> tileDeck;

    private int handSlotsExhausted;

    public bool endTilePlacement;

    private void Awake()
    {
        gameManager = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
    }

    public void StartRound()
    {
       for(int i = 0; i < handSize; i++)
        {
            tileHand[i] = this.transform.GetChild(i).GetComponent<HandTile>();
        }

        tileDeck = CreateTileDeck(deckSize);

        for (int i = 0; i < handSize; i++)
        {
            RefreshHand(i);
        }
    }

    public void RefreshHand(int index)
    {
        if (tileDeck.Count > 0)
        {
            tileHand[index].heldTile = tileDeck[0] as Tile;
            tileHand[index].tile = tileDeck[0];
            tileHand[index].myIndex = index;

            tileDeck.RemoveAt(0);
        }
        else if (tileDeck.Count == 0)
        {
            Destroy(tileHand[index].gameObject);
            handSlotsExhausted++;

            if(handSlotsExhausted == handSize)
            {
                endTilePlacement = true;
            }
        }
    }

    private List<TileBase> CreateTileDeck(int size)
    {
        List<TileBase> tempDeck = new List<TileBase>(size);

        for (int i = 0; i < size; i++)
        {
            int randInt = Random.Range(0, 100);

            if(randInt > percentVariant)
            {
                tempDeck.Add(gameManager.baseTileReference[Random.Range(0, gameManager.baseTileReference.Count)]);
            }
            else
            {
                tempDeck.Add(gameManager.variantTileReference[Random.Range(0, gameManager.variantTileReference.Count)]);
            }
            
        }

        return tempDeck;
    }
}
