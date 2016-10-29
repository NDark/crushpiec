/**
@file MorphingStruct.cs
@author NDark
@date 20161029 file started.

*/
using UnityEngine;
using System.Collections.Generic;


public class MorphingObj
{
	public GameObject GameObj { get; set ; }
	public Vector3 Target { get; set ; }
}

public class MorphingStruct
{
	static float threashold = 0.1f ;
	static float speed = 7.0f ;
	
	public bool isInMorphing = false ;
	public List<MorphingObj> morphVec = new List<MorphingObj>() ;
	
	public void UpdateMorphData()
	{
		if( false == isInMorphing )
		{
			return ;
		}
		
		bool allisInAnimation = false ;
		foreach( var morphObj in this.morphVec )
		{
			if( null != morphObj.GameObj )
			{
				Vector3 vecToTarget = morphObj.Target - morphObj.GameObj.transform.position ;
				// Debug.Log("vecToTarget.magnitude=" + vecToTarget.magnitude ) ;
				
				if( vecToTarget.magnitude > threashold )
				{
					Vector3 normalVec = vecToTarget ;
					normalVec.Normalize() ;
					normalVec *= ( speed * Time.deltaTime ) ;
					// Debug.Log("normalVec.magnitude=" + normalVec.magnitude ) ;
					
					if( normalVec.magnitude >= vecToTarget.magnitude )
					{
						morphObj.GameObj.transform.position = morphObj.Target ;
					}	
					else
					{
						morphObj.GameObj.transform.Translate( normalVec ) ;
						allisInAnimation = true ;
					}
				}
			}
		}
		
		if( false == allisInAnimation )
		{
			isInMorphing = false ;
		}
	}
}
