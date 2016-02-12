using UnityEngine;
using System.Collections;

public class TransformCache {
    public Vector3 m_PositionRef;
    public Vector3 m_ScaleRef;
    public Quaternion m_RotationRef;
    Transform m_TransformRef;

    public TransformCache(Transform _Trans)
    {
        m_PositionRef = _Trans.localPosition;
        m_ScaleRef = _Trans.localScale;
        m_RotationRef = _Trans.localRotation;

        m_TransformRef = _Trans;
    }

    public void Restore()
    {
        m_TransformRef.localPosition = m_PositionRef;
        m_TransformRef.localScale = m_ScaleRef;
        m_TransformRef.localRotation = m_RotationRef;
    }
}
