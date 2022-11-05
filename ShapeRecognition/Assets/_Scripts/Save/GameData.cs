/// <summary>
/// Serializable class savable with the save system
/// </summary>
[System.Serializable]
public class GameData {

    #region Variables
    public int testValue;
    #endregion

    public GameData(Game player) {
        this.testValue = player.testValue;
    }
}
