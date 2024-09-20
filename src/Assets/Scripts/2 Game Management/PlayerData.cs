[System.Serializable]
public class PlayerData
{
    public int Score;

    public float Volume;

    public int Width;

    public int Height;

    public bool FullScreen;

    public PlayerData(int score, float volume, int width, int height, bool fullScreen)
    {
        Score = score;
        Volume = volume;
        Width = width;
        Height = height;
        FullScreen = fullScreen;
    }
}
