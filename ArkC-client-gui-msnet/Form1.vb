Imports System.Web.Script.Serialization
Imports System.IO
Imports System.Text.Encoding
Imports System.Threading
Imports System.Management

Public Class Form1

    Dim Process1 As Process = Nothing

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            Button4.Enabled = True
            TextBox12.Enabled = True
        Else
            Button4.Enabled = False
            TextBox12.Enabled = False
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim result As DialogResult = OpenFileDialog1.ShowDialog()
        OpenFileDialog1.Title = "Executable of Server Public Key"
        If result = Windows.Forms.DialogResult.OK Then
            TextBox4.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim result As DialogResult = OpenFileDialog1.ShowDialog()
        OpenFileDialog1.Title = "Executable of Client Private Key"
        If result = Windows.Forms.DialogResult.OK Then
            TextBox5.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim result As DialogResult = OpenFileDialog1.ShowDialog()
        OpenFileDialog1.Title = "Executable of Client Public Key"
        If result = Windows.Forms.DialogResult.OK Then
            TextBox6.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim result As DialogResult = OpenFileDialog1.ShowDialog()
        OpenFileDialog1.Title = "Executable of obfs4proxy"
        If result = Windows.Forms.DialogResult.OK Then
            TextBox12.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        RichTextBox1.Text = ""
        RichTextBox2.Text = ""
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        For Each tb As TextBox In Me.Controls
            tb.Text = ""
        Next
        TextBox9.Text = "[[""114.114.114.114"", 53], [""8.8.8.8"", 53]]]"
        TextBox10.Text = "0.0.0.0"
        TextBox10.Text = "127.0.0.1"
        TextBox7.Text = "8000"
        TextBox8.Text = "18001"
        CheckBox1.Checked = False
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Execute()
    End Sub

    Private Sub Execute()
        Dim Configuration As New config
        Dim serializer As New JavaScriptSerializer()
        Dim sTempFileName As String = System.IO.Path.GetTempFileName()
        Dim fsTemp As New System.IO.FileStream(sTempFileName, IO.FileMode.Create)
        If IsValidPort(TextBox7.Text) And IsValidPort(TextBox8.Text) Then
            Configuration.control_domain = TextBox3.Text
            Configuration.local_cert = TextBox6.Text.Replace("\", "/")
            Configuration.local_cert_pub = TextBox5.Text.Replace("\", "/")
            Configuration.remote_cert = TextBox4.Text.Replace("\", "/")
            Configuration.remote_port = CInt(TextBox7.Text)
            Configuration.local_port = CInt(TextBox8.Text)
            Configuration.local_port = CInt(ComboBox1.SelectedItem.ToString)
            Configuration.remote_host = TextBox10.Text
            Configuration.local_host = TextBox11.Text
            Configuration.obfs4_exec = TextBox12.Text
            Configuration.debug_ip = TextBox14.Text
            If CheckBox1.Checked Then Configuration.obfs_level = 3 Else Configuration.obfs_level = 0
            Dim dns = serializer.Deserialize(Of List(Of List(Of Object)))(TextBox9.Text)
            Configuration.dns_servers = dns
            Dim serializedResult = serializer.Serialize(Configuration)
            fsTemp.Write(UTF8.GetBytes(serializedResult.ToCharArray()), 0, UTF8.GetByteCount(serializedResult.ToCharArray()))
            fsTemp.Close()
            If Process1 IsNot Nothing Then Process1.Dispose()
            Process1 = New Process
            AddHandler Process1.OutputDataReceived, AddressOf Process1_OutputDataReceived_Process
            With Process1
                .StartInfo.UseShellExecute = False
                .StartInfo.RedirectStandardOutput = True
                .StartInfo.CreateNoWindow = True
                .StartInfo.FileName = TextBox15.Text
                .StartInfo.Arguments = " " & TextBox13.Text & " -v -c """ & sTempFileName & """"

                .Start()
            End With
            Dim runThread = New Thread(AddressOf Process1_starting)
            runThread.Start()
        Else
            MsgBox("Port must be valid integer 1~65535.", MsgBoxStyle.Exclamation, "Error")
        End If
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        Dim result As DialogResult = OpenFileDialog1.ShowDialog()
        OpenFileDialog1.Title = "Executable of ArkC Client"
        If result = Windows.Forms.DialogResult.OK Then
            TextBox15.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Try
            If Not (Process1.HasExited) Then killtree(Process1.Id)
        Catch
        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not (System.IO.File.Exists(Application.StartupPath & "/arkc-client.exe")) Then
            MsgBox("ArkC client executable not found in default directory. Please set manually.", MsgBoxStyle.Information, "Loading Executable")
            OpenFileDialog1.Title = "Executable of ArkC Client"
            Dim result As DialogResult = OpenFileDialog1.ShowDialog()
            If result = Windows.Forms.DialogResult.OK Then
                TextBox15.Text = OpenFileDialog1.FileName
            End If
        End If
        ComboBox1.SelectedIndex = 4
        Form1.CheckForIllegalCrossThreadCalls = False
    End Sub

    Private Function IsValidPort(input As String) As Boolean
        If IsNumeric(input) And CInt(input) <= 65535 And CInt(input) >= 1 Then
            Return True
        Else
            Return False
        End If
    End Function

    'Private Sub Process1_Exited(sender As Object, e As EventArgs)
    '    RichTextBox1.Text = RichTextBox1.Text + vbCrLf + "Execution Terminated" + vbCrLf
    'End Sub

    Private Sub Process1_starting()
        Try
            Process1.CancelOutputRead()
        Catch ex As Exception

        End Try
        Process1.BeginOutputReadLine()
        Process1.WaitForExit()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Stop_exec()
    End Sub

    Private Sub Stop_exec()
        If Process1 IsNot Nothing Then
            Try
                Process1.CancelOutputRead()
                If Not (Process1.HasExited) Then
                    killtree(Process1.Id)
                End If
            Catch
            End Try
            Process1.Dispose()
            Process1 = Nothing
            RichTextBox1.Text = RichTextBox1.Text + vbCrLf + "Execution Terminated" + vbCrLf
        End If
    End Sub

    Private Sub Process1_OutputDataReceived_Process(sender As Object, e As DataReceivedEventArgs)
        Try
            RichTextBox1.Text += e.Data + vbCrLf
            RichTextBox1.SelectionStart = RichTextBox1.Text.Length
            RichTextBox1.ScrollToCaret()
            If InStr(e.Data, "INFO") = 0 And InStr(e.Data, "DEBUG") = 0 Then
                RichTextBox2.Text += e.Data + vbCrLf
                RichTextBox2.SelectionStart = RichTextBox2.Text.Length
                RichTextBox1.ScrollToCaret()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub killtree(myId As Integer)
        Dim selectQuery As SelectQuery = New SelectQuery("Win32_Process")
        Dim searcher As New ManagementObjectSearcher(selectQuery)
        Dim aProcess As Process
        Dim flag As Boolean = False
        For Each proc As ManagementObject In searcher.Get
            If proc("ParentProcessId") = myId Or proc("ProcessId") = myId Then
                aProcess = System.Diagnostics.Process.GetProcessById(Int(proc("ProcessId")))
                aProcess.Kill()
            End If
        Next
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Execute()
        Stop_exec()
    End Sub
End Class
