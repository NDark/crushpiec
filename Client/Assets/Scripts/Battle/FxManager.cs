using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FxManager : MonoBehaviour {
    List<GameObject> m_DummyPoints = new List<GameObject>();
    static int s_DummyCacheSize = 5;
    static int s_DummyIndex = 0;

    public void DoBreakBoneAnimation(
        ref Character _CharacterRef, 
        int _PartIndex = 0 /* <= 0, for random */) {
        if (_PartIndex <= 0) {
            _PartIndex = Random.Range(1, _CharacterRef.mesh.m_nPartCount);
        }

        GlobalSingleton.DEBUG("DoBreakBoneAnimation Obj = "
            + _CharacterRef.ToString()
            + ", nPartIndex = "
            + _PartIndex);

        s_DummyIndex = (s_DummyIndex + 1) % m_DummyPoints.Count;
        GameObject obj = m_DummyPoints[s_DummyIndex];
        obj.transform.position = _CharacterRef.transform.position;
        _CharacterRef.mesh.BreakPart(_PartIndex, obj);

        BoneAnimation boneAnimation = obj.AddComponent<BoneAnimation>();
        boneAnimation.m_StartPoint = _CharacterRef.transform.position;
        boneAnimation.m_EndPoint = new Vector3(
            Random.Range(-32.0f, 32.0f),
            Random.Range(-1.0f, 1.0f),
            Random.Range(-10.0f, 5.0f));
        
        Invoke("OnBoneAnimationFinish", 5.0f);
    }

    void OnBoneAnimationFinish() {

        foreach (GameObject obj in m_DummyPoints) {
            BoneAnimation boneAnimation = obj.GetComponent<BoneAnimation>();
            if (boneAnimation == null)
                continue;
            if (boneAnimation.m_ElapsedTime < 3.0f)
                continue;

            foreach (Transform child in obj.transform) {
                GameObject.Destroy(child.gameObject);
            }

            Destroy(obj.GetComponent<BoneAnimation>());
        }    
    }

    // Use this for initialization
    void Start () {
	    for (int i = 0; i < s_DummyCacheSize; ++i) {
            GameObject obj = new GameObject("DummyPoint_" + i);
            obj.transform.parent = gameObject.transform;
            m_DummyPoints.Add(obj);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
