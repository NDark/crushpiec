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

    // override interface
    public override bool IsInAnimation() 
    {
        return (m_State > GameState.WaitForInput && m_State < GameState.RequestNext);
    }

    public override void Attack(float _Ratio)
    {
        base.Attack(_Ratio);

        m_State = GameState.ActionForCharacter;
    }

    public override void Defend(float _Ratio)
    {
        base.Defend(_Ratio);

        m_State = GameState.ActionForCharacter;
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
                Invoke("OnActionForCharacterFinish", 3.0f);
                m_CharacterRef.GetComponent<TransformAnimation>().DoAttack();
                m_State = GameState.ActionInAnimation;
                break;

            case GameState.ActionForMonster:
                GlobalSingleton.DEBUG("ActionForMonster");
                Invoke("OnActionForMonsterFinish", 3.0f);
                m_State = GameState.ActionInAnimation;
                break;

            case GameState.ValidateVictory:
                // if HP = 0, else
                GlobalSingleton.DEBUG("ValidateVictory");
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
