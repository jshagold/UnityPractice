using System;
using System.Collections;
using UnityEngine;

public abstract class FSMBase
{
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}

public class IdleState : FSMBase
{
    private readonly Player player;

    public IdleState(Player player)
    {
        this.player = player;
    }

    public override void EnterState()
    {
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
        if (player.IsIdle)
        {
            player.animator.Play("Idle");
        }
    }
}

public class MoveState : FSMBase
{
    private readonly Player player;

    public MoveState(Player player)
    {
        this.player = player;
    }

    public override void EnterState()
    {
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
        if (player.IsMove)
        {
            player.animator.Play("Move");
            player.soundManager.PlaySound(0, false);
        }
    }
}

public class AttackState : FSMBase
{
    private readonly Player player;

    public AttackState(Player player)
    {
        this.player = player;
    }

    public override void EnterState()
    {
        player.animator.Play("Attack");
        player.soundManager.PlaySound(1, true);
        player.StartCoroutine(C_AttackStateFinish());
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
    }

    private IEnumerator C_AttackStateFinish()
    {
        // 애니메이션 재생 시간을 받아오는 방법도 있다.
        yield return new WaitForSeconds(1f);
        player.ChangeState<IdleState>();
    }
}

public class Player : MonoBehaviour
{

    public Animator animator; // cache
    public SoundManager soundManager; // cache
    public EffectManager effectManager; // cache

    private FSMBase currentState;
    public FSMBase CurrentState { get { return currentState; } }

    public bool IsMove
    {
        get
        {
            return currentState.GetType() != typeof(AttackState);
        }
    }

    public bool IsIdle
    {
        get
        {
            return currentState.GetType() != typeof(AttackState);
        }
    }

    public bool IsDuplicationState
    {
        get
        {
            return currentState.GetType() == typeof(AttackState);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        soundManager = FindAnyObjectByType<SoundManager>();
        effectManager = FindAnyObjectByType<EffectManager>();

        currentState = new IdleState(this);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizontal) > Mathf.Epsilon) // Epsilon -> float은 0f로 입력하면 부동소수점 부분이 인식이 잘 안될 때가 있기에 Epsilon을 사용해서 정확한 0을 확인
        {
            if (IsMove)
            { 
                ChangeState<MoveState>();
            }
        }
        else
        {
            if (IsIdle)
            {
                ChangeState<IdleState>();
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            ChangeState<AttackState>();
        }

        UpdateState();
    }

    private void UpdateState()
    {
        currentState.UpdateState();
    }

    public void ChangeState<T>() where T : FSMBase
    {
        if (IsDuplicationState)
        {
            if (this.currentState.GetType() == typeof(T))
            {
                return;
            }
        }

        if(currentState.GetType() != typeof(T))
        {
            currentState.ExitState();
        }

        T t = (T)Activator.CreateInstance(typeof(T), args: this); // reflection
        this.currentState = t;
        t.EnterState();
    }
}
