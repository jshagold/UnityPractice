using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //[SerializeField] GameObject gameObject;

    //PlayerController playerController; // cache 1

    [SerializeField] LayerMask layerMask;


    //[삼각함수 이동] 
    private int degree = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //playerController = FindAnyObjectByType<PlayerController>();

        // [LayerMask]
        layerMask = 1 << LayerMask.NameToLayer("Player"); // Unity에서 6번 레이어가 Player
        layerMask = 1 << 6; // 0000 0000 0000 0000 0000 0000 0010 0000
                            //layerMask = 1 << 0 | 1 << 6; // 0000 0000 0000 0000 0000 0000 0000 0001 // 1 << 0
                            // 0000 0000 0000 0000 0000 0000 0010 0000 // 1 << 6
                            // 0000 0000 0000 0000 0000 0000 0010 0001 // 1 << 0 | 1 << 6



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
        //        //transform.LookAt(collider.transform.position);
        //        //collider.GetComponent<PlayerController>().Warning();
        //    }
        //}


        // Rotation은 벡터를 받을수 없고 Quaternion을 받을수 있다. 그래서 Euler를 통해 벡터를 Quaternion으로 변환한다.
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


        // [삼각함수 이동] - Raidan
        Debug.Log(Mathf.Cos(Mathf.Deg2Rad * 60)); // 0.5 출력
        //transform.position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * degree), transform.position.y, 0);
        transform.position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * degree), transform.position.y, Mathf.Sin(Mathf.Deg2Rad * degree));
        degree++;
    }

    public void Attack()
    {
        Debug.Log($"Enemy Attack ()");
    }
}
