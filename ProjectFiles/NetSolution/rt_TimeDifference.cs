#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.NativeUI;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.Report;
using FTOptix.RAEtherNetIP;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.Alarm;
using FTOptix.CommunicationDriver;
using FTOptix.Core;
using FTOptix.WebUI;
using FTOptix.EventLogger;
using System.Globalization;
using FTOptix.DataLogger;
using FTOptix.Recipe;
using FTOptix.AuditSigning;
#endregion

public class rt_TimeDifference : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        _FromDate = LogicObject.GetVariable("FromDate");
        _ToDate = LogicObject.GetVariable("ToDate");
        _TimeDiff = LogicObject.GetVariable("TimeDiff");
        _FromString = LogicObject.GetVariable("FromString");
        _ToString = LogicObject.GetVariable("ToString");
        _StringFormat = LogicObject.GetVariable("StringFormat");

        _FromString.Value = alternateFormat(_FromDate.Value, _StringFormat.Value);
        _ToString.Value = alternateFormat(_ToDate.Value, _StringFormat.Value);
        TimeSpan duration = (DateTime)_ToDate.Value - (DateTime)_FromDate.Value;
        _TimeDiff.Value = duration.TotalMilliseconds;

        _FromDate.VariableChange += FromDate_VariableChange;
        _ToDate.VariableChange += ToDate_VariableChange;
    }

    private void ToDate_VariableChange(object sender, VariableChangeEventArgs e)
    {
        _FromString.Value = alternateFormat(_FromDate.Value, _StringFormat.Value);
        _ToString.Value = alternateFormat(_ToDate.Value, _StringFormat.Value);
        TimeSpan duration = (DateTime)_ToDate.Value - (DateTime)_FromDate.Value;
        _TimeDiff.Value = duration.TotalMilliseconds;
    }

    private void FromDate_VariableChange(object sender, VariableChangeEventArgs e)
    {
        _FromString.Value = alternateFormat(_FromDate.Value, _StringFormat.Value);
        _ToString.Value = alternateFormat(_ToDate.Value, _StringFormat.Value);
        TimeSpan duration = (DateTime)_ToDate.Value - (DateTime)_FromDate.Value;
        _TimeDiff.Value = duration.TotalMilliseconds;
    }
    private String alternateFormat(DateTime date, String format)
    {
        CultureInfo ci = CultureInfo.InvariantCulture;
        string newFormat = date.ToString(format, ci);
        return newFormat;
    }
    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
        _FromDate.VariableChange -= FromDate_VariableChange;
        _ToDate.VariableChange -= ToDate_VariableChange;
    }
    IUAVariable _FromDate;
    IUAVariable _ToDate;
    IUAVariable _TimeDiff;
    IUAVariable _FromString;
    IUAVariable _ToString;
    IUAVariable _StringFormat;

    [ExportMethod]
    public void Method1()
    {
        // Insert code to be executed by the method
    }
}

