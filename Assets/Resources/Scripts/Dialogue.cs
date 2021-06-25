using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;


public class Dialogue : Interactable
{
	[SerializeField] private protected DialogueData dialogueData;
	[Tooltip("Time delay between each character")] public float delayTime = 0.02f;
	[Tooltip("How many characters will be displayed after each iteration of delayTime")] int countIncrement = 2;
	private protected CanvasController canvasController;
	private protected TMP_Text dialogueBox;

	protected override void Awake()
	{
		base.Awake();

		//player.inputActionAsset.UI.Accept.performed += ctx => {};
	}

	protected void Start()
	{
		canvasController = GameObject.Find("Interaction Canvas").GetComponent<CanvasController>();
		dialogueBox = canvasController.dialogueBoxDefault;
	}

	public override void Interact()
	{
		StartCoroutine(ReadText());
	}

	protected virtual IEnumerator ReadText()
	{
		dialogueBox.text = dialogueData.textBoxes[0];
		dialogueBox.maxVisibleCharacters = 0;
		player.SetUIMode(true);
		canvasController.SetPanelVisible(true);

														
		yield return EnumerateThroughCharacters();

		canvasController.SetPanelVisible(false);
		player.SetUIMode(false);
		yield break;
	}

	protected IEnumerator EnumerateThroughCharacters() //allow escaping the entire thing
	{
		for (int i = 0; i < dialogueData.textBoxes.Count; i++)
		{
			dialogueBox.maxVisibleCharacters = 0;
			dialogueBox.text = dialogueData.textBoxes[i];

			for (int j = 0; j < dialogueData.textBoxes[i].Length + countIncrement; j += countIncrement) //for each letter
			{
				//if pressed and released again, skip to end of the text box. Maybe add a thing to speed up delayTime as well. read up in input action button action type
				if (player.inputActionAsset.UI.Accept.phase == InputActionPhase.Started)
				{
					dialogueBox.maxVisibleCharacters = dialogueData.textBoxes[i].Length;
					while (player.inputActionAsset.UI.Accept.phase == InputActionPhase.Started)
					{
						yield return new WaitForEndOfFrame();
					}
					break;
				}

				dialogueBox.maxVisibleCharacters = j;
				yield return new WaitForSeconds(delayTime);
			}

			yield return StartCoroutine(WaitForInput());
			while (player.inputActionAsset.UI.Accept.phase == InputActionPhase.Started) //////////////////////////////////////////////////////move to WaitForINput()
			{
				yield return new WaitForEndOfFrame();
			}
		}
	}

	public IEnumerator WaitForInput()
	{
		while (player.inputActionAsset.UI.Accept.phase != InputActionPhase.Started)
		{
			yield return null;
		}
		yield break;
	}

}

