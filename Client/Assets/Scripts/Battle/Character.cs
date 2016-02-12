using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
    IMesh m_ShareMeshData = null;
    UnitData m_ShareUnitData = null;
    static MeshCreator s_ShareMeshCreator = GlobalSingleton.GetMeshCreator();
    public string m_MeshName = "Unit_1";
    public Vector3 m_TransformOffset = Vector3.zero;

    public UnitData data {
        get { return m_ShareUnitData; }
    }

    public void DoRender() {
        if (m_ShareMeshData != null) {
            s_ShareMeshCreator.DestroyMesh(ref m_ShareMeshData);
        }

        m_ShareMeshData = s_ShareMeshCreator.CreateMesh(m_MeshName);
        if (m_ShareMeshData != null) {
            m_ShareMeshData.Draw(this.gameObject);
        }

        this.transform.position += m_TransformOffset;
    }

    public void DoAction() {

    }

    // Use this for initialization
    void Start () {

        if (m_ShareUnitData == null) {
            GlobalSingleton.GetDataManager().GetUnitData(m_MeshName, ref m_ShareUnitData);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
