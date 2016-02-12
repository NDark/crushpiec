using UnityEngine;
using System.Collections;

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
}
