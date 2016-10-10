using UnityEngine;
using System.Collections;

public enum GameState
{
    Initization,
    NextBattle,
    Idle,
    WaitForInput,
    WaitForAnimation,
    ActionForCharacterAttack,
    ActionForCharacterDefend,
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
    public GetaPieceUnitDataComponent m_UnitDataRef = null;

    static int s_WaitTurnForMonsterMax = 2;
    int m_WaitTurnForMonsterAttack = s_WaitTurnForMonsterMax;

    // override interface
    public override bool IsInitialized()
    {
        return true;
    }

    public override bool IsInAnimation()
    {
        return true;
    }

    public override void StartBattle()
    {

    }

    void DoAttck(ref Character _Hitter, ref Character _Receiver, float _Ratio)
    {    
    }

    void DoDefend(ref Character _Receiver, float _Ratio)
    {  
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
                // DoUpdateCountDown();
                // m_CharacterRef.DoChangeModel(2, MODELTYPE.E_DEFENSE);

                m_State = GameState.Idle;
                break;

            case GameState.NextBattle:
                DoCreateOneCharacter(ref m_MonsterRef);
                // DoUpdateCountDown();
                m_State = GameState.Idle;
                break;

            case GameState.Idle:
                m_State = GameState.WaitForInput;
                break;

            case GameState.WaitForInput:
                if (Input.GetKeyUp(KeyCode.A))
                {
                    OnAction(m_CharacterRef, 0, m_UnitDataRef.m_Player.m_Action[0]);
                    OnAction(m_MonsterRef, 0, m_UnitDataRef.m_Enemy.m_Action[0]);
                    OnAction(m_CharacterRef, 1, m_UnitDataRef.m_Player.m_Action[1]);
                    OnAction(m_MonsterRef, 1, m_UnitDataRef.m_Enemy.m_Action[1]);
                    OnAction(m_CharacterRef, 2, m_UnitDataRef.m_Player.m_Action[2]);
                    OnAction(m_MonsterRef, 2, m_UnitDataRef.m_Enemy.m_Action[2]);
                }
                break;

            case GameState.RequestNext:
                GlobalSingleton.DEBUG("RequestNext");
                m_State = GameState.Idle;
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
                _CharRef.DoAction(_ChunkIndex, AnimationState.Attack, 2.0f);
                break;

            case ActionKey.Concentrate:
                _CharRef.DoChangeModel(_ChunkIndex, MODELTYPE.E_CONCENTRATE);
                break;
        }
    }

    void OnActionForCharacterFinish() {
        m_MonsterRef.DoUpdateHP();
        m_State = GameState.ActionForMonster;
    }

    void OnActionForMonsterFinish() {
        // DoUpdateCountDown();
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
