using UnityEngine;
using System.Collections;

public class DummyBattlePlay : MonoBehaviour 
{
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
