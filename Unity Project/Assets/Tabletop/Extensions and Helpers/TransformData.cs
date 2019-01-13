using UnityEngine;

/// <summary>
/// Helper class to store transform data.
/// </summary>
public class TransformData
{
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 scale;

	public TransformData()
	{
		this.position = Vector3.zero;
		this.rotation = Quaternion.identity;
		this.scale = Vector3.one;
	}

	public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
	{
		this.position = position;
		this.rotation = rotation;
		this.scale = scale;
	}

	public TransformData(Vector3 position, Vector3 rotation, Vector3 scale)
	{
		this.position = position;
		this.rotation = Quaternion.Euler(rotation);
		this.scale = scale;
	}
}