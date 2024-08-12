/********************************************************************************
# This is a practice project from Biao Yan (Bob Yan), and it's free to be 
# downloaded for study and test project.
*********************************************************************************/

using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace SpecFlow_For_WoolWorths.Support
{

    /// <summary>
    /// The class is used to obtain the user configuraiton information 
    /// and location definitions.
    /// The class is a singleton mode as we don't need to construct 
    /// multiple _instance for the same configuration information
    /// </summary>
    public class UserConfiguration
    {
        //the property is used to store the original json content.
        private IConfiguration configuration;
        
        //the dictionary is used to store user settings
        private Dictionary<string, string> settings;

        //the dictionary is used to store the location definitions
        private Dictionary<string, string> applicationLocations;


        //the actual instance of the class
        private static UserConfiguration? _instance;

        //lock to implement thread safty
        private static readonly object lockObject = new object();

        /// <summary>
        /// Read the configuration one time in constructor method
        /// </summary>
        private UserConfiguration()
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(TestContext.CurrentContext.TestDirectory)
                .AddJsonFile("usersettings.json")
                .AddJsonFile("elementlocations.json", optional: true, reloadOnChange: true)
                .AddJsonFile("usersettings.testdata.json", optional: true, reloadOnChange: true);

            configuration = configBuilder.Build();
            settings = GetDictionary(configuration, "user_settings");
            applicationLocations = GetDictionary(configuration, "gui_locations");

        }

        /// <summary>
        /// The method is used to convert the configuation section into dictionary object
        /// </summary>
        /// <param name="configuration">IConfiguration _instance</param>
        /// <param name="sectionName">section name</param>
        /// <returns>Dictionary(string, string)</returns>
        public Dictionary<string, string> GetDictionary(IConfiguration configuration, string sectionName)
        {
            var dictionary = new Dictionary<string, string>();
            var section = configuration.GetSection(sectionName);

            foreach (var child in section.GetChildren())
            {
                if (child != null && child.Value != null)
                {
                    dictionary[child.Key] = child.Value;
                    Console.WriteLine($"store {child.Key} = {child.Value}");
                }
            }
            return dictionary;
        }

        /// <summary>
        /// The singleton property for outside class to access this class
        /// </summary>
        public static UserConfiguration Instance
        {
            get
            {
                // Double-checked locking for thread safety
                if (_instance == null)
                {
                    lock (lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new UserConfiguration();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Clear the instance in case we need to read the configuration again.
        /// </summary>
        public static void ClearInstance()
        {
            lock (lockObject)
            {
                _instance = null;
            }
        }

        /// <summary>
        /// The method is used to obtain one value of a key
        /// </summary>
        /// <param name="key">string key of one defintion in user settings</param>
        /// <returns>string value of the key</returns>
        public string GetAppParameter(string key)
        {
            
            var value = settings.GetValueOrDefault<string, string>(key);
            if (value == null)
            {
                value = applicationLocations.GetValueOrDefault<string, string>(key, "");
            }
            return value;
        }

    }
}
