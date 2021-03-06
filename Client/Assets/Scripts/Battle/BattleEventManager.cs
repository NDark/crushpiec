﻿/**
@date 20161023 by NDark 
. move code from Start() to StartInitialize()
. remove useless GameState.NextBattle.
. modify start from ActionForChaanel_R_0 at StartBattle()
. modify code to continue GameState.Idle at GameState.ActionForChaanel_L_2:
. modify code to continue GameState.ActionForChaanel_L_0 at GameState.ActionForChaanel_R_2:
. add class method IsActionNeedsPlayAnimation() to check skip animation().

@date 20161029 by NDark
. replace WaitForAnimation by WaitForMorphing.
. make m_State to be public.
. modify checking of game state at IsInAnimation()
. modify adding WaitForMorphing before the state ActionForChaanel_R_0.
. add class method StartMorphingState()

@date 20161031 by NDark 
. add class method Handler_CharacterBeenHit()
. add class method Handler_EnemyBeenHit()
. add class member m_PlayerHPSinceBattleStart
. add class member m_EnemyHPSinceBattleStart
. add class method UpdateHitPointPlayerFromInput()
. add class method UpdateHitPointEnemyFromInput()
	
*/
using UnityEngine;
using System.Collections;

public enum GameState
{
    Initization,
    
    Idle,
    WaitForInput,
    
	WaitForMorphing ,
	
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
    public GameState m_State = GameState.InValid;
    static float AnimationTime = 1.5f;
    Character m_CharacterRef = null;
    Character m_MonsterRef = null;
    public GetaPieceUnitDataComponent m_UnitDataRef = null;
    float m_ElapsedTime = 0.0f;
     
	public override void StartInitialize()
	{
		m_CharacterRef = GameObject.Find("Unit_Character").GetComponent<Character>();
		m_MonsterRef = GameObject.Find("Unit_Monster").GetComponent<Character>();
		
		if( null == m_CharacterRef )
		{
			Debug.LogError("null == m_CharacterRef");
		}
		
		if( null == m_MonsterRef )
		{
			Debug.LogError("null == m_MonsterRef");
		}
		
		if( null != m_CharacterRef 
			&& null != m_MonsterRef )
		{
			m_State = GameState.Initization ;
		}
		
	}
	
    // override interface
    public override bool IsInitialized()
    {
        return m_State>GameState.Initization;
    }

    public override bool IsInAnimation()
    {
        return m_State > GameState.WaitForInput && m_State <= GameState.ActionForChaanel_R_2;
    }

    public override void StartBattle()
    {
		StartMorphingState();
		
		m_State = GameState.WaitForMorphing;
		
    }

    void DoCreateOneCharacter(ref Character _CharacterRef, bool _Random = true)
    {
        // re-create
        _CharacterRef.ReCreate();
        _CharacterRef.DoUpdateHP();
    }

    // Use this for initialization
    void Start () {
		
		
    }
    
	void OnDestroy()
	{
		if( null != m_CharacterRef )
		{
			m_CharacterRef.DoBeenHit -= Handler_CharacterBeenHit ;
		}
		if( null != m_MonsterRef )
		{
			m_MonsterRef.DoBeenHit -= Handler_EnemyBeenHit ;
		}
	}
	

    // Update is called once per frame
    void Update () {
        switch (m_State)
        {
            case GameState.Initization:
                DoCreateOneCharacter(ref m_CharacterRef, false);
				if( null != m_CharacterRef )
				{
					m_CharacterRef.DoBeenHit += Handler_CharacterBeenHit ;
				}
				DoCreateOneCharacter(ref m_MonsterRef , false);
				if( null != m_MonsterRef )
				{
					m_MonsterRef.DoBeenHit += Handler_EnemyBeenHit ;
				}
                m_State = GameState.Idle;
                break;

            case GameState.Idle:
                m_CharacterRef.DoChangeModel(0, MODELTYPE.E_CONCENTRATE);
                m_CharacterRef.DoChangeModel(1, MODELTYPE.E_CONCENTRATE);
                m_CharacterRef.DoChangeModel(2, MODELTYPE.E_CONCENTRATE);
                m_MonsterRef.DoChangeModel(0, MODELTYPE.E_CONCENTRATE);
                m_MonsterRef.DoChangeModel(1, MODELTYPE.E_CONCENTRATE);
                m_MonsterRef.DoChangeModel(2, MODELTYPE.E_CONCENTRATE);
                m_State = GameState.WaitForInput;
                break;

            case GameState.WaitForInput:
                if (Input.GetKeyUp(KeyCode.A))
                {
                    StartBattle();
                }
                break;

			
			case GameState.WaitForMorphing:
				UpdateMorphing() ;
				if ( isMorphingEnded() )
				{
					OnInitAction(m_CharacterRef, 0, m_UnitDataRef.m_Player.m_Action[0]);
					OnInitAction(m_CharacterRef, 1, m_UnitDataRef.m_Player.m_Action[1]);
					OnInitAction(m_CharacterRef, 2, m_UnitDataRef.m_Player.m_Action[2]);
					OnInitAction(m_MonsterRef, 0, m_UnitDataRef.m_Enemy.m_Action[0]);
					OnInitAction(m_MonsterRef, 1, m_UnitDataRef.m_Enemy.m_Action[1]);
					OnInitAction(m_MonsterRef, 2, m_UnitDataRef.m_Enemy.m_Action[2]);
					
					OnAction(m_MonsterRef, m_CharacterRef, 0, m_UnitDataRef.m_Enemy.m_Action[0]);
					
					m_State = GameState.ActionForChaanel_R_0;
					
					m_ElapsedTime = 0.0f;
				}
				
				break;
			
			
            case GameState.ActionForChaanel_L_0:
                m_ElapsedTime += Time.deltaTime;
                if (m_ElapsedTime > AnimationTime
			    || false == IsActionNeedsPlayAnimation( m_UnitDataRef.m_Player.m_Action[0] , m_UnitDataRef.m_Enemy.m_Action[0] )
                )
                {
                    OnAction(m_CharacterRef, m_MonsterRef, 1, m_UnitDataRef.m_Player.m_Action[1]);
                    // OnReAction(m_MonsterRef, 1, m_UnitDataRef.m_Enemy.m_Action[1]);
                    m_State = GameState.ActionForChaanel_L_1;
                    m_ElapsedTime = 0;
                }
                break;

            case GameState.ActionForChaanel_L_1:
                m_ElapsedTime += Time.deltaTime;
                if (m_ElapsedTime > AnimationTime
			    || false == IsActionNeedsPlayAnimation( m_UnitDataRef.m_Player.m_Action[1] , m_UnitDataRef.m_Enemy.m_Action[1] )
                )
                {
                    OnAction(m_CharacterRef, m_MonsterRef, 2, m_UnitDataRef.m_Player.m_Action[2]);
                    // OnReAction(m_MonsterRef, 2, m_UnitDataRef.m_Enemy.m_Action[2]);
                    m_State = GameState.ActionForChaanel_L_2;
                    m_ElapsedTime = 0;
                }
                break;

            case GameState.ActionForChaanel_L_2:
			
				m_ElapsedTime += Time.deltaTime;
				if (m_ElapsedTime > AnimationTime
			    || false == IsActionNeedsPlayAnimation( m_UnitDataRef.m_Player.m_Action[2] , m_UnitDataRef.m_Enemy.m_Action[2] )
				)
				{
					m_State = GameState.Idle;
				}
				
                break;
            case GameState.ActionForChaanel_R_0:
                m_ElapsedTime += Time.deltaTime;
                if (m_ElapsedTime > AnimationTime
			    || false == IsActionNeedsPlayAnimation( m_UnitDataRef.m_Enemy.m_Action[0] , m_UnitDataRef.m_Player.m_Action[0] )
                )
                {
                    OnAction(m_MonsterRef, m_CharacterRef, 1, m_UnitDataRef.m_Enemy.m_Action[1]);
                    // OnReAction(m_CharacterRef, 1, m_UnitDataRef.m_Player.m_Action[1]);
                    m_State = GameState.ActionForChaanel_R_1;
                    m_ElapsedTime = 0;
                }
                break;

            case GameState.ActionForChaanel_R_1:
                m_ElapsedTime += Time.deltaTime;
                if (m_ElapsedTime > AnimationTime
			    || false == IsActionNeedsPlayAnimation( m_UnitDataRef.m_Enemy.m_Action[1] , m_UnitDataRef.m_Player.m_Action[1] )
                )
                {
                    OnAction(m_MonsterRef, m_CharacterRef, 2, m_UnitDataRef.m_Enemy.m_Action[2]);
                    // OnReAction(m_CharacterRef, 2, m_UnitDataRef.m_Player.m_Action[2]);
                    m_State = GameState.ActionForChaanel_R_2;
                    m_ElapsedTime = 0;
                }
                break;

            case GameState.ActionForChaanel_R_2:
				m_ElapsedTime += Time.deltaTime;
				if (m_ElapsedTime > AnimationTime
			    || false == IsActionNeedsPlayAnimation( m_UnitDataRef.m_Enemy.m_Action[2] , m_UnitDataRef.m_Player.m_Action[2] )
				)
				{
					OnAction(m_CharacterRef, m_MonsterRef, 0
						, m_UnitDataRef.m_Player.m_Action[0]);
					// OnReAction(m_CharacterRef, 1, m_UnitDataRef.m_Player.m_Action[1]);
					m_State = GameState.ActionForChaanel_L_0;
					m_ElapsedTime = 0;
				}
				            
                break;
        } // End for switch
	}

    void OnAction(Character _CharRef, Character _OtherRef, int _ChunkIndex, ActionKey _Action)
    {
        GlobalSingleton.DEBUG(_CharRef.name + "," + _ChunkIndex + "," + _Action);
        switch (_Action)
        {
            case ActionKey.Attack:
                _CharRef.DoChangeModel(_ChunkIndex, MODELTYPE.E_ATTACK);
                _CharRef.DoAction(_ChunkIndex, AnimationState.Attack, 1.2f);
				_CharRef.SetOpponentTarget( _ChunkIndex ) ;
				
                _OtherRef.DoAction(_ChunkIndex, AnimationState.Hitted, 1.2f);
				_OtherRef.SetOpponentTarget( _ChunkIndex ) ;
                break;

            case ActionKey.Defend:
                _CharRef.DoChangeModel(_ChunkIndex, MODELTYPE.E_DEFENSE);
				_OtherRef.ClearOpponentTarget( _ChunkIndex );
                // _CharRef.DoAction(_ChunkIndex, AnimationState.Defend, 2.0f);
                break;

            case ActionKey.Concentrate:
                _CharRef.DoChangeModel(_ChunkIndex, MODELTYPE.E_CONCENTRATE);
				_OtherRef.ClearOpponentTarget( _ChunkIndex );
                break;
        }
    }
    
	bool IsActionNeedsPlayAnimation( ActionKey _AttackAction , ActionKey _DefenseAction )
    {
    	bool ret = false ;
    	if( _AttackAction == ActionKey.Attack )
    	{
    		ret = true ;
    	}
    	return ret ;
    }

    void OnInitAction(Character _CharRef, int _ChunkIndex, ActionKey _Action)
    {
        switch (_Action)
        {
            case ActionKey.Defend:
                _CharRef.DoChangeModel(_ChunkIndex, MODELTYPE.E_DEFENSE);
                // _CharRef.DoAction(_ChunkIndex, AnimationState.Defend, 2.0f);
                break;
        }
		_CharRef.ClearAllOpponentTargets() ;
    }
    
	
	void StartMorphingState()
	{
		StartMorphingModel(m_CharacterRef, 0, m_UnitDataRef.m_Player.m_Action[0]);
		StartMorphingModel(m_CharacterRef, 1, m_UnitDataRef.m_Player.m_Action[1]);
		StartMorphingModel(m_CharacterRef, 2, m_UnitDataRef.m_Player.m_Action[2]);
		StartMorphingModel(m_MonsterRef, 0, m_UnitDataRef.m_Enemy.m_Action[0]);
		StartMorphingModel(m_MonsterRef, 1, m_UnitDataRef.m_Enemy.m_Action[1]);
		StartMorphingModel(m_MonsterRef, 2, m_UnitDataRef.m_Enemy.m_Action[2]);
		
	}
	
	void StartMorphingModel(Character _CharRef, int _ChunkIndex, ActionKey _Action)
	{
		switch (_Action)
		{
		// so far, we only morph defense, the other action will stay in not morphing status.
		case ActionKey.Defend:
			_CharRef.StartMorphingModel(_ChunkIndex, MODELTYPE.E_DEFENSE);
			break;
		}
	}
	
	bool isMorphingEnded()
	{
		return m_CharacterRef.isMorphingEnded() && m_MonsterRef.isMorphingEnded() ;
	}
	
	void UpdateMorphing()
	{
		m_CharacterRef.UpdateMorphing() ;
		m_MonsterRef.UpdateMorphing() ;
	}

	
	void Handler_CharacterBeenHit( int _ChunkIndex )
	{
		// Debug.Log("BattleEventManager Handler_DoBeenHit _ChunkIndex" + _ChunkIndex);
		m_UnitDataRef.m_BattleEvent.Enqueue( 
			new GetaPieceBattleEvent
			{ 
				Type = GetaPieceBattleEventType.Damage.ToString() 
				, Target = "Player"
				, Value = _ChunkIndex.ToString() 
			} ) ;
				
	}	
	
	
	void Handler_EnemyBeenHit( int _ChunkIndex )
	{
		// Debug.Log("BattleEventManager Handler_DoBeenHit _ChunkIndex" + _ChunkIndex);
		m_UnitDataRef.m_BattleEvent.Enqueue( 
		                                    new GetaPieceBattleEvent
		                                    { 
			Type = GetaPieceBattleEventType.Damage.ToString() 
				, Target = "Enemy"
					, Value = _ChunkIndex.ToString() 
		} ) ;
		
	}	
}
