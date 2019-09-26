using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Object References")]
    public GameObject promptPanel;
    public GameObject shoppingPanel;
    public GameObject itemPanel;
    public GameObject mercenaryPanel;
    public GameObject currentPartyPanel;

    [Header("Button Templates")]
    public GameObject itemTemplate;
    public GameObject mercenaryTemplate;

    [Header("Current Items/Mercenaries Available")]
    public List<Item> itemsForPurchase;
    public List<HeroClass> mercenariesForHire;

    [Header("Item Sale Variables")]
    public int numOfItemsToSell = 9;

    [Header("Mercenary Variables")]
    public int numOfMercenariesForHire = 3;

    [Header("Script References")]
    public GameManager gameManager;
    public PartyBehaviour partyBehaviour;

    private Item currentlySelectedItem;
    private HeroClass currentlySelectedMercenary;

    /*
     * TODO:
        -Generate items and merc to sell, by referencing the respective Lists.

     *Steps for using Shop
     *  If Player accidentally closes the menu early, they may click on the town tile before placing another tile to reopen the menu;
     * 
     *  1. Player Arrives in town
     *  2. Player is prompted to choose item shop or hero shop or leave
     *  3. Player shops (Detailed Below)
     *  4. When exiting the choosen shop, player is prompted again to choose shop or leave
        5. Player leaving moves the Current Phase to Tile Placement.

     *  3a. Player is presented with a grid of items
     *      Player selects an item, button opens second panel to select which hero to buy the item for.
     *      Player is returned to the item grid
     *      
     *  3b. Player is presented with a grid of Mercenaries
     *      Player selects an Mercenary, button opens second panel of current Heros in party and the player can select which slot to place the new hero.
     *      Player is returned to the mercenary grid;
    */
    private void Awake()
    {
        gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
        partyBehaviour = FindObjectOfType(typeof(PartyBehaviour)) as PartyBehaviour;
    }


    public void OpenShop()
    {
        promptPanel.SetActive(true);

        PopulateSalesLists();

        
    }

    void PopulateSalesLists()
    {
        itemsForPurchase.Clear();
        mercenariesForHire.Clear();

        for (int i = 0; i < numOfItemsToSell / 3; i++)
        {
            itemsForPurchase.Add(gameManager.AllArmor[Random.Range(0, gameManager.AllArmor.Count)]);
            itemsForPurchase.Add(gameManager.AllAccessories[Random.Range(0, gameManager.AllAccessories.Count)]);
            itemsForPurchase.Add(gameManager.AllWeapons[Random.Range(0, gameManager.AllWeapons.Count)]);
        }

        for (int i = 0; i < numOfMercenariesForHire; i++)
        {
            mercenariesForHire.Add(gameManager.AllHeroClasses[Random.Range(0, gameManager.AllHeroClasses.Count)]);
        }
    }

    void GenerateShop(ShopType shopType)
    {
        switch (shopType)
        {
            case ShopType.Items:

                foreach(Item item in itemsForPurchase)
                {
                    GameObject newItem = Instantiate(itemTemplate, itemPanel.transform);
                    newItem.name = item.name;
                    newItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.name;
                    newItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.statToBuff + " +" + item.buffValue;
                    newItem.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = item.goldPrice + " Gold";
                    newItem.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { OpenCurrentPartyPanel(); currentlySelectedItem = item; });
                }

                break;

            case ShopType.Mercenaries:

                foreach (HeroClass mercenary in mercenariesForHire)
                {
                    GameObject newMercenary = Instantiate(mercenaryTemplate, mercenaryPanel.transform);

                }

                break;
        }
    }

    private void OpenCurrentPartyPanel()
    {
        currentPartyPanel.SetActive(true);

        foreach (HeroClass hero in partyBehaviour.heroParty)
        {
            GameObject newHero = Instantiate(mercenaryTemplate, currentPartyPanel.transform);
        }
    }

    public enum ShopType { Items, Mercenaries }
}
