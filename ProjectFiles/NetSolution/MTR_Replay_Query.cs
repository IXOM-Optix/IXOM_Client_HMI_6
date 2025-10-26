#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.SQLiteStore;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.WebUI;
using FTOptix.Store;
using FTOptix.RAEtherNetIP;
using FTOptix.S7TiaProfinet;
using FTOptix.System;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.CommunicationDriver;
using FTOptix.Alarm;
using FTOptix.DataLogger;
using FTOptix.SerialPort;
using FTOptix.UI;
using FTOptix.Core;
using FTOptix.EventLogger;
using FTOptix.AuditSigning;
using FTOptix.OPCUAClient;
using FTOptix.MQTTClient;
using FTOptix.Report;
#endregion

public class MTR_Replay_Query : BaseNetLogic
{
    private Store myStore;
    private object[,] resultSet;
    DateTime dateQuery;
    string itemRtn;

    public override void Start()
    {                                                 
    }
    public override void Stop()
    {
    }
    [ExportMethod]
    public void Motor_Query()
    {
     string PLC = Owner.Owner.Owner.Owner.BrowseName;    // "AB_MagnaPak";
     string Area = Owner.Owner.Owner.BrowseName;         // "Brine";
     string Device_Name = Owner.Owner.BrowseName;        // "P8100";

     myStore = Project.Current.Get<Store>($"DataStores/" + PLC);
        
        // Build SQL Date Time Query String
         dateQuery = DateTime.Parse(Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/MTR/Rewind/Date_Time").Value);
         string sqlFormattedDate = dateQuery.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");

        //Speed_Actual
         //Query Database for Speed_Actual
         string queryString = $"SELECT Speed_Actual FROM "+Device_Name+" WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
         myStore.Query(queryString, out _, out resultSet);
         //Store Speed_Actual from Query
         Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/MTR/Rewind/Speed_Actual").Value = (dynamic)resultSet[0, 0];

        //STS 
         //Query Data Base for STS
         queryString = $"SELECT STS FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
         myStore.Query(queryString, out _, out resultSet);
         //Store STS from Query
        Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/MTR/Rewind/STS").Value = (dynamic)resultSet[0, 0];

    }


     
}
