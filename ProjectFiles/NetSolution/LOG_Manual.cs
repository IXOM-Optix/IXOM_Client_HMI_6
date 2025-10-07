#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.DataLogger;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.NativeUI;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.CoreBase;
using FTOptix.Core;
#endregion

public class LOG_Manual : BaseNetLogic
{
    public override void Start()
    {


 


          // Insert code to be executed when the user-defined logic is started
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

    [ExportMethod]
    public void LOG_ShoppingCart(
        string Make,
        string Model,
        string PartNumber,
        string Description_Line_1,
        string Description_Line_2,
        string Description_Line_3)
    {


        /*
                // Get references to the Log variables
                var Log_Make = LogicObject.GetVariable("Values/Make");
                var Log_Model = LogicObject.GetVariable("Values/Model");
                var Log_PartNumber = LogicObject.GetVariable("Values/PartNumber");
                var Log_Description_Line_1 = LogicObject.GetVariable("Values/Description_Line_1");
                var Log_Description_Line_2 = LogicObject.GetVariable("Values/Description_Line_2");
                var Log_Description_Line_3 = LogicObject.GetVariable("Values/Description_Line_3");

                */
        
        var Log_Make = Project.Current.GetVariable("Loggers/ShoppingCart/VariablesToLog/Make");
        var Log_Model = Project.Current.GetVariable("Loggers/ShoppingCart/VariablesToLog/Model");
        var Log_PartNumber = Project.Current.GetVariable("Loggers/ShoppingCart/VariablesToLog/PartNumber");
        var Log_Description_Line_1 = Project.Current.GetVariable("Loggers/ShoppingCart/VariablesToLog/Description_Line_1");
        var Log_Description_Line_2 = Project.Current.GetVariable("Loggers/ShoppingCart/VariablesToLog/Description_Line_2");
        var Log_Description_Line_3 = Project.Current.GetVariable("Loggers/ShoppingCart/VariablesToLog/Description_Line_3");

        // Move values using .Value command
        if (Make != null && Log_Make != null)
        {
            Log_Make.Value = Make;
        }
        else
        {
            Log.Warning("Cannot move _Make value - source or target IUAVariable not found");
        }

        if (Model != null && Log_Model != null)
        {
            Log_Model.Value = Model;
        }
        else
        {
            Log.Warning("Cannot move _Model value - source or target IUAVariable not found");
        }

        if (PartNumber != null && Log_PartNumber != null)
        {
            Log_PartNumber.Value = PartNumber;
        }
        else
        {
            Log.Warning("Cannot move _PartNumber value - source or target IUAVariable not found");
        }

        if (Description_Line_1 != null && Log_Description_Line_1 != null)
        {
            Log_Description_Line_1.Value = Description_Line_1;
        }
        else
        {
            Log.Warning("Cannot move _Description_Line_1 value - source or target IUAVariable not found");
        }
        if (Description_Line_2 != null && Log_Description_Line_2 != null)
        {
            Log_Description_Line_2.Value = Description_Line_2;
        }
        else
        {
            Log.Warning("Cannot move _Description_Line_2 value - source or target IUAVariable not found");
        }

        if (Description_Line_3 != null && Log_Description_Line_3 != null)
        {
            Log_Description_Line_3.Value = Description_Line_3;
        }
        else
        {
            Log.Warning("Cannot move _Description_Line_3 value - source or target IUAVariable not found");
        }

        // Wait 5 seconds
        System.Threading.Thread.Sleep(5000);

        // Call DataLogger Log method
        try
        {
            var dataLogger = Project.Current.Get<FTOptix.DataLogger.DataLogger>("Loggers/ShoppingCart");
            if (dataLogger != null)
            {
                // Call the Log method
                dataLogger.Log();
                Log.Info("Successfully called DataLogger Log method");
            }
            else
            {
                Log.Warning("DataLogger not found at Loggers/ShoppingCart");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Error calling DataLogger Log method: {ex.Message}");
        }

    }

/*
    IUAVariable _Make;
    IUAVariable _Model;
    IUAVariable _PartNumber;
    IUAVariable _Description_Line_1;
    IUAVariable _Description_Line_2;
    IUAVariable _Description_Line_3;



    IUAVariable Log_Make;
    IUAVariable Log_Model;
    IUAVariable Log_PartNumber;
    IUAVariable Log_Description_Line_1;
    IUAVariable Log_Description_Line_2;
    IUAVariable Log_Description_Line_3;

    */
}

