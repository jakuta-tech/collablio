using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

namespace collablio
{

	
    class ConfigManager
    {
        Dictionary<string,string> defaults = new Dictionary<string,string> { 
            { "listenurl", "http://127.0.0.1:5000" },
            { "sessiontimeout" , "120" }
        };

		private static ConfigManager _singleton = null;
		public static ConfigManager Instance()
		{
			if(_singleton == null)
			{
				_singleton = new ConfigManager();
				Task.Run(() => _singleton.Initialise()).Wait();
			}
			return _singleton;			
		}
		
		private ConfigManager()
		{
        }

        private static Dictionary<string, object> _conf = new Dictionary<string, object>();

		private async Task Initialise()
		{
			string cdir = Directory.GetCurrentDirectory();
            string conffile = Path.Combine(cdir,"config.json");
            try {
                string jsonstr = File.ReadAllText(conffile);
                Dictionary<string, object> confdict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonstr);
                _conf = confdict;
                LogService.Log(LOGLEVEL.DEBUG,$"ConfigManager: loaded {confdict}");
            }
            catch (IOException e) {
                LogService.Log(LOGLEVEL.DEBUG,$"ConfigManager: failed to open file {conffile}");
            }

		}

        public string GetValue(string key) {
            if(_conf.ContainsKey(key))
                return _conf[key].ToString();
            else if(defaults.ContainsKey(key))
                return defaults[key].ToString();
            return "";
        }


    }
}