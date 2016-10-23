/**
@date 20161023 by NDark 
. add class method StartInitialize()

*/
using UnityEngine;
using System.Collections;

public class DummyBattlePlay : MonoBehaviour 
{
	public virtual void StartInitialize()
	{
	
	}
	
	public virtual bool IsInitialized() 
	{
		return false ;
	}

	public virtual bool IsInAnimation() 
	{
		return false ;
	}

	public virtual void StartBattle() 
	{

	}
	 
	// deprecated
	public virtual void Attack( float _Ratio ) 
	{
		Debug.Log("Attack() _Ratio" + _Ratio );
	}

	// deprecated
	public virtual void Defend( float _Ratio ) 
	{
		Debug.Log("Defend() _Ratio" + _Ratio );
	}

}
