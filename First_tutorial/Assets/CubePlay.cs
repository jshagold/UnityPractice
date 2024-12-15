using System.Collections;
using UnityEngine;

public class CubePlay : MonoBehaviour
{
    private Animator animator; // cache
    private bool isAttack = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        if(Mathf.Abs(horizontal) > Mathf.Epsilon) // Epsilon -> float은 0f로 입력하면 부동소수점 부분이 인식이 잘 안될 때가 있기에 Epsilon을 사용해서 정확한 0을 확인
        {
            if(!isAttack)
                animator.Play("Move");
        }
        else
        {
            if(!isAttack)
                animator.Play("Idle");
        }

        if(Input.GetKeyDown(KeyCode.A))
        {
            AttackState();
        }
    }

    private void AttackState()
    {
        isAttack = true;
        animator.Play("Attack");
        StartCoroutine(C_AttackStateFinish());
    }

    private IEnumerator C_AttackStateFinish()
    {
        yield return new WaitForSeconds(1f);
        isAttack = false;
    }
}
