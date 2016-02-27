Imports System.Web.Script.Serialization
Imports System.IO
Imports System.Text.Encoding
Imports System.Threading
Imports System.Management

Public Class Form1

    Private Property exec_path As String
    Private Property argv As String

    Dim Process1 As Process = Nothing
    Dim Process2 As Process = Nothing

    Private Sub Clean()
        RichTextBox1.Text = ""
        RichTextBox2.Text = ""
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Clean()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Clean()
        Execute()
    End Sub

    Private Sub Execute()
        If Process1 IsNot Nothing Then Process1.Dispose()
        Process1 = New Process
        AddHandler Process1.OutputDataReceived, AddressOf Process1_OutputDataReceived_Process
        With Process1
            .StartInfo.UseShellExecute = False
            .StartInfo.RedirectStandardOutput = True
            .StartInfo.CreateNoWindow = True
            .StartInfo.FileName = Me.exec_path
            .StartInfo.Arguments = " -v -c """ & Application.LocalUserAppDataPath + "\client.json" & """ " & Me.argv
            .Start()
            ToolStripStatusLabel1.Text = Me.exec_path + Me.argv
            ToolStripStatusLabel2.Text = "Running"
        End With
        Dim runThread = New Thread(AddressOf Process1_starting)
        runThread.Start()
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Try
            If Not (Process1.HasExited) Then killtree(Process1.Id)
        Catch
        End Try
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Check_config()
    End Sub

    Private Sub Check_config()
        If System.IO.File.Exists(Application.LocalUserAppDataPath + "\client.json") Then
            Load_config()
        Else
            Form2.ShowDialog()
        End If
    End Sub

    Public Sub Load_config()
        Dim serializer As New JavaScriptSerializer()
        Dim fsTemp As New System.IO.StreamReader(Application.LocalUserAppDataPath + "\client.json", IO.FileMode.Open)
        Dim cfg As config
        Dim contents As String
        contents = fsTemp.ReadToEnd()
        fsTemp.Close()
        cfg = serializer.Deserialize(Of config)(contents)
        Me.exec_path = cfg.executable
        Me.argv = " -v -c """ & Application.LocalUserAppDataPath & "\client.json" & """ " & cfg.argv
    End Sub

    Private Sub Process1_starting()
        Try
            Process1.CancelOutputRead()
        Catch ex As Exception

        End Try
        Process1.BeginOutputReadLine()
        Process1.WaitForExit()
    End Sub

    Private Sub Process2_starting()
        Try
            Process2.CancelOutputRead()
        Catch ex As Exception

        End Try
        Process2.BeginOutputReadLine()
        Process2.WaitForExit()
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
            ToolStripStatusLabel1.Text = "Using executable: " + Me.exec_path
            ToolStripStatusLabel2.Text = "Not running"
        End If
    End Sub

    Private Sub Process1_OutputDataReceived_Process(sender As Object, e As DataReceivedEventArgs)
        Try
            RichTextBox1.Text += e.Data + vbCrLf
            RichTextBox1.SelectionStart = RichTextBox1.Text.Length
            RichTextBox1.ScrollToCaret()
            If InStr(e.Data, "ERROR") <> 0 Or InStr(e.Data, "CRITICAL") <> 0 Or InStr(e.Data, "WARNING") <> 0 Then
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


    'TODO: Capture end of execution
    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        If Process2 IsNot Nothing Then Process2.Dispose()
        Process2 = New Process
        AddHandler Process2.OutputDataReceived, AddressOf Process1_OutputDataReceived_Process
        With Process2
            .StartInfo.UseShellExecute = False
            .StartInfo.RedirectStandardOutput = True
            .StartInfo.CreateNoWindow = True
            .StartInfo.FileName = Me.exec_path
            .StartInfo.Arguments = " --get-meek"
            .Start()
            ToolStripStatusLabel1.Text = Me.exec_path + .StartInfo.Arguments
            ToolStripStatusLabel2.Text = "Running"
        End With
        Dim runThread = New Thread(AddressOf Process2_starting)
        runThread.Start()
    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        If Process2 IsNot Nothing Then Process2.Dispose()
        Process2 = New Process
        AddHandler Process2.OutputDataReceived, AddressOf Process1_OutputDataReceived_Process
        With Process2
            .StartInfo.UseShellExecute = False
            .StartInfo.RedirectStandardOutput = True
            .StartInfo.CreateNoWindow = True
            .StartInfo.FileName = Me.exec_path
            .StartInfo.Arguments = " -kg"
            .Start()
            ToolStripStatusLabel1.Text = Me.exec_path + .StartInfo.Arguments
            ToolStripStatusLabel2.Text = "Running"
        End With
        Dim runThread = New Thread(AddressOf Process2_starting)
        runThread.Start()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form2.ShowDialog()
    End Sub
End Class
