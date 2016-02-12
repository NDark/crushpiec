using UnityEngine;
using System.Collections;

public class ReadVersion : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		TextAsset ta = Resources.Load("version") as TextAsset;
		if( null != ta )
		{
			UnityEngine.UI.Text text = this.GetComponent<UnityEngine.UI.Text>() ;
			if( null != text )
			{
				text.text = ta.text ;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
