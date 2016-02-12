using UnityEngine;
using System.Collections;

public class UnitData {
    public int m_InstanceID;
    public Attribute m_HP = new Attribute();
    public Attribute m_ATK = new Attribute();
    public Attribute m_DEF = new Attribute();

    public void Copy(UnitData _InUnitData)
    {
        m_HP.Set(_InUnitData.m_HP);
        m_ATK.Set(_InUnitData.m_ATK);
        m_DEF.Set(_InUnitData.m_DEF);
    }

    public bool DoParse(string _SourceStr)
    {
        /// parse attribute from file
        return true;    
    }
}
