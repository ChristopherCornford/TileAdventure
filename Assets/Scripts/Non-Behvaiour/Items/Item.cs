using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Sprite sprite;

    public string statToBuff;

    public int buffValue;

    public int goldPrice;

    public Item EquipItem(HeroClass hero)
    {
        if (this.GetType() == typeof(Armor))
        {
            if (hero.armorSlot != null)
            {
                Destroy(hero.armorSlot.gameObject);
                this.transform.SetParent(hero.transform);
                return this;
            }
            else
            {
                return this;
            }
        }
        else if (this.GetType() == typeof(Accessory))
        {
            if (hero.armorSlot != null)
            {
                Destroy(hero.armorSlot.gameObject);
                this.transform.SetParent(hero.transform);
                return this;
            }
            else
            {
                return this;
            }
        }
        else if (this.GetType() == typeof(Weapon))
        {
            if (hero.weaponSlot != null)
            {
                Destroy(hero.armorSlot.gameObject);
                this.transform.SetParent(hero.transform);
                return this;
            }
            else
            {
                return this;
            }
        }

        return null;
    }
}
