Imports System.Runtime.InteropServices
Imports Microsoft.Win32
Module ProxyConfig
    Public Const INTERNET_OPTION_SETTINGS_CHANGED As Integer = 39
    Public Const INTERNET_OPTION_REFRESH As Integer = 37
    Public settingsReturn, refreshReturn As Boolean
    <DllImport("wininet.dll")> _
    Public Function InternetSetOption(ByVal hInternet As IntPtr, ByVal dwOption As Integer, ByVal lpBuffer As IntPtr, ByVal dwBufferLength As Integer) As Boolean

    End Function

    Public Function SetProxy(addr As String, port As UInteger) As Boolean
        Dim reg As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", True)
        reg.SetValue("ProxyEnable", 1)
        reg.SetValue("ProxyServer", addr + ":" + port)
        settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
        refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
    End Function

    Public Function DisableProxy(addr As String, port As UInteger) As Boolean
        Dim reg As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", True)
        reg.SetValue("ProxyEnable", 0)
        reg.DeleteValue("ProxyServer")
        settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
        refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
    End Function
End Module
