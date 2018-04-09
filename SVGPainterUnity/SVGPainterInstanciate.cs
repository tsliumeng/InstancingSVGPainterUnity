using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace SVGPainterUnity{

    public class SVGInstanceData {
        public Matrix4x4[] matArry = new Matrix4x4[1]; // maximum:1023
        public Material material;
        public Mesh mesh;
    }

    public class SVGPainterInstanciate : MonoBehaviour
    {
        public List<Painter> painters = new List<Painter>();
        public List<SVGInstanceData> instanceDatas = new List<SVGInstanceData>();

        public TextAsset svgFile;
        public float lineWidth;
        public Color lineColor;

        private Matrix4x4 _mat = Matrix4x4.identity;

        void Start()
        {
            SVGDataParser parser = new SVGDataParser();

            List<string> paths = parser.Load(svgFile);

            ToPoints toPoints = new ToPoints();
            LineMesh lm = new LineMesh();

            Camera c = Camera.main;

            for (int i = 0; i < paths.Count; i++)
            {
                PathData data = toPoints.GetPointsFromPath(paths[i]);

                Vector3 eulerAngles = new Vector3(0f, 180f, 180f);
                Quaternion rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
                Matrix4x4 m = Matrix4x4.Rotate(rotation);

                for (int j = 0; j < data.points.Count; j++)
                {
                    Vector3 pos = new Vector3(data.points[j].x + ((Screen.width - parser.GetSize().x) * 0.5f), data.points[j].y + ((Screen.height - parser.GetSize().y) * 0.5f), c.nearClipPlane);
                    Vector3 p = c.ScreenToWorldPoint(pos);
                    p.y -= c.transform.localPosition.y * 2f;
                    p = m.MultiplyPoint3x4(p);
                    p.z = 0f;
                    data.points[j] = p;
                }

                var lineMesh = lm.CreateLine(data.points, lineWidth, lineColor);

                Material mat = new Material(Shader.Find("Custom/SVGLine"));
                mat.enableInstancing = true;

                var instanceData = new SVGInstanceData();
                instanceData.mesh = lineMesh;
                instanceData.material = mat;
                instanceDatas.Add(instanceData);

                var pObj = new Painter();
                pObj.lineMat = mat;
                painters.Add(pObj);
            }

            int pID = Shader.PropertyToID("_SVGLineMaskValue");
            for (int i = 0; i < painters.Count; i++)
            {
                painters[i].sMaskValueID = pID;
            }

            Vector3 _scale = transform.localScale;
            Vector3 _pos = transform.position;
            Quaternion q = transform.rotation;
            _mat.SetTRS(_pos, q, _scale);

            for (int i = 0; i < instanceDatas.Count; i++)
            {
                instanceDatas[i].matArry[0] = _mat;
            }
        }

		void Update()
		{
            for (int i = 0; i < instanceDatas.Count; i++){
                SVGInstanceData data = instanceDatas[i];
                Vector3 _scale = transform.localScale;
                Vector3 _pos = transform.position;
                Quaternion q = transform.rotation;
                _mat.SetTRS(_pos, q, _scale);
                instanceDatas[i].matArry[0] = _mat;
                Graphics.DrawMeshInstanced(data.mesh, 0, data.material, data.matArry, 1, null, UnityEngine.Rendering.ShadowCastingMode.Off, false);
            }
		}

	}
}