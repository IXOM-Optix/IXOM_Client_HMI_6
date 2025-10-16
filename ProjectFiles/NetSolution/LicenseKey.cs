#region Using directives
using System;
using System.Text;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.DataLogger;
using FTOptix.HMIProject;
using FTOptix.NativeUI;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.Report;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.NetLogic;
using FTOptix.Core;
#endregion

public class LicenseKey : BaseNetLogic
{
    IUAVariable _License_Key;
    IUAVariable _System_Name;
    IUAVariable _Hash_Key;
    IUAVariable _valid;
    IUAVariable _Expires_In;
    IUAVariable _Valid;

    private CancellationTokenSource cancellationTokenSource;
    private Task longRunningTask;

    public override void Start()
    {
        // Create cancellation token source
        cancellationTokenSource = new CancellationTokenSource();
        
        // Start the long-running task that calls VerifyKey every 5 minutes
        longRunningTask = Task.Run(() => RunPeriodicLicenseCheck(cancellationTokenSource.Token));
        
        Log.Info("Key_Gen2", "Long-running license verification task started - executes every 5 minutes");
    }

    public override void Stop()
    {
        // Cancel the long-running task
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
            Log.Info("Key_Gen2", "License verification task cancellation requested");
        }

        // Wait for the task to complete (with timeout)
        if (longRunningTask != null)
        {
            try
            {
                longRunningTask.Wait(TimeSpan.FromSeconds(10));
            }
            catch (AggregateException)
            {
                // Task was cancelled, which is expected
                Log.Info("Key_Gen2", "License verification task stopped");
            }
        }

        // Dispose resources
        cancellationTokenSource?.Dispose();
    }

    private async Task RunPeriodicLicenseCheck(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Call the VerifyKey method
                VerifyKey();

                // Wait for 5 minutes or until cancellation
                await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Task was cancelled - this is expected when stopping
                Log.Info("Key_Gen2", "License verification task cancelled");
                break;
            }
            catch (Exception ex)
            {
                // Log any other exceptions and continue running
                Log.Error("Key_Gen2", $"Error in license verification task: {ex.Message}");
                
                // Wait a shorter time before retrying after an error
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    [ExportMethod]
    

    private static void WriteUInt32BE(byte[] buffer, ref int offset, uint value)
    {
        buffer[offset++] = (byte)((value >> 24) & 0xFF);
        buffer[offset++] = (byte)((value >> 16) & 0xFF);
        buffer[offset++] = (byte)((value >> 8) & 0xFF);
        buffer[offset++] = (byte)(value & 0xFF);
    }

    private static string Base64UrlEncode(byte[] data)
    {
        var s = Convert.ToBase64String(data);
        s = s.Replace('+', '-').Replace('/', '_');
        return s.TrimEnd('=');
    }

    /// <summary>
    /// Verifies a compact short license string and outputs validity and days remaining.
    /// Expects the same compact format produced by the short generator.
    /// Inputs: hashKey, systemName, license.
    /// Outputs: isValid, daysRemaining.
    /// </summary>
    [ExportMethod]
    public void VerifyKey()
    {
            _License_Key = LogicObject.GetVariable("License_Key");
            _System_Name = LogicObject.GetVariable("License_Key/System_Name");
            _Hash_Key = LogicObject.GetVariable("License_Key/Hash_Key");
            _Valid = LogicObject.GetVariable("License_Key/Valid");
            _Expires_In = LogicObject.GetVariable("License_Key/Expires_In");

        bool isValid;
        int daysRemaining;
        string hashKey = _Hash_Key.Value;
        string systemName = _System_Name.Value;
        string license = _License_Key.Value;

        isValid = false;
        daysRemaining = 0;
        try
        {
            if (string.IsNullOrWhiteSpace(hashKey))
            {
                Log.Warning("RuntimeNetLogic1", "hashKey cannot be empty");
                return;
            }
            if (string.IsNullOrWhiteSpace(systemName))
            {
                Log.Warning("RuntimeNetLogic1", "systemName cannot be empty");
                return;
            }
            if (string.IsNullOrWhiteSpace(license))
            {
                Log.Warning("RuntimeNetLogic1", "license cannot be empty");
                return;
            }

            byte[] data;
            try
            {
                data = Base64UrlDecode(license);
            }
            catch (Exception ex)
            {
                Log.Warning("RuntimeNetLogic1", $"License is not valid Base64Url: {ex.Message}");
                return;
            }

            // Minimal structure length check
            if (data.Length < 16)
            {
                Log.Warning("RuntimeNetLogic1", "License too short or malformed");
                return;
            }

            int i = 0;
            byte version = data[i++];
            if (version != 1)
            {
                Log.Warning("RuntimeNetLogic1", $"Unsupported license version: {version}");
                return;
            }

            uint exp = ReadUInt32BE(data, ref i);
            uint iss = ReadUInt32BE(data, ref i);
            byte nameLen = data[i++];

            if (i + nameLen + 6 > data.Length)
            {
                Log.Warning("RuntimeNetLogic1", "License fields are inconsistent");
                _Valid.Value = isValid;
                _Expires_In.Value = daysRemaining;
                return;
            }

            var nameBytes = new byte[nameLen];
            Buffer.BlockCopy(data, i, nameBytes, 0, nameLen);
            i += nameLen;
            string licSystem = Encoding.UTF8.GetString(nameBytes);

            // Extract tag and payload
            int tagOffset = data.Length - 6;
            var payloadLen = tagOffset;
            var payload = new byte[payloadLen];
            Buffer.BlockCopy(data, 0, payload, 0, payloadLen);
            var tag = new byte[6];
            Buffer.BlockCopy(data, tagOffset, tag, 0, 6);

            // Recompute HMAC
            byte[] expectedTag;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(hashKey)))
            {
                var mac = hmac.ComputeHash(payload);
                expectedTag = new byte[6];
                Buffer.BlockCopy(mac, 0, expectedTag, 0, 6);
            }

            if (!ConstantTimeEquals(tag, expectedTag))
            {
                Log.Warning("RuntimeNetLogic1", "License integrity check failed (HMAC mismatch)");
                _Valid.Value = isValid;
                _Expires_In.Value = daysRemaining;
                return;
            }

            if (!string.Equals(licSystem, systemName, StringComparison.Ordinal))
            {
                Log.Warning("RuntimeNetLogic1", $"System name mismatch. Expected '{systemName}', got '{licSystem}'");
                _Valid.Value = isValid;
                _Expires_In.Value = daysRemaining;
                return;
            }

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (iss > (uint)(now + 86400))
            {
                Log.Warning("RuntimeNetLogic1", "Issue time is in the future");
                _Valid.Value = isValid;
                _Expires_In.Value = daysRemaining;
                return;
            }

            long secondsRemaining = (long)exp - now;
            daysRemaining = (int)Math.Ceiling(secondsRemaining / 86400.0);
            if (daysRemaining < 0) daysRemaining = 0;

            isValid = secondsRemaining > 0;
            if (isValid)
            {
                Log.Info("RuntimeNetLogic1", $"License is valid. Days remaining: {daysRemaining}");
            }
            else
            {
                Log.Info("RuntimeNetLogic1", "License has expired (0 days remaining)");
            }
        }
        catch (Exception ex)
        {
            Log.Error("RuntimeNetLogic1", $"Verification failed: {ex.Message}");
            isValid = false;
            daysRemaining = 0;
        }
        _Valid.Value = isValid;
        _Expires_In.Value = daysRemaining;




    }

    private static uint ReadUInt32BE(byte[] buffer, ref int offset)
    {
        uint b0 = (uint)buffer[offset++];
        uint b1 = (uint)buffer[offset++];
        uint b2 = (uint)buffer[offset++];
        uint b3 = (uint)buffer[offset++];
        return (b0 << 24) | (b1 << 16) | (b2 << 8) | b3;
    }

    private static byte[] Base64UrlDecode(string s)
    {
        s = s.Replace('-', '+').Replace('_', '/');
        switch (s.Length % 4)
        {
            case 2: s += "=="; break;
            case 3: s += "="; break;
        }
        return Convert.FromBase64String(s);
    }

    private static bool ConstantTimeEquals(byte[] a, byte[] b)
    {
        if (a == null || b == null || a.Length != b.Length) return false;
        int diff = 0;
        for (int k = 0; k < a.Length; k++) diff |= a[k] ^ b[k];
        return diff == 0;
    }

    /// <summary>
    /// Same as GenerateAndLogShortNewportLicense but also returns the license via an out parameter
    /// so it can be bound as an output in MethodInvocation.
    /// </summary>
    /// <param name="systemName">Name of the system for the license</param>
    /// <param name="hashKey">Hash key for license generation</param>
    /// <param name="expirationDate">Exact date when the license should expire</param>
    /// <param name="license">Output: Base64Url short license string</param>
    [ExportMethod]
    public void GenerateKey(string systemName, string hashKey, DateTime expirationDate, out string license)
    {
        try
        {
            const byte version = 1;
            if (string.IsNullOrWhiteSpace(systemName))
            {
                Log.Error("RuntimeNetLogic1", "systemName cannot be empty");
                license = string.Empty;
                return;
            }
            if (string.IsNullOrWhiteSpace(hashKey))
            {
                Log.Error("RuntimeNetLogic1", "hashKey cannot be empty");
                license = string.Empty;
                return;
            }

            var nowUtc = DateTimeOffset.UtcNow;
            uint iss = (uint)nowUtc.ToUnixTimeSeconds();
            uint exp = (uint)new DateTimeOffset(expirationDate, TimeSpan.Zero).ToUnixTimeSeconds();

            byte[] nameBytes = Encoding.UTF8.GetBytes(systemName);
            if (nameBytes.Length > 255)
            {
                Log.Error("RuntimeNetLogic1", "System name too long for compact license format.");
                license = string.Empty;
                return;
            }

            var payload = new byte[1 + 4 + 4 + 1 + nameBytes.Length];
            int o = 0;
            payload[o++] = version;
            WriteUInt32BE(payload, ref o, exp);
            WriteUInt32BE(payload, ref o, iss);
            payload[o++] = (byte)nameBytes.Length;
            Buffer.BlockCopy(nameBytes, 0, payload, o, nameBytes.Length);
            o += nameBytes.Length;

            byte[] tag;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(hashKey)))
            {
                var mac = hmac.ComputeHash(payload);
                tag = new byte[6];
                Buffer.BlockCopy(mac, 0, tag, 0, tag.Length);
            }

            var final = new byte[payload.Length + tag.Length];
            Buffer.BlockCopy(payload, 0, final, 0, payload.Length);
            Buffer.BlockCopy(tag, 0, final, payload.Length, tag.Length);

            license = Base64UrlEncode(final);
            Log.Info("RuntimeNetLogic1", $"Short license for '{systemName}': {license}");
        }
        catch (Exception ex)
        {
            Log.Error("RuntimeNetLogic1", $"Failed to generate short license: {ex.Message}");
            license = string.Empty;
        }
    }



     
}
