namespace ToSic.Sxc.Security
{
    public interface IInputSanitizer
    {
        string SanitizeSql(string input);
    }

    public class InputSanitizer : IInputSanitizer
    {
        public string SanitizeSql(string input)
        {
            return System.Security.SecurityElement.Escape(input);
        }
    }
}
