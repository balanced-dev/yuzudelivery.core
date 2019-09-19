using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuzuDelivery.Core
{

    public class Yuzu
    {
        private const string InstanceNotSet = "Yuzu definition instance not valid";

        private const string ConfigurationNotSet = "Yuzu definition configuration not set";
        private const string ConfigurationAlreadySet = "Yuzu definition configuration already set, can't initialise more than once";

        private static IYuzuConfiguration _configuration;
        private static Yuzu _instance;

        /// <summary>
        /// Configuration provider for performing maps
        /// </summary>
        public static IYuzuConfiguration Configuration
        {
            get => _configuration ?? throw new InvalidOperationException(ConfigurationNotSet);
            private set => _configuration = (_configuration == null) ? value : throw new InvalidOperationException(ConfigurationAlreadySet);
        }

        /// <summary>
        /// Initialize static configuration instance
        /// </summary>
        /// <param name="config">Configuration action</param>
        public static void Initialize(IYuzuConfiguration config)
        {
            Instance = new Yuzu(config);

        }

        /// <summary>
        /// Resets the mapper configuration. Not intended for production use, but for testing scenarios.
        /// </summary>
        public static void Reset()
        {
            _configuration = null;
            _instance = null;
        }

        public static Yuzu Instance
        {
            get => _instance ?? throw new InvalidOperationException(InstanceNotSet);
            private set => _instance = value;
        }

        public Yuzu(IYuzuConfiguration _configuration)
        {
            Configuration = _configuration ?? throw new ArgumentNullException(nameof(_configuration));
        }
    }
}
