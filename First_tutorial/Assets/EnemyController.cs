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
        // Ư�� ��� ���� 1. ��ü ã�ƿͼ� cache�� ����
        //float distance = Vector3.Distance(transform.position, playerController.transform.position);
        //if(distance < 5)
        //{
        //    transform.transform.LookAt(playerController.transform.position);
        //    playerController.Warning();
        //}

        // Ư�� ��� ���� 2. Collider ���
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
