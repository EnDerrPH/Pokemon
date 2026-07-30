using System;

public class LoadingBarModel : AModel
{
    public int LoadedCount { get; private set; }
    public int TotalCount { get; private set; }
    public LoadingFetchPhase FetchPhase { get; private set; }

    public Action SubmitStartLoading;
    public Action ProgressUpdated;
    public Action LoadingComplete;

    public void SetProgress(int loadedCount, int totalCount, LoadingFetchPhase fetchPhase)
    {
        LoadedCount = loadedCount;
        TotalCount = totalCount;
        FetchPhase = fetchPhase;
        ProgressUpdated?.Invoke();
    }
}
