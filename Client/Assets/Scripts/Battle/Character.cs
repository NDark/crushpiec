using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
    // private
    IMesh m_ShareMeshData = null;
    UnitData m_ShareUnitData = null;
    TransformAnimation m_ShareAnimation = null;

    // manager reference 
    static MeshCreator s_ShareMeshCreator = GlobalSingleton.GetMeshCreator();
    static DataManager s_DataManager = GlobalSingleton.GetDataManager();

    // public
    public string m_MeshName = "Unit_1";
    public GameObject m_UIHPBar = null;

    // attribute
    public Attribute hp { get { return m_ShareUnitData.m_HP; } }
    public Attribute atk { get { return m_ShareUnitData.m_ATK; } }
    public Attribute def { get { return m_ShareUnitData.m_DEF; } }
    public IMesh mesh { get { return m_ShareMeshData; } }

    public void ReCreate() {
        if (m_ShareMeshData != null) {
            s_ShareMeshCreator.DestroyMesh(ref m_ShareMeshData);
        }

        if (m_ShareUnitData != null){
            s_DataManager.GetUnitData(m_MeshName, ref m_ShareUnitData);
        }

        DoRender();
    }

    public void DoRender() {
        if (m_ShareMeshData == null) {
            m_ShareMeshData = s_ShareMeshCreator.CreateMesh(m_MeshName);
        }

        if (m_ShareMeshData != null) {
            m_ShareMeshData.Draw(this.gameObject);
        }
    }

    public void DoAction(AnimationState _State, float _Delay = 0.0f) {
        m_ShareAnimation.ChangeAnimationState(_State, _Delay);
    }
    
    public void DoUpdateHP() {
        if (m_UIHPBar != null ) {
            float hpRatio = Mathf.Max(0.0f, (float)m_ShareUnitData.m_HP.value / m_ShareUnitData.m_HP.max);
            Vector3 localScale = m_UIHPBar.transform.localScale;
            localScale.x = hpRatio;
            m_UIHPBar.transform.localScale = localScale;
        }
    }

    // Use this for initialization
    void Start () {
        if (m_ShareUnitData == null) {
            s_DataManager.GetUnitData(m_MeshName, ref m_ShareUnitData);
        }

        if (m_ShareAnimation == null) {
            m_ShareAnimation = GetComponent<TransformAnimation>();
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
