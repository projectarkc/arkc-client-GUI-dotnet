Public Class config
    Public Property local_cert As String
    Public Property remote_cert As String
    Public Property local_cert_pub As String
    Public Property control_domain As String
    Public Property local_host As String
    Public Property remote_host As String
    Public Property local_port As UInteger
    Public Property remote_port As UInteger
    Public Property number As Byte
    Public Property obfs4_exec As String
    Public Property obfs_level As Byte
    Public Property debug_ip As String
    Public Property dns_servers As List(Of List(Of Object))
    Public Property executable As String
    Public Property argv As String
    Private ReadOnly Property check_variables As Collection
        Get
            Dim vars As Collection = New Collection
            vars.Add(local_cert)
            vars.Add(remote_cert)
            vars.Add(local_cert_pub)
            vars.Add(control_domain)
            vars.Add(executable)
            vars.Add(argv)
            Return vars
        End Get
    End Property
    Public Function Check_Validity() As Boolean
        If Me.number = 0 Then Return False
        For Each str_var As String In Me.check_variables
            If str_var Is Nothing Then Return False
        Next
        Return True
    End Function
End Class

