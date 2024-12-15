using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //[SerializeField] private float speed = 10f;
    Rigidbody rb; // cache
    private bool isGround = true;

    // Ư�� ��� ���� 1.
    EnemyManager enemyManager;

    CameraShake cameraShake; // cache

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 120;

        Debug.Log(transform.position);

        rb = GetComponent<Rigidbody>();

        // Ư�� ��� ���� 1.
        enemyManager = FindAnyObjectByType<EnemyManager>();

        cameraShake = FindAnyObjectByType<CameraShake>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        // Ư�� ��� ���� 1.
        //foreach (var item in enemyManager.Enemies)
        //{
        //    float distance = Vector3.Distance(transform.position, item.transform.position);
        //    if(distance <= 3f)
        //    {
        //        item.Attack();
        //    }
        //}

        // Ư�� ��� ���� 2.
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);
        foreach (Collider collider in colliders)
        {
            if(collider.gameObject.tag == "Enemy")
            {
                collider.GetComponent<EnemyController>().Attack();
            }
        }

    }

    float yRotation = 0;
    float xRotation = 0;

    void Movement()
    {
        // �����Ӹ��� �ӵ��� �ٸ���. � �������� 0.1�ʰ� �ɸ��� ��� 0.3�ʰ� �ɸ��⵵ �Ѵ�.
        //Debug.Log(Time.deltaTime);
        //transform.position += new Vector3(0.1f, 0, 0.1f) * Time.deltaTime * speed; // deltaTime�� ���ϸ� �յ��� �ӵ��� ĳ���Ͱ� �̵��ϰ� �ȴ�.
        //transform.position += new Vector3(0.1f, 0, 0.1f);
        //transform.position += new Vector3(0.1f, 0, 0.1f);

        //float horizontal = Input.GetAxis("Horizontal"); // Input�� ������ Unity�� Project Settings�� Input Manager���� Ȯ�� �� �� �ִ�.
        //float vertical = Input.GetAxis("Vertical");

        //Debug.Log($"keyborad h:{horizontal} v:{vertical}");
        //Vector3 mov = new Vector3 (horizontal, 0, vertical);
        //Vector3 movNormalized = mov.normalized * Time.deltaTime * speed; // normalized = Ű���� ���⸦ 1�� ����
        ////transform.position += movNormalized;
        //transform.Translate(movNormalized); // + ����� ������ ���

        //Vector3 mov = new Vector3(horizontal, 0, vertical);
        //Vector3 movNormalized = mov.normalized * speed;
        //rb.linearVelocity = new Vector3(movNormalized.x, rb.linearVelocity.y, movNormalized.z); // linearVelocity�� ����ϸ� deltaTime�� ������� �ʾƵ� �ڵ� �����Ǿ��ִ�.



        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 mov = new Vector3(horizontal, 0, vertical); // 1,0,0
        Vector3 worldPos = transform.TransformDirection(mov); // to world coordinate
        transform.Translate(worldPos * Time.deltaTime * 10f, Space.World);

        // [���콺�� ȭ�� ȸ��, ������ ���� ����]
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * 100;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * 100;
        Debug.Log($"{mouseX} {mouseY}");

        yRotation += mouseX;
        xRotation += mouseY;
        Mathf.Clamp(xRotation, -90, 90);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);


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

    public void Warning()
    {
        Debug.Log("Warning!!!");
    }

    public void Attack()
    {
        Debug.Log("Player Attack!");
        cameraShake.Shake(0.1f, 0.01f);
    }
}
