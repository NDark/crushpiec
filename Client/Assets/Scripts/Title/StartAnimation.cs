using UnityEngine;
using System.Collections;

public class StartAnimation : MonoBehaviour 
{
	public Animator m_PressAnyKeyAnimator = null ;
	public Animator [] m_Animations = null ;


	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( null != m_PressAnyKeyAnimator )
		{
			m_PressAnyKeyAnimator.enabled = true ;
			m_PressAnyKeyAnimator = null ;
		}

		if( null != m_Animations )
		{
			foreach( Animator anim in m_Animations )
			{
				anim.enabled = true ;
			}

			m_Animations = null ;
		}
	}
}
