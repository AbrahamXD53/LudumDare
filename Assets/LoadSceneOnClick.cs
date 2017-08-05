using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour {
    public string sceneName;

	public void loadGame () {
		SceneManager.LoadScene(sceneName);
	}

    public void loadCredits()
    {
        SceneManager.LoadScene("Credits");
    }
}
