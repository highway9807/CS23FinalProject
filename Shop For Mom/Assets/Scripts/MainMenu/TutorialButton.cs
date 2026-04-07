using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
	public float speed = 5f;
	public float targetY = 12f;
	public float moveDuration = 2f;

	public float rotateSpeed = 100f;
	public float targetAngle = 90f;

	private float remainingAngle = 0f;

	//public AudioSource moveAudio;

	private bool isRunning = false;
	private bool isRunning1 = false;
	private float timer = 0f;
	private RectTransform rect;


	public void StartRotating()
	{
		if(remainingAngle <= 0)
		{
			remainingAngle = targetAngle;
		}
	}

	void Start()
	{
		rect = GetComponent<RectTransform>();
	}

	void Update()
	{
		if (isRunning)
		{
			if(remainingAngle > 0)
			{
				float rotationThisFrame = rotateSpeed * Time.deltaTime;
				if(rotationThisFrame > remainingAngle)
				{
					rotationThisFrame = remainingAngle;
				}
				transform.Rotate(0,0,-rotationThisFrame);
				remainingAngle -= rotationThisFrame;
			}
			
			rect.anchoredPosition += Vector2.down*speed*Time.deltaTime;
			
			//transform.Translate(Vector3.up * speed * Time.deltaTime);
			timer += Time.deltaTime;
			
			//SceneManager.LoadScene(1);
			if (timer >= moveDuration)
			{
				isRunning = false;
				//moveAudio.Stop();
				SceneManager.LoadScene("Tutorial");
			}
		}
		else if (isRunning1)
		{
			if(remainingAngle > 0)
			{
				float rotationThisFrame = rotateSpeed * Time.deltaTime;
				if(rotationThisFrame > remainingAngle)
				{
					rotationThisFrame = remainingAngle;
				}
				transform.Rotate(0,0,-rotationThisFrame);
				remainingAngle -= rotationThisFrame;
			}
			
			rect.anchoredPosition += Vector2.down*speed*Time.deltaTime;
			
			//transform.Translate(Vector3.up * speed * Time.deltaTime);
			timer += Time.deltaTime;
			
			//SceneManager.LoadScene(1);
			if (timer >= moveDuration)
			{
				isRunning1 = false;
				//moveAudio.Stop();
				Debug.Log("HI");
				SceneManager.LoadScene("Level1");
			}
		}
	}
	


	public void GameStart()
	{
		isRunning = true;
		timer = 0f;
		//moveAudio.Play();
		
	}
	public void GameStart1()
	{
		isRunning1 = true;
		timer = 0f;
		//moveAudio.Play();
		
	}
	public void ExitGame()
	{
		Application.Quit();
	}
}
