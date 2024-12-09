using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //[SerializeField] EnemyManager enemyManager;
    //EnemyManager enemyManager; // cache

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 10;

        //enemyManager = FindObjectOfType<EnemyManager>();
        //enemyManager = FindAnyObjectByType<EnemyManager>();
        //enemyManager.AttackAll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.tag == "Enemy")
        {
            EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();
            enemyController.Attack();
        }
    }
}
