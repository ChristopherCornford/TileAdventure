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

    [Header("Base Tile Variables")]
    public float percentT;
    public float percentL;
    public float percentX;

    [Header("Variant Tile Variables")]
    public int percentVariant;

    public int percentEnemy;
    public int percentAdventurer;
    public int percentTreasure;

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

        int numOfT = Mathf.RoundToInt(size * (percentT / 100));
        int numOfL = Mathf.RoundToInt(size * (percentL / 100));
        int numOfX = Mathf.RoundToInt(size * (percentX / 100));

        
        for (int i = 0; i < size; i++)
        {
            int randType = Random.Range(0, 100);

            if(randType > percentVariant) //Base Tiles
            {
                if (numOfT > 0)
                {
                    tempDeck.Add(gameManager.AllBaseTTiles[Random.Range(0, gameManager.AllBaseTTiles.Count)]);
                    numOfT--;
                }
                else
                {
                    if (numOfL > 0)
                    {
                        tempDeck.Add(gameManager.AllBaseLTiles[Random.Range(0, gameManager.AllBaseLTiles.Count)]);
                        numOfL--;
                    }
                    else
                    {
                        if (numOfX > 0)
                        {
                            tempDeck.Add(gameManager.AllBaseXTiles[Random.Range(0, gameManager.AllBaseXTiles.Count)]);
                            numOfX--;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            else // Variant Tiles
            {
                int randVariant = Random.Range(0, 100);

                if (randVariant >= 0 && randVariant <= (99 - (100 - percentEnemy))) // Enemy Variant
                {
                    if (numOfT > 0)
                    {
                        tempDeck.Add(gameManager.AllVariantTTiles_Enemy[Random.Range(0, gameManager.AllVariantTTiles_Enemy.Count)]);
                        numOfT--;
                    }
                    else
                    {
                        if (numOfL > 0)
                        {
                            tempDeck.Add(gameManager.AllVariantLTiles_Enemy[Random.Range(0, gameManager.AllVariantLTiles_Enemy.Count)]);
                            numOfL--;
                        }
                        else
                        {
                            if (numOfX > 0)
                            {
                                tempDeck.Add(gameManager.AllVariantXTiles_Enemy[Random.Range(0, gameManager.AllVariantXTiles_Enemy.Count)]);
                                numOfX--;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
                else if (randVariant >= ((99 - (100 - percentEnemy)) + 1) && randVariant <= (99 - (100 - percentTreasure))) //Adventurer Variant
                {
                    if (numOfT > 0)
                    {
                        tempDeck.Add(gameManager.AllVariantTTiles_Adventure[Random.Range(0, gameManager.AllVariantTTiles_Adventure.Count)]);
                        numOfT--;
                    }
                    else
                    {
                        if (numOfL > 0)
                        {
                            tempDeck.Add(gameManager.AllVariantLTiles_Adventure[Random.Range(0, gameManager.AllVariantLTiles_Adventure.Count)]);
                            numOfL--;
                        }
                        else
                        {
                            if (numOfX > 0)
                            {
                                tempDeck.Add(gameManager.AllVariantXTiles_Adventure[Random.Range(0, gameManager.AllVariantXTiles_Adventure.Count)]);
                                numOfX--;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
                else if (randVariant >= ((99 - (100 - percentTreasure)) + 1) && randVariant <= 99) //Treasure Variant
                {
                    if (numOfT > 0)
                    {
                        tempDeck.Add(gameManager.AllVariantTTiles_Treasure[Random.Range(0, gameManager.AllVariantTTiles_Treasure.Count)]);
                        numOfT--;
                    }
                    else
                    {
                        if (numOfL > 0)
                        {
                            tempDeck.Add(gameManager.AllVariantLTiles_Treasure[Random.Range(0, gameManager.AllVariantLTiles_Treasure.Count)]);
                            numOfL--;
                        }
                        else
                        {
                            if (numOfX > 0)
                            {
                                tempDeck.Add(gameManager.AllVariantXTiles_Treasure[Random.Range(0, gameManager.AllVariantXTiles_Treasure.Count)]);
                                numOfX--;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
            }
        }

        for (int j = 10; j > 0; j--)
        {
            tempDeck = ShuffleDeck(tempDeck);
        }
        
        return tempDeck;
    }


    private List<TileBase> ShuffleDeck(List<TileBase> deck)
    {
       
        List<TileBase> tempDeck = new List<TileBase>();

        List<TileBase> topHalf = new List<TileBase>();
        List<TileBase> bottomHalf = new List<TileBase>();

        for (int j = 0; j < deck.Count; j++)
        {
            if (j < (deck.Count / 2))
            {
                topHalf.Add(deck[j]);
            }
            else
            {
                bottomHalf.Add(deck[j]);
            }
        }

        for (int x = 0; x < (deck.Count / 2); x++)
        {
            tempDeck.Add(topHalf[0]);
            topHalf.RemoveAt(0);

            tempDeck.Add(bottomHalf[0]);
            bottomHalf.RemoveAt(0);
        }
        
        return tempDeck;
    }
}
