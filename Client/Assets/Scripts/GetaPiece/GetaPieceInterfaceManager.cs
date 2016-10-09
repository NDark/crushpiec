using UnityEngine;
using System.Collections.Generic;


public enum GetaPieceInterfaceState
{
	UnActive = 0 ,
	InterfaceInitialize ,
	BattleInitialize ,
	WaitBattleInitialize ,

	EnterRound ,
	WaitPlayerInput ,
	EnterAnimation ,
	WaitAnimation ,
	JudgeVictory ,

	EndGame ,
}

public class GetaPieceInterfaceManager : MonoBehaviour 
{
	public UnityEngine.UI.Image [] m_Component0ButtonsImages = null ;
	public UnityEngine.UI.Image [] m_Component1ButtonsImages = null ;
	public UnityEngine.UI.Image [] m_Component2ButtonsImages = null ;
	
	public UnityEngine.UI.Image m_EnergyLabelBackground = null ;
	public UnityEngine.UI.Text m_EnergyLabel = null ;
	public GameObject m_EnergyGridParent = null ;
	public List<UnityEngine.UI.Image> m_EnergyGrids = new List<UnityEngine.UI.Image>() ;
	public GameObject m_StartButton = null ;

	private ActionKey [] m_SelectedActions = new ActionKey[3] ;

	public int m_EnergyNow = 0 ;

	public void TrySetAction0( string _ActionStr )
	{
		ActionKey key = m_ActionKeyEnumHelper.GetKey( _ActionStr ) ;

		PressComponentButton( 0 , key ) ;

	}

	public void TrySetAction1( string _ActionStr )
	{
		ActionKey key = m_ActionKeyEnumHelper.GetKey( _ActionStr ) ;

		PressComponentButton( 1 , key ) ;

	}

	public void TrySetAction2( string _ActionStr )
	{
		ActionKey key = m_ActionKeyEnumHelper.GetKey( _ActionStr ) ;
		PressComponentButton( 2 , key ) ;

	}
	
	public void SetAction( int _ComponentIndex , ActionKey _Action )
	{
		Debug.Log("SetAction() _ComponentIndex=" + _ComponentIndex + " _Action=" + _Action );
		m_SelectedActions[ _ComponentIndex ] = _Action ;

		int currentCostEnergy = CalculateCostEnergy() ;
		SetEnergyGrid( m_EnergyNow , currentCostEnergy , 0 ) ;
	}

	
	public void TryStart()
	{
		Debug.Log("TryStart" );

		m_State = GetaPieceInterfaceState.EnterAnimation ;
	}

	
	// Update is called once per frame
	void Update () 
	{
		/*
		public enum GetaPieceInterfaceState
		{
			UnActive = 0 ,
			InterfaceInitialize ,
			BattleInitialize ,
			WaitBattleInitialize ,
			
			EnterRound ,
			WaitPlayerInput ,
			EnterAnimation ,
			WaitAnimation ,
			JudgeVictory ,
			
			EndGame ,
		}
		//*/

		switch( m_State ) 
		{
		case GetaPieceInterfaceState.UnActive :
			m_State = GetaPieceInterfaceState.InterfaceInitialize ;
			break ;
		case GetaPieceInterfaceState.InterfaceInitialize :
			InterfaceInitialize() ;
			break ;
		case GetaPieceInterfaceState.BattleInitialize :
			BattleInitialize() ;
			break ;
		case GetaPieceInterfaceState.WaitBattleInitialize :
			WaitBattleInitialize() ;
			break ;
		case GetaPieceInterfaceState.EnterRound :
			EnterRound() ;
			break ;
		case GetaPieceInterfaceState.WaitPlayerInput :
			break ;
		case GetaPieceInterfaceState.EnterAnimation :
			EnterAnimation() ;
			break ;
		case GetaPieceInterfaceState.WaitAnimation :
			WaitAnimation() ;
			break ;
		case GetaPieceInterfaceState.JudgeVictory :
			JudgeVictory() ;
			break ;
		case GetaPieceInterfaceState.EndGame : 
			break ;


		}

	}


	private void InterfaceInitialize()
	{
		if( null == m_EnergyGridParent )
		{
			Debug.LogError("null == m_EnergyGridParent");
			return ;
		}

		InterfaceInitialize_Data() ;

		InterfaceInitialize_EnergyGrid() ;

		m_State = GetaPieceInterfaceState.BattleInitialize ;
	}
	
	private void InterfaceInitialize_Data()
	{

		for( int i = 0 ; i < m_SelectedActions.Length ; ++i )
		{
			m_SelectedActions[ i ] = ActionKey.Concentrate ;
		}

	}
	
	private void InterfaceInitialize_EnergyGrid()
	{
		UnityEngine.UI.Image image = null ;
		Transform trans = null ;
		int size = 10 ;
		for( int i = 0 ; i < size ; ++i )
		{
			trans = m_EnergyGridParent.transform.FindChild("E" + i.ToString() ) ;
			if( null == trans )
			{
				Debug.LogWarning("null == trans i=" + i );
				continue ;
			}
			
			image = trans.gameObject.GetComponent<UnityEngine.UI.Image>() ;
			if( null != image )
			{
				m_EnergyGrids.Add( image ) ;
			}
			
		}
		Debug.Log("m_EnergyGrids.Count=" + m_EnergyGrids.Count );
	}

	private void BattleInitialize()
	{
		m_State = GetaPieceInterfaceState.WaitBattleInitialize ;
	}
	
	private void WaitBattleInitialize()
	{
		m_State = GetaPieceInterfaceState.EnterRound ;
	}
	
	private void EnterRound()
	{

		PressComponentButton( 0 , ActionKey.Concentrate ) ;
		PressComponentButton( 1 , ActionKey.Concentrate ) ;
		PressComponentButton( 2 , ActionKey.Concentrate ) ;

		m_State = GetaPieceInterfaceState.WaitPlayerInput ;
	}
	
	private void EnterAnimation()
	{
		m_State = GetaPieceInterfaceState.WaitAnimation ;
	}
	
	private void WaitAnimation()
	{
		m_State = GetaPieceInterfaceState.JudgeVictory ;
	}
	
	private void JudgeVictory()
	{
		m_State = GetaPieceInterfaceState.EnterRound ;
		// m_State = GetaPieceInterfaceState.EndGame ;

	}

	private void SetEnergyGrid( int _EnergyNow , int _PreCostValue , int _PreBuffValue )
	{
		int maxSize = m_EnergyGrids.Count ;
		int costedValue = _EnergyNow - _PreCostValue ;
		// Debug.Log("costedValue="+costedValue);

		for( int i = 0 ; i < maxSize ; ++i )
		{

			ShowEnergyTooLow( costedValue < 0 ) ;

			if( i >= _EnergyNow ) 
			{
				m_EnergyGrids[ i ].color = Color.grey ;
			}
			else if( i < _EnergyNow && i >= costedValue )
			{
				m_EnergyGrids[ i ].color = Color.red ;
			}
			else 
			{
				m_EnergyGrids[ i ].color = Color.green ;
			}
		}


	}

	private void ShowEnergyTooLow( bool _Show )
	{
		if( null == m_EnergyLabel )
		{
			return ;
		}

		m_EnergyLabel.color = (_Show) ? COLOR_PURPLE : COLOR_HIDE ;
		m_EnergyLabel.text = (_Show) ? "Energy Too Low!!!" : "Energy" ;
		m_EnergyLabelBackground.enabled = _Show ;

		m_StartButton.SetActive( !_Show ) ;
	}

	private void PressComponentButton( int _ComponentIndex , ActionKey _Action )
	{
		int keyIndex = (int) _Action ;
		UnityEngine.UI.Image [] _Images = null ;
		switch( _ComponentIndex )
		{
		case 0 :
			_Images = m_Component0ButtonsImages ;
			break ;
		case 1 :
			_Images = m_Component1ButtonsImages ;
			break ;
		case 2 :
			_Images = m_Component2ButtonsImages ;
			break ;
		}

		SetColorForSelectComponentButtons( _Images , keyIndex ) ;
		
		SetAction( _ComponentIndex , _Action ) ;
	}

	private void SetColorForSelectComponentButtons( UnityEngine.UI.Image [] _ImageArray , int _SelectIndex )
	{
		if( null == _ImageArray )
		{
			return ;
		}

		for( int i = 0 ; i < _ImageArray.Length ; ++i )
		{
			_ImageArray[ i ].color = ( i != _SelectIndex ) ? ( Color.white ) : (Color.green) ;
		}
	}

	private int CalculateCostEnergy()
	{
		int ret = 0 ;
		for( int i = 0 ; i < this.m_SelectedActions.Length ; ++i )
		{
			if( this.m_SelectedActions[ i ] == ActionKey.Attack )
			{
				ret += 2 ;
			}
			else if( this.m_SelectedActions[ i ] == ActionKey.Defend )
			{
				ret += 1 ;
			}
		}
		return ret ;
	}

	private ActionKeyEnumHelper m_ActionKeyEnumHelper = new ActionKeyEnumHelper() ;
	private GetaPieceInterfaceState m_State = GetaPieceInterfaceState.UnActive ;
	private Color COLOR_PURPLE = new Color( 1 , 0 , 1 ) ;
	private Color COLOR_HIDE = new Color( 50.0f/255.0f  ,  50.0f/255.0f ,  50.0f/255.0f , 114.0f / 255.0f  ) ;
}

public class GetaPieceUnitData
{
	public int HitPoint {get;set;}
	public int Energy {get;set;}
	public ActionKey [] m_Action = new ActionKey[3] ;
	public bool m_PowerAttack = false ;

	public int CalculateSufferDamage( GetaPieceUnitData _Enemy )
	{
		int ret = 0 ;
		for( int i = 0 ; i < m_Action.Length && i < _Enemy.m_Action.Length ; ++i )
		{
			if( _Enemy.m_Action[ i ] == ActionKey.Attack )
			{
				if( m_Action[ i ] == ActionKey.Defend )
				{
					ret += (_Enemy.m_PowerAttack) ? GetaPieceConst.DAMAGE_POWER_ATTACK_TO_DEFEND : GetaPieceConst.DAMAGE_ATTACK_TO_DEFEND ;
				}
				else if( m_Action[ i ] == ActionKey.Attack )
				{
					ret += (_Enemy.m_PowerAttack) ? GetaPieceConst.DAMAGE_POWER_ATTACK_TO_ATTACK : GetaPieceConst.DAMAGE_ATTACK_TO_ATTACK ;
				}
				else if( m_Action[ i ] == ActionKey.Concentrate )
				{
					ret += (_Enemy.m_PowerAttack) ? GetaPieceConst.DAMAGE_POWER_ATTACK_TO_CONCENTRATE : GetaPieceConst.DAMAGE_ATTACK_TO_CONCENTRATE ;
				}
			}
		}
		return ret ;
	}
	
	public int CalculateEnergyBuff( GetaPieceUnitData _Enemy )
	{
		int ret = 0 ;
		for( int i = 0 ; i < m_Action.Length && i < _Enemy.m_Action.Length ; ++i )
		{
			if( _Enemy.m_Action[ i ] == ActionKey.Attack 
				&& m_Action[ i ] == ActionKey.Defend ) 
			{
				ret += GetaPieceConst.BUFF_ENERGY_ATTACK_TO_DEFEND ;
			}
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
}

public static class GetaPieceConst
{
	public static int COST_ENERGY_ATTACK = 3 ;
	public static int COST_ENERGY_DEFEND = 1 ;
	public static int COST_ENERGY_CONCENTRATE = 0 ;

	public static int BUFF_ENERGY_ATTACK_TO_DEFEND = 1 ;

	public static int DAMAGE_ATTACK_TO_ATTACK = 2 ;
	public static int DAMAGE_ATTACK_TO_CONCENTRATE = 2 ;
	public static int DAMAGE_ATTACK_TO_DEFEND = 0 ;
	public static int DAMAGE_POWER_ATTACK_TO_CONCENTRATE = 3 ;
	public static int DAMAGE_POWER_ATTACK_TO_ATTACK = 3 ;
	public static int DAMAGE_POWER_ATTACK_TO_DEFEND = 1 ;

}

public enum ActionKey 
{
	Attack = 0 , 
	Defend , 
	Concentrate ,
}

public class ActionKeyEnumHelper
{
	public Dictionary< string , ActionKey > m_Map = null ;
	public void Initialize()
	{
		m_Map = new Dictionary<string, ActionKey>() ;
		m_Map.Add("Attack",ActionKey.Attack ) ;
		m_Map.Add("Defend",ActionKey.Defend ) ;
		m_Map.Add("Concentrate",ActionKey.Concentrate ) ;
	}
	
	
	public void TryInitialize()
	{
		if( null == m_Map )
		{
			Initialize() ;
		}
	}
	
	public string GetString( ActionKey _Key )
	{
		TryInitialize() ;
		return _Key.ToString() ;
	}
	
	public ActionKey GetKey( string _Str )
	{
		TryInitialize() ;
		ActionKey ret = ActionKey.Attack ;
		m_Map.TryGetValue( _Str , out ret ) ;
		return ret ;
	}
}
