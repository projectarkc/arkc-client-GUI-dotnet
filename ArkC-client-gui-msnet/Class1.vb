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
End Class

