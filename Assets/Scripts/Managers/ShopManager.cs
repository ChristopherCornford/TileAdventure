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


    public void OpenShop(ShopType shopType)
    {
        promptPanel.SetActive(true);

        PopulateSalesLists();

        GenerateShop(shopType);
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
                    newItem.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = item.goldPrice.ToString() + " Gold";
                    newItem.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { OpenCurrentPartyPanel(shopType); currentlySelectedItem = item; });
                }

                break;

            case ShopType.Mercenaries:

                foreach (HeroClass mercenary in mercenariesForHire)
                {
                    GameObject newMercenary = Instantiate(mercenaryTemplate, mercenaryPanel.transform);
                    newMercenary.name = mercenary.name;
                    newMercenary.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = mercenary.name;
                    newMercenary.transform.GetChild(1).GetComponent<Image>().sprite = mercenary.sprite;
                    newMercenary.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = mercenary.PrimaryStat;
                    newMercenary.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = mercenary.goldCost.ToString() + " Gold";
                    newMercenary.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { OpenCurrentPartyPanel(shopType); currentlySelectedMercenary = mercenary; });
                }

                break;
        }
    }

    private void OpenCurrentPartyPanel(ShopType shopType)
    {
        currentPartyPanel.SetActive(true);

        foreach (HeroClass hero in partyBehaviour.heroParty)
        {
            if (hero != null)
            {
                GameObject newHero = Instantiate(mercenaryTemplate, currentPartyPanel.transform);
                newHero.name = hero.name;
                newHero.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = hero.name;
                newHero.transform.GetChild(1).GetComponent<Image>().sprite = hero.sprite;
                newHero.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { EquipCurrentThing(shopType, hero); });
            }
            
        }
    }

    private void EquipCurrentThing(ShopType shopType, HeroClass hero)
    {
        switch (shopType)
        {
            case ShopType.Items:

                if(currentlySelectedItem.GetType() == typeof(Armor))
                {
                    hero.armorSlot = currentlySelectedItem as Armor;
                        
                }
                else if(currentlySelectedItem.GetType() == typeof(Accessory))
                {
                    hero.accessorySlot = currentlySelectedItem as Accessory;
                }
                else if(currentlySelectedItem.GetType() == typeof(Weapon))
                {
                    hero.weaponSlot = currentlySelectedItem as Weapon;
                }

                break;

            case ShopType.Mercenaries:

                hero = currentlySelectedMercenary;

                break;
        }
    }

    public enum ShopType { Items, Mercenaries }
}
