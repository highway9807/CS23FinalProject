using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
	public float speed = 5f;
	public float targetY = 12f;
	public float moveDuration = 2f;

	public AudioSource moveAudio;

	private bool isRunning = false;
	private float timer = 0f;
	private RectTransform rect;


	void Start()
	{
		rect = GetComponent<RectTransform>();
	}

	void Update()
	{
		if (isRunning)
		{
			rect.anchoredPosition += Vector2.up*speed*Time.deltaTime;
			//transform.Translate(Vector3.up * speed * Time.deltaTime);
			timer += Time.deltaTime;
			SceneManager.LoadScene(1);
			if (timer >= moveDuration)
			{
				isRunning = false;
				moveAudio.Stop();
				SceneManager.LoadScene(1);
			}
		}
	}

	public void GameStart()
	{
		isRunning = true;
		timer = 0f;
		moveAudio.Play();
		
	}
	public void ExitGame()
	{
		Application.Quit();
	}
}
