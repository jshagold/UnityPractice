using System.Collections;
using UnityEngine;

public class MonsterA : MonoBehaviour
{

    [SerializeField][Range(10f, 20f)] private float speed = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    private void OnEnable()
    {
        StartCoroutine(C_DisableObj());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }


    private IEnumerator C_DisableObj()
    {
        yield return new WaitForSeconds(2f);
        //Destroy(this.gameObject); // 삭제하는데에 리소스를 꽤 잡아먹는다.
        this.gameObject.SetActive(false); // Object 비활성화
    }

    private void OnDisable()
    {
        
    }
}
