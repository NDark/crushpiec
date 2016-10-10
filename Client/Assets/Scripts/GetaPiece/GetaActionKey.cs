using UnityEngine;
using System.Collections.Generic;

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