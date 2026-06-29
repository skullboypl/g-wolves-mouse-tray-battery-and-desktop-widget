namespace FenrirBatteryTray.Protocol;

internal enum DongleCommand : byte
{
    BatteryLevel = 4,
}

internal enum BatteryStatus
{
    Unknown = 0,
    Discharging = 1,
    Charging = 2,
    Full = 3,
}

internal readonly record struct BatteryReading(int Percent, BatteryStatus Status);

internal static class GwolvesConstants
{
    public const int VendorId = 0x33E4;
    public const int FenrirMaxWirelessPid = 0x3717;
    public const int FenrirMaxWiredPid = 0x3708;
    public const byte FeatureReportId = 0;
    public const int FeatureReportSize = 64;
    public const byte HidAck = 0xA1;
}
