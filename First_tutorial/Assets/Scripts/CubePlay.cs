using System.Collections;
using UnityEngine;

public enum CUBE_PLAY_STATE_TYPE
{
    idle,
    move,
    attack,
}
public class CubePlay : MonoBehaviour
{
    private Animator animator; // cache

    // [Sound]
    //private CubeSoundPlay cubeSoundPlay; // cache
    private SoundManager soundManager; // cache

    // [Effect]
    //[SerializeField] private GameObject effectPrefab;
    [SerializeField] private Transform effectPos;
    private EffectManager effectManager; // cache

    private CUBE_PLAY_STATE_TYPE currentState = CUBE_PLAY_STATE_TYPE.idle;

    public bool IsMove
    {
        get
        {
            return currentState != CUBE_PLAY_STATE_TYPE.attack;
        }
    }

    public bool IsIdle
    {
        get
        {
            return currentState != CUBE_PLAY_STATE_TYPE.attack;
        }
    }

    public bool IsDuplicationState
    {
        get
        {
            return currentState == CUBE_PLAY_STATE_TYPE.attack;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();

        //[Sound]
        //cubeSoundPlay = GetComponent<CubeSoundPlay>();
        //soundManager = FindAnyObjectByType<SoundManager>();
        //soundManager.PlaySound(2, false);
        //Invoke("FadeOutSound", 1f);


        //Instantiate(effectPrefab, effectPos);
        effectManager = FindAnyObjectByType<EffectManager>();
        effectManager.PlayEffect(0, effectPos);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizontal) > Mathf.Epsilon) // Epsilon -> float은 0f로 입력하면 부동소수점 부분이 인식이 잘 안될 때가 있기에 Epsilon을 사용해서 정확한 0을 확인
        {
            if(IsMove)
                ChangeState(CUBE_PLAY_STATE_TYPE.move);
        }
        else
        {
            if(IsIdle)
                ChangeState(CUBE_PLAY_STATE_TYPE.idle);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            ChangeState(CUBE_PLAY_STATE_TYPE.attack);
        }

        UpdateState();
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case CUBE_PLAY_STATE_TYPE.idle:
                IdleUpdateState();
                break;
            case CUBE_PLAY_STATE_TYPE.move:
                MoveUpdateState();
                break;
            case CUBE_PLAY_STATE_TYPE.attack:
                AttackUpdateState();
                break;
        }
    }

    public void ChangeState(CUBE_PLAY_STATE_TYPE stateType)
    {
        if(IsDuplicationState)
        {
            if (this.currentState == stateType)
            {
                return;
            }
        }
        

        this.currentState = stateType; 

        switch (currentState)
        {
            case CUBE_PLAY_STATE_TYPE.idle:
                IdleEnterState();
                break;
            case CUBE_PLAY_STATE_TYPE.move:
                MoveEnterState();
                break;
            case CUBE_PLAY_STATE_TYPE.attack:
                AttackEnterState();
                break;
        }
    }

    private void AttackEnterState()
    {
        animator.Play("Attack");
        StartCoroutine(C_AttackStateFinish());

        //[Sound]
        //cubeSoundPlay.PlaySound(1, true);
        //soundManager.PlaySound(1, true);
    }

    private void AttackUpdateState()
    {
        
    }

    private IEnumerator C_AttackStateFinish()
    {
        // 애니메이션 재생 시간을 받아오는 방법도 있다.
        yield return new WaitForSeconds(1f);
        ChangeState(CUBE_PLAY_STATE_TYPE.idle);
    }

    private void MoveEnterState()
    {

    }

    private void MoveUpdateState()
    {
        if (IsMove)
        {
            animator.Play("Move");

            //[Sound]
            //cubeSoundPlay.PlaySound(0, false);
            //soundManager.PlaySound(0, false);
        }
    }

    private void IdleEnterState()
    {

    }

    private void IdleUpdateState()
    {
        if (IsIdle)
        {
            animator.Play("Idle");
        }
    }


    //[Sound]
    //private void FadeOutSound()
    //{
    //    soundManager.Fadeout(2);
    //}
}
