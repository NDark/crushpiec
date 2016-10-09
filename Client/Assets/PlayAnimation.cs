using UnityEngine;
using System.Collections;

public class PlayAnimation : MonoBehaviour 
{
	public string m_AnimationName = "" ; 
	public void Play()
	{
		Animation anim = this.GetComponent<Animation>() ;
		if( anim )
		{
			anim.Play( m_AnimationName );
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
