using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
    public IMesh m_ShareMeshData = null;
    UnitData m_ShareUnitData = null;
    TransformAnimation m_ShareAnimation = null;

    static MeshCreator s_ShareMeshCreator = GlobalSingleton.GetMeshCreator();
    public string m_MeshName = "Unit_1";

    public Attribute hp { get { return m_ShareUnitData.m_HP; } }
    public Attribute atk { get { return m_ShareUnitData.m_ATK; } }
    public Attribute def { get { return m_ShareUnitData.m_DEF; } }

    public void DoRender() {
        if (m_ShareMeshData != null) {
            s_ShareMeshCreator.DestroyMesh(ref m_ShareMeshData);
        }

        m_ShareMeshData = s_ShareMeshCreator.CreateMesh(m_MeshName);
        if (m_ShareMeshData != null) {
            m_ShareMeshData.Draw(this.gameObject);
        }
    }

    public void DoAction(AnimationState _State, float _Delay = 0.0f) {
        m_ShareAnimation.ChangeAnimationState(_State, _Delay);
    }
    
    // Use this for initialization
    void Start () {

        if (m_ShareUnitData == null) {
            GlobalSingleton.GetDataManager().GetUnitData(m_MeshName, ref m_ShareUnitData);
        }

        if (m_ShareAnimation == null) {
            m_ShareAnimation = GetComponent<TransformAnimation>();
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
