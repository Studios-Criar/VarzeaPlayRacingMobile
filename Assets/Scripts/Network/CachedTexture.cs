namespace Network
{
    [System.Serializable]
    public struct CachedTexture
    {
        public System.Uri Uri;
        public string name;
        public string etag;
        public string path;
    }
}