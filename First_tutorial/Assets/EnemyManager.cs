using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{


    // Script로 GameObject 검색하는 방법들

    // Unity에서 드래그해서 연결
    // 직접 연결하는 경우, 속도 빠르다.
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

        // 프리팹으로 적군 오브젝트 생성.
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

        // GetComponentsInChildren 사용
        // 비용은 조금 낮을 수 있다. 숨겨진 오브젝트 검색 가능 (true/false) value값
        //EnemyController[] enemyControllers = gameObject.GetComponentsInChildren<EnemyController>();
        //foreach (EnemyController enemyController in enemyControllers)
        //{
        //    Debug.Log(enemyController.gameObject.name);
        //}

        // SerializeField 사용
        //foreach (EnemyController controller in enemyControllers)
        //{
        //    Debug.Log(controller.gameObject.name + "  aa");
        //}


        // FindObjectsOfType / FindObjectsByType 사용
        // 비용이 높고, 숨겨진 오브젝트는 찾을 수 없다.
        // FindObjectsOftype<T>(true); 를 사용하면 숨겨진 오브젝트도 검색 가능하다.
        //enemyControllers = FindObjectsOfType<EnemyController>();
        //enemyControllers = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        //foreach (EnemyController controller in enemyControllers)
        //{
        //    Debug.Log(controller.gameObject.name + "  bb");
        //}

        // FindGameObjectsWithTag 사용
        // 비용이 높고, 숨겨진 오브젝트는 찾을 수 없다.
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
