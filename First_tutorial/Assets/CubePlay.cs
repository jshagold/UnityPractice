using System.Collections;
using UnityEngine;

public class CubePlay : MonoBehaviour
{
    private Animator animator; // cache
    private bool isAttack = false;

    //private CubeSoundPlay cubeSoundPlay; // cache
    private SoundManager soundManager; // cache

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();


        //cubeSoundPlay = GetComponent<CubeSoundPlay>();
        soundManager = FindAnyObjectByType<SoundManager>();

        soundManager.PlaySound(2, false);
        Invoke("FadeOutSound", 1f);

    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        if(Mathf.Abs(horizontal) > Mathf.Epsilon) // Epsilon -> float�� 0f�� �Է��ϸ� �ε��Ҽ��� �κ��� �ν��� �� �ȵ� ���� �ֱ⿡ Epsilon�� ����ؼ� ��Ȯ�� 0�� Ȯ��
        {
            MoveState();
        }
        else
        {
            IdleState();
        }

        if(Input.GetKeyDown(KeyCode.Z))
        {
            AttackState();
        }
    }

    private void AttackState()
    {
        isAttack = true;
        animator.Play("Attack");
        StartCoroutine(C_AttackStateFinish());

        //cubeSoundPlay.PlaySound(1, true);
        soundManager.PlaySound(1, true);
    }

    private IEnumerator C_AttackStateFinish()
    {
        // �ִϸ��̼� ��� �ð��� �޾ƿ��� ����� �ִ�.
        yield return new WaitForSeconds(1f);
        isAttack = false;
    }

    private void MoveState()
    {
        if (!isAttack)
        {
            animator.Play("Move");

            //cubeSoundPlay.PlaySound(0, false);
            soundManager.PlaySound(0, false);
        }
    }

    private void IdleState()
    {
        if (!isAttack)
        {
            animator.Play("Idle");
        }
    }


    private void FadeOutSound()
    {
        soundManager.Fadeout(2);
    }
}
