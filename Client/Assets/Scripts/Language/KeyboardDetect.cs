using UnityEngine;
using System.Collections;

public class KeyboardDetect : MonoBehaviour {

	public UnityEngine.UI.Text m_Text = null ;

	// Use this for initialization
	void Start () {

	}

	
	// Update is called once per frame
	void Update () 
	{

	}

	void OnFocus()
	{
		Debug.Log("OnFocus");
		m_Text.text = "OnFocus" ;
	}

	void OnDeselect( UnityEngine.EventSystems.BaseEventData eventData)
	{
		Debug.Log("OnDeselect");
		m_Text.text = "OnDeselect" ;
	}

	public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
	{
		Debug.Log("OnPointerClick");
		m_Text.text = "OnPointerClick" ;
	}

	public void OnSubmit(UnityEngine.EventSystems.BaseEventData eventData)
	{
		Debug.Log("OnSubmit");
		m_Text.text = "OnSubmit" ;
	}
}
