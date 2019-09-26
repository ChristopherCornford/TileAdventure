using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accessory : Item
{
    public Gem gemSlot;
    public Accessory EquipItem(HeroClass hero)
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
}
