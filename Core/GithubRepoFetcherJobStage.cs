namespace Core
{
    public enum GithubRepoFetcherJobStage
    {
        None,
        PreparingDatabase,
        Fetching,
        Uploading,
        Done,
        Error
    }
}