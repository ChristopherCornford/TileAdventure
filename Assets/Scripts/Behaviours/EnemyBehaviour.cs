using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public PartyBehaviour partyBehaviour;

    public Sprite previewSprite;
    
    public List<GameObject> enemyParty = new List<GameObject>();
    public List<Enemy> enemyClasses = new List<Enemy>();

    public void GenerateEnemyParty(int partyLevel, int[,] levelGrid)
    {
        GameManager gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;

        int[,] levelingGrid = gameManager.enemyLevelingGrid;


    }
}
