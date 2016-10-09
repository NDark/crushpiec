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

	private ActionKey [] m_SelectedActions = new ActionKey[3] ;

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
	
	public void TrySetAction( int _ComponentIndex , ActionKey _Action )
	{
		Debug.Log("TrySetAction _ComponentIndex=" + _ComponentIndex + " _Action=" + _Action );
		m_SelectedActions[ _ComponentIndex ] = _Action ;

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
		m_State = GetaPieceInterfaceState.BattleInitialize ;
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
		
		TrySetAction( _ComponentIndex , _Action ) ;
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

	private ActionKeyEnumHelper m_ActionKeyEnumHelper = new ActionKeyEnumHelper() ;
	private GetaPieceInterfaceState m_State = GetaPieceInterfaceState.UnActive ;
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
