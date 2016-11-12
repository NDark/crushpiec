/**
@file MorphingStruct.cs
@author NDark
@date 20161029 file started.

@date 20161112 by NDark 
. add code of color morphing at UpdateMorphData()

*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;



public class MorphingObj
{
	public GameObject GameObj { get; set ; }
	public Vector3 Target { get; set ; }
	
	public Color DestinationColor { get; set ; }
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
				
				Color morphColor ;
				bool setColor = false ;
				
				if( vecToTarget.magnitude > threashold )
				{
					Vector3 normalVec = vecToTarget ;
					normalVec.Normalize() ;
					normalVec *= ( speed * Time.deltaTime ) ;
					// Debug.Log("normalVec.magnitude=" + normalVec.magnitude ) ;
					
					if( normalVec.magnitude >= vecToTarget.magnitude )
					{
						morphObj.GameObj.transform.position = morphObj.Target ;
						morphColor = morphObj.DestinationColor ;
						setColor = true ;
					}	
					else
					{
						morphObj.GameObj.transform.Translate( normalVec ) ;
						allisInAnimation = true ;
					}
				}
				
				
				MeshFilter meshFilter = morphObj.GameObj.GetComponent<MeshFilter>();
				Vector3[] vertices = meshFilter.mesh.vertices;
				
				if( false == setColor 
				&& meshFilter.mesh.colors.Length > 0 )
				{
					Vector3 currentColor = new Vector3( meshFilter.mesh.colors[0].r , meshFilter.mesh.colors[0].g , meshFilter.mesh.colors[0].b ) ;
					Vector3 destinationColor = new Vector3( morphObj.DestinationColor.r , morphObj.DestinationColor.g , morphObj.DestinationColor.b ) ;
					Vector3 colorToTarget = destinationColor - currentColor ;
					
					
					Vector3 normalVec = colorToTarget ;
					normalVec.Normalize() ;
					normalVec *= ( speed * Time.deltaTime ) ;
					// Debug.Log("normalVec.magnitude=" + normalVec.magnitude ) ;
					
					if( normalVec.magnitude >= colorToTarget.magnitude )
					{
						morphColor = morphObj.DestinationColor ;
					}	
					else
					{
						currentColor = currentColor + normalVec ;
						morphColor.r = currentColor.x ;
						morphColor.g = currentColor.y ;
						morphColor.b = currentColor.z ;
					}
					
				}
				
				Color[] colors = System.Linq.Enumerable.Repeat( morphColor, vertices.Length).ToArray() ;
				meshFilter.mesh.colors = colors;
				
			}
			
			
			
		}
		
		if( false == allisInAnimation )
		{
			isInMorphing = false ;
		}
	}
}
