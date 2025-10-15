#region Using directives
using System;
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

public class Replay_LVL : BaseNetLogic
{

  private Store myStore;
  private object[,] resultSet;
  DateTime dateQuery;
  string itemRtn;




    public override void Start()
    {
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
        string Device_Name = _Device_Name.Value;        // "LT_6100";

        myStore = Project.Current.Get<Store>($"DataStores/" + PLC);

        _STS = LogicObject.GetVariable("Replay/STS");
        _Percent = LogicObject.GetVariable("Replay/Percent");
        _Rate_of_Change = LogicObject.GetVariable("Replay/Rate_of_Change");

    // Build SQL Date Time Query String
        dateQuery = DateTime.Parse(_ReplayTime.Value);
     string sqlFormattedDate = dateQuery.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");

    //Percent
     //Query Percent from Value
     string queryString = $"SELECT Percent FROM "+Device_Name+" WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
        myStore.Query(queryString, out _, out resultSet);
     //Store Percent from Query
      _Percent.Value = (dynamic)resultSet[0, 0];

    //Rate_of_Change
     //Query Date Base for Rate_of_Change        
     queryString = $"SELECT Rate_of_Change FROM "+Device_Name+" WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
        myStore.Query(queryString, out _, out resultSet);
     //Store Rate_of_Change from Query 
      _Rate_of_Change.Value = (dynamic)resultSet[0, 0];   
        
    //STS
     //Query Data Base for STS    
     queryString = $"SELECT STS FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
        myStore.Query(queryString, out _, out resultSet);
     //Store STS from Query
      _STS.Value = (dynamic)resultSet[0, 0];
    
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
    IUAVariable _Percent;
    IUAVariable _Rate_of_Change;
    
}
