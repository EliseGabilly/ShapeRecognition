using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// Player class containing the information that are "translated" in playerdata then saved
/// </summary>
public class Game : Singleton<Game> {

    #region Variables
    public int testValue = 0;
    #endregion

    protected override void Awake() {
        base.Awake();
        SaveSystem.LoadData();
    }

    public Game ChangeData(GameData data) {
        this.testValue = data.testValue;
        return this;
    }

}
