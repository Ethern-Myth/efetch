using System.Text.Json;

namespace efetch.Policies
{
    public class LowercaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name.ToLower();
    }
}
