using UnityEngine;
using System.Collections.Generic;

public class GetaPieceInterfaceManager : MonoBehaviour 
{
	public void TrySetAction0( string _ActionStr )
	{
		ActionKey key = m_ActionKeyEnumHelper.GetKey( _ActionStr ) ;
		TrySetAction( 0 , key ) ;
	}

	public void TrySetAction1( string _ActionStr )
	{
		ActionKey key = m_ActionKeyEnumHelper.GetKey( _ActionStr ) ;
		TrySetAction( 1 , key ) ;
	}

	public void TrySetAction2( string _ActionStr )
	{
		ActionKey key = m_ActionKeyEnumHelper.GetKey( _ActionStr ) ;
		TrySetAction( 2 , key ) ;
	}
	
	public void TrySetAction( int _ComponentIndex , ActionKey _Action )
	{
		Debug.Log("TrySetAction _ComponentIndex=" + _ComponentIndex + " _Action=" + _Action );

	}

	
	public void TryStart()
	{
		Debug.Log("TryStart" );

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private ActionKeyEnumHelper m_ActionKeyEnumHelper = new ActionKeyEnumHelper() ;

}


public enum ActionKey 
{
	Attack , 
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
