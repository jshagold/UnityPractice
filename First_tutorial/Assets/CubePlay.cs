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
        if(Mathf.Abs(horizontal) > Mathf.Epsilon) // Epsilon -> float은 0f로 입력하면 부동소수점 부분이 인식이 잘 안될 때가 있기에 Epsilon을 사용해서 정확한 0을 확인
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
        // 애니메이션 재생 시간을 받아오는 방법도 있다.
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
