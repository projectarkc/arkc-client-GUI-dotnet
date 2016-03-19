Imports ArkC_Client_GUI_dotnet
Imports System.IO
Imports System.Runtime.Serialization.Json
Module Module1
    Sub Main(ByVal args() As String)
        Dim fsTemp As New System.IO.FileStream(args(0), FileMode.Open)
        Dim ser As New DataContractJsonSerializer(GetType(config))
        Dim cfg As config = Nothing
        Dim success As Boolean = True
        Try
            cfg = ser.ReadObject(fsTemp)
        Finally
            fsTemp.Close()
        End Try
        If cfg Is Nothing Then cfg = New config
        If cfg.Check_Validity() Then
            If System.IO.File.Exists(cfg.executable.Replace("/", "\")) Then
                success = success And firewall.Add_Exception(cfg.executable.Replace("/", "\"), "ArkC Client")
                If cfg.obfs_level = 3 Then
                    success = success And firewall.Add_Exception(cfg.pt_exec.Replace("/", "\"), "MEEK Server")
                End If
            End If
        End If
        If success Then MsgBox("Adding exceptions succeed!", vbOKOnly, "Success")
    End Sub
End Module
