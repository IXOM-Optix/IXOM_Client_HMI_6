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

public class Set_Part_info : BaseNetLogic
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
    public void CopyMakeFromStartValuesToValues(string _Make, string _Model, string _PartNumber, string _Description_Line_1, string _Description_Line_2, string _Description_Line_3)
    {
        // Read the variables from Start_Values in the logic object
        /*
         _Make = LogicObject.GetVariable("Start_Values/Make");
         _Model = LogicObject.GetVariable("Start_Values/Model");
         _PartNumber = LogicObject.GetVariable("Start_Values/PartNumber");
         _Description_Line_1 = LogicObject.GetVariable("Start_Values/Description_Line_1");
         _Description_Line_2 = LogicObject.GetVariable("Start_Values/Description_Line_2");
         _Description_Line_3 = LogicObject.GetVariable("Start_Values/Description_Line_3");
         */
       

        // Get references to the Log variables
         var Log_Make = LogicObject.GetVariable("Values/Make");
         var Log_Model = LogicObject.GetVariable("Values/Model");
         var Log_PartNumber = LogicObject.GetVariable("Values/PartNumber");
         var Log_Description_Line_1 = LogicObject.GetVariable("Values/Description_Line_1");
         var Log_Description_Line_2 = LogicObject.GetVariable("Values/Description_Line_2");
         var Log_Description_Line_3 = LogicObject.GetVariable("Values/Description_Line_3");

        // Move values using .Value command
        if (_Make != null && Log_Make != null)
        {
            Log_Make.Value = _Make;
        }
        else
        {
            Log.Warning("Cannot move _Make value - source or target IUAVariable not found");
        }

        if (_Model != null && Log_Model != null)
        {
            Log_Model.Value = _Model;
        }
        else
        {
            Log.Warning("Cannot move _Model value - source or target IUAVariable not found");
        }

        if (_PartNumber != null && Log_PartNumber != null)
        {
            Log_PartNumber.Value = _PartNumber;
        }
        else
        {
            Log.Warning("Cannot move _PartNumber value - source or target IUAVariable not found");
        }

        if (_Description_Line_1 != null && Log_Description_Line_1 != null)
        {
            Log_Description_Line_1.Value = _Description_Line_1;
        }
        else
        {
            Log.Warning("Cannot move _Description_Line_1 value - source or target IUAVariable not found");
        }
        if (_Description_Line_2 != null && Log_Description_Line_2 != null)
        {
            Log_Description_Line_2.Value = _Description_Line_2;
        }
        else
        {
            Log.Warning("Cannot move _Description_Line_2 value - source or target IUAVariable not found");
        }

        if (_Description_Line_3 != null && Log_Description_Line_3 != null)
        {
            Log_Description_Line_3.Value = _Description_Line_3;
        }
        else
        {
            Log.Warning("Cannot move _Description_Line_3 value - source or target IUAVariable not found");
        }

        // Wait 5 seconds
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





/*
    IUAVariable Log_Make;
    IUAVariable Log_Model;
    IUAVariable Log_PartNumber;
    IUAVariable Log_Description_Line_1;
    IUAVariable Log_Description_Line_2;
    IUAVariable Log_Description_Line_3;
    */

}
