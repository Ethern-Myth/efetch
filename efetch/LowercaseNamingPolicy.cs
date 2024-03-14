using System.Text.Json;

namespace efetch
{
    public class LowercaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name.ToLower();
    }
}
