using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class MenuControl : MonoBehaviour {

    public GameObject _menuDisplay;

    private AudioSource[] _carAudio;

	// Use this for initialization
	void Start () {

        StartCoroutine("GetCarAudio");
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Cancel"))
        {
            PauseGame();
        }
	}

    public void PauseGame()
    {
        _menuDisplay.SetActive(!_menuDisplay.activeSelf);

        Time.timeScale = _menuDisplay.activeSelf ? 0 : 1;

        ChangeCarAudio(_menuDisplay.activeSelf == false);
    }

    public void StartGame()
    {
        if(GameController.MainGameController._isGameOver)
        {
            GameController.MainGameController.Initialize();
        }

        
        ChangeCarAudio(true);
        Time.timeScale = 1;
        _menuDisplay.SetActive(false);
    }

    public void ResetHighScore()
    {
        GameController.MainGameController.ResetHighScore();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
#else
        Application.Quit();
#endif
    }

    private IEnumerator GetCarAudio()
    {
        while(_carAudio == null || _carAudio.Length == 0)
        {
            _carAudio = GameController.MainGameController._player.GetComponents<AudioSource>();

            yield return null;
        }

        ChangeCarAudio(false);
    }

    private void ChangeCarAudio(bool enable)
    {
        foreach (AudioSource audio in _carAudio)
        {
            audio.enabled = enable;
        }
    }
}
