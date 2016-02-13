using UnityEngine;
using System.Collections;

public class StartAnimation : MonoBehaviour 
{
	public Animator [] m_Animators = null ;
	public Animation [] m_Animations = null ;


	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{

		if( null != m_Animators )
		{
			foreach( Animator anim in m_Animators )
			{
				anim.enabled = true ;
			}
			
			m_Animators = null ;
		}
		if( null != m_Animations )
		{
			foreach( Animation anim in m_Animations )
			{
				// anim.enabled = true ;
				anim.Play() ;
			}
			
			m_Animations = null ;
		}
	}
}
