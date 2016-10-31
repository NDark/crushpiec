using UnityEngine;
using System.Collections;

public class UGUIImageVibration : MonoBehaviour 
{
	public float RandomRange = 4.0f ;
	
	public void ActiveVibration( float _ElapsedTime )
	{
		m_IsActive = true ;
		m_OrgPos = m_Rect.anchoredPosition ;
		m_StopTime = Time.time + _ElapsedTime ;
	}

	public void StopVibration()
	{
		m_IsActive = false ;
		if( null != m_Rect )
		{
			m_Rect.anchoredPosition = m_OrgPos ;
		}
	}
	
	UnityEngine.RectTransform m_Rect = null ;
	bool m_IsActive = false ;
	float m_StopTime { get; set;}
	Vector2 m_OrgPos { get; set;}
	
	// Use this for initialization
	void Start () 
	{
		m_Rect = this.gameObject.GetComponent<RectTransform>() ;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( true == m_IsActive )
		{
			if( Time.time >= m_StopTime )
			{
				StopVibration() ;
				return; 
			}
			
			Vector2 randomVec = new Vector2( 
			                                Random.Range ( -1 * RandomRange , RandomRange ) 
			                                , Random.Range ( -1 * RandomRange , RandomRange ) ) ;
			randomVec += m_OrgPos ;
			m_Rect.anchoredPosition = randomVec ;
		}
	}
}
