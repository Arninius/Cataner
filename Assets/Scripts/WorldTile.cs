using UnityEngine;

[System.Serializable]
public struct WorldTile {
	float r, g, b;

	public void setColor(Color col) {
		r = col.r; g = col.g; b = col.b;
	}
	public void setColor(float r, float g, float b) {
		this.r = r; this.g = g; this.b = b;
	}
	public Color getColor() {
		return new Color (r, g, b);
	}
}