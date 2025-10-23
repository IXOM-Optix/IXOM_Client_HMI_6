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
using System.Linq.Expressions;
#endregion

public class SpinBox_Reset_Upon_License_Invalid : BaseNetLogic
{
    public override void Start()
    {
        var spinBox = Owner as SpinBox;

        if (spinBox != null)
        {
            if (!(bool)spinBox.Enabled)
            {
                spinBox.Value = spinBox.MinValue;
                Log.Info("Reset_Upon_License_Invalid", "License Invalid, SpinBox Value set to Minimum Value");
            }
            else
            {
                Log.Info("Reset_Upon_License_Invalid", "License Valid, SpinBox Value not Altered");
            }
        }
    }
}