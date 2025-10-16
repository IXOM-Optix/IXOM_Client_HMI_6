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
        string Description_Line_3,
        string Quantity,
        string Status)
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
        var Log_Quantity = Project.Current.GetVariable("Loggers/ShoppingCart/VariablesToLog/Quantity");
        var Log_Status = Project.Current.GetVariable("Loggers/ShoppingCart/VariablesToLog/Status");

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

        if (Quantity != null && Log_Quantity != null)
        {
            Log_Quantity.Value = Quantity;
        }
        else
        {
            Log.Warning("Cannot move _Quantity value - source or target IUAVariable not found");
        }

        if (Status != null && Log_Status != null)
        {
            Log_Status.Value = Status;
        }
        else
        {
            Log.Warning("Cannot move _Status value - source or target IUAVariable not found");
        }


        // Wait 1 seconds
        System.Threading.Thread.Sleep(1000);

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
    













[ExportMethod]
    public void LOG_Maintenance(
        string Operator,
        string Device_Name,
        string Action,
        string Notes,
        string Hours,
        string Quantity,
        string Total
        )
    {



        
        var Log_Operator = Project.Current.GetVariable("Loggers/Maintenance_Log/VariablesToLog/Operator");
        var Log_Device_Name = Project.Current.GetVariable("Loggers/Maintenance_Log/VariablesToLog/Device_Name");
        var Log_Action = Project.Current.GetVariable("Loggers/Maintenance_Log/VariablesToLog/Action");
        var Log_Notes = Project.Current.GetVariable("Loggers/Maintenance_Log/VariablesToLog/Notes");
        var Log_Hours = Project.Current.GetVariable("Loggers/Maintenance_Log/VariablesToLog/Hours");
        var Log_Quantity = Project.Current.GetVariable("Loggers/Maintenance_Log/VariablesToLog/Quantity");
        var Log_Total = Project.Current.GetVariable("Loggers/Maintenance_Log/VariablesToLog/Total");

        // Move values using .Value command
        if (Operator != null && Log_Operator != null)
        {
            Log_Operator.Value = Operator;
        }
        else
        {
            Log.Warning("Cannot move _Operator value - source or target IUAVariable not found");
        }

        if (Device_Name != null && Log_Device_Name != null)
        {
            Log_Device_Name.Value = Device_Name;
        }
        else
        {
            Log.Warning("Cannot move _Device_Name value - source or target IUAVariable not found");
        }

        if (Action != null && Log_Action != null)
        {
            Log_Action.Value = Action;
        }
        else
        {
            Log.Warning("Cannot move _Action value - source or target IUAVariable not found");
        }

        if (Notes != null && Log_Notes != null)
        {
            Log_Notes.Value = Notes;
        }
        else
        {
            Log.Warning("Cannot move _Notes value - source or target IUAVariable not found");
        }

        if (Hours != null && Log_Hours != null)
        {
            Log_Hours.Value = Hours;
        }
        else
        {
            Log.Warning("Cannot move _Hours value - source or target IUAVariable not found");
        }

        if (Quantity != null && Log_Quantity != null)
        {
            Log_Quantity.Value = Quantity;
        }
        else
        {
            Log.Warning("Cannot move _Quantity value - source or target IUAVariable not found");
        }

        if (Total != null && Log_Total != null)
        {
            Log_Total.Value = Total;
        }
        else
        {
            Log.Warning("Cannot move _Total value - source or target IUAVariable not found");
        }
      

        // Wait 1 seconds
        System.Threading.Thread.Sleep(1000);

        // Call DataLogger Log method
        try
        {
            var dataLogger = Project.Current.Get<FTOptix.DataLogger.DataLogger>("Loggers/Maintenance_Log");
            if (dataLogger != null)
            {
                // Call the Log method
                dataLogger.Log();
                Log.Info("Successfully called DataLogger Log method");
            }
            else
            {
                Log.Warning("DataLogger not found at Loggers/Maintenance_Log");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Error calling DataLogger Log method: {ex.Message}");
        }

    }




















}

