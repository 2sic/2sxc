using Microsoft.Build.Utilities;
using System;

namespace ToSic.Sxc.BuildTasks
{
    public class ColorMessage : Task
    {
        public string Text { get; set; } = string.Empty;

        public string ForegroundColor { get; set; }
        public string BackgroundColor { get; set; }

        public override bool Execute()
        {
            var originalForegroundColor = Console.ForegroundColor;
            var originalBgColor = Console.BackgroundColor;

            if (!string.IsNullOrEmpty(ForegroundColor))
                if (Enum.TryParse(ForegroundColor, true, out ConsoleColor foregroundColor))
                    Console.ForegroundColor = foregroundColor;

            if (!string.IsNullOrEmpty(BackgroundColor))
                if (Enum.TryParse(BackgroundColor, true, out ConsoleColor backgroundColor))
                    Console.BackgroundColor = backgroundColor;

            Console.Write(Text);
            // Log.LogMessage(MessageImportance.High, Text);

            Console.ForegroundColor = originalForegroundColor;
            Console.BackgroundColor = originalBgColor;

            Console.Write("\n");

            return true;
        }
    }
}