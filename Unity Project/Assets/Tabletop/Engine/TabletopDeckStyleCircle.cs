using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCircleDeckStyle", menuName = "Tabletop/Deck style/Circle")]
public class TabletopDeckStyleCircle : TabletopDeckStyle
{
	[SerializeField]
	private float _radius = 4f;

	/// <summary>
	/// Radius.
	/// </summary>
	public float radius
	{
		get { return _radius; }
		set { _radius = Mathf.Max(0, value); }
	}

	[SerializeField]
	private float _startAngle = 0f;

	/// <summary>
	/// Starting angle in degrees.
	/// </summary>
	public float startAngle
	{
		get { return _startAngle; }
		set { _startAngle = Mathf.Clamp(value, 0f, 360f); }
	}

	public enum AlignmentType
	{
		Clockwise,
		Center,
		Anticlockwise
	}

	public AlignmentType alignment;

	public enum DirectionType
	{
		Anticlockwise,
		Clockwise
	}

	public DirectionType direction;

	[SerializeField]
	private float _gap = 4f;

	/// <summary>
	/// The angle gap between the cards in degrees.
	/// </summary>
	public float gap
	{
		get { return _gap; }
		set { _gap = Mathf.Clamp(value, 0f, 360f); }
	}

	public enum CardsOrientationType { In, Out, Normal }

	public CardsOrientationType cardsOrientation;

	private TransformData CalculateTreansformData(int numberOfCards, int index, float startAngle, CardsOrientationType cardsOrientation)
	{
		TransformData transformData = new TransformData();

		float mult = direction == DirectionType.Clockwise ? -1f : 1f;
		float angle = startAngle;

		// Complete circle or angle gap?
		if (gap * (numberOfCards - 1) <= 360f)
		{
			angle += mult * gap * index;
		}
		else
		{
			angle += (Mathf.PI * Mathf.Rad2Deg * 2) / numberOfCards * mult * index;
		}

		// Alignment
		if (alignment == AlignmentType.Clockwise && direction != DirectionType.Clockwise)
		{
			angle -= gap * (numberOfCards - 1);
		}
		if (alignment == AlignmentType.Anticlockwise && direction != DirectionType.Anticlockwise)
		{
			angle += gap * (numberOfCards - 1);
		}
		else if (alignment == AlignmentType.Center && direction == DirectionType.Clockwise)
		{
			angle += gap * (numberOfCards - 1) / 2;
		}
		else if (alignment == AlignmentType.Center && direction == DirectionType.Anticlockwise)
		{
			angle -= gap * (numberOfCards - 1) / 2;
		}

		transformData.position = new Vector2(radius * Mathf.Cos(angle * Mathf.Deg2Rad), radius * Mathf.Sin(angle * Mathf.Deg2Rad));

		// Cards orientation
		if (cardsOrientation == CardsOrientationType.In)
		{
			transformData.rotation = Quaternion.Euler(new Vector3(0f, 0f, -90f + angle));
		}
		else if (cardsOrientation == CardsOrientationType.Out)
		{
			transformData.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f + angle));
		}

		return transformData;
	}

	public override TransformData GetCardTransformData(int numberOfCards, int index)
	{
		TransformData transformData = CalculateTreansformData(numberOfCards, index, startAngle, cardsOrientation);

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
		// Corda
		return new Vector2(2 * radius * Mathf.Sin(gap * Mathf.Deg2Rad / 2), 2.0f);
	}

	public override Vector2 GetEdgeDropAreaSize()
	{
		return new Vector2(2.0f, 2.0f);
	}

	public override List<TransformData> GetDropAreasTransformData(int numberOfCards)
	{
		List<TransformData> transformDatas = new List<TransformData>();
		
		for (int i = 0; i < numberOfCards + 1; i++)
		{
			switch (direction)
			{
				case DirectionType.Anticlockwise:
					transformDatas.Add(CalculateTreansformData(numberOfCards, i, startAngle - (gap / 2.0f), CardsOrientationType.In));
					break;
				case DirectionType.Clockwise:
					transformDatas.Add(CalculateTreansformData(numberOfCards, i, startAngle + (gap / 2.0f), CardsOrientationType.In));
					break;
			}
		}

		return transformDatas;
	}
}
