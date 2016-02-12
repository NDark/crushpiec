using UnityEngine;
using System.Collections;

public enum GameState
{
    Initization,
    Idle,
    WaitForInput,
    ActionInAnimation,
    ActionForCharacter,
    ActionForMonster,
    ValidateVictory,
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
        int atk = (int)(_Hitter.data.m_ATK.randomOneValue * _Ratio * 10.0f);
        int def = _Receiver.data.m_DEF.value;
        _Receiver.data.m_HP.Offset(def - atk);
        _Receiver.data.m_DEF.ToMin();
    }

    void DoDefend(ref Character _Receiver, float _Ratio)
    {
        int def = (int)(_Receiver.data.m_DEF.min * _Ratio);
        _Receiver.data.m_DEF.Offset(def);
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
                m_CharacterRef.DoRender();
                m_MonsterRef.DoRender();

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

                m_State = GameState.ActionInAnimation;
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

                m_State = GameState.ActionInAnimation;
                break;

            case GameState.ValidateVictory:
                // if HP = 0, else
                GlobalSingleton.DEBUG(
                    "ValidateVictory : Character HP = "
                  + m_CharacterRef.data.m_HP.value
                  + ", Monster HP = " 
                  + m_MonsterRef.data.m_HP.value);

                m_State = GameState.RequestNext;
                break;

            case GameState.RequestNext:
                GlobalSingleton.DEBUG("RequestNext");
                m_State = GameState.Idle;
                break;
        } // End for switch
	}

    void OnActionForCharacterFinish() { m_State = GameState.ActionForMonster; }
    void OnActionForMonsterFinish() { m_State = GameState.ValidateVictory; }
}
