Imports System.Runtime.InteropServices
Imports Microsoft.Win32
Module ProxyConfig
    Public Const INTERNET_OPTION_SETTINGS_CHANGED As Integer = 39
    Public Const INTERNET_OPTION_REFRESH As Integer = 37
    Public settingsReturn, refreshReturn As Boolean
    Public original_bool As Integer = 0
    Public original_str As String = ""
    <DllImport("wininet.dll")> _
    Public Function InternetSetOption(ByVal hInternet As IntPtr, ByVal dwOption As Integer, ByVal lpBuffer As IntPtr, ByVal dwBufferLength As Integer) As Boolean

    End Function

    Public Function SetProxy(addr As String, port As UInteger) As Boolean
        Try
            Dim reg As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", True)
            original_bool = reg.GetValue("ProxyEnable")
            original_str = reg.GetValue("ProxyServer")
            If original_bool = Nothing Then original_bool = 0
            If original_str = Nothing Then original_str = ""
            reg.SetValue("ProxyEnable", 1)
            reg.SetValue("ProxyServer", addr + ":" + CStr(port))
            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0)
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0)
        Catch
            Return False
        End Try
        Return True
    End Function

    Public Function DisableProxy() As Boolean
        Try
            Dim reg As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", True)
            reg.SetValue("ProxyEnable", original_bool)
            reg.SetValue("ProxyServer", original_str)
            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0)
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0)
        Catch
            Return False
        End Try
        Return True
    End Function
End Module
