using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtFlashEffect : MonoBehaviour // spin 용
{
	private int _flashCount = 1;
	private Color _flashColor = new Color(0.5f, 0, 0);
	private float _interval = 1.0f / 15;
	private string _fillPhaseProperty = "_FillPhase";
	private string _fillColorProperty = "_FillColor";

	MaterialPropertyBlock _mpb;
	MeshRenderer _meshRenderer;

    #region Init

    void Awake()
    {
        Init();
    }

    bool _init = false;

    public virtual bool Init() // 최초 실행일 떄는 true를 반환, 한 번이라도 실행한 내역이 있을 경우 false를 반환
    {
        if (_init)
            return false;

        _mpb = new MaterialPropertyBlock();
        _meshRenderer = GetComponent<MeshRenderer>();

        _init = true;
        return true;
    }

    #endregion

    public void Flash()
	{
		_meshRenderer.GetPropertyBlock(_mpb);
		StartCoroutine(FlashRoutine());
	}

	IEnumerator FlashRoutine()
	{
		int fillPhase = Shader.PropertyToID(_fillPhaseProperty);
		int fillColor = Shader.PropertyToID(_fillColorProperty);

		WaitForSeconds wait = new WaitForSeconds(_interval);

		for (int i = 0; i < _flashCount; i++)
		{
			_mpb.SetColor(fillColor, _flashColor);
			_mpb.SetFloat(fillPhase, 1.0f);
			_meshRenderer.SetPropertyBlock(_mpb);
			yield return wait;

			_mpb.SetFloat(fillPhase, 0f);
			_meshRenderer.SetPropertyBlock(_mpb);
			yield return wait;
		}

		yield return null;
	}
}
