#region Using directives
using System;
using System.Threading;
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
using FTOptix.UI;
using FTOptix.OPCUAClient;
using FTOptix.Core;
#endregion

public class Replay_SEQ : BaseNetLogic
{

  private Store myStore;
  private object[,] resultSet;
  DateTime dateQuery;
  string itemRtn;




    public override void Start()
    {
        // Add 100ms delay before any other code is executed
        //had to add to give time for variables to initialize
        Thread.Sleep(100);
        
        // Insert code to be executed when the user-defined logic is started
        _PLC = LogicObject.GetVariable("PLC");
        _Area = LogicObject.GetVariable("Area");
        _Device_Name = LogicObject.GetVariable("Device_Name");
        _ReplayTime = LogicObject.GetVariable("Replay/Time");
        _ReplayEnabled = LogicObject.GetVariable("Replay");

        _ReplayTime.VariableChange += ReplayTime_VariableChange;

        // Check if replay is enabled and call ReplayTime_VariableChange if true
        if (_ReplayEnabled != null && (bool)_ReplayEnabled.Value == true)
        {
            // Call ReplayTime_VariableChange to process current ReplayTime value
            ReplayTime_VariableChange(_ReplayTime, new VariableChangeEventArgs(_ReplayTime, _ReplayTime.Value, _ReplayTime.Value, null, 0));
        }

    }

    private void ReplayTime_VariableChange(object sender, VariableChangeEventArgs e)
      {
        string PLC = _PLC.Value;    // "AB_MagnaPak";
        string Area = _Area.Value;         // "Regen_1";
        string Device_Name = _Device_Name.Value;        // "CT_6100";

        myStore = Project.Current.Get<Store>($"DataStores/" + PLC);

        _STS = LogicObject.GetVariable("Replay/STS");
        _Step_Number = LogicObject.GetVariable("Replay/Step_Number");
        _Step_Time_Elapsed = LogicObject.GetVariable("Replay/Step_Time_Elapsed");
        _Step_Time_Remaining = LogicObject.GetVariable("Replay/Step_Time_Remaining");
        _Step_Time_Elapsed_Units = LogicObject.GetVariable("Replay/Step_Time_Elapsed_Units");
        _Step_Time_Remaining_Units = LogicObject.GetVariable("Replay/Step_Time_Remaining_Units");
        _Last_Pause_Reason = LogicObject.GetVariable("Replay/Last_Pause_Reason");
        _Test_1_Value = LogicObject.GetVariable("Replay/Test_1_Value");
        _Test_1_Compare = LogicObject.GetVariable("Replay/Test_1_Compare");
        _Test_1_Setpoint = LogicObject.GetVariable("Replay/Test_1_Setpoint");
        _Test_1_Units = LogicObject.GetVariable("Replay/Test_1_Units");
        _Test_2_Value = LogicObject.GetVariable("Replay/Test_2_Value");
        _Test_2_Compare = LogicObject.GetVariable("Replay/Test_2_Compare");
        _Test_2_Setpoint = LogicObject.GetVariable("Replay/Test_2_Setpoint");
        _Test_2_Units = LogicObject.GetVariable("Replay/Test_2_Units");
        _Test_3_Value = LogicObject.GetVariable("Replay/Test_3_Value");
        _Test_3_Compare = LogicObject.GetVariable("Replay/Test_3_Compare");
        _Test_3_Setpoint = LogicObject.GetVariable("Replay/Test_3_Setpoint");
        _Test_3_Units = LogicObject.GetVariable("Replay/Test_3_Units");
    




    // Build SQL Date Time Query String
        dateQuery = DateTime.Parse(_ReplayTime.Value);
        string sqlFormattedDate = dateQuery.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");


    //Periodic
         //Query Data Base for Step_Time_Elapsed
         string queryString = $"SELECT Step_Time_Elapsed FROM "+Device_Name+" WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
         myStore.Query(queryString, out _, out resultSet);
         //Store Value from Query
         _Step_Time_Elapsed.Value = (dynamic)resultSet[0, 0];

         //Query Data Base for Step_Time_Remaining
         queryString = $"SELECT Step_Time_Remaining FROM "+Device_Name+" WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
         myStore.Query(queryString, out _, out resultSet);
         //Store Value from Query
         _Step_Time_Remaining.Value = (dynamic)resultSet[0, 0];
       
         //Query Data Base for TEST_1_Value
            queryString = $"SELECT TEST_1_Value FROM "+Device_Name+" WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);
            //Store Value from Query
            _Test_1_Value.Value = (dynamic)resultSet[0, 0];

         //Query Data Base for TEST_2_Value
            queryString = $"SELECT TEST_2_Value FROM "+Device_Name+" WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);
            //Store Value from Query
            _Test_2_Value.Value = (dynamic)resultSet[0, 0];
       
         //Query Data Base for TEST_3_Value
            queryString = $"SELECT TEST_3_Value FROM "+Device_Name+" WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);
            //Store Value from Query
            _Test_3_Value.Value = (dynamic)resultSet[0, 0];


    //Change in Value
         //Query Data Base for STS
         queryString = $"SELECT STS FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
         myStore.Query(queryString, out _, out resultSet);                          
         //Store STS from Query
         _STS.Value = (dynamic)resultSet[0, 0];

         //Query Data Base for Step_Number
         queryString = $"SELECT Step_Number FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
         myStore.Query(queryString, out _, out resultSet);                          
         //Store STS from Query
         _Step_Number.Value = (dynamic)resultSet[0, 0];

         //Query Data Base for Step_Time_Elapsed_Units
         queryString = $"SELECT Step_Time_Elapsed_Units FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
         myStore.Query(queryString, out _, out resultSet);                          
         //Store STS from Query
         _Step_Time_Elapsed_Units.Value = (dynamic)resultSet[0, 0];

         //Query Data Base for Step_Time_Remaining_Units
         queryString = $"SELECT Step_Time_Remaining_Units FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
         myStore.Query(queryString, out _, out resultSet);                          
         //Store STS from Query
         _Step_Time_Remaining_Units.Value = (dynamic)resultSet[0, 0];

         //Query Data Base for Last_Pause_Reason
         queryString = $"SELECT Last_Pause_Reason FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
         myStore.Query(queryString, out _, out resultSet);                          
         //Store STS from Query
         _Last_Pause_Reason.Value = (dynamic)resultSet[0, 0];

        //Query Data Base for Test_1 //
         //Query Data Base for Test_1_Compare
            queryString = $"SELECT Test_1_Compare FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);                          
            //Store STS from Query
            _Test_1_Compare.Value = (dynamic)resultSet[0, 0];

            //Query Data Base for Test_1_Setpoint
            queryString = $"SELECT Test_1_Setpoint FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);                          
            //Store STS from Query
            _Test_1_Setpoint.Value = (dynamic)resultSet[0, 0];

            //Query Data Base for Test_1_Units
            queryString = $"SELECT Test_1_Units FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);                          
             //Store STS from Query
            _Test_1_Units.Value = (dynamic)resultSet[0, 0];


        //Query Data Base for Test_2 //
            //Query Data Base for Test_2_Compare
            queryString = $"SELECT Test_2_Compare FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);                          
            //Store STS from Query
            _Test_2_Compare.Value = (dynamic)resultSet[0, 0];

            //Query Data Base for Test_2_Setpoint
            queryString = $"SELECT Test_2_Setpoint FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);                          
            //Store STS from Query
            _Test_2_Setpoint.Value = (dynamic)resultSet[0, 0];

            //Query Data Base for Test_2_Units
            queryString = $"SELECT Test_2_Units FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);                          
            //Store STS from Query
            _Test_2_Units.Value = (dynamic)resultSet[0, 0];


        //Query Data Base for Test_3 //
            //Query Data Base for Test_3_Compare
            queryString = $"SELECT Test_3_Compare FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);                          
            //Store STS from Query
            _Test_3_Compare.Value = (dynamic)resultSet[0, 0];

            //Query Data Base for Test_3_Setpoint
            queryString = $"SELECT Test_3_Setpoint FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);                          
            //Store STS from Query
            _Test_3_Setpoint.Value = (dynamic)resultSet[0, 0];

            //Query Data Base for Test_3_Units
            queryString = $"SELECT Test_3_Units FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);                          
            //Store STS from Query
            _Test_3_Units.Value = (dynamic)resultSet[0, 0];


    
    }

    public override void Stop()
    {
          _ReplayTime.VariableChange -= ReplayTime_VariableChange;
    }

    IUAVariable _PLC;
    IUAVariable _Area;
    IUAVariable _Device_Name;
    IUAVariable _ReplayTime;
    IUAVariable _ReplayEnabled;
    IUAVariable _STS;
    IUAVariable _Step_Time_Elapsed;
    IUAVariable _Step_Time_Elapsed_Units;
    IUAVariable _Step_Time_Remaining;
    IUAVariable _Step_Time_Remaining_Units;
    IUAVariable _Step_Number;
    IUAVariable _Test_1_Value;
    IUAVariable _Test_1_Compare;
    IUAVariable _Test_1_Setpoint;
    IUAVariable _Test_1_Units;
    IUAVariable _Test_2_Value;
    IUAVariable _Test_2_Compare;
    IUAVariable _Test_2_Setpoint;
    IUAVariable _Test_2_Units;
    IUAVariable _Test_3_Value;
    IUAVariable _Test_3_Compare;
    IUAVariable _Test_3_Setpoint;
    IUAVariable _Test_3_Units;
    IUAVariable _Last_Pause_Reason;


    
}
