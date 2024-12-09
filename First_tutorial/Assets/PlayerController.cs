using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float speed = 10f;
    Rigidbody rb; // cache
    private bool isGround = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 120;

        Debug.Log(transform.position);

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // 프레임마다 속도가 다르다. 어떤 프레임은 0.1초가 걸리고 어떤건 0.3초가 걸리기도 한다.
        //Debug.Log(Time.deltaTime);
        //transform.position += new Vector3(0.1f, 0, 0.1f) * Time.deltaTime * speed; // deltaTime을 곱하면 균등한 속도로 캐릭터가 이동하게 된다.
        //transform.position += new Vector3(0.1f, 0, 0.1f);
        //transform.position += new Vector3(0.1f, 0, 0.1f);

        float horizontal = Input.GetAxis("Horizontal"); // Input의 값들은 Unity의 Project Settings의 Input Manager에서 확인 할 수 있다.
        float vertical = Input.GetAxis("Vertical");

        //Debug.Log($"keyborad h:{horizontal} v:{vertical}");
        //Vector3 mov = new Vector3 (horizontal, 0, vertical);
        //Vector3 movNormalized = mov.normalized * Time.deltaTime * speed; // normalized = 키보드 세기를 1로 고정
        ////transform.position += movNormalized;
        //transform.Translate(movNormalized); // + 연산과 동일한 기능

        Vector3 mov = new Vector3(horizontal, 0, vertical);
        Vector3 movNormalized = mov.normalized * speed;
        rb.linearVelocity = new Vector3(movNormalized.x, rb.linearVelocity.y, movNormalized.z); // linearVelocity를 사용하면 deltaTime을 계산하지 않아도 자동 설정되어있다.

        if (Input.GetButtonDown("Jump") && isGround)
        {
            Jump();
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 5f, rb.linearVelocity.z);
        isGround = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.tag == "Enemy")
        {
            EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();
            enemyController.Attack();
        }

        if(collision.gameObject.name == "Plane")
        {
            isGround = true;
        }
    }
}
