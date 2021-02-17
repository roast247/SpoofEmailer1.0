Imports System.Net.Mail
Imports System.Runtime.InteropServices
Imports System.IO



'GIVE credit where credit is due please! -Roast247



Public Class Form1

    Public Declare Sub Sleep Lib "kernel32" Alias "Sleep" ()

    Dim file As String

    'Width of the 'resizable border', the area where you can resize.
    Private Const BorderWidth As Integer = 6
    Private _resizeDir As ResizeDirection = ResizeDirection.None
    Private draggable As Boolean
    Private mouseY As Integer
    Private mouseX As Integer
    Private drag As Boolean

    Public Enum ResizeDirection
        None = 0
        Left = 1
        TopLeft = 2
        Top = 3
        TopRight = 4
        Right = 5
        BottomRight = 6
        Bottom = 7
        BottomLeft = 8
    End Enum

    Public Property resizeDir() As ResizeDirection
        Get
            Return _resizeDir
        End Get
        Set(ByVal value As ResizeDirection)
            _resizeDir = value

            'Change cursor
            Select Case value
                Case ResizeDirection.Left
                    Me.Cursor = Cursors.SizeWE

                Case ResizeDirection.Right
                    Me.Cursor = Cursors.SizeWE

                Case ResizeDirection.Top
                    Me.Cursor = Cursors.SizeNS

                Case ResizeDirection.Bottom
                    Me.Cursor = Cursors.SizeNS

                Case ResizeDirection.BottomLeft
                    Me.Cursor = Cursors.SizeNESW

                Case ResizeDirection.TopRight
                    Me.Cursor = Cursors.SizeNESW

                Case ResizeDirection.BottomRight
                    Me.Cursor = Cursors.SizeNWSE

                Case ResizeDirection.TopLeft
                    Me.Cursor = Cursors.SizeNWSE

                Case Else
                    Me.Cursor = Cursors.Default
            End Select
        End Set
    End Property

#Region " Functions and Constants "

    <DllImport("user32.dll")>
    Public Shared Function ReleaseCapture() As Boolean
    End Function

    <DllImport("user32.dll")>
    Public Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
    End Function

    Private Const WM_NCLBUTTONDOWN As Integer = &HA1
    Private Const HTBORDER As Integer = 18
    Private Const HTBOTTOM As Integer = 15
    Private Const HTBOTTOMLEFT As Integer = 16
    Private Const HTBOTTOMRIGHT As Integer = 17
    Private Const HTCAPTION As Integer = 2
    Private Const HTLEFT As Integer = 10
    Private Const HTRIGHT As Integer = 11
    Private Const HTTOP As Integer = 12
    Private Const HTTOPLEFT As Integer = 13
    Private Const HTTOPRIGHT As Integer = 14

#End Region

#Region " Moving & Resizing methods "

    Private Sub MoveForm()
        ReleaseCapture()
        SendMessage(Me.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0)
    End Sub

    Private Sub ResizeForm(ByVal direction As ResizeDirection)
        Dim dir As Integer = -1
        Select Case direction
            Case ResizeDirection.Left
                dir = HTLEFT
            Case ResizeDirection.TopLeft
                dir = HTTOPLEFT
            Case ResizeDirection.Top
                dir = HTTOP
            Case ResizeDirection.TopRight
                dir = HTTOPRIGHT
            Case ResizeDirection.Right
                dir = HTRIGHT
            Case ResizeDirection.BottomRight
                dir = HTBOTTOMRIGHT
            Case ResizeDirection.Bottom
                dir = HTBOTTOM
            Case ResizeDirection.BottomLeft
                dir = HTBOTTOMLEFT
        End Select

        If dir <> -1 Then
            ReleaseCapture()
            SendMessage(Me.Handle, WM_NCLBUTTONDOWN, dir, 0)
        End If
    End Sub
#End Region

    Private Sub Form1_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left And Me.WindowState <> FormWindowState.Maximized Then
            ResizeForm(resizeDir)
        End If
    End Sub

    Private Sub Form1_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove
        'Calculate which direction to resize based on mouse position

        If e.Location.X < BorderWidth And e.Location.Y < BorderWidth Then
            resizeDir = ResizeDirection.TopLeft

        ElseIf e.Location.X < BorderWidth And e.Location.Y > Me.Height - BorderWidth Then
            resizeDir = ResizeDirection.BottomLeft

        ElseIf e.Location.X > Me.Width - BorderWidth And e.Location.Y > Me.Height - BorderWidth Then
            resizeDir = ResizeDirection.BottomRight

        ElseIf e.Location.X > Me.Width - BorderWidth And e.Location.Y < BorderWidth Then
            resizeDir = ResizeDirection.TopRight

        ElseIf e.Location.X < BorderWidth Then
            resizeDir = ResizeDirection.Left

        ElseIf e.Location.X > Me.Width - BorderWidth Then
            resizeDir = ResizeDirection.Right

        ElseIf e.Location.Y < BorderWidth Then
            resizeDir = ResizeDirection.Top

        ElseIf e.Location.Y > Me.Height - BorderWidth Then
            resizeDir = ResizeDirection.Bottom

        Else
            resizeDir = ResizeDirection.None
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            suc.Text = ""
            Dim Smtp_Server As New SmtpClient
            Dim e_mail As New MailMessage()
            Smtp_Server.UseDefaultCredentials = False
            Smtp_Server.Credentials = New Net.NetworkCredential(userMail.Text, userPass.Text)
            Smtp_Server.Port = (smtpPort.Text)
            Smtp_Server.EnableSsl = True
            Smtp_Server.Host = (smtpHost.Text)

            e_mail = New MailMessage()
            e_mail.From = New MailAddress(txtFrom.Text, txtName.Text)
            e_mail.To.Add(mainMail.Text)
            e_mail.Subject = txtSubject.Text
            e_mail.IsBodyHtml = True
            e_mail.Body = (txtMessage.Text)
            e_mail.Priority = MailPriority.Low
            e_mail.Priority = MailPriority.Normal
            e_mail.Priority = MailPriority.High



            If Not attachf.Text = Nothing Then
                Dim attach As New Attachment(attachf.Text)
                e_mail.Attachments.Add(attach)
            End If

            Smtp_Server.Send(e_mail)
            Timer1.Interval = 10000
            Timer1.Start()
            suc.Text = "Mail Sent, Success!"


        Catch error_t As Exception
            Timer1.Interval = 10000
            Timer1.Start()
            suc.Text = "Failed To Send, Error"
            MsgBox(error_t.ToString)


        End Try
    End Sub

    Private Sub attachFile_Click(sender As Object, e As EventArgs) Handles attachFile.Click
        With OpenFileDialog2
            .FileName = ""
            If .ShowDialog = Windows.Forms.DialogResult.OK Then
                Dim spath As String = .FileName
                Dim SR As New IO.StreamReader(spath)
                Dim data As String = SR.ReadToEnd
                SR.Close()
                attachf.Text = data
            End If
        End With
    End Sub

    Private Sub showPass_CheckedChanged(sender As Object, e As EventArgs) Handles showPass.CheckedChanged
        If showPass.Checked = True Then
            userPass.UseSystemPasswordChar = False

        ElseIf showPass.CheckState = False Then
            userPass.UseSystemPasswordChar = True
        End If
    End Sub

    Private Sub KryptonButton1_Click(sender As Object, e As EventArgs) Handles KryptonButton1.Click
        Close()
    End Sub

    Private Sub KryptonLinkLabel1_LinkClicked(sender As Object, e As EventArgs) Handles KryptonLinkLabel1.LinkClicked
        Process.Start("https://github.com/roast247")
    End Sub

    Private Sub Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Panel1.Paint

    End Sub

    Private Sub Panel1_MouseDown(sender As Object, e As MouseEventArgs) Handles Panel1.MouseDown
        draggable = True
        mouseX = Cursor.Position.X - Me.Left
        mouseY = Cursor.Position.Y - Me.Top
    End Sub

    Private Sub Panel1_MouseMove(sender As Object, e As MouseEventArgs) Handles Panel1.MouseMove
        If draggable Then
            Me.Top = Cursor.Position.Y - mouseY
            Me.Left = Cursor.Position.X - mouseX
        End If
    End Sub

    Private Sub Panel1_MouseUp(sender As Object, e As MouseEventArgs) Handles Panel1.MouseUp
        draggable = False
    End Sub

    Private Sub TabPage1_Click(sender As Object, e As EventArgs) Handles TabPage1.Click

    End Sub

    Private Sub TabPage1_MouseDown(sender As Object, e As MouseEventArgs) Handles TabPage1.MouseDown
        draggable = True
        mouseX = Cursor.Position.X - Me.Left
        mouseY = Cursor.Position.Y - Me.Top
    End Sub

    Private Sub TabPage1_MouseMove(sender As Object, e As MouseEventArgs) Handles TabPage1.MouseMove
        If draggable Then
            Me.Top = Cursor.Position.Y - mouseY
            Me.Left = Cursor.Position.X - mouseX
        End If
    End Sub

    Private Sub TabPage1_MouseUp(sender As Object, e As MouseEventArgs) Handles TabPage1.MouseUp
        draggable = False
    End Sub

    Private Sub Label11_Click(sender As Object, e As EventArgs) Handles Label11.Click

    End Sub

    Private Sub Label11_MouseDown(sender As Object, e As MouseEventArgs) Handles Label11.MouseDown
        draggable = True
        mouseX = Cursor.Position.X - Me.Left
        mouseY = Cursor.Position.Y - Me.Top
    End Sub

    Private Sub Label11_MouseMove(sender As Object, e As MouseEventArgs) Handles Label11.MouseMove
        If draggable Then
            Me.Top = Cursor.Position.Y - mouseY
            Me.Left = Cursor.Position.X - mouseX
        End If
    End Sub

    Private Sub Label11_MouseUp(sender As Object, e As MouseEventArgs) Handles Label11.MouseUp
        draggable = False
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

    End Sub

    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseDown
        draggable = True
        mouseX = Cursor.Position.X - Me.Left
        mouseY = Cursor.Position.Y - Me.Top
    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        If draggable Then
            Me.Top = Cursor.Position.Y - mouseY
            Me.Left = Cursor.Position.X - mouseX
        End If
    End Sub

    Private Sub PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseUp
        draggable = False
    End Sub

    Private Sub savesetting_CheckedChanged(sender As Object, e As EventArgs) Handles savesetting.CheckedChanged
        If savesetting.Checked = True Then My.Settings.Username = userMail.Text
        If savesetting.Checked = True Then My.Settings.Password = userPass.Text
        If savesetting.Checked = True Then My.Settings.smtpserver = smtpHost.Text
        If savesetting.Checked = True Then My.Settings.smtpport = smtpPort.Text
        My.Settings.Save()
    End Sub

    Private Sub loadSet_Click(sender As Object, e As EventArgs) Handles loadSet.Click
        userMail.Text = My.Settings.Username
        userPass.Text = My.Settings.Password
        smtpHost.Text = My.Settings.smtpserver
        smtpPort.Text = My.Settings.smtpport
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        userPass.UseSystemPasswordChar = True
    End Sub

    Private Sub CutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CutToolStripMenuItem.Click
        txtMessage.Cut()
    End Sub

    Private Sub CopyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyToolStripMenuItem.Click
        txtMessage.Copy()
    End Sub

    Private Sub PasteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PasteToolStripMenuItem.Click
        txtMessage.Paste()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Stop()
        suc.Text = ""
    End Sub

    Private Sub txtMessage_TextChanged(sender As Object, e As EventArgs) Handles txtMessage.TextChanged
        With txtMessage
            If .Text = "" Then
                .Text = "Text or HTML Code"
                .ForeColor = Color.Gray
            End If
            If .Text = "Text or HTML Code" And .ForeColor = Color.Gray Then
                .ShortcutsEnabled = False
            Else
                .ShortcutsEnabled = True
            End If
            If .TextLength > 17 Then
                If StrReverse(StrReverse(.Text).Remove(17)) = "Text or HTML Code" Then
                    .Text = .Text.Remove(.TextLength - 17)
                    .ForeColor = Color.White
                    .SelectionStart = .TextLength
                    .ScrollToCaret()
                End If
            End If
        End With
    End Sub

    Private Sub txtMessage_KeyDown(sender As Object, e As KeyEventArgs) Handles txtMessage.KeyDown
        With txtMessage
            If .Text = "Text or HTML Code" And .ForeColor = Color.Gray Then
                If e.KeyCode = Keys.Right Or e.KeyCode = Keys.Left Or e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down Or e.KeyCode = Keys.Home Or e.KeyCode = Keys.End Then
                    e.Handled = True
                End If
            End If
        End With
    End Sub

    Private Sub txtMessage_MouseDown(sender As Object, e As MouseEventArgs) Handles txtMessage.MouseDown
        drag = True
        With txtMessage
            If .Text = "Text or HTML Code" And .ForeColor = Color.Gray Then
                .SelectionStart = .TextLength
                .SelectionLength = 0
                .SelectionStart = 0
                .ScrollToCaret()
            End If
        End With
    End Sub

    Private Sub txtMessage_MouseMove(sender As Object, e As MouseEventArgs) Handles txtMessage.MouseMove
        If drag Then
            With txtMessage
                If .Text = "Text or HTML Code" And .ForeColor = Color.Gray Then
                    txtMessage.Select(0, 0)
                End If
            End With
        End If
    End Sub

    Private Sub txtMessage_MouseUp(sender As Object, e As MouseEventArgs) Handles txtMessage.MouseUp
        drag = False
    End Sub
End Class