using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : Item
{
    public Gem gemSlot;

    public Item EquipItem(HeroClass hero)
    {
        if(hero.armorSlot != null)
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
}
