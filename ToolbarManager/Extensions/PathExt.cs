namespace ToolbarManager.Extensions
{
    public static class PathExt
    {
        public static string SanitizeFileName(string name)
        {
            return name
                .Replace(':', '.')
                .Replace('?', '.')
                .Replace('*', '-')
                .Replace('/', '-')
                .Replace('\\', '_');
        }
    }
}