namespace Screna.Native
{
    enum GetWindowEnum { Owner = 4 }

    enum SetWindowPositionFlags
    {
        ShowWindow = 0x400,
        NoActivate = 0x0010
    }

    enum WindowStyles : long
    {
        Child = 0x40000000,
        ToolWindow = 0x00000080,
        AppWindow = 0x00040000,
        SizeBox = 0x00040000L
    }

    enum GetWindowLongValue
    {
        Style = -16,
        ExStyle = -20
    }
}