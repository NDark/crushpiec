using UnityEngine;
using System.Collections;

public class KeyboardDetect : MonoBehaviour {

	public UnityEngine.UI.Text m_Text = null ;
	TouchScreenKeyboard keyboard = null ;
	// Use this for initialization
	void Start () {
		keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
	}

	
	// Update is called once per frame
	void Update () 
	{
		/*
		if ( null != keyboard && 
		    keyboard.text != null && keyboard.done)
		{
			print ("User input is: " + keyboard.text);

		}
*/
		if( null != m_Text )
		{
			Event e = Event.current;


			if( null != e )
			{
				if (e.type == EventType.keyDown && e.keyCode == KeyCode.Return)
					//Put in what you want here
				{
					
				}
				print ("null != e" );
				m_Text.text = e.type.ToString() ;
			}
		}
	}
}
