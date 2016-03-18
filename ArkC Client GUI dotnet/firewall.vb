Imports NetFwTypeLib

Public Module firewall
    Private Const PROGID_AUTHORIZED_APPLICATION As String = "HNetCfg.FwAuthorizedApplication"
    ' Provides access to the firewall settings for a computer.
    Public Function GetFwMgr() As NetFwTypeLib.INetFwMgr
        Dim oINetFwMgr As NetFwTypeLib.INetFwMgr
        Dim NetFwMgrObject As Object
        Dim NetFwMgrType As Type

        ' Use the COM CLSID to get the associated .NET System.Type
        NetFwMgrType = Type.GetTypeFromCLSID( _
         New Guid("{304CE942-6E39-40D8-943A-B913C40C9CD4}"))

        ' Create an instance of the object
        NetFwMgrObject = Activator.CreateInstance(NetFwMgrType)
        oINetFwMgr = NetFwMgrObject

        Return oINetFwMgr
    End Function

    ' Provides access to the firewall settings profile.
    Public Function GetProfile() As NetFwTypeLib.INetFwProfile

        Dim oINetPolicy As NetFwTypeLib.INetFwPolicy
        Dim oINetFwMgr As NetFwTypeLib.INetFwMgr

        oINetFwMgr = GetFwMgr()

        oINetPolicy = oINetFwMgr.LocalPolicy
        Return oINetPolicy.CurrentProfile

    End Function


    Public Function Add_Exception(dir As String, title As String, Optional manager As INetFwMgr = Nothing) As Boolean
        Dim tp As Type = Type.GetTypeFromProgID(PROGID_AUTHORIZED_APPLICATION)
        Dim auth As NetFwTypeLib.INetFwAuthorizedApplication = Activator.CreateInstance(tp)
        auth.Name = title
        auth.ProcessImageFileName = dir
        auth.Scope = NET_FW_SCOPE_.NET_FW_SCOPE_ALL
        auth.IpVersion = NET_FW_IP_VERSION_.NET_FW_IP_VERSION_ANY
        auth.Enabled = True
        If manager Is Nothing Then manager = GetFwMgr()
        Try
            manager.LocalPolicy.CurrentProfile.AuthorizedApplications.Add(auth)
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Public Function Run_Add() As Boolean
        Dim proc As New ProcessStartInfo
        proc.UseShellExecute = True
        proc.WorkingDirectory = Environment.CurrentDirectory
        proc.FileName = Application.StartupPath + "\FwAddException.exe"
        proc.Verb = "runas"
        Try
            Process.Start(proc)
            Return True
        Catch
            MsgBox("Fail to add Windows Firewall exceptions.", vbExclamation, "Add Exceptions failed.")
            Return False
        End Try
    End Function

    Public Function Check_Exception_Exec(dir As String) As Boolean
        Dim oINetPolicy As NetFwTypeLib.INetFwProfile = GetProfile()
        If Not (oINetPolicy.FirewallEnabled) Then Return True
        For Each app As INetFwAuthorizedApplication In oINetPolicy.AuthorizedApplications
            If app.ProcessImageFileName = dir And app.Enabled Then
                Return True
            End If
        Next
        Return False
    End Function
End Module


