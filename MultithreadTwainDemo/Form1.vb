Imports Microsoft.VisualBasic
Imports System
Imports System.Drawing
Imports System.Collections
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Data
Imports System.IO
Imports System.Threading

Namespace MultithreadTwainDemo
    ''' <summary>
    ''' This demo shows how to run DotTwain in a separate thread.
    ''' </summary>
    Public Class Form1
        Inherits System.Windows.Forms.Form
        Private _scanner As ScanControl

        Private mainMenu1 As System.Windows.Forms.MainMenu
        Private menuFile As System.Windows.Forms.MenuItem
        Private WithEvents menuSelectSource As System.Windows.Forms.MenuItem
        Private WithEvents menuAcquire As System.Windows.Forms.MenuItem
        Private menuItem4 As System.Windows.Forms.MenuItem
        Private WithEvents menuHideInterface As System.Windows.Forms.MenuItem
        Private menuItem6 As System.Windows.Forms.MenuItem
        Private WithEvents menuExit As System.Windows.Forms.MenuItem
        Private WithEvents thumbnailView1 As Atalasoft.Imaging.WinControls.ThumbnailView
        Private workspaceViewer1 As Atalasoft.Imaging.WinControls.WorkspaceViewer
        Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
        Friend WithEvents MenuAbout As System.Windows.Forms.MenuItem
        Private components As System.ComponentModel.IContainer
        ''' <summary>
        ''' Required designer variable.
        ''' </summary>

        Public Sub New()
            '
            ' Required for Windows Form Designer support
            '
            InitializeComponent()

        End Sub

        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If Not components Is Nothing Then
                    components.Dispose()
                End If
            End If

            MyBase.Dispose(disposing)
        End Sub

#Region "Windows Form Designer generated code"
        ''' <summary>
        ''' Required method for Designer support - do not modify
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Me.mainMenu1 = New System.Windows.Forms.MainMenu(Me.components)
            Me.menuFile = New System.Windows.Forms.MenuItem
            Me.menuSelectSource = New System.Windows.Forms.MenuItem
            Me.menuAcquire = New System.Windows.Forms.MenuItem
            Me.menuItem4 = New System.Windows.Forms.MenuItem
            Me.menuHideInterface = New System.Windows.Forms.MenuItem
            Me.menuItem6 = New System.Windows.Forms.MenuItem
            Me.menuExit = New System.Windows.Forms.MenuItem
            Me.thumbnailView1 = New Atalasoft.Imaging.WinControls.ThumbnailView
            Me.workspaceViewer1 = New Atalasoft.Imaging.WinControls.WorkspaceViewer
            Me.MenuItem1 = New System.Windows.Forms.MenuItem
            Me.MenuAbout = New System.Windows.Forms.MenuItem
            Me.SuspendLayout()
            '
            'mainMenu1
            '
            Me.mainMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuFile, Me.MenuItem1})
            '
            'menuFile
            '
            Me.menuFile.Index = 0
            Me.menuFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuSelectSource, Me.menuAcquire, Me.menuItem4, Me.menuHideInterface, Me.menuItem6, Me.menuExit})
            Me.menuFile.Text = "&File"
            '
            'menuSelectSource
            '
            Me.menuSelectSource.Index = 0
            Me.menuSelectSource.Text = "&Select Source..."
            '
            'menuAcquire
            '
            Me.menuAcquire.Index = 1
            Me.menuAcquire.Text = "&Acquire"
            '
            'menuItem4
            '
            Me.menuItem4.Index = 2
            Me.menuItem4.Text = "-"
            '
            'menuHideInterface
            '
            Me.menuHideInterface.Index = 3
            Me.menuHideInterface.Text = "&Hide Driver Interface"
            '
            'menuItem6
            '
            Me.menuItem6.Index = 4
            Me.menuItem6.Text = "-"
            '
            'menuExit
            '
            Me.menuExit.Index = 5
            Me.menuExit.Text = "E&xit"
            '
            'thumbnailView1
            '
            Me.thumbnailView1.BackColor = System.Drawing.SystemColors.Window
            Me.thumbnailView1.Dock = System.Windows.Forms.DockStyle.Left
            Me.thumbnailView1.DragSelectionColor = System.Drawing.Color.Red
            Me.thumbnailView1.ForeColor = System.Drawing.SystemColors.WindowText
            Me.thumbnailView1.HighlightBackgroundColor = System.Drawing.SystemColors.Highlight
            Me.thumbnailView1.HighlightTextColor = System.Drawing.SystemColors.HighlightText
            Me.thumbnailView1.LoadErrorMessage = ""
            Me.thumbnailView1.Location = New System.Drawing.Point(0, 0)
            Me.thumbnailView1.Margins = New Atalasoft.Imaging.WinControls.Margin(4, 4, 4, 4)
            Me.thumbnailView1.SelectionMode = Atalasoft.Imaging.WinControls.ThumbnailSelectionMode.SingleSelect
            Me.thumbnailView1.Name = "thumbnailView1"
            Me.thumbnailView1.SelectionRectangleBackColor = System.Drawing.Color.Transparent
            Me.thumbnailView1.SelectionRectangleDashStyle = System.Drawing.Drawing2D.DashStyle.Solid
            Me.thumbnailView1.SelectionRectangleLineColor = System.Drawing.Color.Black
            Me.thumbnailView1.Size = New System.Drawing.Size(160, 430)
            Me.thumbnailView1.TabIndex = 0
            Me.thumbnailView1.Text = "thumbnailView1"
            Me.thumbnailView1.ThumbnailBackground = Nothing
            Me.thumbnailView1.ThumbnailOffset = New System.Drawing.Point(0, 0)
            Me.thumbnailView1.ThumbnailSize = New System.Drawing.Size(100, 100)
            '
            'workspaceViewer1
            '
            Me.workspaceViewer1.AntialiasDisplay = Atalasoft.Imaging.WinControls.AntialiasDisplayMode.ScaleToGray
            Me.workspaceViewer1.DisplayProfile = Nothing
            Me.workspaceViewer1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.workspaceViewer1.Location = New System.Drawing.Point(160, 0)
            Me.workspaceViewer1.Magnifier.BackColor = System.Drawing.Color.White
            Me.workspaceViewer1.Magnifier.BorderColor = System.Drawing.Color.Black
            Me.workspaceViewer1.Magnifier.Size = New System.Drawing.Size(100, 100)
            Me.workspaceViewer1.Name = "workspaceViewer1"
            Me.workspaceViewer1.OutputProfile = Nothing
            Me.workspaceViewer1.Selection = Nothing
            Me.workspaceViewer1.Size = New System.Drawing.Size(496, 430)
            Me.workspaceViewer1.TabIndex = 1
            Me.workspaceViewer1.Text = "workspaceViewer1"
            '
            'MenuItem1
            '
            Me.MenuItem1.Index = 1
            Me.MenuItem1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuAbout})
            Me.MenuItem1.Text = "&Help"
            '
            'MenuAbout
            '
            Me.MenuAbout.Index = 0
            Me.MenuAbout.Text = "&About ..."
            '
            'Form1
            '
            Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
            Me.ClientSize = New System.Drawing.Size(656, 430)
            Me.Controls.Add(Me.workspaceViewer1)
            Me.Controls.Add(Me.thumbnailView1)
            Me.Menu = Me.mainMenu1
            Me.Name = "Form1"
            Me.Text = "Multithread DotTwain Demo"
            Me.ResumeLayout(False)

        End Sub
#End Region

        ''' <summary>
        ''' The main entry point for the application.
        ''' </summary>
        <STAThread()> _
        Shared Sub Main()
            Application.Run(New Form1())
        End Sub

        Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Me._scanner = New ScanControl(Me)
            AddHandler _scanner.ImageAcquired, AddressOf _scanner_ImageAcquired
            AddHandler _scanner.AcquireCanceled, AddressOf _scanner_AcquireCanceled
            AddHandler _scanner.AcquireFinished, AddressOf _scanner_AcquireFinished
        End Sub

        Private Sub Form1_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
            If Not Me._scanner Is Nothing Then
                Me._scanner.Dispose()
            End If
        End Sub

        Private Sub menuSelectSource_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuSelectSource.Click
            Me._scanner.ShowSelectSource()
        End Sub

        Private Sub menuAcquire_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuAcquire.Click
            Me.menuSelectSource.Enabled = False
            Me.menuAcquire.Enabled = False

            Me._scanner.StartScan(Me.menuHideInterface.Checked)
        End Sub

        Private Sub menuHideInterface_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuHideInterface.Click
            Me.menuHideInterface.Checked = Not Me.menuHideInterface.Checked
        End Sub

        Private Sub menuExit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuExit.Click
            Me.Close()
        End Sub

        Private Sub thumbnailView1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles thumbnailView1.SelectedIndexChanged
            ' Load the selected image into the WorkspaceViewer.
            If Not Me.thumbnailView1.FocusedItem Is Nothing Then
                Me.workspaceViewer1.Image = Me.workspaceViewer1.Images(Me.thumbnailView1.SelectedIndices(0))
            Else
                Me.workspaceViewer1.Image = Nothing
            End If
        End Sub

#Region "Scan Events"

        Private Sub _scanner_ImageAcquired(ByVal sender As Object, ByVal e As Atalasoft.Twain.AcquireEventArgs)
            ' Convert the .NET Bitmap to an AtalaImage and dispose the Bitmap.
            Dim image As Atalasoft.Imaging.AtalaImage = Atalasoft.Imaging.AtalaImage.FromBitmap(e.Image)
            e.Image.Dispose()

            ' For this demo we keep the images in memory.
            ' If several images are being scanned it's better to 
            ' save the to a folder and load them as needed.
            Me.workspaceViewer1.Images.Add(image)
            Me.thumbnailView1.Items.Add(image, "")
        End Sub

        Private Sub _scanner_AcquireCanceled(ByVal sender As Object, ByVal e As EventArgs)
            ' The user canceled the scan.
            Me.menuSelectSource.Enabled = True
            Me.menuAcquire.Enabled = True
        End Sub

        Private Sub _scanner_AcquireFinished(ByVal sender As Object, ByVal e As EventArgs)
            ' The scan is finished.
            Me.menuSelectSource.Enabled = True
            Me.menuAcquire.Enabled = True
        End Sub

#End Region

        Private Sub MenuAbout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuAbout.Click
            Dim aboutBox As AtalaDemos.AboutBox.About = New AtalaDemos.AboutBox.About("About Atalasoft Multithread TWAIN Demo", "Multithread TWAIN Demo")
            aboutBox.Description = "A no-frills version of our Acquisition Demo which shows how to place the scanning control into a separate thread. Unlike the Acquisition demo, this requires either DotImage Document Imaging or a combination of DotTwain and DotImage PhotoPro licenses to run, as it makes use of our ThumbnailViewer and WorkspaceViewer controls."
            aboutBox.ShowDialog()
        End Sub
    End Class
End Namespace
