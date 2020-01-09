namespace Core
{
    public enum JobStage
    {
        None,
        PreparingDatabase,
        FetchingFromGithub,
        CountingTagged,
        UploadingToDatabase,
        Done,
        Error,
    }
}