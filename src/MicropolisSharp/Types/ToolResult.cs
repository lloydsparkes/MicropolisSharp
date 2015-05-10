/// <summary>
/// From micropolis.h
/// </summary>

namespace MicropolisSharp.Types
{
    public enum ToolResult
    {
        NoMoney = -2,  ///< User has not enough money for tool.
        NeedBulldoze = -1, ///< Clear the area first.
        Failed = 0, ///< Cannot build here.
        Ok = 1, ///< Build succeeded.
    }
}
