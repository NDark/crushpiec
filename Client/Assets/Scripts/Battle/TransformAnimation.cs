using UnityEngine;
using System.Collections;

public enum AnimationState
{
    Idle,
    Attack,
    Attack_Start,
    Attack_DoAttack,
    Attack_End,
    Hitted,
    Defend,

    InValid
}

public class TransformAnimation : MonoBehaviour {
    public class Define {
        static public float TIME_IDLE = 2.5f;
        static public float TIME_ATTACK_START = 0.1f;
        static public float TIME_ATTACK_DO = 0.05f;
        static public float TIME_ATTACK_END = 0.1f;
        static public float TIME_HITTED = 0.1f;

        static public float TIME_ATTACK = TIME_ATTACK_START + TIME_ATTACK_DO + TIME_ATTACK_END/2.0f;
    };

    AnimationState m_State = AnimationState.InValid;
    AnimationState m_NextState = AnimationState.InValid;
    GameObject m_RootRef = null;
    float m_ElapsedTime = 0.0f;
    float m_ScaleFactor = 1.0f;
    Vector3 m_PositionRef = Vector3.zero;
    TransformCache m_TransformRef;

    IEnumerator PerformChangeAnimationState(AnimationState _State, float _DelayTime, bool _RestoreTransform)
    {
        yield return new WaitForSeconds(_DelayTime);

        // perform change
        m_State = _State;
        m_NextState = AnimationState.InValid;
        m_ElapsedTime = 0.0f;

        if (_RestoreTransform)
        {
            m_TransformRef.Restore();
        }
    }

    public void ChangeAnimationState(AnimationState _NextState, float _DelayTime, bool _RestoreTransform = false)
    {
        if (m_NextState != _NextState) {
            m_NextState = _NextState;
            StartCoroutine(PerformChangeAnimationState(_NextState, _DelayTime, _RestoreTransform));
        }
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
                m_PositionRef.y = 0.5f * Mathf.Sin(Define.TIME_IDLE * m_ElapsedTime);
                m_RootRef.transform.position = m_PositionRef;
                break;

            case AnimationState.Attack:
                float ratio = Mathf.Min(m_ElapsedTime / Define.TIME_ATTACK_START, 1.0f);
                float targetRot = (m_ScaleFactor > 0) ? 30.0f * ratio : 150.0f * ratio;
                float targetScale = 1.0f + 1.0f * ratio;
                m_RootRef.transform.rotation = Quaternion.AngleAxis(targetRot, Vector3.up);
                m_RootRef.transform.localScale = new Vector3(m_ScaleFactor * targetScale, targetScale, targetScale);
                m_PositionRef.z = -5.0f * ratio;
                m_RootRef.transform.position = m_PositionRef;

                ChangeAnimationState(AnimationState.Attack_DoAttack, Define.TIME_ATTACK_START);                
                break;

            case AnimationState.Attack_DoAttack:
                float attackRatio = Mathf.Min(m_ElapsedTime / Define.TIME_ATTACK_DO, 1.0f);
                m_PositionRef.x = m_ScaleFactor * attackRatio * 1.0f;
                m_RootRef.transform.position = m_PositionRef;
                
                ChangeAnimationState(AnimationState.Attack_End, Define.TIME_ATTACK_DO);
                break;

            case AnimationState.Attack_End:
                ChangeAnimationState(AnimationState.Idle, Define.TIME_ATTACK_END, true);
                break;

            case AnimationState.Hitted:
                m_PositionRef = m_TransformRef.m_PositionRef;
                m_PositionRef.x += 2.0f * Random.Range(-1.0f, 1.0f);
                m_PositionRef.y += 2.0f * Random.Range(-1.0f, 1.0f);
                m_RootRef.transform.position = m_PositionRef;

                // bone animation
                Character selfRef = GetComponent<Character>();
                GlobalSingleton.GetFxManager().DoBreakBoneAnimation(ref selfRef);

                ChangeAnimationState(AnimationState.Idle, Define.TIME_HITTED, true);
                break;

            case AnimationState.Defend:
                break;
        }
    }
}
