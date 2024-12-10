using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] GameObject gameObject;

    //PlayerController playerController; // cache 1


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //playerController = FindAnyObjectByType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        // 특정 대상 감지 1. 객체 찾아와서 cache에 저장
        //float distance = Vector3.Distance(transform.position, playerController.transform.position);
        //if(distance < 5)
        //{
        //    transform.transform.LookAt(playerController.transform.position);
        //    playerController.Warning();
        //}

        // 특정 대상 감지 2. Collider 사용
        //Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);
        //foreach (Collider collider in colliders)
        //{
        //    if (collider.gameObject.tag == "Player")
        //    {
        //        transform.LookAt(collider.transform.position);
        //        collider.GetComponent<PlayerController>().Warning();
        //    }
        //}
        
    }

    public void Attack()
    {
        Debug.Log($"Enemy Attack ()");
    }
}
