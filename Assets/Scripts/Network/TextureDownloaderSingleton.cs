namespace Network
{
    public class TextureDownloaderSingleton : TextureDownloader
    {
        public static TextureDownloader Instance;

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}