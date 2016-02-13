using UnityEngine;
using System.Collections;

public class KeyboardDetect : UnityEngine.UI.InputField {

	public UnityEngine.UI.Text m_DebugText = null ;
	public UnityEngine.UI.Text m_QuestionLabel = null ;

	// Use this for initialization
	void Start () {
		this.onEndEdit.AddListener(val =>
		                                 {
			if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
				Debug.Log("End edit on enter");
		});

		GameObject obj = GameObject.Find("DebugText");
		if( null != obj )
		{
			m_DebugText = obj.GetComponent<UnityEngine.UI.Text>() ;
		}


	}

	
	// Update is called once per frame
	void Update () 
	{
		if( null != m_DebugText && null != m_QuestionLabel  )
		{
			m_DebugText.text = m_QuestionLabel.text ;
			m_DebugText.enabled = this.isFocused ;
		}
	}

	void OnFocus()
	{
		Debug.Log("OnFocus");
		if( null != m_DebugText ) m_DebugText.text = "OnFocus" ;
	}

	void OnDeselect( UnityEngine.EventSystems.BaseEventData eventData)
	{
		Debug.Log("OnDeselect");
		if( null != m_DebugText ) m_DebugText.text = "OnDeselect" ;
	}

	public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
	{
		Debug.Log("OnPointerClick");
		if( null != m_DebugText ) m_DebugText.text = "OnPointerClick" ;
	}

	public void OnSubmit(UnityEngine.EventSystems.BaseEventData eventData)
	{
		Debug.Log("OnSubmit");
		if( null != m_DebugText ) m_DebugText.text = "OnSubmit" ;
	}
}
