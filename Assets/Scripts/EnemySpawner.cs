using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    float timer = 0f;
    public GameObject[] enemy;

    void Start()
    {

    }

    void Update()
    {
        timer += 1f * Time.deltaTime;

        if (timer > 5f)
        {
            Spawn(enemy[Random.Range(0, enemy.Length)]);
            timer = 0f;
        }
      
    }
    
    void Spawn(GameObject enemy)
    {
        Instantiate(enemy);
    }
}
