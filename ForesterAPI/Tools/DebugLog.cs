using SD = System.Diagnostics;

namespace ForesterAPI.Tools
{
    public static class DebugLog
    {
        public static void WriteLine(string text)
        {
            SD.Debug.WriteLine("===============");
            SD.Debug.WriteLine(text);
            SD.Debug.WriteLine("===============");
        }
    }
}
