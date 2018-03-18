using System;
using System.IO;
using Newtonsoft.Json;

namespace GitConsoleExtension
{
    internal class ConfigModel
    {
        public string MinttyPath { get; set; }
    }

    internal class Config
    {
        private static Config _instance;
        public static Config Instance => _instance ?? (_instance = new Config());

        private ConfigModel _config;

        private string ConfigFile
        {
            get
            {
                var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var configFile = Path.Combine(appdata, "GitConsoleExtension", "Config.conf");
                if (!File.Exists(configFile))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(configFile));
                    File.Create(configFile);
                }
                return configFile;
            }
        }

        /// <summary>
        /// Tries to extract the mintty path from the PATH variable and the standard paths
        /// </summary>
        /// <returns></returns>
        public static string FindMinttyPath()
        {
            try
            {
                string progdir;
                if (!Environment.Is64BitProcess && Environment.Is64BitOperatingSystem)
                    progdir = System.Environment.GetEnvironmentVariable("ProgramW6432");
                else
                    progdir = System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                var strFile = Path.Combine(progdir, "Git\\usr\\bin\\mintty.exe");
                if (File.Exists(strFile))
                    return strFile;

                progdir = System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                strFile = Path.Combine(progdir, "Git\\usr\\bin\\mintty.exe");
                if (File.Exists(strFile))
                    return strFile;

                var paths = System.Environment.GetEnvironmentVariable("PATH").Split(';');
                foreach (var path in paths)
                {
                    strFile = Path.Combine(path, "mintty.exe");
                    if (File.Exists(strFile))
                        return strFile;
                }
            }
            catch { }

            return "";
        }

        public bool FindAndSaveMinttyPath()
        {
            string strFound = FindMinttyPath();
            if (strFound == "")
                return false;

            MinttyPath = strFound;
            return true;
        }

        public string MinttyPath
        {
            get { return _config.MinttyPath; }
            set
            {
                if (_config.MinttyPath != value)
                {
                    _config.MinttyPath = value;
                    SaveConfig();
                }
            }
        }

        private Config()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                var configs = File.ReadAllText(ConfigFile);
                if (!string.IsNullOrEmpty(configs))
                {
                    _config = JsonConvert.DeserializeObject<ConfigModel>(configs);
                }
                else
                {
                    _config=new ConfigModel();
                }
            }
            catch (Exception)
            {
                _config = new ConfigModel();
            }
        }

        private void SaveConfig()
        {
            try
            {
                var configs = JsonConvert.SerializeObject(_config);
                File.WriteAllText(ConfigFile, configs);
            }
            catch (Exception)
            {
            }
        }
    }
}
