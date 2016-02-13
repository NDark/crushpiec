using UnityEngine;
using System.Collections;

public enum GameState
{
    Initization,
    NextBattle,
    Idle,
    WaitForInput,
    WaitForAnimation,
    ActionForCharacter,
    ActionForMonster,
    ValidateVictory,
    Win,
    Lose,

    RequestNext,

    InValid,
}
public class BattleEventManager : DummyBattlePlay {
    static GameState m_State = GameState.InValid;
    Character m_CharacterRef = null;
    Character m_MonsterRef = null;

    static int s_WaitTurnForMonsterMax = 2;
    int m_WaitTurnForMonsterAttack = s_WaitTurnForMonsterMax;

    // override interface
    public override bool IsInAnimation() 
    {
        return (m_State > GameState.WaitForInput && m_State < GameState.RequestNext);
    }

    public override void Attack(float _Ratio)
    {
        base.Attack(_Ratio);
        DoAttck(ref m_CharacterRef, ref m_MonsterRef, _Ratio);
        m_State = GameState.ActionForCharacter;
    }

    public override void Defend(float _Ratio)
    {
        base.Defend(_Ratio);
        DoDefend(ref m_CharacterRef, _Ratio);
        m_State = GameState.ActionForCharacter;
    }

    void DoAttck(ref Character _Hitter, ref Character _Receiver, float _Ratio)
    {
        int atk = (int)(_Hitter.atk.randomOneValue * _Ratio * 100.0f);
        int def = _Receiver.def.value;

        GlobalSingleton.DEBUG("DoAttck atk = " + atk + ",def = " + def);

        _Receiver.hp.Offset(def - atk);
        _Receiver.def.ToMin();
    }

    void DoDefend(ref Character _Receiver, float _Ratio)
    {
        int def = (int)(_Receiver.def.min * _Ratio);
        _Receiver.def.Offset(def);
    }

    // Use this for initialization
    void Start () {
        m_CharacterRef = GameObject.Find("Unit_Character").GetComponent<Character>();
        m_MonsterRef = GameObject.Find("Unit_Monster").GetComponent<Character>();

        m_State = GameState.Initization;
    }

    // Update is called once per frame
    void Update () {
        switch (m_State)
        {
            case GameState.Initization:
                m_CharacterRef.ReCreate();
                m_MonsterRef.ReCreate();
                m_CharacterRef.DoUpdateHP();
                m_MonsterRef.DoUpdateHP();

                m_State = GameState.Idle;
                break;

            case GameState.NextBattle:
                // re-create monster
                int seed = Random.Range(1, 7);
                m_MonsterRef.m_MeshName = "Unit_" + seed;
                m_MonsterRef.ReCreate();
                m_MonsterRef.DoUpdateHP();
                m_State = GameState.Idle;
                break;

            case GameState.Idle:
                m_State = GameState.WaitForInput;
                break;

            case GameState.WaitForInput:
                break;

            case GameState.ActionForCharacter:
                GlobalSingleton.DEBUG("ActionForCharacter");

                m_CharacterRef.DoAction(AnimationState.Attack);
                m_MonsterRef.DoAction(AnimationState.Hitted, TransformAnimation.Define.TIME_ATTACK);

                Invoke("OnActionForCharacterFinish", 1.0f);

                m_State = GameState.WaitForAnimation;
                break;

            case GameState.ActionForMonster:
                GlobalSingleton.DEBUG("ActionForMonster");

                if (m_WaitTurnForMonsterAttack <= 0)
                {
                    m_MonsterRef.DoAction(AnimationState.Attack);
                    m_CharacterRef.DoAction(AnimationState.Hitted, TransformAnimation.Define.TIME_ATTACK);

                    DoAttck(ref m_MonsterRef, ref m_CharacterRef, Random.Range(0.5f, 2.0f));

                    m_WaitTurnForMonsterAttack = s_WaitTurnForMonsterMax;
                }

                --m_WaitTurnForMonsterAttack;

                Invoke("OnActionForMonsterFinish", 1.0f);

                m_State = GameState.WaitForAnimation;
                break;

            case GameState.ValidateVictory:
                GlobalSingleton.DEBUG(
                    "ValidateVictory : Character HP = "
                   + m_CharacterRef.hp.value
                   + ", Monster HP = "
                   + m_MonsterRef.hp.value);

                if (m_CharacterRef.hp.value <= 0)
                {
                    m_State = GameState.Lose;
                }
                else if (m_MonsterRef.hp.value <= 0)
                {
                    m_State = GameState.Win;
                }
                else
                {
                    m_State = GameState.RequestNext;
                }                
                break;

            case GameState.Win:
                GlobalSingleton.DEBUG("Win");
                Invoke("OnWinAnimationFinish", 1.0f);
                m_State = GameState.WaitForAnimation;
                break;

            case GameState.Lose:
                GlobalSingleton.DEBUG("Lose");
                Invoke("OnLoseAnimationFinish", 1.0f);
                m_State = GameState.WaitForAnimation;
                break;

            case GameState.RequestNext:
                GlobalSingleton.DEBUG("RequestNext");
                m_State = GameState.Idle;
                break;
        } // End for switch
	}

    void OnActionForCharacterFinish() {
        m_MonsterRef.DoUpdateHP();
        m_State = GameState.ActionForMonster;
    }

    void OnActionForMonsterFinish() {
        m_CharacterRef.DoUpdateHP();
        m_State = GameState.ValidateVictory;
    }

    void OnWinAnimationFinish() {
        m_State = GameState.NextBattle;
    }

    void OnLoseAnimationFinish() {
        m_State = GameState.Initization;
    }
}
