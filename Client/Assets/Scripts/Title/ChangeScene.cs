using UnityEngine;
using System.Collections;

public class ChangeScene : MonoBehaviour 
{
	public string m_Scene = "" ;

	public void ChangeSceneTo()
	{
		Application.LoadLevel(m_Scene);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
