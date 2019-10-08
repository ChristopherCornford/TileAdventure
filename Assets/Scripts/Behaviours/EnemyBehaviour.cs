using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public PartyBehaviour partyBehaviour;

    public Sprite previewSprite;

    public GameObject baseEnemy;

    public List<GameObject> enemyParty = new List<GameObject>();
    public List<Enemy> enemyClasses = new List<Enemy>();

    private void Awake()
    {
        partyBehaviour = FindObjectOfType(typeof(PartyBehaviour)) as PartyBehaviour;

        if (!baseEnemy.GetComponent<Enemy>().isBoss)
        {
            for (int i = 0; i < partyBehaviour.heroParty.Count; i++)
            {
                enemyParty.Add(baseEnemy);
                enemyClasses.Add(baseEnemy.GetComponent<Enemy>());
            }
            
        }
        else
        {
            enemyParty.Add(baseEnemy);
            enemyClasses.Add(baseEnemy.GetComponent<Enemy>());
        }
        this.name = baseEnemy.name + " x" + enemyParty.Count.ToString();

        Instantiate(baseEnemy, this.transform);
    }
}
