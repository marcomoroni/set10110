using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SmoothAnimations : MonoBehaviour
{
	// SMOOTH TRANSLATION //////////////////////////////////////////////////////

	[SerializeField]
	private float _smoothTranslationTime = 0.2f;

	public float smoothTranslationTime
	{
		get { return _smoothTranslationTime; }
		set { _smoothTranslationTime = Mathf.Max(0, value); }
	}

	public AnimationCurve smoothTranslationAnimationCurve = new AnimationCurve();

	private Vector3 _smoothMoveTarget;

	/// <summary>
	/// Smoothly translate to a target position.
	/// </summary>
	/// <param name="target">Target position.</param>
	public void SmoothTranslate(Vector3 target)
	{
		_smoothMoveTarget = target;

		StopCoroutine("SmoothTranslationCorutine");
		StartCoroutine("SmoothTranslationCorutine");
	}

	private IEnumerator SmoothTranslationCorutine()
	{
		float currentLerpTime = 0;
		Vector3 startPos = transform.position;

		//while (currentLerpTime < _smoothTranslationTime)
		while (transform.position != _smoothMoveTarget)
		{
			Debug.DrawLine(startPos, _smoothMoveTarget, Color.red);

			currentLerpTime += Time.deltaTime;

			if (currentLerpTime > _smoothTranslationTime)
			{
				// If animation is over, hard code position
				currentLerpTime = _smoothTranslationTime;
				transform.position = _smoothMoveTarget;
			}
			else
			{
				float t = currentLerpTime / _smoothTranslationTime;
				float perc = smoothTranslationAnimationCurve.Evaluate(t);
				transform.position = Vector3.Lerp(startPos, _smoothMoveTarget, perc);
			}

			yield return null;
		}
	}

	// SMOOTH ROTATION /////////////////////////////////////////////////////////

	[SerializeField]
	float _smoothRotationTime = 0.2f;

	public float smoothRotationTime
	{
		get { return _smoothRotationTime; }
		set { _smoothRotationTime = Mathf.Max(0, value); }
	}

	public AnimationCurve smoothRotationAnimationCurve = new AnimationCurve();

	private Quaternion _smoothRotationTarget;

	public void SmoothRotate(Quaternion target)
	{
		_smoothRotationTarget = target;

		StopCoroutine("SmoothRotationCorutine");
		StartCoroutine("SmoothRotationCorutine");
	}

	private IEnumerator SmoothRotationCorutine()
	{
		float currentLerpTime = 0;
		Quaternion startRot = transform.rotation;

		//while (currentLerpTime <= _smoothTranslationTime)
		while (transform.rotation != _smoothRotationTarget)
		{
			Debug.DrawLine(transform.position + Vector3.down, transform.position + Vector3.up, Color.green);
			Debug.DrawLine(transform.position + Vector3.left, transform.position + Vector3.right, Color.green);

			currentLerpTime += Time.deltaTime;

			if (currentLerpTime > _smoothTranslationTime)
			{
				// If animation is over, hard code rotation
				currentLerpTime = _smoothTranslationTime;
				transform.rotation = _smoothRotationTarget;
			}
			else
			{
				float t = currentLerpTime / _smoothTranslationTime;
				float perc = smoothRotationAnimationCurve.Evaluate(t);
				transform.rotation = Quaternion.Slerp(startRot, _smoothRotationTarget, perc);
			}

			yield return null;
		}
	}

	// SMOOTH SCALE ////////////////////////////////////////////////////////////

	[SerializeField]
	float _smoothScaleTime = 0.2f;

	public float smoothScaleTime
	{
		get { return _smoothScaleTime; }
		set { _smoothScaleTime = Mathf.Max(0, value); }
	}

	public AnimationCurve smoothScaleAnimationCurve = new AnimationCurve();

	private Vector3 _smoothScaleTarget;

	public void SmoothScale(Vector3 target)
	{
		_smoothScaleTarget = target;

		StopCoroutine("SmoothScaleCorutine");
		StartCoroutine("SmoothScaleCorutine");
	}

	private IEnumerator SmoothScaleCorutine()
	{
		float currentLerpTime = 0;
		Vector3 startScale = transform.localScale;

		//while (currentLerpTime <= _smoothTranslationTime)
		while (transform.localScale != _smoothScaleTarget)
		{
			Debug.DrawLine(transform.position + Vector3.down, transform.position + Vector3.up, Color.yellow);
			Debug.DrawLine(transform.position + Vector3.left, transform.position + Vector3.right, Color.yellow);

			currentLerpTime += Time.deltaTime;

			if (currentLerpTime > _smoothTranslationTime)
			{
				// If animation is over, hard code scale
				currentLerpTime = _smoothTranslationTime;
				transform.localScale = _smoothScaleTarget;
			}
			else
			{
				float t = currentLerpTime / _smoothTranslationTime;
				float perc = smoothScaleAnimationCurve.Evaluate(t);
				transform.localScale = Vector3.Lerp(startScale, _smoothScaleTarget, perc);
			}

			yield return null;
		}
	}
}