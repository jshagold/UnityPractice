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
        if(Mathf.Abs(horizontal) > Mathf.Epsilon) // Epsilon -> float�� 0f�� �Է��ϸ� �ε��Ҽ��� �κ��� �ν��� �� �ȵ� ���� �ֱ⿡ Epsilon�� ����ؼ� ��Ȯ�� 0�� Ȯ��
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
