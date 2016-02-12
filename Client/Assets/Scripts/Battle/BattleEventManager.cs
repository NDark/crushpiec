using UnityEngine;
using System.Collections;

public enum GameState
{
    Initization,
    Idle,
    WaitForInput,
    ActionForCharacter,
    ActionForMonster,
    ValidateVictory,
    RequestNext,

    InValid,
}
public class BattleEventManager : DummyBattlePlay {
    private static GameState m_State = GameState.InValid;

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
        m_State = GameState.Initization;
    }
	
	// Update is called once per frame
	void Update () {
        switch (m_State)
        {
            case GameState.Initization:
                GameObject objCharacter = GameObject.Find("Unit_Character");
                objCharacter.GetComponent<Character>().DoRender();
                GameObject objMonster = GameObject.Find("Unit_Monster");
                objMonster.GetComponent<Character>().DoRender();

                m_State = GameState.Idle;
                break;

            case GameState.Idle:
                m_State = GameState.WaitForInput;
                break;

            case GameState.WaitForInput:
                break;

            case GameState.ActionForCharacter:
                GlobalSingleton.DEBUG("ActionForCharacter");
                m_State = GameState.ActionForMonster;
                break;

            case GameState.ActionForMonster:
                GlobalSingleton.DEBUG("ActionForMonster");
                m_State = GameState.ValidateVictory;
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
}
