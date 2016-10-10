using UnityEngine;
using System.Collections.Generic;

public class GetaPieceBattleEvent
{
	public string Type {get;set;} 
	public string Target {get;set;}
	public string Value {get;set;}

	public GetaPieceBattleEventType GetBattleType()
	{
		return GetaPieceBattleEventTypeEnumHelperHub.m_Helper.GetKey( this.Type ) ;
	}

	public int AsInt()
	{
		int ret = 0 ;
		int.TryParse( this.Value , out ret ) ;
		return ret ;
	}
	
	public float AsFloat()
	{
		float ret = 0 ;
		float.TryParse( this.Value , out ret ) ;
		return ret ;
	}
	
	public void DEBUG_Print()
	{
		Debug.Log( this.ToString() );
	}

	public string ToString()
	{
		return "GetaPieceBattleEvent: " +
		          "/n Type=" + this.Type +
		          "/n Target=" + this.Target +
			"/n Value=" + this.Value ;
	}
}


public enum GetaPieceBattleEventType
{
	Damage,
}


public class GetaPieceBattleEventTypeEnumHelper
{
	public Dictionary< string , GetaPieceBattleEventType > m_Map = null ;
	public void Initialize()
	{
		m_Map = new Dictionary<string, GetaPieceBattleEventType>() ;
		m_Map.Add("Damage",GetaPieceBattleEventType.Damage ) ;
		
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
	
	public GetaPieceBattleEventType GetKey( string _Str )
	{
		TryInitialize() ;
		GetaPieceBattleEventType ret = GetaPieceBattleEventType.Damage ;
		m_Map.TryGetValue( _Str , out ret ) ;
		return ret ;
	}
}

public static class GetaPieceBattleEventTypeEnumHelperHub
{
	public static GetaPieceBattleEventTypeEnumHelper m_Helper = new GetaPieceBattleEventTypeEnumHelper() ;
}
