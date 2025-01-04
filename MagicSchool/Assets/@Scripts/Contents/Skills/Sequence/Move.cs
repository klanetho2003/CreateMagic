using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : SequenceSkill
{
	Rigidbody2D _rb;
	Coroutine _coroutine;

	private void Awake()
	{

	}

	public override void DoSkill(Action callback = null)
	{
		if (_coroutine != null)
			StopCoroutine(_coroutine);

		_coroutine = StartCoroutine(CoMove(callback));
	}

	float Speed { get; } = 2.0f;
	string AnimationName { get; } = "Moving";

	IEnumerator CoMove(Action callback = null)
	{
		_rb = GetComponent<Rigidbody2D>();
		GetComponent<Animator>().Play(AnimationName);
		float elapsed = 0;

		while (true)
		{
			elapsed += Time.deltaTime;
			if (elapsed > 5.0f)
				break;

			Vector3 dir = ((Vector2)Managers.Game.Player.transform.position - _rb.position).normalized;
			Vector2 targetPosition = Managers.Game.Player.transform.position + dir * UnityEngine.Random.Range(1, 4);

			if (Vector3.Distance(_rb.position, targetPosition) <= 0.2f)
				continue;

			Vector2 dirVec = targetPosition - _rb.position;
			Vector2 nextVec = dirVec.normalized * Speed * Time.fixedDeltaTime;
			_rb.MovePosition(_rb.position + nextVec);

			yield return null;
		}

		callback?.Invoke();
	}
}
