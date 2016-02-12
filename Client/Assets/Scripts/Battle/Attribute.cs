using UnityEngine;
using System.Collections;

public class Attribute {
    int m_MaxValue;
    int m_MinValue;
    int m_Value;

    public int value 
    {
        get { return m_Value; }
    }
         
    public int randomOneValue
    {
        get { return Random.Range(m_MinValue, m_MaxValue); }
    }   

    public Attribute() {
        m_MaxValue = m_MinValue = m_Value = 0;
    }

    public void Set(int _Max, int _Min = 0)
    {
        m_MaxValue = _Max;
        m_MinValue = _Min;
        m_Value = _Max;
    }

    public void Set(Attribute _Attr)
    {
        m_MaxValue = _Attr.m_MaxValue;
        m_MinValue = _Attr.m_MinValue;
        m_Value = _Attr.value;
    }

    public void Offset(int _OffsetValue)
    {
        m_Value += _OffsetValue;
        m_Value = Mathf.Min(Mathf.Max(m_Value, m_MinValue), m_MaxValue);
    }
}
