using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DFC.App.Pages.Models.Robots
{
    [ExcludeFromCodeCoverage]
    public class Robot
    {
        private readonly StringBuilder robotData;

        public Robot()
        {
            robotData = new StringBuilder();
        }

        public string Data => robotData.ToString();

        public void Add(string text)
        {
            robotData.AppendLine(text);
        }
    }
}
