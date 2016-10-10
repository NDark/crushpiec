using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class GlobalSingleton
{
    public static void ERROR(string _LogStr) {
        Debug.LogError(_LogStr); 
    }

    public static void DEBUG(string _LogStr) {
        Debug.Log(_LogStr);
    }

    private static MeshCreator m_MeshCreator = null;
    public static MeshCreator GetMeshCreator()
    {
        if (null == m_MeshCreator)
        {
            m_MeshCreator = new MeshCreator();
        }
        return m_MeshCreator;
    }

    private static DataManager m_DataManager = null;
    public static DataManager GetDataManager()
    {
        if (null == m_DataManager)
        {
            m_DataManager = new DataManager();
        }
        return m_DataManager;
    }

    private static FxManager m_FxManager = null;
    public static FxManager GetFxManager()
    {
        if (null == m_FxManager)
        {
            GameObject obj = GameObject.Find("FxManager");
            if (obj != null) {
                m_FxManager = obj.GetComponent<FxManager>();
            }
        }
        return m_FxManager;
    }

    static Dictionary<string, GameObject> m_CacheMap = new Dictionary<string, GameObject>();
    public static GameObject Find(string _name, bool _findInactive = false)
    {
        GameObject ret = null;
        if (false == m_CacheMap.TryGetValue(_name, out ret))
        {
            ret = GameObject.Find(_name);
            if (ret == null && _findInactive == true)
            {
                ret = Array.Find
                    (
                        Resources.FindObjectsOfTypeAll<GameObject>() as GameObject[],
                        go => go.name == _name
                    );
            }

            // cache game object
            if (null != ret)
            {
                m_CacheMap.Add(_name, ret);
            }
        }

        if (null == ret)
        {
            DEBUG("Cannot find game object by name = " + _name);
        }
        return ret;
    }
}
