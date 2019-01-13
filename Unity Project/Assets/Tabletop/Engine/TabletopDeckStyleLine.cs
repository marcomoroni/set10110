using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Line deck style.
/// </summary>
[CreateAssetMenu(fileName = "NewLineDeckStyle", menuName = "Tabletop/Deck style/Line")]
public class TabletopDeckStyleLine : TabletopDeckStyle
{
	[SerializeField]
	private float _gap = 0.6f;

	/// <summary>
	/// The max gap between cards. It must be a positive value.
	/// </summary>
	public float gap
	{
		get { return _gap; }
		set { _gap = Mathf.Max(0f, value); }
	}

	[SerializeField]
	private float _angle = 0f;

	/// <summary>
	/// The angle of the line in degrees.
	/// </summary>
	public float angle
	{
		get { return _angle; }
		set { _angle = Mathf.Clamp(value, 0f, 360f); }
	}

	public enum AlignmentType
	{
		CenterToOut,
		OutToCenter,
		Center
	}

	[SerializeField]
	public AlignmentType alignment = AlignmentType.CenterToOut;

	public enum CardsOrientationType
	{
		Normal,
		/// <summary>
		/// The bottom side of the card will face the center of the deck.
		/// </summary>
		In,
		/// <summary>
		/// The top side of the card will face the center of the deck.
		/// </summary>
		Out
	}

	[SerializeField]
	public CardsOrientationType cardsOrientation;

	public bool hasMaxNumberOfCards = false;

	[SerializeField]
	private int _maxNumberOfCards = 4;

	public int maxNumberOfCards
	{
		get { return _maxNumberOfCards; }
		set { _maxNumberOfCards = Mathf.Max(1, value); }
	}

	public override TransformData GetCardTransformData(int numberOfCards, int index)
	{
		TransformData transformData = new TransformData();

		// Calculate the lenght of the line
		float lineLenght;
		if(!hasMaxNumberOfCards || numberOfCards <= maxNumberOfCards)
			lineLenght = gap * (numberOfCards - 1);
		else
			lineLenght = gap * (maxNumberOfCards - 1);
		//float lineLenght = gap * (numberOfCards - 1);

		// Calculate the start and end position of the line
		if (lineLenght > 0)
		{
			Vector2 lineStart = new Vector2(0, 0);
			Vector2 lineEnd = new Vector2(0, 0);

			switch(alignment)
			{
				case AlignmentType.CenterToOut:
					lineEnd = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * lineLenght, Mathf.Sin(angle * Mathf.Deg2Rad) * lineLenght);
					break;

				case AlignmentType.OutToCenter:
					lineStart = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * lineLenght, Mathf.Sin(angle * Mathf.Deg2Rad) * lineLenght);
					break;

				case AlignmentType.Center:
					lineStart = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * -lineLenght / 2f, Mathf.Sin(angle * Mathf.Deg2Rad) * -lineLenght / 2f);
					lineEnd = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * lineLenght / 2, Mathf.Sin(angle * Mathf.Deg2Rad) * lineLenght / 2);
					break;
			}

			// Get the interpolated position
			if (!hasMaxNumberOfCards || numberOfCards <= maxNumberOfCards)
			{
				transformData.position = new Vector2(Mathf.Lerp(lineStart.x, lineEnd.x, (float)index / (float)(numberOfCards - 1)), Mathf.Lerp(lineStart.y, lineEnd.y, (float)index / (float)(numberOfCards - 1)));
			}
			else
			{
				// If there are hidden cards, keep the top cards visible and hide the others
				transformData.position = new Vector2(Mathf.Lerp(lineStart.x, lineEnd.x, (float)(index + numberOfCards - maxNumberOfCards - 1) / (float)maxNumberOfCards), Mathf.Lerp(lineStart.y, lineEnd.y, (float)(index + numberOfCards - maxNumberOfCards - 1) / (float)maxNumberOfCards));
				//Debug.Log("Index: " + index + "; Pos: " + transformData.position + "Lerp: " + ((float)(index + numberOfCards - maxNumberOfCards - 1) / (float)maxNumberOfCards));
			}

			//transformData.position = new Vector2(Mathf.Lerp(lineStart.x, lineEnd.x, (float)index / (float)(numberOfCards - 1)), Mathf.Lerp(lineStart.y, lineEnd.y, (float)index / (float)(numberOfCards - 1)));
		}

		// Card orientation
		switch (cardsOrientation)
		{
			case CardsOrientationType.In:
				transformData.rotation = Quaternion.Euler(new Vector3(0, 0, -90f + angle));
				break;
			case CardsOrientationType.Out:
				transformData.rotation = Quaternion.Euler(new Vector3(0, 0, 90f + angle));
				break;
		}

		return transformData;
	}

	public override List<TransformData> GetCardsTransformData(int numberOfCards)
	{
		List<TransformData> transformDatas = new List<TransformData>();

		for (int i = 0; i < numberOfCards; i++)
		{
			transformDatas.Add(GetCardTransformData(numberOfCards, i));
		}

		return transformDatas;
	}

	public override Vector2 GetMiddleDropAreaSize()
	{
		return new Vector2(gap, 1.0f);
	}

	public override Vector2 GetEdgeDropAreaSize()
	{
		return new Vector2(1.0f, 1.0f);
	}

	public override List<TransformData> GetDropAreasTransformData(int numberOfCards)
	{
		int numberOfDropAreas = numberOfCards + 1;

		List<TransformData> output = new List<TransformData>();
		output = GetCardsTransformData(numberOfDropAreas);

		if (numberOfDropAreas > 1)
		{
			switch (alignment)
			{
				case AlignmentType.CenterToOut:
					{
						Vector2 displacement = (output[0].position - output[1].position) / 2.0f;
						for (int i = 0; i < output.Count; i++)
						{
							output[i].position += (Vector3)displacement;
						}
					}
					break;

				case AlignmentType.OutToCenter:
					{
						Vector2 displacement = (output[0].position - output[1].position) / 2.0f;
						for (int i = 0; i < output.Count; i++)
						{
							output[i].position -= (Vector3)displacement;
						}
					}
					break;

				default:
					break;
			}
		}

		// Override rotation
		for (int i = 0; i < output.Count; i++)
		{
			output[i].rotation = Quaternion.Euler(new Vector3(0, 0, angle));
		}

		return output;
	}
}
