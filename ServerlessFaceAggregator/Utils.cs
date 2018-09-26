namespace ServerlessFaceAggregator
{
    public static class Utils
    {
        public static string GetStorageName(string patternDirectory)
        {
            string withoutLeadingSlash = patternDirectory.TrimStart('/');
            return withoutLeadingSlash.Substring(0, withoutLeadingSlash.IndexOf('/'));
        }

        public static string GetPathWithoutStorageNameAndLeadingSlash(string path)
        {
            string withoutLeadingSlash = path.TrimStart('/');
            return withoutLeadingSlash.Substring(withoutLeadingSlash.IndexOf('/') + 1);
        }
    }
}
