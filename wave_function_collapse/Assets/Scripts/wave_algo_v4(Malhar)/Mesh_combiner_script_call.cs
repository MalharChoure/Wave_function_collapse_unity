using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mesh_combiner_script_call : MonoBehaviour
{
    public void call_mesh_combiner()
    {
        MeshCombiner meshCombiner = gameObject.AddComponent<MeshCombiner>();
        meshCombiner.CreateMultiMaterialMesh = true;
        meshCombiner.DestroyCombinedChildren = true;

        meshCombiner.CombineMeshes(false);
    }
}
