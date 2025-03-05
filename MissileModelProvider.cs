using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DisplayAScene
{
    /// <summary>
    /// Static provider class that allows external applications to set missile model information
    /// for use in the DisplayAScene application.
    /// </summary>
    public static class MissileModelProvider
    {
        private static int _selectedMissileId;
        public static int SelectedMissileId
        {
            get { return _selectedMissileId; }
            set { _selectedMissileId = value; }
        }

        private static string _selectedMissilePart = "UN";
        public static string SelectedMissilePart
        {
            get { return _selectedMissilePart; }
            set { _selectedMissilePart = value; }
        }

        private static string _modelPath = "C:\\Work\\display-a-scene\\3D Objects\\singleX.obj";
        public static string ModelPath
        {
            get { return _modelPath; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    // If path exists, use it
                    if (File.Exists(value))
                    {
                        _modelPath = value;
                    }
                }
            }
        }

        // Flag to disable the provider's functionality
        private static bool _isDisabled = false;

        /// <summary>
        /// Gets whether the model provider is disabled
        /// </summary>
        public static bool IsDisabled => _isDisabled;

        /// <summary>
        /// Disables the model provider functionality to avoid interfering with other applications
        /// </summary>
        public static void Disable()
        {
            _isDisabled = true;
        }

        /// <summary>
        /// Enables the model provider functionality
        /// </summary>
        public static void Enable()
        {
            _isDisabled = false;
        }

        // Event to notify when model info has changed
        public static event EventHandler ModelInfoChanged;

        /// <summary>
        /// Updates the missile model information
        /// </summary>
        /// <param name="missileId">The ID of the selected missile</param>
        /// <param name="missilePart">The missile part (UN, RV, BT)</param>
        /// <param name="modelPath">Full path to the 3D model file</param>
        public static void UpdateMissileModelInfo(int missileId, string missilePart, string modelPath)
        {
            if (_isDisabled) return;

            SelectedMissileId = missileId;
            SelectedMissilePart = missilePart;
            ModelPath = modelPath;

            // Notify listeners
            ModelInfoChanged?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Try to get the model path from MetisDB using reflection
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public static bool TryGetModelPathFromMetisDB()
        {
            if (_isDisabled) return false;

            try
            {
                // Use Assembly.LoadFrom instead of Assembly.Load to specify the full path
                // This avoids interfering with MetisDB's own assembly loading
                string dataFetcherPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataFetcher.dll");
                if (!File.Exists(dataFetcherPath))
                {
                    // Try to find it in the MetisDB directory
                    string metisDbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"Work\MetisDB\DataFetcher\bin\Debug\net6.0\DataFetcher.dll");
                    if (File.Exists(metisDbPath))
                    {
                        dataFetcherPath = metisDbPath;
                    }
                    else
                    {
                        Console.WriteLine("Could not find DataFetcher.dll");
                        return false;
                    }
                }

                var assembly = Assembly.LoadFrom(dataFetcherPath);
                if (assembly == null) return false;

                // Get the ServicesPOC type
                var servicesPocType = assembly.GetType("DataFetcher.ServicesPOC");
                if (servicesPocType == null) return false;

                // Create a new instance - don't try to get a singleton instance
                object instanceObj = Activator.CreateInstance(servicesPocType);
                if (instanceObj == null) return false;

                // Use direct field access instead of properties to avoid triggering side effects
                var selectedMissileIdField = servicesPocType.GetField("selectedMissileID", BindingFlags.Public | BindingFlags.Instance);
                if (selectedMissileIdField == null) return false;

                // Get the current selected missile ID
                var missileId = (int)selectedMissileIdField.GetValue(instanceObj);
                if (missileId <= 0)
                {
                    // If no missile is selected, don't try to get a model
                    Console.WriteLine("No missile selected (ID = 0)");
                    return false;
                }

                // Get the Get3DModel method
                var get3DModelMethod = servicesPocType.GetMethod("Get3DModel", new Type[] { typeof(int), typeof(string) });
                if (get3DModelMethod == null) return false;

                // Try to get the model path for the "UN" part
                var modelPathObj = get3DModelMethod.Invoke(instanceObj, new object[] { missileId, "UN" });
                if (modelPathObj == null) return false;

                var modelPath = modelPathObj.ToString();
                if (string.IsNullOrEmpty(modelPath)) return false;

                // Look for AppConfigSvc in a safer way
                string metisDbAssemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MetisDB.dll");
                if (!File.Exists(metisDbAssemblyPath))
                {
                    string alternateMetisDbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"Work\MetisDB\MetisDB\bin\Debug\net6.0\MetisDB.dll");
                    if (File.Exists(alternateMetisDbPath))
                    {
                        metisDbAssemblyPath = alternateMetisDbPath;
                    }
                    else
                    {
                        // If we can't find the assembly, use a hardcoded path
                        string hardcodedDataFolder = @"C:\Work\MetisDB\Data";
                        string hardcodedModelPath = Path.Combine(hardcodedDataFolder, "3DModel", modelPath);

                        if (File.Exists(hardcodedModelPath))
                        {
                            // Update our properties
                            SelectedMissileId = missileId;
                            SelectedMissilePart = "UN";
                            ModelPath = hardcodedModelPath;
                            ModelInfoChanged?.Invoke(null, EventArgs.Empty);
                            return true;
                        }

                        return false;
                    }
                }

                var metisDbAssembly = Assembly.LoadFrom(metisDbAssemblyPath);
                if (metisDbAssembly == null) return false;

                var appConfigSvcType = metisDbAssembly.GetType("MetisDB.AppConfigSvc");
                if (appConfigSvcType == null) return false;

                var appCfgField = appConfigSvcType.GetField("appCfg", BindingFlags.Public | BindingFlags.Static);
                if (appCfgField == null) return false;

                var appCfgObj = appCfgField.GetValue(null);
                if (appCfgObj == null) return false;

                var baseDataFolderProp = appCfgObj.GetType().GetProperty("baseDataFolder");
                if (baseDataFolderProp == null) return false;

                var baseDataFolder = baseDataFolderProp.GetValue(appCfgObj)?.ToString();
                if (string.IsNullOrEmpty(baseDataFolder))
                {
                    // Fallback to a default path
                    baseDataFolder = @"C:\Work\MetisDB\Data";
                }

                // Construct the full model path
                var fullModelPath = Path.Combine(baseDataFolder, "3DModel", modelPath);
                if (!File.Exists(fullModelPath)) return false;

                // Update our properties
                SelectedMissileId = missileId;
                SelectedMissilePart = "UN";
                ModelPath = fullModelPath;

                // Notify listeners
                ModelInfoChanged?.Invoke(null, EventArgs.Empty);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error accessing MetisDB: " + ex.Message);
                return false;
            }
        }
    }
}
