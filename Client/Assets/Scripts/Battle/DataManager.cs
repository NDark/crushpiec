using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataManager {
    static int s_UniqueInstanceID = 0;
    static string s_Path = "Battle/Data/";

    Dictionary<string, UnitData> m_DataTemplateContainer = new Dictionary<string, UnitData>();
    Dictionary<int, UnitData> m_DataInstanceContainer = new Dictionary<int, UnitData>();

    public void GetUnitData(string _TypeName, ref UnitData _UnitDataRef)
    {
        if (!m_DataTemplateContainer.ContainsKey(_TypeName))
        {
            UnitData data = new UnitData();
            data.m_HP.Set(10, 0);
            data.m_ATK.Set(5, 1);
            data.m_DEF.Set(2, 1);

            m_DataTemplateContainer.Add(_TypeName, data);            
        }

        if ((_UnitDataRef != null) && m_DataInstanceContainer.ContainsKey(_UnitDataRef.m_InstanceID))
        {
            m_DataInstanceContainer.Remove(_UnitDataRef.m_InstanceID);
            _UnitDataRef = null;
        }

        _UnitDataRef = new UnitData();
        _UnitDataRef.Copy(m_DataTemplateContainer[_TypeName]);
        _UnitDataRef.m_InstanceID = s_UniqueInstanceID++;

        m_DataInstanceContainer.Add(_UnitDataRef.m_InstanceID, _UnitDataRef);
    }

    UnitData Load(string _FilePathName)
    {
        string filepath = s_Path + _FilePathName;
        TextAsset textAsset = Resources.Load(filepath) as TextAsset;
        if (!textAsset)
        {
            GlobalSingleton.ERROR("Load unit data failed : " + filepath);
            return null;
        }
       
        UnitData data = new UnitData();
        if (false == data.DoParse(textAsset.text))
        {
            GlobalSingleton.ERROR("Parse unit data failed : " + textAsset.text);
            return null;
        }

        return data;
    }
}