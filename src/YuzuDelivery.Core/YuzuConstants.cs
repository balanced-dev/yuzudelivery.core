using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuzuDelivery.Core
{

    public class YuzuConstants
    {
        private const string InstanceNotSet = "Yuzu definition instance not valid";

        private const string ConfigurationNotSet = "Yuzu definition configuration not set";
        private const string ConfigurationAlreadySet = "Yuzu definition configuration already set, can't initialise more than once";

        private static IYuzuConstantsConfig _configuration;
        private static YuzuConstants _instance;

        /// <summary>
        /// Configuration provider for performing maps
        /// </summary>
        public static IYuzuConstantsConfig Configuration
        {
            get => _configuration ?? throw new InvalidOperationException(ConfigurationNotSet);
            private set => _configuration = (_configuration == null) ? value : throw new InvalidOperationException(ConfigurationAlreadySet);
        }

        /// <summary>
        /// Initialize static configuration instance
        /// </summary>
        /// <param name="config">Configuration action</param>
        public static void Initialize(IYuzuConstantsConfig config)
        {
            Instance = new YuzuConstants(config);

        }

        /// <summary>
        /// Resets the mapper configuration. Not intended for production use, but for testing scenarios.
        /// </summary>
        public static void Reset()
        {
            _configuration = null;
            _instance = null;
        }

        public static YuzuConstants Instance
        {
            get => _instance ?? throw new InvalidOperationException(InstanceNotSet);
            private set => _instance = value;
        }

        public YuzuConstants(IYuzuConstantsConfig _configuration)
        {
            Configuration = _configuration ?? throw new ArgumentNullException(nameof(_configuration));
        }
    }
}
