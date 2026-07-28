using System;

public class LoadingBarModel : AModel
{
    public int LoadedCount { get; private set; }
    public int TotalCount { get; private set; }

    public Action SubmitStartLoading;
    public Action ProgressUpdated;
    public Action LoadingComplete;

    public void SetProgress(int loadedCount, int totalCount)
    {
        LoadedCount = loadedCount;
        TotalCount = totalCount;
        ProgressUpdated?.Invoke();
    }
}
