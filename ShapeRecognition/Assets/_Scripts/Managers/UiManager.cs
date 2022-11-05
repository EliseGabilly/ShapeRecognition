using UnityEngine;
using UnityEngine.UI;

public class UiManager : Singleton<UiManager> {

    #region Variables
    #endregion

    private void Start() {
    }

    private void Update() {
        if (Input.GetKey("escape")) {
            Application.Quit();
        }
    }

    public void Quit() {
        Application.Quit();
    }
}
