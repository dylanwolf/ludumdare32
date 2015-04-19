using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageWindow : MonoBehaviour {

	public static MessageWindow Current;
	public float Margin = 0.1f;
	public float MessageTime = 5.0f;
	public float FadeTime = 0.5f;
	public float CharSpacing = 0.05f;

	Renderer _r;
	TextMesh _tm;
	Camera _c;
	Color originalColor;

	void Awake()
	{
		Current = this;
		_r = GetComponent<Renderer>();
		_tm = GetComponent<TextMesh>();
		_c = GetComponentInParent<Camera>();
		originalColor = _tm.color;
	}

	float wordWrapWidth;

	string[] words;
	List<string> lines = new List<string>();
	List<string> currentLine = new List<string>();
	const string SPACE = " ";
	const string NEWLINE = "\n";
	int letters = 0;
	string WordWrapText(string text)
	{
		text = text.Replace("\\n", NEWLINE);

		currentLine.Clear();
		lines.Clear();
		words = text.Split(new string[] { SPACE }, System.StringSplitOptions.RemoveEmptyEntries);
		letters = 0;

		foreach (string word in words)
		{
			if (((letters + word.Length) * (_tm.characterSize + CharSpacing)) >= wordWrapWidth)
			{
				lines.Add(string.Join(SPACE, currentLine.ToArray()));
				currentLine.Clear();
				letters = 0;
			}
			currentLine.Add(word);
			letters += word.Length + 1;

			if (word.EndsWith(NEWLINE))
			{
				lines.Add(string.Join(SPACE, currentLine.ToArray()));
				currentLine.Clear();
				letters = 0;
			}
		}
		if (currentLine.Count > 0)
			lines.Add(string.Join(SPACE, currentLine.ToArray()));
		return string.Join(NEWLINE, lines.ToArray());
	}

	void Start()
	{
		Vector3 tmpPos = transform.localPosition;
		tmpPos.x = -(_c.aspect * _c.orthographicSize) + Margin;
		tmpPos.y = _c.orthographicSize - Margin;
		transform.localPosition = tmpPos;
		wordWrapWidth = ((_c.aspect * _c.orthographicSize) - Margin) * 2;
		_r.enabled = false;
	}

	const string FADE_TEXT_COROUTINE = "FadeText";
	bool showingText = false;
	Color _color;
	IEnumerator FadeText()
	{
		showingText = true;
		_color = originalColor;

		// Fade in
		_r.enabled = true;
		for (float timer = 0; timer <= FadeTime; timer += Time.deltaTime)
		{
			_color.a = (timer / FadeTime);
			_tm.color = _color;
			yield return 0;
		}
		_color.a = 1;
		_tm.color = _color;

		// Wait
		for (float timer = 0; timer <= MessageTime; timer += Time.deltaTime)
			yield return 0;

		// Fade out
		for (float timer = 0; timer <= FadeTime; timer += Time.deltaTime)
		{
			_color.a = 1 - (timer / FadeTime);
			_tm.color = _color;
			yield return 0;
		}
		_r.enabled = false;

		if (!string.IsNullOrEmpty(newLevel))
		{
			Application.LoadLevel(newLevel);
			GameState.Current.SetStatus(ActionState.Playing);
		}
	}

	public void DisplayText(string message)
	{
		_tm.text = WordWrapText(message);
		if (showingText)
			StopCoroutine(FADE_TEXT_COROUTINE);
		StartCoroutine(FADE_TEXT_COROUTINE);
	}

	string newLevel = null;
	public void DisplayFinalText(string message, string nextLevel)
	{
		GameState.Current.SetStatus(ActionState.FinishedLevel);
		MessageTime = 2.0f;
		DisplayText(message);
		newLevel = nextLevel;
	}
}
