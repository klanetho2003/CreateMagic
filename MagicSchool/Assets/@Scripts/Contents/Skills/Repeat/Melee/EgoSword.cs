using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EgoSword : RepeatSkill
{
	[SerializeField]
	ParticleSystem[] _swingParticles;

	protected enum SwingType
	{
		First,
		Second,
		Third,
		Fourth
	}

	public EgoSword()
	{

	}

	protected override IEnumerator CoStartSkill() // "DoSkillJob"을 override하면 되지만,
												  // 이 스킬은 반복 주기가 SwingType에 따라 다르기 떄문에 (Wait해야하는 시간이 달라) 반복 주기까지 수정할 수 있는 코루틴을 override했다
	{
		WaitForSeconds wait = new WaitForSeconds(CoolTime);

		while (true)
		{
			SetParticles(SwingType.First);
			_swingParticles[(int)SwingType.First].gameObject.SetActive(true);
			yield return new WaitForSeconds(_swingParticles[(int)SwingType.First].main.duration);

			SetParticles(SwingType.Second);
			_swingParticles[(int)SwingType.Second].gameObject.SetActive(true);
			yield return new WaitForSeconds(_swingParticles[(int)SwingType.Second].main.duration);

			SetParticles(SwingType.Third);
			_swingParticles[(int)SwingType.Third].gameObject.SetActive(true);
			yield return new WaitForSeconds(_swingParticles[(int)SwingType.Third].main.duration);

			SetParticles(SwingType.Fourth);
			_swingParticles[(int)SwingType.Fourth].gameObject.SetActive(true);
			yield return new WaitForSeconds(_swingParticles[(int)SwingType.Fourth].main.duration);

			yield return wait;
		}
	}

	public override bool Init()
	{
		base.Init();

		return true;
	}

	void SetParticles(SwingType swingType)
	{
		if (Managers.Game.Player == null)
			return;

		Vector3 tempAngle = Managers.Game.Player.Indicator.transform.eulerAngles;
		transform.localEulerAngles = tempAngle;
		transform.position = Managers.Game.Player.transform.position;

		float radian = Mathf.Deg2Rad * tempAngle.z * -1;

		var main = _swingParticles[(int)swingType].main;
		main.startRotation = radian;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		MonsterController mc = collision.transform.GetComponent<MonsterController>();
		if (mc.IsValid() == false)
			return;

		//mc.OnDamaged(Owner, Damage);
	}

	protected override void DoSkill()
	{

	}



    ///temp
    protected override void OnAttackTargetHandler()
    {
        throw new System.NotImplementedException();
    }

    /*protected override void OnAnimComplateHandler()
    {
        throw new System.NotImplementedException();
    }*/
}
