using UnityEngine;
using System.Collections;

public class OpenURL : MonoBehaviour 
{
	public string m_URL = "" ;// 需指定URL字串
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Open()
	{
		// Debug.Log( Application.platform ) ;
		if( Application.platform == RuntimePlatform.WindowsWebPlayer )
		{
			string url = "window.open('" + m_URL + "','aNewWindow')" ;
			// Debug.Log( url ) ;
			Application.ExternalEval( url );					
		}
		else /*if( Application.platform == RuntimePlatform.WindowsPlayer ||
				 Application.platform == RuntimePlatform.WindowsEditor )*/
		{
			Application.OpenURL( m_URL ) ;		
		}
	}
}
