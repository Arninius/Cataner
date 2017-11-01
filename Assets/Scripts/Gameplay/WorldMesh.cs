using System.Collections;
using UnityEngine;

public class WorldMesh : MonoBehaviour {

    MeshCollider mesh_collider;
    MeshRenderer mesh_renderer;
    MeshFilter mesh_filter;
    Texture2D texture;

    bool renderer_changed;
    bool collider_changed;

    void Awake()
    {
        mesh_filter = GetComponent<MeshFilter>();
        mesh_collider = GetComponent<MeshCollider>();
        mesh_renderer = GetComponent<MeshRenderer>();
    }

    void LateUpdate()
    {
        if (renderer_changed) {
            texture.Apply();
            renderer_changed = false;
        }
    }

	public void construct(Mesh mesh, Texture2D texture)
	{
		this.texture = texture;
        mesh_filter.mesh = mesh;
        mesh_collider.sharedMesh = mesh;
        mesh_renderer.material.mainTexture = texture;
    }

    public void setTexture(int pos_x, int pos_z, Color col)
    {
		pos_z = texture.height - pos_z - 1;
        texture.SetPixel(pos_x, pos_z, col);
        renderer_changed = true;
    }
}