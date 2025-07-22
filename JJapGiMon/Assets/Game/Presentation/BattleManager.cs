using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    // --- 데이터 보관 --- //
    private List<CharacterModel> players = new();
    private List<CharacterModel> enemies = new();

    private Queue<CharacterModel> turnQueue = new();

    // --- 상태값 --- //
    private bool battleRunning;

    // --- 1. 초기화 --- //
    public void SetupBattle(List<string> playerIdList, List<string> enemyIdList)
    {

        CharacterFactory characterFactory = new CharacterFactory();

        players = playerIdList.Select(id => characterFactory.Create(id)).ToList();
        enemies = enemyIdList.Select(id => characterFactory.Create(id)).ToList();


        BuildTurnQueue();
        battleRunning = true;

        StartCoroutine(TurnLoop());
    }

    void BuildTurnQueue()
    {
        var allCharacter = players.Concat(enemies).OrderByDescending(c => c.CurrentStat.agility);
        turnQueue = new Queue<CharacterModel>(allCharacter);
    }


    // --- 전투 로직 --- ///
    // 1. Phase0 - 캐릭터 상태 로직 계산
    // 2. Phase1 - 적 캐릭터가 아군 캐릭터를 지정하고 공격스킬 지정
    // 3. Phase2 - 플레이어가 공격 타겟을 지정 (플레이어 캐릭터 선택, 타깃 선택, 스킬 선택, 반복, 모든 캐릭터 지정후 준비 완료)
    // 4. Phase3 - [데미지 페이즈 시작] 캐릭터들의 속도에 따라서 공격 / 수비를 결정. 속도 수치가 높은 캐릭터가 먼저 공격. 속도가 똑같다면 무작위 캐릭터 우선.
    // 5. Phase4 - [데미지 페이즈] QTE 행동.
    // 6. Phase5 - [데미지 페이즈 종료] 데미지 계산
    // 7. Phase6 - 데미지 계산후 캐릭터 상태 변경 (사망 등등)
    // 8. Phase7 - 모든 아군 또는 모든 적군 캐릭터가 죽었다면 전투 종료. 전투 종료가 아니라면 다음 턴(Phase0 부터 재시작)으로.



    // --- 2. 턴 반복 --- //
    IEnumerator TurnLoop()
    {
        while(battleRunning)
        {
            yield return Phase0();
        }

        EndBattle();
    }

    // --- Phase 0 --- //
    IEnumerator Phase0()
    {


        yield return Phase1();
    }

    // --- Phase 1 --- //
    IEnumerator Phase1()
    {


        yield return Phase2();
    }

    // --- Phase 2 --- //
    IEnumerator Phase2()
    {


        yield return Phase3();
    }

    // --- Phase 3 --- //
    IEnumerator Phase3()
    {


        yield return Phase4();
    }

    // --- Phase 4 --- //
    IEnumerator Phase4()
    {


        yield return Phase5();
    }

    // --- Phase 5 --- //
    IEnumerator Phase5()
    {


        yield return Phase6();
    }

    // --- Phase 6 --- //
    IEnumerator Phase6()
    {


        yield return Phase7();
    }

    // --- Phase 7 --- //
    IEnumerator Phase7()
    {


        yield return Phase7();
    }


    // --- 3. 전투 종료 --- //
    void EndBattle()
    {

    }



}