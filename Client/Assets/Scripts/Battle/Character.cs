/**
@date 20161029 by NDark 
. add class member m_MorphingData at Character.
. add class method ForceEndMorphing()
. add class method StartMorphingModel()
. add class method UpdateMorphing()
. add class method isMorphingEnded()

*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic ;

public enum MODELTYPE
{
    E_DEFENSE,
    E_ATTACK,
    E_CONCENTRATE,

    E_MODELTYPE_NUM
}

public class Character : MonoBehaviour {
    // private
    IMesh m_ShareMeshData = null;
    UnitData m_ShareUnitData = null;
    ChunkAnimation m_ShareAnimation = null;

    // manager reference 
    static MeshCreator s_ShareMeshCreator = GlobalSingleton.GetMeshCreator();
    // static DataManager s_DataManager = GlobalSingleton.GetDataManager();

    // public
    public string m_MeshName = "Unit_1";
    public GameObject m_UIHPBar = null;

    // attribute
    public Attribute hp { get { return m_ShareUnitData.m_HP; } }
    public Attribute atk { get { return m_ShareUnitData.m_ATK; } }
    public Attribute def { get { return m_ShareUnitData.m_DEF; } }
    public IMesh mesh { get { return m_ShareMeshData; } }

	Dictionary<int,MorphingStruct> m_MorphingData =
		 new Dictionary<int, MorphingStruct>() ;
	
    public void ReCreate() {
        if (m_ShareMeshData != null) {
            s_ShareMeshCreator.DestroyMesh(ref m_ShareMeshData);
        }

        //if (m_ShareUnitData != null){
        //   s_DataManager.GetUnitData(m_MeshName, ref m_ShareUnitData);
        //}

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

	public void ForceEndMorphing()
	{
		foreach( var data in m_MorphingData.Values )
		{
			data.isInMorphing = false ;
		}
	}
	
	public void StartMorphingModel(int _ChunkIndex, MODELTYPE _ModelType)
	{
		
		if( false == m_MorphingData.ContainsKey( _ChunkIndex ) )
		{
			m_MorphingData.Add( _ChunkIndex , new MorphingStruct() ) ;
		}
		
		MorphingStruct morphData = null ;
		morphData = m_MorphingData[ _ChunkIndex ] ;
		morphData.isInMorphing = true ;
		
		
		string modelstr = "body_" + _ChunkIndex;
		string[] def_models = { "shield" };
		string[] atk_models = { "axe", "sword" };
		
		switch (_ModelType)
		{
		case MODELTYPE.E_DEFENSE:
			modelstr = def_models[Random.Range(0, 100) % def_models.Length];
			break;
			
		case MODELTYPE.E_ATTACK:
			modelstr = atk_models[Random.Range(0, 100) % atk_models.Length];
			break;
			
		case MODELTYPE.E_CONCENTRATE:
			modelstr = "body_" + _ChunkIndex;
			break;
		}
		
		
		Mesh_VoxelChunk Mesh = m_ShareMeshData as Mesh_VoxelChunk;
		if (null != Mesh)
		{
			// GlobalSingleton.DEBUG("ChangeModel : " + _ChunkIndex + "," + model);
			Mesh.StartMorphingModel("bone" + _ChunkIndex, modelstr , ref morphData );
		}
	}
	
	public void UpdateMorphing()
	{
		foreach( var morphChunk in this.m_MorphingData.Values )
		{
			morphChunk.UpdateMorphData() ;
		}
	}
	
	public bool isMorphingEnded()
	{
		bool isMorphing = false ;
		foreach( MorphingStruct data in this.m_MorphingData.Values )
		{
			if( true == data.isInMorphing )
			{
				isMorphing = true ;
				break ;
			}
		}
		return false == isMorphing  ;
	}
	
    public void DoChangeModel(int _ChunkIndex, MODELTYPE _ModelType)
    {
        string model = "body_" + _ChunkIndex;
        string[] def_models = { "shield" };
        string[] atk_models = { "axe", "sword" };

        switch (_ModelType)
        {
            case MODELTYPE.E_DEFENSE:
                model = def_models[Random.Range(0, 100) % def_models.Length];
                break;

            case MODELTYPE.E_ATTACK:
                model = atk_models[Random.Range(0, 100) % atk_models.Length];
                break;

            case MODELTYPE.E_CONCENTRATE:
                model = "body_" + _ChunkIndex;
                break;
        }

        Mesh_VoxelChunk Mesh = m_ShareMeshData as Mesh_VoxelChunk;
        if (null != Mesh)
        {
            // GlobalSingleton.DEBUG("ChangeModel : " + _ChunkIndex + "," + model);
            Mesh.ChangeModel("bone" + _ChunkIndex, model);
        }
    }

    public void DoAction(int _ChunkIndex, AnimationState _State, float _Delay = 0.0f) {
        float dx = gameObject.transform.localScale.x;
        string sx = (dx > 0.0f) ? "R" : "L";
        m_ShareAnimation.DoAnimation(this,
            string.Format("Chunk-{0}-{1}", sx, _ChunkIndex),
            _ChunkIndex,
            _State,
            _Delay);
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
        if (m_ShareAnimation == null) {
            // m_ShareAnimation = GetComponent<TransformAnimation>();
            m_ShareAnimation = GetComponent<ChunkAnimation>();
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
