using UnityEngine;
using System.Collections.Generic;

public class CollectionUtility 
{
	
	public static void List_Vector3Color_Shuffle( List<Vector3> _Vector3List , List<Color> _ColorList)
	{
		for(int i=0; i < _Vector3List.Count && i < _ColorList.Count ; i++)
		{
			int shuffleIndex = Random.Range(i, _Vector3List.Count); 
			CollectionUtility.List_Vector3_Swap(_Vector3List, i , shuffleIndex ) ;
			CollectionUtility.List_T_Swap( _ColorList , i , shuffleIndex ) ;
			
		}
	}
	
	public static void List_Vector3_Shuffle( List<Vector3> list )
	{
		for(int i=0; i < list.Count; i++)
		{
			CollectionUtility.List_Vector3_Swap(list, i , Random.Range(i, list.Count) ) ;
		}
	}
	
	public static void List_Vector3_Swap(List<Vector3> list, int i, int j)
	{
		var temp = list[i];
		list[i] = list[j];
		list[j] = temp;
	}
	
	public static void List_T_Swap<T>(List<T> list, int i, int j)
	{
		var temp = list[i];
		list[i] = list[j];
		list[j] = temp;
	}
	
}
