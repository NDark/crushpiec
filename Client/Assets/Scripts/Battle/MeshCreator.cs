using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshCreator {
    static private string s_Path = "Battle/Datas/";
    // static private string s_Ext = ".txt";
    static private int s_UniqueInstanceID = 0;
    Dictionary<string, IMesh> m_MeshTemplateContainer = new Dictionary<string, IMesh>();
    Dictionary<int, IMesh> m_MeshInstanceContainer = new Dictionary<int, IMesh>();

    public IMesh CreateMesh(string meshname) {
        if (!m_MeshTemplateContainer.ContainsKey(meshname))
        {
            IMesh mesh = Load(meshname);
            if (mesh != null && mesh.DoParse()) {
                m_MeshTemplateContainer.Add(meshname, mesh);
            }
        }

        IMesh meshInstance = new IMesh(m_MeshTemplateContainer[meshname]);
        meshInstance.m_InstanceID = s_UniqueInstanceID++;
        m_MeshInstanceContainer.Add(meshInstance.m_InstanceID, meshInstance);
        return meshInstance;
    }

    public void DestroyMesh(ref IMesh _OutMesh) {
        if (m_MeshInstanceContainer.ContainsKey(_OutMesh.m_InstanceID)) {
            m_MeshInstanceContainer.Remove(_OutMesh.m_InstanceID);

            _OutMesh = null;
        }
    }

    IMesh Load(string filename) {
        string filepath = s_Path + filename;
        TextAsset textAsset = Resources.Load(filepath) as TextAsset;
        if (!textAsset) {
            GlobalSingleton.ERROR("Load file failed : " + filepath);
            return null;
        }
        return new IMesh(textAsset.text);
    }
}
