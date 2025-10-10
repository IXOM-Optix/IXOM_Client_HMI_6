#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.NativeUI;
using FTOptix.WebUI;
using FTOptix.Alarm;
using FTOptix.DataLogger;
using FTOptix.EventLogger;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.Report;
using FTOptix.RAEtherNetIP;
using FTOptix.S7TiaProfinet;
using FTOptix.MQTTClient;
using FTOptix.System;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.CommunicationDriver;
using FTOptix.OPCUAClient;
using FTOptix.Core;
#endregion

public class PrepDelete_Newport : BaseNetLogic
{
    // ==================================================
    // DELETION CONFIGURATION - Easy to modify and extend
    // ==================================================
    
    /// <summary>
    /// Configure objects to delete here. Add new paths to this list as needed.
    /// Format: "Category|Path1/Path2/ObjectName"
    /// </summary>
    private readonly List<string> ObjectsToDelete = new List<string>
    {
        // UI Screen Objects
        "UI Screens|UI/Screens/Main/MIEX_Mini",
        "UI Screens|UI/Screens/Main/Magna_Pak",
        "UI Screens|UI/Screens/Main/RHQ",
        
        // Deployment Objects
        "Deployment|Deployment/AB_MIEX_Mini",
        "Deployment|Deployment/AB_MagnaPak",
        "Deployment|Deployment/SIE_MIEX_Mini",
        "Deployment|Deployment/SIE_MagnaPak",
        "Deployment|Deployment/SIE_RHQ",
        
        
        // DataStore Objects
        "DataStores|DataStores/AB_MIEX_Mini",
        "DataStores|DataStores/AB_MagnaPak",
        "DataStores|DataStores/SIE_MIEX_Mini",
        "DataStores|DataStores/SIE_MagnaPak",
        "DataStores|DataStores/RHQ",
        
        // Communication Driver Objects
        "CommDrivers|CommDrivers/RAEtherNet_IPDriver1/AB_MIEX_Mini",
        "CommDrivers|CommDrivers/RAEtherNet_IPDriver1/AB_MagnaPak",
        "CommDrivers|CommDrivers/S7TIAPROFINETDriver1/SIE_MIEX_Mini",
        "CommDrivers|CommDrivers/S7TIAPROFINETDriver1/SIE_MagnaPak",
        "CommDrivers|CommDrivers/S7TIAPROFINETDriver1/SIE_RHQ",
        "CommDrivers|CommDrivers/S7TIAPROFINETDriver1"
        
        // TO ADD MORE OBJECTS: Simply add new lines above following the same format
        // Example: "Category|Path/To/Object"
    };

    // ==================================================
    // PUBLIC METHODS
    // ==================================================

    [ExportMethod]
    public void DeleteAllConfiguredObjects()
    {
        Log.Info("DesignTimeNetLogic1", "=== Starting deletion of configured objects ===");
        
        var results = new Dictionary<string, List<string>>
        {
            ["Deleted"] = new List<string>(),
            ["NotFound"] = new List<string>(),
            ["Errors"] = new List<string>()
        };

        foreach (var objectConfig in ObjectsToDelete)
        {
            var (category, path) = ParseObjectConfig(objectConfig);
            DeleteSingleObject(category, path, results);
        }

        LogDeletionSummary(results);
    }

    [ExportMethod]
    public void DeleteObjectsByCategory(string categoryFilter)
    {
        Log.Info("DesignTimeNetLogic1", $"=== Deleting objects in category: {categoryFilter} ===");
        
        var results = new Dictionary<string, List<string>>
        {
            ["Deleted"] = new List<string>(),
            ["NotFound"] = new List<string>(),
            ["Errors"] = new List<string>()
        };

        var filteredObjects = ObjectsToDelete
            .Where(obj => obj.StartsWith(categoryFilter + "|"))
            .ToList();

        if (!filteredObjects.Any())
        {
            Log.Warning("DesignTimeNetLogic1", $"No objects found for category: {categoryFilter}");
            return;
        }

        foreach (var objectConfig in filteredObjects)
        {
            var (category, path) = ParseObjectConfig(objectConfig);
            DeleteSingleObject(category, path, results);
        }

        LogDeletionSummary(results);
    }

    [ExportMethod]
    public void ListConfiguredObjects()
    {
        Log.Info("DesignTimeNetLogic1", "=== Configured Objects for Deletion ===");
        
        var categories = ObjectsToDelete
            .Select(obj => ParseObjectConfig(obj).Category)
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        foreach (var category in categories)
        {
            Log.Info("DesignTimeNetLogic1", $"\n[{category}]");
            
            var objectsInCategory = ObjectsToDelete
                .Where(obj => obj.StartsWith(category + "|"))
                .Select(obj => ParseObjectConfig(obj).Path)
                .ToList();

            foreach (var path in objectsInCategory)
            {
                Log.Info("DesignTimeNetLogic1", $"  • {path}");
            }
        }
        
        Log.Info("DesignTimeNetLogic1", $"\nTotal objects configured: {ObjectsToDelete.Count}");
    }

    [ExportMethod]
    public void DeleteUIScreensOnly()
    {
        DeleteObjectsByCategory("UI Screens");
    }

    [ExportMethod]
    public void DeleteCommDriversOnly()
    {
        DeleteObjectsByCategory("CommDrivers");
    }

    [ExportMethod]
    public void DeleteDataStoresOnly()
    {
        DeleteObjectsByCategory("DataStores");
    }

    [ExportMethod]
    public void DeleteDeploymentOnly()
    {
        DeleteObjectsByCategory("Deployment");
    }

    // ==================================================
    // PRIVATE HELPER METHODS
    // ==================================================

    private (string Category, string Path) ParseObjectConfig(string config)
    {
        var parts = config.Split('|');
        if (parts.Length != 2)
            throw new ArgumentException($"Invalid object configuration format: {config}");
        
        return (parts[0], parts[1]);
    }

    private void DeleteSingleObject(string category, string path, Dictionary<string, List<string>> results)
    {
        try
        {
            var pathSegments = path.Split('/');
            var targetObject = NavigateToPath(pathSegments);

            if (targetObject != null)
            {
                targetObject.Delete();
                Log.Info("DesignTimeNetLogic1", $"✓ [{category}] Deleted: {path}");
                results["Deleted"].Add($"[{category}] {path}");
            }
            else
            {
                Log.Warning("DesignTimeNetLogic1", $"- [{category}] Not found: {path}");
                results["NotFound"].Add($"[{category}] {path}");
            }
        }
        catch (Exception ex)
        {
            Log.Error("DesignTimeNetLogic1", $"✗ [{category}] Failed to delete {path}: {ex.Message}");
            results["Errors"].Add($"[{category}] {path}: {ex.Message}");
        }
    }

    private IUANode NavigateToPath(string[] pathSegments)
    {
        try
        {
            IUANode currentNode = Project.Current;

            foreach (var segment in pathSegments)
            {
                currentNode = currentNode.Get(segment);
                if (currentNode == null)
                    return null;
            }

            return currentNode;
        }
        catch
        {
            return null;
        }
    }

    private void LogDeletionSummary(Dictionary<string, List<string>> results)
    {
        Log.Info("DesignTimeNetLogic1", "\n=== DELETION SUMMARY ===");
        Log.Info("DesignTimeNetLogic1", $"Successfully deleted: {results["Deleted"].Count}");
        Log.Info("DesignTimeNetLogic1", $"Not found: {results["NotFound"].Count}");
        Log.Info("DesignTimeNetLogic1", $"Errors: {results["Errors"].Count}");
        Log.Info("DesignTimeNetLogic1", $"Total processed: {results["Deleted"].Count + results["NotFound"].Count + results["Errors"].Count}");
        
        if (results["Errors"].Any())
        {
            Log.Info("DesignTimeNetLogic1", "\nERRORS:");
            foreach (var error in results["Errors"])
            {
                Log.Error("DesignTimeNetLogic1", $"  {error}");
            }
        }
    }
}
