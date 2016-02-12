using UnityEngine;
using System.Collections;

public class DummyBattlePlay : MonoBehaviour 
{
	public virtual bool IsInAnimation() 
	{
		return false ;
	}

	public virtual void Attack( float _Ratio ) 
	{
		Debug.Log("Attack() _Ratio" + _Ratio );
	}

	public virtual void Defend( float _Ratio ) 
	{
		Debug.Log("Defend() _Ratio" + _Ratio );
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
