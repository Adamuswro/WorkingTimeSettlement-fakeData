using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingTimeSettlement
{
    public static class Logs
    {
        private static string chapterSign() =>
            "------------------------------";
        private static string subChapterSign() =>
            "----------------";

        public static string GetChapter(string title) =>
            $"{Environment.NewLine}{chapterSign()}{title}{chapterSign()}";

        public static string GetSubChapter(string title) =>
            $"{Environment.NewLine}{subChapterSign()}{title}{subChapterSign()}{Environment.NewLine}";

    }
}
