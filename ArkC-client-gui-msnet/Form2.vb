﻿Imports System.Web.Script.Serialization
Imports System.IO
Imports System.Text.Encoding
Imports System.Threading
Imports System.Management

Public Class Form2
    Private Property show_advanced As Boolean = False
    Private Property saved As Boolean = False
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
        With OpenFileDialog1
            .InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
            Dim result As DialogResult = .ShowDialog()
            .Title = "Executable of Server Public Key"
            If result = Windows.Forms.DialogResult.OK Then
                TextBox4.Text = .FileName
            End If
        End With
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        With OpenFileDialog1
            .InitialDirectory = Application.UserAppDataPath
            Dim result As DialogResult = .ShowDialog()
            .Title = "Executable of Client Private Key"
            If result = Windows.Forms.DialogResult.OK Then
                TextBox5.Text = .FileName
            End If
        End With
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        With OpenFileDialog1
            .InitialDirectory = Application.UserAppDataPath
            Dim result As DialogResult = .ShowDialog()
            .Title = "Executable of Client Public Key"
            If result = Windows.Forms.DialogResult.OK Then
                TextBox6.Text = .FileName
            End If
        End With
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        With OpenFileDialog1
            .InitialDirectory = Application.StartupPath
            Dim result As DialogResult = .ShowDialog()
            .Title = "Executable of obfs4proxy"
            If result = Windows.Forms.DialogResult.OK Then
                TextBox12.Text = .FileName
            End If
        End With
    End Sub

    Private Sub Save()
        Dim Configuration As New config
        Dim serializer As New JavaScriptSerializer()
        Dim sTempFileName As String = Application.LocalUserAppDataPath + "\client.json"
        Dim fsTemp As New System.IO.FileStream(sTempFileName, IO.FileMode.Create)

        Configuration.control_domain = TextBox3.Text
        Configuration.local_cert = TextBox6.Text.Replace("\", "/")
        Configuration.local_cert_pub = TextBox5.Text.Replace("\", "/")
        Configuration.remote_cert = TextBox4.Text.Replace("\", "/")
        Configuration.remote_port = CInt(TextBox7.Text)
        Configuration.local_port = CInt(TextBox8.Text)
        Configuration.number = CInt(ComboBox1.SelectedItem.ToString)
        If TextBox10.Text = "<empty means :: or 0.0.0.0>" Then TextBox10.Text = ""
        Configuration.remote_host = TextBox10.Text
        Configuration.local_host = TextBox11.Text
        Configuration.obfs4_exec = TextBox12.Text
        Configuration.debug_ip = TextBox14.Text
        Configuration.executable = TextBox15.Text
        Configuration.argv = TextBox13.Text
        If CheckBox1.Checked Then Configuration.obfs_level = 3 Else Configuration.obfs_level = 0
        Dim dns = serializer.Deserialize(Of List(Of List(Of Object)))(TextBox9.Text)
        Configuration.dns_servers = dns
        Dim serializedResult = serializer.Serialize(Configuration)
        fsTemp.Write(UTF8.GetBytes(serializedResult.ToCharArray()), 0, UTF8.GetByteCount(serializedResult.ToCharArray()))
        fsTemp.Close()
        
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        With OpenFileDialog1
            .InitialDirectory = Application.StartupPath
            Dim result As DialogResult = .ShowDialog()
            .Title = "Executable of ArkC Client"
            If result = Windows.Forms.DialogResult.OK Then
                TextBox15.Text = .FileName
            End If
        End With
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If Not Me.saved Then
            If MsgBox("Save config before closing?", vbYesNo, "Closing Settings") = vbYes Then
                Save()
            End If
        End If
    End Sub

    Private Sub Load_Config()
        Dim serializer As New JavaScriptSerializer()
        Dim fsTemp As New System.IO.StreamReader(Application.LocalUserAppDataPath + "\client.json", IO.FileMode.Open)
        Dim Configuration As config
        Dim contents As String
        contents = fsTemp.ReadToEnd()
        fsTemp.Close()
        Configuration = serializer.Deserialize(Of config)(contents)
        If Configuration Is Nothing Then Configuration = New config
        With Configuration
            If Not (.Check_Validity()) Then
                ComboBox1.SelectedIndex = 4
                Exit Sub
            End If
            TextBox3.Text = .control_domain
            TextBox6.Text = .local_cert
            TextBox5.Text = .local_cert_pub
            TextBox4.Text = .remote_cert
            TextBox7.Text = CStr(.remote_port)
            TextBox8.Text = CStr(.local_port)
            ComboBox1.SelectedIndex = .number - 1
            If Configuration.remote_host = "" Then TextBox10.Text = "<empty means :: or 0.0.0.0>"
            TextBox11.Text = .local_host
            TextBox12.Text = .obfs4_exec
            TextBox14.Text = .debug_ip
            If System.IO.File.Exists(Configuration.executable) Then
                TextBox15.Text = .executable
            Else
                Find_exec()
            End If
            TextBox13.Text = .argv
            If Configuration.obfs_level = 3 Then CheckBox1.Checked = True Else CheckBox1.Checked = False
            TextBox9.Text = serializer.Serialize(Configuration.dns_servers)
        End With
    End Sub

    Private Function Find_exec()
        If Not (System.IO.File.Exists(Application.StartupPath & "/arkc-client.exe")) Then
            MsgBox("ArkC client executable not found in default directory. Please set manually.", MsgBoxStyle.Information, "Loading Executable")
            OpenFileDialog1.Title = "Executable of ArkC Client"
            Dim result As DialogResult = OpenFileDialog1.ShowDialog()
            If result = Windows.Forms.DialogResult.OK Then
                TextBox15.Text = OpenFileDialog1.FileName
                Return True
            Else
                Return False
            End If
        Else
            TextBox15.Text = Application.StartupPath & "/arkc-client.exe"
            Return True
        End If
    End Function

    Private Sub Find_meek()
        If System.IO.File.Exists(Application.StartupPath & "/meek-server.exe") Then
            TextBox12.Text = Application.StartupPath & "/meek-server.exe"
        End If
    End Sub

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles Me.Load
        shrink()
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        If System.IO.File.Exists(Application.LocalUserAppDataPath + "\client.json") Then
            Load_Config()
        Else
            If Find_exec() Then
                Form1.Key_Gen(TextBox15.Text)
                MsgBox("Key pair generated and stored. Path loaded to configuration file.", vbOKOnly, "Key Generated")
                TextBox5.Text = System.Environment.GetFolderPath("%APPDATA") + "\ArkC\arkc_pub.asc"
                TextBox6.Text = System.Environment.GetFolderPath("%APPDATA") + "\ArkC\arkc_pri.asc"
            Else
                MsgBox("Keys cannot be generated. Please specify it manually.", vbExclamation, "Key Not Generated")
            End If
            ComboBox1.SelectedIndex = 4
        End If
    End Sub

    Private Function IsValidPort(input As String) As Boolean
        Return (IsNumeric(input) And CInt(input) <= 65535 And CInt(input) >= 1)
    End Function

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Me.saved = True
        If IsValidPort(TextBox7.Text) And IsValidPort(TextBox8.Text) Then
            Save()
            Form1.Load_config()
            Me.Close()
        Else
            MsgBox("Port must be valid integer 1~65535.", MsgBoxStyle.Exclamation, "Error")
        End If

        
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Me.saved = True
        Me.Close()
        Me.Dispose()
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        If Me.show_advanced Then
            shrink()
        Else
            expand()
        End If
    End Sub

    Private Sub shrink()
        GroupBox3.Visible = False
        Panel1.Top -= GroupBox3.Height
        Me.Height -= GroupBox3.Height
        Button7.Text = "Show Advanced Settings"
        Me.show_advanced = False
    End Sub

    Private Sub expand()
        GroupBox3.Visible = True
        Panel1.Top += GroupBox3.Height
        Me.Height += GroupBox3.Height
        Button7.Text = "Hide Advanced Settings"
        Me.show_advanced = True
    End Sub

End Class