using UnityEngine;
using System.Collections;

public class ColourFade : MonoBehaviour {
	
	
	public class AnimatedText
	{
		public AnimatedText(string text, float timeBetweenLetters, float fullMessageDisplayTime)
		{
			this.completeText = text;
			this.timeBetweenLetters = timeBetweenLetters;
			this.fullMessageDisplayTime = fullMessageDisplayTime;
			this.letterCounter = 0;
			this.letterTimer = 0.0f;
		}
		
		public void Update()
		{
			letterTimer += Time.deltaTime;
			if(completeText.Length == 0 || letterCounter == completeText.Length)
			{
				if(fullMessageDisplayTime > 0.0f && letterTimer >= fullMessageDisplayTime)
				{
					currentText = "";
				}
			}
			else if(letterTimer >= timeBetweenLetters)
			{
				letterTimer = 0.0f;
				++letterCounter;
				currentText = completeText.Substring(0, letterCounter);
			}
		}
		
		string completeText;
		float timeBetweenLetters;
		int letterCounter;
		float fullMessageDisplayTime;
		float letterTimer;
		public string currentText;
	}
	
	private AnimatedText animatedText;
	
	public Texture fadeTexture;
	
	public float fadeTime = 2.0f;
	private float fadeTimer = 0.0f;
	
	public Hand leftHand;
	public Hand rightHand;
	
	public Font font;
	
	public float timeBetweenLetters = 0.15f;
	public float fullMessageDisplayTime = 5.0f;
	
	private bool drawIntroText = true;
	public string introText;
	private bool drawInstructions = false;
	public string instructionsText;
	public Texture instructionsTexture;
	
	public string goodLuckText;
	
	private bool fadeOut = false;
	
	
	// Use this for initialization
	void Start () {
		animatedText = new AnimatedText(introText, timeBetweenLetters, fullMessageDisplayTime);
		leftHand.canSetShoulders = false;
		leftHand.fistsDisabled = true;
		rightHand.canSetShoulders = false;
		rightHand.fistsDisabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(animatedText != null)
		{
			animatedText.Update ();
			if(animatedText.currentText == "")
			{
				animatedText = null;
			}
		}
		
		if(drawIntroText && animatedText == null)
		{
			drawIntroText = false;
			drawInstructions = true;
			animatedText = new AnimatedText(instructionsText, timeBetweenLetters, 0.0f);
		}
		
		if(drawInstructions && animatedText.currentText == instructionsText)
		{
			leftHand.canSetShoulders = true;
			rightHand.canSetShoulders = true;

			if(leftHand.shoulderSet && rightHand.shoulderSet)
			{
				animatedText = new AnimatedText(goodLuckText, timeBetweenLetters, 0.0f);
				drawInstructions = false;
				fadeOut = true;
			}
		}
		
		if(fadeOut)
		{
			fadeTimer += Time.deltaTime;
			if(fadeTimer >= fadeTime)
			{
				leftHand.fistsDisabled = false;
				rightHand.fistsDisabled = false;
				Destroy (gameObject);
			}
		}
	}
	
	void DrawTextbox(string text)
	{
		GUI.Label (new Rect(Screen.width / 8.0f, Screen.height / 3.5f, Screen.width / 3.0f, Screen.height / 2.0f), text);
		GUI.Label (new Rect(Screen.width / 2.0f + Screen.width / 8.0f, Screen.height / 3.5f, Screen.width / 3.0f, Screen.height / 2.0f), text);
	}
	
	void OnGUI()
	{
		if(fadeOut)
		{
			GUI.color = new Color(0.0f, 0.0f, 0.0f, (fadeTime - fadeTimer) / fadeTime);
		}
		else
		{
			GUI.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
		}
		GUI.DrawTexture(new Rect(0.0f, 0.0f, Screen.width, Screen.height), fadeTexture);
		
		GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		if(animatedText != null)
		{
			GUI.skin.label.font = font;
			DrawTextbox (animatedText.currentText);
		}
		
		if(drawInstructions)
		{
			GUI.DrawTexture (new Rect(Screen.width / 8.0f, 2.0f * Screen.height / 4.0f, Screen.width / 3.5f, Screen.height / 3.0f), instructionsTexture);
			GUI.DrawTexture (new Rect(Screen.width / 2.0f + Screen.width / 8.0f, 2.0f * Screen.height / 4.0f, Screen.width / 3.5f, Screen.height / 3.0f), instructionsTexture);
		}
	}
}
