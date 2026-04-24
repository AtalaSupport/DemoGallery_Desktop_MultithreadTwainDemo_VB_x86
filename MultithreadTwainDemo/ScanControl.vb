Imports Microsoft.VisualBasic
Imports System
Imports System.Threading

Namespace MultithreadTwainDemo
    ''' <summary>
    ''' This class controls the scanning thread and DotTwain objects.
    ''' </summary>
    Public Class ScanControl
        Implements IDisposable
        Private _scanningThread As Thread
        Private _isDisposed As Boolean
        Private _scanning As Boolean
        Private _hideInterface As Boolean
        Private _action As ScanControlAction
        Private _acquisition As Atalasoft.Twain.Acquisition
        Private _device As Atalasoft.Twain.Device
        Private _uiForm As System.Windows.Forms.Form

        ' This variable is used to notify the scanning thread
        ' when it needs to perform an action.
        Private _scanControlMessage As Object

        Public Event AcquireCanceled As EventHandler
        Public Event AcquireFinished As EventHandler
        Public Event ImageAcquired As Atalasoft.Twain.ImageAcquiredEventHandler

        Private _onAcquireFinished As System.Windows.Forms.MethodInvoker
        Private _onAcquireCanceled As System.Windows.Forms.MethodInvoker
        Private _onImageAcquired As Atalasoft.Twain.ImageAcquiredEventHandler

        Public Sub New(ByVal uiForm As System.Windows.Forms.Form)
            ' The main UI form is used for invoking.
            Me._uiForm = uiForm

            Me._onImageAcquired = New Atalasoft.Twain.ImageAcquiredEventHandler(AddressOf OnImageAcquired)
            Me._onAcquireCanceled = New System.Windows.Forms.MethodInvoker(AddressOf OnAcquireCanceled)
            Me._onAcquireFinished = New System.Windows.Forms.MethodInvoker(AddressOf OnAcquireFinished)

            Me._scanControlMessage = New Object()

            Me._scanningThread = New Thread(AddressOf RunScanningThread)
            Me._scanningThread.Name = "Scanning Thread"
            Me._scanningThread.Start()
        End Sub

        Protected Overrides Sub Finalize()
            Dispose()
        End Sub

#Region "Methods and Properties"

        ''' <summary>
        ''' Stops the scanning process and disposes any resources.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            If Me._isDisposed Then
                Return
            End If

            SyncLock Me._scanControlMessage
                Me._action = ScanControlAction.Dispose
                Monitor.Pulse(Me._scanControlMessage)
            End SyncLock

            Me._isDisposed = True
            GC.SuppressFinalize(Me)
        End Sub

        ''' <summary>
        ''' Gets a value indicating if the ScanControl has been disposed.
        ''' </summary>
        Public ReadOnly Property IsDisposed() As Boolean
            Get
                Return Me._isDisposed
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether or not we are currently scanning.
        ''' </summary>
        Public ReadOnly Property Scanning() As Boolean
            Get
                Return Me._scanning
            End Get
        End Property

        ''' <summary>
        ''' Displays the 'Select Source' dialog so the user can select the device.
        ''' </summary>
        Public Sub ShowSelectSource()
            SyncLock Me._scanControlMessage
                Me._action = ScanControlAction.SelectSource
                Monitor.Pulse(Me._scanControlMessage)
            End SyncLock
        End Sub

        ''' <summary>
        ''' Starts the scan process.
        ''' </summary>
        ''' <param name="hideInterface">If True, the driver interface will be hidden.</param>
        Public Sub StartScan(ByVal hideInterface As Boolean)
            Me._hideInterface = hideInterface

            SyncLock Me._scanControlMessage
                Me._action = ScanControlAction.Start
                Monitor.Pulse(Me._scanControlMessage)
            End SyncLock
        End Sub

#End Region

        ''' <summary>
        ''' This method is run from the scanning thread.
        ''' The Acquisition and Device objects are created
        ''' used and destroyed in this thread.
        ''' </summary>
        Private Sub RunScanningThread()
            _acquisition = New Atalasoft.Twain.Acquisition()

            ' If the system does not have TWAIN installed or devices, end.
            If (Not _acquisition.SystemHasTwain) OrElse _acquisition.Devices.Count = 0 Then
                _acquisition.Dispose()
                _acquisition = Nothing
                Return
            End If

            _device = _acquisition.Devices.Default

            ' Setup the TWAIN events.
            AddHandler _acquisition.ImageAcquired, AddressOf _acquisition_ImageAcquired
            AddHandler _acquisition.AcquireCanceled, AddressOf _acquisition_AcquireCanceled
            AddHandler _acquisition.AcquireFinished, AddressOf _acquisition_AcquireFinished

            ' We loop until the control has been disposed.
            Do
                Try
                    SyncLock Me._scanControlMessage
                        ' Wait for the Scan Controller to tell us to do something.
                        Monitor.Wait(_scanControlMessage)
                    End SyncLock

                    ' If we are told to dispose, clean up and exit the thread.
                    If _action = ScanControlAction.Dispose Then
                        DisposeObjects()
                        Exit Try
                    End If

                    If _action = ScanControlAction.SelectSource Then
                        If Not _acquisition Is Nothing Then
                            Dim dev As Atalasoft.Twain.Device = _acquisition.ShowSelectSource()
                            If Not dev Is Nothing Then
                                ' Ignore if it's the same device.
                                If Not _device Is dev Then
                                    If Not _device Is Nothing Then
                                        _device.Close()
                                    End If
                                    _device = dev
                                    _scanning = False
                                End If
                            End If
                        End If
                    ElseIf _action = ScanControlAction.Start Then
                        ' Start a new scanning session.
                        ' We must perform a modal acquire here to
                        ' prevent the thread from going into a Wait.
                        If Not _device Is Nothing Then
                            _scanning = True
                            _device.HideInterface = Me._hideInterface
                            _device.ModalAcquire = True
                            _device.Acquire()
                        End If
                    End If
                Catch e1 As ThreadAbortException
                    ' Abort was called on the Thread.
                    DisposeObjects()
                    Exit Do
                End Try

            Loop While Me._isDisposed = False

        End Sub

        Private Sub DisposeObjects()
            If Not _device Is Nothing Then
                ' Calling Close will force any current scan to stop.
                _device.Close()
                _device = Nothing
            End If

            If Not Me._acquisition Is Nothing Then
                _acquisition.Dispose()
                _acquisition = Nothing
            End If

            _scanning = False
        End Sub

#Region "TWAIN Events"

        Private Sub _acquisition_ImageAcquired(ByVal sender As Object, ByVal e As Atalasoft.Twain.AcquireEventArgs)
            ' If the form is closed, force a cancel.
            If Me._isDisposed Then
                e.Image.Dispose()
                e.CancelPending = True
                Return
            End If

            Me._uiForm.Invoke(_onImageAcquired, New Object() {Nothing, e})
        End Sub

        Private Sub OnImageAcquired(ByVal sender As Object, ByVal e As Atalasoft.Twain.AcquireEventArgs)
            If Not Me.ImageAcquiredEvent Is Nothing Then
                RaiseEvent ImageAcquired(Me, e)
            End If
        End Sub

        Private Sub _acquisition_AcquireCanceled(ByVal sender As Object, ByVal e As EventArgs)
            If Me._isDisposed Then
                Return
            End If
            Me._scanning = False
            Me._uiForm.Invoke(_onAcquireCanceled)
        End Sub

        Private Sub OnAcquireCanceled()
            If Not Me.AcquireCanceledEvent Is Nothing Then
                RaiseEvent AcquireCanceled(Me, EventArgs.Empty)
            End If
        End Sub

        Private Sub _acquisition_AcquireFinished(ByVal sender As Object, ByVal e As EventArgs)
            If Me._isDisposed Then
                Return
            End If
            Me._scanning = False
            Me._uiForm.Invoke(_onAcquireFinished)
        End Sub

        Private Sub OnAcquireFinished()
            If Not Me.AcquireFinishedEvent Is Nothing Then
                RaiseEvent AcquireFinished(Me, EventArgs.Empty)
            End If
        End Sub

#End Region

    End Class

    Public Enum ScanControlAction
        Start
        Dispose
        SelectSource
    End Enum
End Namespace
