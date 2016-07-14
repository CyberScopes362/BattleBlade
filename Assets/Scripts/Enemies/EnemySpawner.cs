using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public float startTime;
    float timer = 0f;
    public GameObject[] enemy;

    void Start()
    {
        timer = startTime;
    }

    void Update()
    {
        timer += 1f * Time.deltaTime;

        if (timer > startTime)
        {
            Spawn(enemy[Random.Range(0, enemy.Length)/*1*/]);
            timer = 0f;
        }
      
    }
    
    void Spawn(GameObject enemy)
    {
        Instantiate(enemy);
    }
}
