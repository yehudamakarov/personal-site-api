namespace Core
{
    public enum JobStage
    {
        None,
        PreparingDatabase,
        FetchingFromGithub,
        CountingTagged,
        UploadingToDatabase,
        InProgress,
        Done,
        Warning,
        Error,
    }
}