﻿using UnityEngine;
using System.Collections;

public class BoneAnimation : MonoBehaviour {
    public Vector3 m_StartPoint = Vector3.zero;
    public Vector3 m_EndPoint = new Vector3(32.0f, 0.0f, -10.0f);
    public float m_ElapsedTime = 3.0f;
    public float m_Speed = 100.0f;
    public float m_Rotation = 0.0f;
    public float m_Distance = 0;

    // Use this for initialization
    void Start () {

        foreach (Transform child in transform) {
            Debug.LogError(child.position + "  ,  " + transform.position);
            Vector3 forceDir = (child.position - transform.position) * Random.Range(100.0f, 400.0f);
            Rigidbody rigidBody = child.gameObject.AddComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.AddForce(forceDir);
        }
        /*
        m_Distance = Vector3.Distance(m_EndPoint, m_StartPoint);
        m_ElapsedTime = 0.0f;
        */
    }
	
	// Update is called once per frame
	void Update () {
        /*
        m_ElapsedTime += Time.deltaTime;
        m_Rotation += (0.1f * m_ElapsedTime);

        transform.position = Vector3.Lerp(m_StartPoint, m_EndPoint, 
            m_Speed * m_ElapsedTime / m_Distance);
        */
    }
}
