using UnityEngine;
using System.Collections;

public class GetaPieceUnitDataComponent : MonoBehaviour 
{
	public GetaPieceUnitData m_Player = new GetaPieceUnitData() ;
	public GetaPieceUnitData m_Enemy = new GetaPieceUnitData() ;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public class GetaPieceUnitData
{
	public int HitPoint 
	{
		get
		{
			return m_HitPoint ;
		}	
		set
		{
			m_HitPoint = value ;
			if( m_HitPoint > MaxHitPoint )
			{
				m_HitPoint = MaxHitPoint ;
			}
			else if( m_HitPoint < 0 )
			{
				m_HitPoint = 0 ;
			}
		}
	}
	public int MaxHitPoint {get;set;}
	public int Energy 
	{
		get
		{
			return m_Energy ;
		}	
		set
		{
			m_Energy = value ;
			if( m_Energy > this.MaxEnergy )
			{
				m_Energy = this.MaxEnergy ;
			}
			else if( m_Energy < 0 )
			{
				m_Energy = 0 ;
			}
		}
	}
	public int MaxEnergy {get;set;}

	public ActionKey [] m_Action = new ActionKey[3] ;
	public bool m_PowerAttack = false ;

	
	public int CalculateSufferDamageSeperatedly( GetaPieceUnitData _Enemy , int _ActionIndex )
	{
		int ret = 0 ;
		if( _ActionIndex < m_Action.Length && _ActionIndex < _Enemy.m_Action.Length )
		{
			if( _Enemy.m_Action[ _ActionIndex ] == ActionKey.Attack )
			{
				if( m_Action[ _ActionIndex ] == ActionKey.Defend )
				{
					ret += (_Enemy.m_PowerAttack) ? GetaPieceConst.DAMAGE_POWER_ATTACK_TO_DEFEND : GetaPieceConst.DAMAGE_ATTACK_TO_DEFEND ;
				}
				else if( m_Action[ _ActionIndex ] == ActionKey.Attack )
				{
					ret += (_Enemy.m_PowerAttack) ? GetaPieceConst.DAMAGE_POWER_ATTACK_TO_ATTACK : GetaPieceConst.DAMAGE_ATTACK_TO_ATTACK ;
				}
				else if( m_Action[ _ActionIndex ] == ActionKey.Concentrate )
				{
					ret += (_Enemy.m_PowerAttack) ? GetaPieceConst.DAMAGE_POWER_ATTACK_TO_CONCENTRATE : GetaPieceConst.DAMAGE_ATTACK_TO_CONCENTRATE ;
				}
			}
		}
		return ret ;
	}

	public int CalculateSufferDamageAsAWhole( GetaPieceUnitData _Enemy )
	{
		int ret = 0 ;
		for( int i = 0 ; i < m_Action.Length && i < _Enemy.m_Action.Length ; ++i )
		{
			ret += CalculateSufferDamageSeperatedly( _Enemy , i ) ;
		}
		return ret ;
	}

	
	public int CalculateEnergyBuffSeperately( GetaPieceUnitData _Enemy , int _ActionIndex )
	{
		int ret = 0 ;
		if( _ActionIndex < m_Action.Length && _ActionIndex < _Enemy.m_Action.Length  )
		{
			if( _Enemy.m_Action[ _ActionIndex ] == ActionKey.Attack 
			   && m_Action[ _ActionIndex ] == ActionKey.Defend ) 
			{
				ret += GetaPieceConst.BUFF_ENERGY_ATTACK_TO_DEFEND ;
			}
		}
		return ret ;
	}


	public int CalculateEnergyBuffAsAWhole( GetaPieceUnitData _Enemy )
	{
		int ret = 0 ;
		for( int i = 0 ; i < m_Action.Length && i < _Enemy.m_Action.Length ; ++i )
		{
			ret += CalculateEnergyBuffSeperately( _Enemy , i ) ;
		}
		return ret ;
	}
	
	public int CalculateCostEnergy()
	{
		int ret = 0 ;
		for( int i = 0 ; i < m_Action.Length  ; ++i )
		{
			switch( m_Action[ i ] )
			{
			case ActionKey.Attack :
				ret += GetaPieceConst.COST_ENERGY_ATTACK ;
				break ;
			case ActionKey.Defend :
				ret += GetaPieceConst.COST_ENERGY_DEFEND ;
				break ;
			case ActionKey.Concentrate :
				ret += GetaPieceConst.COST_ENERGY_CONCENTRATE ;
				break ;
			}
		}
		return ret ;
	}
	
	public GetaPieceUnitData()
	{
		Reset() ;
	}

	public void Reset()
	{
		this.HitPoint = this.MaxHitPoint = GetaPieceConst.INIT_HITPOINT ;
		this.Energy = this.MaxEnergy = GetaPieceConst.INIT_ENERGY ;
		this.m_PowerAttack = false ;

	}

	private int m_HitPoint = 0 ;
	private int m_Energy = 0 ;
}

public static class GetaPieceConst
{
	public static int INIT_ENERGY = 10 ;
	public static int INIT_HITPOINT = 10 ;

	public static int COST_ENERGY_ATTACK = 3 ;
	public static int COST_ENERGY_DEFEND = 1 ;
	public static int COST_ENERGY_CONCENTRATE = 0 ;
	
	public static int BUFF_ENERGY_ATTACK_TO_DEFEND = 1 ;
	public static int ENERGY_REFILL_EACH_TURN = 1 ;
	
	public static int DAMAGE_ATTACK_TO_ATTACK = 2 ;
	public static int DAMAGE_ATTACK_TO_CONCENTRATE = 2 ;
	public static int DAMAGE_ATTACK_TO_DEFEND = 0 ;
	public static int DAMAGE_POWER_ATTACK_TO_CONCENTRATE = 3 ;
	public static int DAMAGE_POWER_ATTACK_TO_ATTACK = 3 ;
	public static int DAMAGE_POWER_ATTACK_TO_DEFEND = 1 ;
	
}