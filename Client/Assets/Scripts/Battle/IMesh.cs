#define VERTEX_COLOR_256
// #define VERTEX_COLOR_8
#define GRAY_SCALE_COLOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
  
public class IMesh  {
    private static string s_VoxelPrefab = "Battle/Prefabs/Voxel";
    private static int s_nSize = 8;

    public string m_SourceStr;
    public string m_Name;
    public int m_nSize = s_nSize;
    public int m_InstanceID;
    public int[] m_Vertices;
    public int[] m_PartMask;

    List<GameObject> m_VoxelCache = new List<GameObject>();
    
    // copy
    public IMesh(IMesh _CopyMesh) {
        m_SourceStr = _CopyMesh.m_SourceStr;
        m_Name = _CopyMesh.m_Name;
        m_nSize = _CopyMesh.m_nSize;
        m_InstanceID = _CopyMesh.m_InstanceID;
        m_Vertices = new int[_CopyMesh.m_Vertices.Length];

        System.Array.Copy(_CopyMesh.m_Vertices, m_Vertices, _CopyMesh.m_Vertices.Length);

        // optional field
        if (_CopyMesh.m_PartMask != null)
        {
            m_PartMask = new int[_CopyMesh.m_PartMask.Length];
            System.Array.Copy(_CopyMesh.m_PartMask, m_PartMask, _CopyMesh.m_PartMask.Length);
        }
    }

    public IMesh(string _SourceStr) {
        m_SourceStr = _SourceStr;
    }

    /// release resource
    public void Release()
    {
        foreach (GameObject voxel in m_VoxelCache) {
            GameObject.Destroy(voxel);
        }
    }

    public void Draw(GameObject _Root) {
        GlobalSingleton.DEBUG("Draw, size = " + m_nSize + ", sizeof vertices = " + m_Vertices.Length);

        for (int i = 0 ; i < m_Vertices.Length ; ++i)
        {
            int voxel = m_Vertices[i];
            if (voxel > 0)
            {
                GameObject obj = Object.Instantiate(Resources.Load(s_VoxelPrefab)) as GameObject;
                m_VoxelCache.Add(obj);

                Vector3 localScale = _Root.transform.localScale;
      
                obj.transform.parent = _Root.transform;
                obj.transform.position = _Root.transform.position
                    + new Vector3(
                        localScale.x * (i % m_nSize - m_nSize / 2),
                        localScale.y * (m_nSize / 2 - i / m_nSize), 
                        0);

                // use Sprites-Default material
#if VERTEX_COLOR_256 || VERTEX_COLOR_8
                MeshFilter cube = obj.GetComponent<MeshFilter>();
                Vector3[] vertices = cube.mesh.vertices;
#if VERTEX_COLOR_256
                int colorScale = 256;
#elif VERTEX_COLOR_8
                int colorScale = 8;

#endif
#if GRAY_SCALE_COLOR
                float grayScale = (voxel % colorScale) / (float)colorScale;
                float r = grayScale, g = grayScale, b = grayScale;
#else
                float r = (voxel % colorScale) / (float)colorScale;
                float g = (voxel % colorScale) / (float)colorScale;
                float b = (voxel % colorScale) / (float)colorScale;  
#endif   
                Color[] colors = Enumerable.Repeat(
                    new Color(r, g, b, 1.0f),
                    vertices.Length).ToArray();
                cube.mesh.colors = colors;
#endif
            }
        }
    }

    public void BreakPart(int _PartIndex, GameObject _Root, bool _OverrideVertices = true)
    {
        GlobalSingleton.DEBUG("BreakPart, part index = " + _PartIndex);

        // size of m_Vertices and m_PartMask should be the same.
        if ((m_PartMask == null) || (m_PartMask.Length != m_Vertices.Length))
            return;

        int voxelIndex = -1;
        for (int i = 0; i < m_PartMask.Length; ++i)
        {
            int voxel = m_Vertices[i];
            if (voxel > 0) { voxelIndex++; }
                      
            if (m_PartMask[i] != _PartIndex)
                continue; 

            GameObject obj = Object.Instantiate(Resources.Load(s_VoxelPrefab)) as GameObject;
            Vector3 localScale = _Root.transform.localScale;

            obj.transform.parent = _Root.transform;
            obj.transform.position = _Root.transform.position
                + new Vector3(
                    localScale.x * (i % m_nSize - m_nSize / 2),
                    localScale.y * (m_nSize / 2 - i / m_nSize),
                    0);

            // use Sprites-Default material
#if VERTEX_COLOR_256 || VERTEX_COLOR_8
            MeshFilter cube = obj.GetComponent<MeshFilter>();
            Vector3[] vertices = cube.mesh.vertices;
#if VERTEX_COLOR_256
            int colorScale = 256;
#elif VERTEX_COLOR_8
            int colorScale = 8;

#endif
#if GRAY_SCALE_COLOR
            float grayScale = (voxel % colorScale) / (float)colorScale;
            float r = grayScale, g = grayScale, b = grayScale;
#else
            float r = (voxel % colorScale) / (float)colorScale;
            float g = (voxel % colorScale) / (float)colorScale;
            float b = (voxel % colorScale) / (float)colorScale;  
#endif   
            Color[] colors = Enumerable.Repeat(
                new Color(r, g, b, 1.0f),
                vertices.Length).ToArray();
            cube.mesh.colors = colors;
#endif

            // hide the voxel from vertices
            if (_OverrideVertices)
            {
                m_VoxelCache[ voxelIndex ].SetActive(false);
            }
        }
    }

    public bool DoParse() {
        GlobalSingleton.DEBUG("DoParse, SourceStr = " + m_SourceStr);

        // remove unused char(space, new line, etc...)
        m_SourceStr = System.Text.RegularExpressions.Regex.Replace(m_SourceStr, @"\t|\n|\r| ", string.Empty);
        string[] attrSegment = m_SourceStr.Split(';');

        for (int i=0;i<attrSegment.Length;++i) {
            string[] element = attrSegment[i].Split(':');
            if (element[0] == "name")
            {
                m_Name = element[1];
            }
            else if (element[0] == "size")
            {
                int.TryParse(element[1], out m_nSize);
            }
            else if (element[0] == "vertices")
            {
                string valueStr = element[1];
                string[] verticesStr = valueStr.Split(',');
                m_Vertices = System.Array.ConvertAll(verticesStr, s => int.Parse(s));
            }
            else if (element[0] == "partmask")
            {
                string valueStr = element[1];
                string[] verticesStr = valueStr.Split(',');
                m_PartMask = System.Array.ConvertAll(verticesStr, s => int.Parse(s));
            }
        }

        return true;
    }
}
