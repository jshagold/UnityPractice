using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private ICharacterRepository _repo;

    // --- 데이터 보관 --- //
    private List<CharacterModel> players = new();
    private List<CharacterModel> enemies = new();

    // --- 계산기 --- //
    private DamageCalculator dmgCalculator;

    // --- 상태값 --- //
    private bool battleRunning;
    private StageEndType stageEndType;
    // 적의 공격 타겟 리스트
    private List<BattleTarget> enemyTargets = new();
    private List<BattleTarget> playerTargets = new();
    private List<BattleTarget> battleOrderList= new();


    private BattleInputManager inputManager;
    void Awake() => inputManager = GetComponent<BattleInputManager>();

    // --- 1. 초기화 --- //
    public void SetupBattle(List<string> playerIdList, List<string> enemyIdList, StageDifficulty difficulty)
    {
        _repo = new LocalCharacterRepository();

        CharacterFactory characterFactory = new(_repo);

        dmgCalculator = new(difficulty);
        players = playerIdList.Select(id => characterFactory.Create(id)).ToList();
        enemies = enemyIdList.Select(id => characterFactory.Create(id)).ToList();

        stageEndType = StageEndType.NOTYET;
        battleRunning = true;


        Debug.Log("SetUp Battle");
    }

    


    // --- 전투 로직 --- ///
    // 1. Phase0 - 캐릭터 상태 로직 계산
    // 2. Phase1 - 적 캐릭터가 공격스킬을 지정하고 아군 캐릭터를 지정
    // 3. Phase2 - 플레이어가 공격 타겟을 지정 (플레이어 캐릭터 선택, 타깃 선택, 스킬 선택, 반복, 모든 캐릭터 지정후 준비 완료)
    // 4. Phase3 - 캐릭터들의 속도에 따라서 공격 / 수비를 결정. 속도 수치가 높은 캐릭터가 먼저 공격. 속도가 똑같다면 무작위 캐릭터 우선.
    // 5. Phase4 - QTE 행동.
    // 6. Phase5 - 데미지 계산
    // 7. Phase6 - 데미지 계산후 캐릭터 상태 변경 (사망 등등)
    // 8. Phase7 - 모든 아군 또는 모든 적군 캐릭터가 죽었다면 전투 종료. 전투 종료가 아니라면 다음 턴(Phase0 부터 재시작)으로.



    // --- 2. 턴 반복 --- //
    public IEnumerator TurnLoop()
    {
        while(battleRunning)
        {
            yield return Phase0();
        }

        EndBattle(stageEndType);
    }

    // --- Phase 0 캐릭터 상태 로직 계산 --- //
    IEnumerator Phase0()
    {
        battleOrderList.Clear();
        playerTargets.Clear();
        enemyTargets.Clear();

        // UI
        //BattleUI.Instance.OnEnterPhase0();
        Debug.Log("Phase0");

        // 플레이어 패시브
        foreach (var player in players)
        {
            player.ApplyPhase0();

            foreach (var effect in player.PassiveList?.SelectMany(passive => passive.effects) ?? Enumerable.Empty<SkillEffect>())
            {
                player.ApplyToCurrentStat(effect);
            }

        }

        // 플레이어 장비
        // 플레이어 아이템
        // 플레이어 디버프

        // 적 패시브
        foreach(var enemy in enemies)
        {
            enemy.ApplyPhase0();

            foreach (var effect in enemy.PassiveList?.SelectMany(passive => passive.effects) ?? Enumerable.Empty<SkillEffect>())
            {
                enemy.ApplyToCurrentStat(effect);
            }
        }

        // 적 장비
        // 적 아이템
        // 적 디버프

        yield return Phase1();
    }

    // --- Phase 1 적 캐릭터가 공격스킬을 지정하고 아군 캐릭터를 지정 --- //
    IEnumerator Phase1()
    {
        // UI
        //BattleUI.Instance.OnEnterPhase1();
        Debug.Log("Phase1");

        foreach (var enemy in enemies)
        {
            // 1. 적 캐릭터가 공격 스킬을 선택.
            ActiveSkill? randomSkill;
            var pool = new List<ActiveSkill>(3);
            if (enemy.MainSkill != null) pool.Add(enemy.MainSkill);
            if (enemy.Sub1Skill != null) pool.Add(enemy.Sub1Skill);
            if (enemy.Sub2Skill != null) pool.Add(enemy.Sub2Skill);

            if(pool.Count > 0)
            {
                randomSkill = pool[Random.Range(0, pool.Count)];
            } 
            else
            {
                randomSkill = null;
                continue;
            }

            // 2. 적 캐릭터가 스킬에 따른 공격 타겟을 선택.
            List<CharacterModel> targetList = randomSkill.TargetType switch
            {
                SkillTargeting.None => new(),
                SkillTargeting.SingleEnemy => players.OrderBy(_ => Random.value).Take(1).ToList(),
                SkillTargeting.WholeEnemy => players.ToList(),
                _ => throw new System.NotImplementedException(),
            };

            BattleTarget pair = new(enemy, randomSkill, targetList);
            enemyTargets.Add(pair);
        }

        yield return Phase2();
    }

    // --- Phase 2 플레이어가 공격 타겟을 지정 --- //
    IEnumerator Phase2()
    {
        // UIrm duv
        //BattleUI.Instance.OnEnterPhase2();
        Debug.Log("Phase2");

        // 아군 캐릭터 선택 -> 캐릭터 스킬 선택 -> 타겟 선택 => 모든 캐릭터 선택할때까지 반복
        yield return inputManager.CollectPlayerTargets(players, enemies, playerTargets);

        yield return Phase3();
    }

    // --- Phase 3 캐릭터들의 속도에 따라서 공격 / 수비를 결정. --- //
    IEnumerator Phase3()
    {
        // UI
        //BattleUI.Instance.OnEnterPhase3();
        Debug.Log("Phase3");


        battleOrderList = playerTargets.Concat(enemyTargets).OrderByDescending( character => character.Caster.CurrentStat.agility ).ToList();

        yield return Phase4();
    }

    // --- Phase 4 QTE 행동 --- //
    IEnumerator Phase4()
    {
        // UI
        //BattleUI.Instance.OnEnterPhase4();

        yield return inputManager.CollectQTEResults(battleOrderList);

        yield return Phase5();
    }

    // --- Phase 5 데미지 계산 --- //
    IEnumerator Phase5()
    {
        // UI
        //BattleUI.Instance.OnEnterPhase5();

        foreach (var battlePair in battleOrderList)
        {
            foreach (var dmgQtePair in battlePair.DmgQtePair) 
            {
                int damagePower = dmgCalculator.CalculateDamage(
                    stat: battlePair.Caster.CurrentStat,
                    damage: dmgQtePair.dmgEffect.damage,
                    qte: dmgQtePair.qte
                );

                battlePair.Targets.ForEach(target => target.TakeDamage(damagePower));
            }
        }

        yield return Phase6();
    }

    // --- Phase 6 캐릭터 상태 변경 --- //
    IEnumerator Phase6()
    {
        // UI
        //BattleUI.Instance.OnEnterPhase6();

        yield return Phase7();
    }

    // --- Phase 7 전투 종료 판별 --- //
    IEnumerator Phase7()
    {
        // UI
        //BattleUI.Instance.OnEnterPhase7();

        bool playerAllDead = players.All(player => player.IsDead);
        bool enemyAllDead = enemies.All(enemy => enemy.IsDead);

        if (playerAllDead)
        {
            stageEndType = StageEndType.CLEAR;
            battleRunning = false;
        }
        else if(enemyAllDead)
        {
            stageEndType = StageEndType.FAIL;
            battleRunning = false;
        }

        yield break;
    }


    // --- 3. 전투 종료 --- //
    void EndBattle(StageEndType stageEndType)
    {
        // 캐릭터 데이터 저장
        foreach (var player in players)
        {
            _repo.Save(player.SaveData);
        }
        
        // UI
        //BattleUI.Instance.OnEnterPhaseEndBattle(stageEndType);
        

    }



}