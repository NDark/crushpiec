// #define VERTEX_COLOR_256
// #define VERTEX_COLOR_8

using UnityEngine;
using System.Collections;
using System.Linq;
  
public class IMesh  {
    private static string s_VoxelPrefab = "Battle/Prefabs/Voxel";
    private static int s_nSize = 8;

    public string m_SourceStr;
    public string m_Name;
    public int m_InstanceId;
    public int[] m_Vertices;

    // copy
    public IMesh(IMesh _CopyMesh) {
        m_SourceStr = _CopyMesh.m_SourceStr;
        m_Name = _CopyMesh.m_Name;
        m_InstanceId = _CopyMesh.m_InstanceId;
        m_Vertices = new int[_CopyMesh.m_Vertices.Length];

        System.Array.Copy(_CopyMesh.m_Vertices, m_Vertices, _CopyMesh.m_Vertices.Length);
    }

    public IMesh(string _SourceStr) {
        m_SourceStr = _SourceStr;
    }

    public void Draw(GameObject _Root) {
        for (int i = 0 ; i < m_Vertices.Length ; ++i)
        {
            int voxel = m_Vertices[i];
            if (voxel > 0)
            {
                GameObject obj = Object.Instantiate(Resources.Load(s_VoxelPrefab)) as GameObject;
                Vector3 localScale = _Root.transform.localScale;

                obj.transform.parent = _Root.transform;
                obj.transform.position = new Vector3(
                    localScale.x * (i % s_nSize - s_nSize / 2),
                    localScale.y * (s_nSize / 2 - i / s_nSize), 
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
                float vertexColor = (voxel % colorScale) / (float)colorScale;
                Color[] colors = Enumerable.Repeat(
                    new Color(vertexColor, vertexColor, vertexColor, 1.0f),
                    vertices.Length).ToArray();
                cube.mesh.colors = colors;
#endif
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
            else if (element[0] == "vertices")
            {
                string valueStr = element[1];
                string[] verticesStr = valueStr.Split(',');
                m_Vertices = System.Array.ConvertAll(verticesStr, s => int.Parse(s));
            }
        }

        return true;
    }
}
