using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{


    // Script�� GameObject �˻��ϴ� �����

    // Unity���� �巡���ؼ� ����
    // ���� �����ϴ� ���, �ӵ� ������.
    //[SerializeField] private EnemyController[] enemyControllers;


    [SerializeField] GameObject enemyPrefab;
    private List<EnemyController> enemyControllers = new List<EnemyController>();

    public List<EnemyController> Enemies {
        get
        {
            return enemyControllers;
        }
    }


    [SerializeField] Transform targetPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        Debug.Log(gameObject.name);

        // ���������� ���� ������Ʈ ����.
        for(int i = 0; i < 4; i++)
        {
            var extra = this.gameObject.transform;
            extra.position += new Vector3(1,0,0);
            GameObject gameObject = Instantiate(enemyPrefab, extra);
            gameObject.name = $"prefab{i}";
            EnemyController enemyController = gameObject.GetComponent<EnemyController>();
            enemyControllers.Add(enemyController);
            enemyController.SetInit(targetPos);
        }




        //transform.position
        //transform.rotation
        //transform.localScale
        //BoxCollider boxCollider = GetComponent<BoxCollider>();

        // GetComponentsInChildren ���
        // ����� ���� ���� �� �ִ�. ������ ������Ʈ �˻� ���� (true/false) value��
        //EnemyController[] enemyControllers = gameObject.GetComponentsInChildren<EnemyController>();
        //foreach (EnemyController enemyController in enemyControllers)
        //{
        //    Debug.Log(enemyController.gameObject.name);
        //}

        // SerializeField ���
        //foreach (EnemyController controller in enemyControllers)
        //{
        //    Debug.Log(controller.gameObject.name + "  aa");
        //}


        // FindObjectsOfType / FindObjectsByType ���
        // ����� ����, ������ ������Ʈ�� ã�� �� ����.
        // FindObjectsOftype<T>(true); �� ����ϸ� ������ ������Ʈ�� �˻� �����ϴ�.
        //enemyControllers = FindObjectsOfType<EnemyController>();
        //enemyControllers = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        //foreach (EnemyController controller in enemyControllers)
        //{
        //    Debug.Log(controller.gameObject.name + "  bb");
        //}

        // FindGameObjectsWithTag ���
        // ����� ����, ������ ������Ʈ�� ã�� �� ����.
        //GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Enemy"); 
        //foreach (GameObject gameObject in gameObjects)
        //{
        //    enemyControllers.Add(gameObject.GetComponent<EnemyController>());
        //}
        //foreach (EnemyController controller in enemyControllers)
        //{
        //    Debug.Log(controller.gameObject.name + "  cc");
        //}
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    //public void AttackAll()
    //{
    //    foreach (EnemyController controller in enemyControllers)
    //    {
    //        controller.Attack();
    //    }
    //}
}
