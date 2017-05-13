using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;

/// <remarks>Based on: https://bytes.com/topic/net/insights/797169-reading-parsing-ini-file-c</remarks>
public class IniParser
{
    private readonly IOrderedDictionary keyPairs = new OrderedDictionary();
    private readonly HashSet<string> sections = new HashSet<string>();

    public readonly string Filepath;

    private readonly RuntimeBytesLoader runtimeLoader;

    private struct SectionPair
    {
        public string Section;
        public string Key;
    }

    public IniParser(string filepath, RuntimeBytesLoader runtimeLoader = null)
    {
        this.Filepath = filepath;
        this.runtimeLoader = runtimeLoader;
    }

    public bool TryGet(string section, string key, out string value)
    {
        var sectionPair = new SectionPair { Section = section, Key = key };

        var found = this.keyPairs.Contains(sectionPair);

        value = found ? (string)this.keyPairs[sectionPair] : null;
        
        return found;
    }

    public void Add(string section, string key, string value)
    {
        var sectionPair = new SectionPair { Section = section, Key = key };

        if (this.keyPairs.Contains(sectionPair))
        {
            this.keyPairs.Remove(sectionPair);
        }

        this.keyPairs.Add(sectionPair, value);

        this.sections.Add(section);
    }

    public void Clear()
    {
        this.keyPairs.Clear();
        this.sections.Clear();
    }

    private void LoadText(Action<MemoryStream> onLoaded)
    {
        if (this.runtimeLoader == null)
        {
            onLoaded(new MemoryStream(File.ReadAllBytes(this.Filepath)));
        }
        else
        {
            this.runtimeLoader.Load(this.Filepath, onLoaded);
        }
    }

    public void Load()
    {
        this.Clear();

        TextReader iniFile = null;
        string strLine;
        string currentRoot = null;
        string[] keyPair = null;

        this.LoadText(stream =>
        {
            try
            {
                iniFile = new StreamReader(stream);

                strLine = iniFile.ReadLine();

                while (strLine != null)
                {
                    if (strLine != "")
                    {
                        if (strLine.StartsWith("[", StringComparison.Ordinal) && strLine.EndsWith("]", StringComparison.Ordinal))
                        {
                            currentRoot = strLine.Substring(1, strLine.Length - 2);
                        }
                        else
                        {
                            keyPair = strLine.Split(new [] { '=' }, 2);

                            currentRoot = currentRoot ?? "ROOT";
                            this.sections.Add(currentRoot);

                            var sectionPair = new SectionPair { Section = currentRoot, Key = keyPair[0] };
                            string value = null;

                            if (keyPair.Length > 1)
                            {
                                value = keyPair[1];
                            }

                            this.keyPairs.Add(sectionPair, value);
                        }
                    }

                    strLine = iniFile.ReadLine();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
                
                if (iniFile != null)
                {
                    iniFile.Close();
                }
            }
        });
    }

    public void Save()
    {
        var tmpValue = "";
        var sb = new StringBuilder();

        foreach (var section in this.sections)
        {
            sb.AppendLine("[" + section + "]");

            foreach (SectionPair sectionPair in this.keyPairs.Keys)
            {
                if (sectionPair.Section == section)
                {
                    tmpValue = (string)this.keyPairs[sectionPair];

                    if (tmpValue != null)
                    {
                        tmpValue = "=" + tmpValue;
                    }

                    sb.AppendLine(sectionPair.Key + tmpValue);
                }
            }

            sb.AppendLine();
        }

        try
        {
            var dirName = Path.GetDirectoryName(this.Filepath);
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            File.WriteAllText(this.Filepath, sb.ToString());
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void ForEachSection(Action<string> action)
    {
        foreach (var section in this.sections)
        {
            action(section);
        }
    }
}
