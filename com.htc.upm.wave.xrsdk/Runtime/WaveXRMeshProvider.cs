using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Wave.XR.Loader
{
    public class MeshProviderManager
    {
        private struct MeshComponent
        {
            public MeshInfo info;
            public GameObject obj;
            public MeshVertexAttributes attributes;
            public Mesh mesh;
            public MeshFilter filter;
            public MeshCollider collider;
            public MeshRenderer renderer;
        }

        private static List<MeshInfo> meshInfos = new List<MeshInfo>();
        private static GameObject controllerTemplate;
        private static List<MeshComponent> controllerComponents = new List<MeshComponent>();

        public static void Process(WaveXRLoader loader)
        {
            Debug.Log("XRMeshProvider Process");
            if (loader.meshSubsystem.TryGetMeshInfos(meshInfos))
            {
                MakeController();

                foreach (var comp in controllerComponents)
                {
                    loader.meshSubsystem.GenerateMeshAsync(comp.info.MeshId, comp.mesh, comp.collider, comp.attributes, OnMeshGenerationComplete);
                }
            }
        }

        public static void MakeController()
        {
            if (controllerTemplate == null)
                controllerTemplate = new GameObject("Controller");
            controllerTemplate.transform.localPosition = Vector3.zero;
            controllerTemplate.transform.localRotation = Quaternion.identity;

            int i = -1;
            foreach (var info in meshInfos)
            {
                i++;
                Debug.Log("TryGetInfo: Info[" + i + "]: MeshId =" + info.MeshId);

                var comp = new MeshComponent();
                comp.obj = new GameObject("Component");
                comp.obj.transform.SetParent(controllerTemplate.transform, false);
                comp.obj.transform.localPosition = Vector3.zero;
                comp.obj.transform.localRotation = Quaternion.identity;
                comp.mesh = new Mesh();
                comp.mesh.name = info.MeshId.ToString();
                comp.filter = comp.obj.AddComponent<MeshFilter>();
                comp.filter.mesh = comp.mesh;
                comp.collider = comp.obj.AddComponent<MeshCollider>();
                comp.renderer = comp.obj.AddComponent<MeshRenderer>();
                comp.renderer.material = new Material(Shader.Find("Standard"));
                comp.attributes = MeshVertexAttributes.UVs | MeshVertexAttributes.Normals;

                controllerComponents.Add(comp);
            }
        }

        public static void OnMeshGenerationComplete(MeshGenerationResult result)
        {
            Debug.Log("OnMeshGenerationComplete: result.Status=" + result.Status + ", MeshId=" + result.MeshId + ", Mesh.name=" + result.Mesh.name);
        }
    }
}
