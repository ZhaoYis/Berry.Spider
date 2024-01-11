using System.Text;
using System.Text.RegularExpressions;

namespace Berry.Spider.Core;

public class EasyConfigHelper
{
    private readonly string _fileName;
    private readonly string _filePath;
    private readonly string defaultFilePath = AppContext.BaseDirectory;
    private readonly Dictionary<string, Dictionary<string, string>> ConfigDict = new();

    public EasyConfigHelper(string fileName)
    {
        _fileName = fileName;
        _filePath = Path.Combine(defaultFilePath, fileName);

        this.LoadToFile();
    }

    public bool Exists(string sectionName)
    {
        return this.ConfigDict.ContainsKey(sectionName);
    }

    public bool Exists(string sectionName, string key)
    {
        return this.ConfigDict.ContainsKey(sectionName) && this.ConfigDict[sectionName].ContainsKey(key);
    }

    public string Get(string sectionName, string key)
    {
        if (!this.ConfigDict.ContainsKey(sectionName)) return string.Empty;

        if (this.ConfigDict.TryGetValue(sectionName, out Dictionary<string, string>? section))
        {
            if (section.TryGetValue(key, out string? value))
            {
                return value;
            }
        }

        return string.Empty;
    }

    public void Set(string sectionName, string key, string value)
    {
        this.Set(sectionName, new Dictionary<string, string>
        {
            { key, value }
        });
    }

    public void Set(string sectionName, Dictionary<string, string> values)
    {
        if (this.ConfigDict.ContainsKey(sectionName))
        {
            var section = this.ConfigDict[sectionName];
            foreach (KeyValuePair<string, string> value in values)
            {
                section[value.Key] = value.Value;
            }

            this.ConfigDict[sectionName] = section;
        }
        else
        {
            this.ConfigDict[sectionName] = values;
        }

        this.SaveToFile();
    }

    private void LoadToFile()
    {
        if (File.Exists(_filePath))
        {
            this.ConfigDict.Clear();

            string[] lines = File.ReadAllLines(_filePath);
            string currentSectionName = string.Empty;
            foreach (string line in lines)
            {
                string lineValue = line.Trim();
                if (string.IsNullOrEmpty(lineValue)) continue;

                //先尝试去匹配section节点
                Match sectionMatch = Regex.Match(lineValue, "\\[([^\\]]+)\\]");
                if (sectionMatch.Success)
                {
                    string sectionName = currentSectionName = sectionMatch.Groups[1].Value;
                    this.ConfigDict.Add(sectionName, new Dictionary<string, string>());
                }

                //再去尝试匹配k-v节点
                Match valuesMatch = Regex.Match(lineValue, "(?<key>\\w+)=(?<value>.*)");
                if (valuesMatch.Success)
                {
                    var values = this.ConfigDict[currentSectionName] ?? new Dictionary<string, string>();

                    string key = valuesMatch.Groups["key"].Value;
                    string value = valuesMatch.Groups["value"].Value;
                    values[key] = value;

                    this.ConfigDict[currentSectionName] = values;
                }
            }
        }
    }

    private void SaveToFile()
    {
        if (File.Exists(_filePath)) File.Delete(_filePath);

        if (this.ConfigDict is { Count: > 0 })
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, Dictionary<string, string>> section in this.ConfigDict)
            {
                string sectionName = section.Key;
                Dictionary<string, string> sectionValues = section.Value;
                if (sectionValues is { Count: > 0 })
                {
                    builder.AppendFormat("[{0}]{1}", sectionName, Environment.NewLine);
                    foreach (KeyValuePair<string, string> value in sectionValues)
                    {
                        builder.AppendFormat("{0}={1}{2}", value.Key, value.Value, Environment.NewLine);
                    }
                }
            }

            File.WriteAllText(_filePath, builder.ToString(), Encoding.UTF8);
        }
    }
}