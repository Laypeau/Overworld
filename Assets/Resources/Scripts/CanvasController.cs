using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasController : MonoBehaviour
{
	enum dialoguePosition
	{
		//	aa	ab ac
		//	ba	bb	bc
		//	ca	cb	cc
		aa,
		ab,
		ac,
		ba,
		bb,
		bc,
		ca,
		cb,
		cc
	}
	enum dialogueBoxType
	{
		boring
	}

	private GameObject dialoguePanel;
	public TMP_Text dialogueBoxDefault;

	void Awake()
	{
		dialoguePanel = transform.Find("Text Panel").gameObject;
		dialogueBoxDefault = transform.Find("Text Panel/Interact Text").GetComponent<TMP_Text>();

		SetPanelVisible(false);


	}


	public void SetPanelVisible(bool value)
	{
		dialoguePanel.SetActive(value);
		if (value)
		{
			//dialoguePanel.GetComponent<RectTransform>().position = Camera.screenTow
		}
	}
}
