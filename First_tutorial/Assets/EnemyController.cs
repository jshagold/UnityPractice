using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //[SerializeField] GameObject gameObject;

    //PlayerController playerController; // cache 1

    [SerializeField] LayerMask layerMask;


    //[�ﰢ�Լ� �̵�] 
    private int degree = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //playerController = FindAnyObjectByType<PlayerController>();

        // [LayerMask]
        layerMask = 1 << LayerMask.NameToLayer("Player"); // Unity���� 6�� ���̾ Player
        layerMask = 1 << 6; // 0000 0000 0000 0000 0000 0000 0010 0000
                            //layerMask = 1 << 0 | 1 << 6; // 0000 0000 0000 0000 0000 0000 0000 0001 // 1 << 0
                            // 0000 0000 0000 0000 0000 0000 0010 0000 // 1 << 6
                            // 0000 0000 0000 0000 0000 0000 0010 0001 // 1 << 0 | 1 << 6



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
        //        //transform.LookAt(collider.transform.position);
        //        //collider.GetComponent<PlayerController>().Warning();
        //    }
        //}


        // Rotation�� ���͸� ������ ���� Quaternion�� ������ �ִ�. �׷��� Euler�� ���� ���͸� Quaternion���� ��ȯ�Ѵ�.
        //transform.rotation *= Quaternion.Euler(0, 5, 0);
        //transform.rotation *= Quaternion.Euler(new Vector3(0, 5, 0));
        //transform.Rotate(0,2,0);
        //// [Raycast]
        //if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, Mathf.Infinity, layerMask))
        //{
        //    if (hitInfo.collider.gameObject.tag == "Player")
        //    {
        //        Debug.Log("I'm"+this.gameObject.name+"HIT!!! "+ hitInfo.collider.gameObject.name);
        //        Debug.DrawLine(transform.position, hitInfo.point, Color.red);
        //        hitInfo.collider.GetComponent<PlayerController>().Warning();
        //    }
        //}


        // [�ﰢ�Լ� �̵�] - Raidan
        Debug.Log(Mathf.Cos(Mathf.Deg2Rad * 60)); // 0.5 ���
        //transform.position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * degree), transform.position.y, 0);
        transform.position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * degree), transform.position.y, Mathf.Sin(Mathf.Deg2Rad * degree));
        degree++;
    }

    public void Attack()
    {
        Debug.Log($"Enemy Attack ()");
    }
}
