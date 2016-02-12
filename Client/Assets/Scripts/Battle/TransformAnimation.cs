using UnityEngine;
using System.Collections;

public class TransformAnimation : MonoBehaviour {
    public enum AnimationState
    {
        Idle,
        Attack,
        Attack_Start,
        Attack_DoAttack,
        Attack_End,
        Hitted,
        Defend,

        InValid,
    }

    AnimationState m_State = AnimationState.InValid;
    GameObject m_RootRef = null;
    float m_ElapsedTime = 0.0f;
    float m_ScaleFactor = 1.0f;
    Vector3 m_PositionRef = Vector3.zero;
    TransformCache m_TransformRef;
    /// 
    float CONST_TIME_IDLE = 5.0f;
    float CONST_TIME_ATTACK_START = 1.0f;
    float CONST_TIME_ATTACK_DOATTACK = 1.0f;
    float CONST_TIME_ATTACK_END = 1.0f;
    /// 

    public void DoAttack() {
        m_State = AnimationState.Attack;
        m_ElapsedTime = 0.0f;
    }

    void DoRestore() {
        m_TransformRef.Restore();
    }

    // Use this for initialization
    void Start () {
        m_RootRef = gameObject;
        m_TransformRef = new TransformCache(m_RootRef.transform);
        m_ScaleFactor = (-m_RootRef.transform.localScale.x) / m_RootRef.transform.localScale.x;
        m_State = AnimationState.Idle;
    }
	
	// Update is called once per frame
	void Update () {

        m_ElapsedTime += Time.deltaTime;
        m_PositionRef = m_RootRef.transform.position;

        switch (m_State)
        {
            case AnimationState.Idle:
                m_PositionRef.y = 1.0f * Mathf.Sin(CONST_TIME_IDLE * m_ElapsedTime);
                m_RootRef.transform.position = m_PositionRef;
                break;

            case AnimationState.Attack:
                float ratio = Mathf.Min(m_ElapsedTime / CONST_TIME_ATTACK_START, 1.0f);
                float targetRot = -m_ScaleFactor * 30.0f * ratio;
                float targetScale = 1.0f + 1.0f * ratio;
                m_RootRef.transform.rotation = Quaternion.AngleAxis(targetRot, Vector3.up);
                m_RootRef.transform.localScale = new Vector3(m_ScaleFactor * targetScale, targetScale, targetScale);
                m_PositionRef.z = -5.0f * ratio;
                m_RootRef.transform.position = m_PositionRef;

                if (m_ElapsedTime > CONST_TIME_ATTACK_START)
                {
                    m_ElapsedTime = 0.0f;
                    m_State = AnimationState.Attack_DoAttack;
                }
                break;

            case AnimationState.Attack_DoAttack:
                float attackRatio = Mathf.Min(m_ElapsedTime / CONST_TIME_ATTACK_DOATTACK, 1.0f);
                m_PositionRef.x = m_ScaleFactor * attackRatio * 6.0f;
                m_RootRef.transform.position = m_PositionRef;
                if (m_ElapsedTime > CONST_TIME_ATTACK_DOATTACK)
                {
                    m_State = AnimationState.Attack_End;
                }
                break;

            case AnimationState.Attack_End:
                if (m_ElapsedTime > CONST_TIME_ATTACK_END)
                {
                    DoRestore();

                    m_State = AnimationState.Idle;
                }
                break;

            case AnimationState.Hitted:
                break;

            case AnimationState.Defend:
                break;
        }
    }
}
