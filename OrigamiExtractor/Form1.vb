Imports System
Imports System.IO
Imports System.IO.Compression
Imports System.Reflection
Imports dnlib.DotNet
Imports dnlib.PE
Public Class Form1

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If OpenFileDialog1.ShowDialog = vbOK Then
            GroupBox1.Enabled = True
            GroupBox2.Enabled = True
        End If
    End Sub
    Sub unpack(ByVal resExt As Boolean, ByVal resetLst As Boolean, Optional ByVal A_0 As String() = Nothing)
        Dim as_ = Assembly.LoadFile(OpenFileDialog1.FileName)
        If resExt = False Then
            'Dim location As String = OpenFileDialog1.FileName
            'ModuleDefMD.Load(New BinaryReader(New streamlocation).BaseStream)
            Dim peimage As dnlib.PE.IPEImage = ModuleDefMD.Load(File.ReadAllBytes(OpenFileDialog1.FileName)).Metadata.PEImage
            Dim a_, headerind, headername
            For x = 0 To peimage.ImageSectionHeaders.Count - 1

                ListBox1.Items.Add("Checking """ & peimage.ImageSectionHeaders(x).DisplayName & """...")
                Try
                    a_ = Assembly.Load(Decompress(peimage.CreateReader(peimage.ImageSectionHeaders(x).VirtualAddress, peimage.ImageSectionHeaders(x).SizeOfRawData).ToArray()))
                    headername = peimage.ImageSectionHeaders(x).DisplayName
                    headerind = x
                    ListBox1.Items.Add("""" & peimage.ImageSectionHeaders(x).DisplayName & """ has been edited by Origami!")
                    ListBox1.Items.Add("")
                    Exit For
                Catch ex As Exception
                    ListBox1.Items.Add("""" & peimage.ImageSectionHeaders(x).DisplayName & """ hasn't been edited by Origami")
                    ListBox1.Items.Add("")
                    Continue For
                End Try
            Next
            ListBox1.Items.Add("Extracting sectors """ & headername & """'s content...")
            ListBox1.Items.Add("")
            'If Not Directory.Exists(My.Application.Info.DirectoryPath & "Origami_Unpacked") Then
            '    Directory.CreateDirectory(My.Application.Info.DirectoryPath & "Origami_Unpacked")
            'End If
            Directory.CreateDirectory("Origami_Unpacked\" & Path.GetFileNameWithoutExtension(OpenFileDialog1.FileName) & "\Sectors\")
            IO.File.WriteAllBytes("Origami_Unpacked\" & Path.GetFileNameWithoutExtension(OpenFileDialog1.FileName) & "\Sectors\Unpacked_000.bin", Decompress(peimage.CreateReader(peimage.ImageSectionHeaders(headerind).VirtualAddress, peimage.ImageSectionHeaders(headerind).SizeOfRawData).ToArray()))
            ListBox1.Items.Add("Sector """ & headername & """ has successfully been extracted.")
            ListBox1.Items.Add("")
        Else
            Directory.CreateDirectory("Origami_Unpacked\" & Path.GetFileNameWithoutExtension(OpenFileDialog1.FileName) & "\Resources\")
            For x = 0 To as_.GetManifestResourceNames.Count - 1
                ListBox1.Items.Add("Extracting """ & as_.GetManifestResourceNames(x) & """ (from resources)")
                ListBox1.Items.Add("")
                ExtractSaveResource(as_.GetManifestResourceNames(x), "Origami_Unpacked\" & Path.GetFileNameWithoutExtension(OpenFileDialog1.FileName) & "\Resources\")
                ListBox1.Items.Add(x + 1 & " Resource(s) extracted.")
            Next
        End If
    End Sub
    Public Sub ExtractSaveResource(ByVal filename As String, ByVal location As String)
        Using resource = System.Reflection.Assembly.LoadFile(OpenFileDialog1.FileName).GetManifestResourceStream(filename)
            Using file = New FileStream(location & filename, FileMode.Create, FileAccess.Write)
                resource.CopyTo(file)
            End Using
        End Using
    End Sub

    Private Shared Function Decompress(ByVal A_0 As Byte()) As Byte()
        Dim memoryStream As MemoryStream = New MemoryStream()
        Using deflateStream As DeflateStream = New DeflateStream(New MemoryStream(A_0), CompressionMode.Decompress)
            deflateStream.CopyTo(memoryStream)
        End Using
        Return memoryStream.ToArray()
    End Function

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim peimage As dnlib.PE.IPEImage = ModuleDefMD.Load(File.ReadAllBytes(OpenFileDialog1.FileName)).Metadata.PEImage
        ListBox1.Items.Clear()
        ListBox1.Items.Add("Sections of """ & Path.GetFileName(OpenFileDialog1.FileName) & """ (" & peimage.ImageSectionHeaders.Count & ")")
        ListBox1.Items.Add("Name | Address | Size")
        ListBox1.Items.Add("")
        For x = 0 To peimage.ImageSectionHeaders.Count - 1
            ListBox1.Items.Add(peimage.ImageSectionHeaders(x).DisplayName & " | 0x" & Hex(peimage.ImageSectionHeaders(x).VirtualAddress) & " | 0x" & Hex(peimage.ImageSectionHeaders(x).VirtualSize))
        Next
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        ListBox1.Items.Clear()
        Dim assembly As Assembly = assembly.LoadFile(OpenFileDialog1.FileName)
        Dim v_ As Boolean = False
        Dim headerName As String = ""
        Dim peimage As dnlib.PE.IPEImage = ModuleDefMD.Load(File.ReadAllBytes(OpenFileDialog1.FileName)).Metadata.PEImage
        If assembly.EntryPoint IsNot Nothing Then
            ListBox1.Items.Add("Entrypoint: " & assembly.EntryPoint.Module.Name & "." & assembly.EntryPoint.Name)
        Else
            ListBox1.Items.Add("Entrypoint: ???")
        End If
        ListBox1.Items.Add("")
        For x = 0 To peimage.ImageSectionHeaders.Count - 1
            ListBox1.Items.Add("Checking """ & peimage.ImageSectionHeaders(x).DisplayName & """...")
            If peimage.ImageSectionHeaders(x).DisplayName = TextBox1.Text Then
                Try
                    assembly = assembly.Load(Decompress(peimage.CreateReader(peimage.ImageSectionHeaders(x).VirtualAddress, peimage.ImageSectionHeaders(x).SizeOfRawData).ToArray()))
                    ListBox1.Items.Add("""" & peimage.ImageSectionHeaders(x).DisplayName & """ has been edited by Origami!")
                    v_ = True
                    headerName = peimage.ImageSectionHeaders(x).DisplayName
                    Exit For
                Catch ex As Exception
                    ListBox1.Items.Add("""" & peimage.ImageSectionHeaders(x).DisplayName & """ hasn't been edited by Origami")
                    Continue For
                End Try
            Else
                ListBox1.Items.Add("""" & peimage.ImageSectionHeaders(x).DisplayName & """ hasn't been edited by Origami")
            End If
        Next
        ListBox1.Items.Add("")
        If v_ Then
            ListBox1.Items.Add("This file is obfuscated with Origami! (header name:""" & headerName & """)")
        Else
            ListBox1.Items.Add("Not obfuscated with Origami!")
        End If
    End Sub

    Private Sub Button2_MouseHover(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.MouseHover
        StatusBar1.Text = "Lists all sectors in the packed application."
    End Sub

    Private Sub Button3_MouseHover(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.MouseHover
        StatusBar1.Text = "Checks if the application has been obfuscated/packed with Origami. (goes through every headers)"
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        ListBox1.Items.Clear()
        Dim assembly As Assembly = assembly.LoadFile(OpenFileDialog1.FileName)
        Dim v_ As Boolean = False
        Dim headerName As String = ""
        Dim peimage As dnlib.PE.IPEImage = ModuleDefMD.Load(File.ReadAllBytes(OpenFileDialog1.FileName)).Metadata.PEImage
        If assembly.EntryPoint IsNot Nothing Then
            ListBox1.Items.Add("Entrypoint: " & assembly.EntryPoint.Module.Name & "." & assembly.EntryPoint.Name)
        Else
            ListBox1.Items.Add("Entrypoint: ???")
        End If
        ListBox1.Items.Add("")
        For x = 0 To peimage.ImageSectionHeaders.Count - 1

            If peimage.ImageSectionHeaders(x).DisplayName = TextBox1.Text Then
                ListBox1.Items.Add("Checking """ & peimage.ImageSectionHeaders(x).DisplayName & """...")
                Try
                    assembly = assembly.Load(Decompress(peimage.CreateReader(peimage.ImageSectionHeaders(x).VirtualAddress, peimage.ImageSectionHeaders(x).SizeOfRawData).ToArray()))
                    ListBox1.Items.Add("""" & peimage.ImageSectionHeaders(x).DisplayName & """ has been edited by Origami!")
                    v_ = True
                    headerName = peimage.ImageSectionHeaders(x).DisplayName
                    Exit For
                Catch ex As Exception
                    ListBox1.Items.Add("""" & peimage.ImageSectionHeaders(x).DisplayName & """ hasn't been edited by Origami")
                    Continue For
                End Try
            End If
        Next
        ListBox1.Items.Add("")
        If v_ Then
            ListBox1.Items.Add("This file is obfuscated with Origami! (header name:""" & headerName & """)")
        Else
            ListBox1.Items.Add("Not obfuscated with Origami!")
        End If
    End Sub

    Private Sub Button4_MouseHover(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.MouseHover
        StatusBar1.Text = "Checks if the application has been obfuscated/packed with Origami. (goes through targetted header)"
    End Sub

    Private Sub TextBox1_Move(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.Move
        If sender.Location.x <= 82 Then
            Label1.Hide()
        Else
            Label1.Show()
        End If
    End Sub

    Private Sub Panel3_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel3.Paint
        Panel3.Location = New Point(Size.Width / 2 - (Panel3.Size.Width / 2), (Size.Height / 2 - (Panel3.Size.Height / 2)) - 30)
    End Sub

    Private Sub Form1_SizeChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.SizeChanged
        Panel3.Location = New Point(Size.Width / 2 - (Panel3.Size.Width / 2), (Size.Height / 2 - (Panel3.Size.Height / 2)) - 30)
        Panel4.Location = New Point(Size.Width / 2 - (Panel4.Size.Width / 2), (Size.Height / 2 - (Panel4.Size.Height / 2)) - 30)
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Panel3.Show()
        SplitContainer1.Enabled = False
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        If CheckBox1.Checked = False AndAlso CheckBox2.Checked = False Then
            MessageBox.Show("You need to check atleast one option to continue.", "OrigamiUnpacker", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Panel3.Hide()
        SplitContainer1.Enabled = True
        Try
            If CheckBox1.Checked Then
                unpack(False, True)
                If CheckBox2.Checked Then
                    unpack(True, False)
                End If
            Else
                unpack(True, True)
            End If
            If MessageBox.Show("The protected files have been extracted, do you want to open the output folder?" & vbCrLf & vbCrLf & "(folder: " & My.Application.Info.DirectoryPath & "\Origami_Unpacked\" & Path.GetFileNameWithoutExtension(OpenFileDialog1.FileName) & ")", "OrigamiUnpacker", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                Process.Start("Origami_Unpacked\" & Path.GetFileNameWithoutExtension(OpenFileDialog1.FileName))
            End If
        Catch ex As Exception
            Throw
            'MessageBox.Show("There has been an error while trying to extract the files!" & vbCrLf & vbCrLf & ex.Message, "OrigamiUnpacker", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Panel3.Hide()
    End Sub

    Private Sub Button5_MouseHover(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.MouseHover
        StatusBar1.Text = "Extracts the protected files out of the packed executable."
    End Sub

    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process.Start("https://sinister.ly")
    End Sub
    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        Dim rgb As New rgb(Label2, "ForeColor", 250, False)
        rgb.RGBStr()
        Timer1.Start()
        SplitContainer1.Enabled = False
        Panel4.Show()
    End Sub

    Private Sub Panel4_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel4.Paint
        Panel4.Location = New Point(Size.Width / 2 - (Panel4.Size.Width / 2), (Size.Height / 2 - (Panel4.Size.Height / 2)) - 30)
    End Sub

    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        SplitContainer1.Enabled = True
        Panel4.Hide()
    End Sub

    Private Sub LinkLabel2_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        Process.Start("https://github.com/dr4k0nia")
    End Sub

    Private Sub LinkLabel3_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
        Process.Start("https://github.com/dr4k0nia/Origami")
    End Sub
End Class