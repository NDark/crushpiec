using UnityEngine;
using System.Collections.Generic;

public class CollectionUtility 
{
	
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
}
