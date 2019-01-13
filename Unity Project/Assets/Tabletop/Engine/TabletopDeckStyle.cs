using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TabletopDeckStyle : ScriptableObject
{
	[SerializeField]
	private float _positionScatter;

	/// <summary>
	/// The max card's position scatter. It must be a positive value.
	/// </summary>
	public float positionScatter
	{
		get { return _positionScatter; }
		set { _positionScatter = Mathf.Max(0f, value); }
	}

	[SerializeField]
	private float _angleScatter;

	/// <summary>
	/// The max card's angle scatter in degrees (between 0 and 180).
	/// </summary>
	public float angleScatter
	{
		get { return _angleScatter; }
		set { _angleScatter = Mathf.Clamp(value, 0f, 180f); }
	}

	/// <summary>
	/// Get random scatter position and rotation.
	/// </summary>
	public TransformData GenerateScatter()
	{
		TransformData scatterData = new TransformData();

		// Position scatter
		if (_positionScatter > 0f + Mathf.Epsilon)
		{
			scatterData.position = new Vector2(Random.Range(-_positionScatter, _positionScatter), Random.Range(-_positionScatter, _positionScatter));
		}

		// Rotation scatter
		if (_angleScatter > 0f + Mathf.Epsilon)
		{
			scatterData.rotation *= Quaternion.Euler(new Vector3(0, 0, Random.Range(-_angleScatter, _angleScatter)));
		}

		return scatterData;
	}

	/// <summary>
	/// Gets the transform data relative to the deck position of one deck's card.
	/// </summary>
	/// <param name="numberOfCards">Nuomber of cards.</param>
	/// <param name="index">Index of the card.</param>
	abstract public TransformData GetCardTransformData(int numberOfCards, int index);

	/// <summary>
	/// Gets the transform data relative to the deck position of the deck's cards.
	/// </summary>
	/// <param name="numberOfCards">Number of cards.</param>
	/// <returns>A list of <c>TransformData</c> for the deck's cards.</returns>
	// This will just call GetCardTransformData for each card.
	abstract public List<TransformData> GetCardsTransformData(int numberOfCards);

	/// <summary>
	/// Gets the suggested size of the box collider 2D of the drop areas between cards.
	/// </summary>
	/// <returns></returns>
	abstract public Vector2 GetMiddleDropAreaSize();

	/// <summary>
	/// Gets the suggested size of the box collider 2D of the drop areas at the edges.
	/// </summary>
	/// <returns></returns>
	abstract public Vector2 GetEdgeDropAreaSize();

	/// <summary>
	/// Gets the transform data relative to the deck position of the deck's drop areas.
	/// </summary>
	/// <param name="numberOfDropAreas"></param>
	/// <returns></returns>
	abstract public List<TransformData> GetDropAreasTransformData(int numberOfCards);
}

/// <summary>
/// Circle deck style.
/// </summary>
/*[CreateAssetMenu(fileName = "NewCircleDeckStyle", menuName = "Tabletop/Deck style/Circle")]
public class TabletopCircleDeckStyle : TabletopDeckStyle
{
	
}*/