using UnityEngine;
using System.Collections;

public enum GameState
{
    Initization,
    NextBattle,
    Idle,
    WaitForInput,
    WaitForAnimation,
    ActionForChaanel_L_0,
    ActionForChaanel_L_1,
    ActionForChaanel_L_2,
    ActionForChaanel_R_0,
    ActionForChaanel_R_1,
    ActionForChaanel_R_2,
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
    public GetaPieceUnitDataComponent m_UnitDataRef = null;
    float m_ElapsedTime = 0.0f;
     
    // override interface
    public override bool IsInitialized()
    {
        return m_State>GameState.Initization;
    }

    public override bool IsInAnimation()
    {
        return m_State >= GameState.ActionForChaanel_L_0 && m_State <= GameState.ActionForChaanel_R_2;
    }

    public override void StartBattle()
    {
        OnAction(m_CharacterRef, 0, m_UnitDataRef.m_Player.m_Action[0]);
        // OnAction(m_MonsterRef, 0, m_UnitDataRef.m_Enemy.m_Action[0]);

        m_State = GameState.ActionForChaanel_L_0;
        m_ElapsedTime = 0.0f;
    }

    void DoCreateOneCharacter(ref Character _CharacterRef, bool _Random = true)
    {
        // re-create
        _CharacterRef.ReCreate();
        _CharacterRef.DoUpdateHP();
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
                DoCreateOneCharacter(ref m_CharacterRef, false);
                DoCreateOneCharacter(ref m_MonsterRef, false);
                m_State = GameState.Idle;
                break;

            case GameState.Idle:
                m_State = GameState.WaitForInput;
                break;

            case GameState.WaitForInput:
                if (Input.GetKeyUp(KeyCode.A))
                {
                    StartBattle();
                }
                break;

            case GameState.ActionForChaanel_L_0:
                m_ElapsedTime += Time.deltaTime;
                if (m_ElapsedTime > 2.0f)
                {
                    OnAction(m_CharacterRef, 1, m_UnitDataRef.m_Player.m_Action[1]);
                    // OnAction(m_MonsterRef, 1, m_UnitDataRef.m_Enemy.m_Action[1]);
                    m_State = GameState.ActionForChaanel_L_1;
                    m_ElapsedTime = 0;
                }
                break;

            case GameState.ActionForChaanel_L_1:
                m_ElapsedTime += Time.deltaTime;
                if (m_ElapsedTime > 2.0f)
                {
                    OnAction(m_CharacterRef, 2, m_UnitDataRef.m_Player.m_Action[2]);
                    // OnAction(m_MonsterRef, 2, m_UnitDataRef.m_Enemy.m_Action[2]);
                    m_State = GameState.ActionForChaanel_L_2;
                    m_ElapsedTime = 0;
                }
                break;

            case GameState.ActionForChaanel_L_2:
                m_ElapsedTime += Time.deltaTime;
                if (m_ElapsedTime > 2.0f)
                {
                    // OnAction(m_CharacterRef, 2, m_UnitDataRef.m_Player.m_Action[2]);
                    OnAction(m_MonsterRef, 0, m_UnitDataRef.m_Enemy.m_Action[0]);
                    m_State = GameState.ActionForChaanel_R_0;
                    m_ElapsedTime = 0;
                }
                break;
            case GameState.ActionForChaanel_R_0:
                m_ElapsedTime += Time.deltaTime;
                if (m_ElapsedTime > 2.0f)
                {
                    // OnAction(m_CharacterRef, 2, m_UnitDataRef.m_Player.m_Action[2]);
                    OnAction(m_MonsterRef, 1, m_UnitDataRef.m_Enemy.m_Action[1]);
                    m_State = GameState.ActionForChaanel_R_1;
                    m_ElapsedTime = 0;
                }
                break;

            case GameState.ActionForChaanel_R_1:
                m_ElapsedTime += Time.deltaTime;
                if (m_ElapsedTime > 2.0f)
                {
                    // OnAction(m_CharacterRef, 2, m_UnitDataRef.m_Player.m_Action[2]);
                    OnAction(m_MonsterRef, 2, m_UnitDataRef.m_Enemy.m_Action[2]);
                    m_State = GameState.ActionForChaanel_R_2;
                    m_ElapsedTime = 0;
                }
                break;

            case GameState.ActionForChaanel_R_2:
                m_ElapsedTime += Time.deltaTime;
                if (m_ElapsedTime > 2.0f)
                {
                    m_State = GameState.Idle;
                }
                break;
        } // End for switch
	}

    void OnAction(Character _CharRef, int _ChunkIndex, ActionKey _Action)
    {
        switch (_Action)
        {
            case ActionKey.Attack:
                _CharRef.DoChangeModel(_ChunkIndex, MODELTYPE.E_ATTACK);
                _CharRef.DoAction(_ChunkIndex, AnimationState.Attack, 2.0f);
                break;

            case ActionKey.Defend:
                _CharRef.DoChangeModel(_ChunkIndex, MODELTYPE.E_DEFENSE);
                _CharRef.DoAction(_ChunkIndex, AnimationState.Defend, 2.0f);
                break;

            case ActionKey.Concentrate:
                _CharRef.DoChangeModel(_ChunkIndex, MODELTYPE.E_CONCENTRATE);
                break;
        }
    }
}
