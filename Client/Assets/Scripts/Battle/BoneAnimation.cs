using UnityEngine;
using System.Collections;

public class BoneAnimation : MonoBehaviour {
    public Vector3 m_StartPoint = Vector3.zero;
    public Vector3 m_EndPoint = new Vector3(32.0f, 0.0f, -10.0f);
    public float m_ElapsedTime = 3.0f;
    public float m_Speed = 5.0f;
    public float m_Rotation = 0.0f;
    public float m_Distance = 0;

    // Use this for initialization
    void Start () {
        m_Distance = Vector3.Distance(m_EndPoint, m_StartPoint);
        m_ElapsedTime = 0.0f;
    }
	
	// Update is called once per frame
	void Update () {
        m_ElapsedTime += Time.deltaTime;
        m_Rotation += (0.1f * m_ElapsedTime);

        transform.position = Vector3.Lerp(m_StartPoint, m_EndPoint, 
            m_Speed * m_ElapsedTime / m_Distance);
    }
}
