Imports System
Imports System.IO
Imports System.Net
Imports System.Net.Sockets

public Module WebHandler
    Private Sub WTL(dnsModel As DNSModel, msg As String, isClient As Boolean)
        dnsModel.WriteToLog(msg, isClient)
    End Sub

    Public Class WebController
        Private client As Client
        Private server As Server
        Private isClient As Boolean
        Private clientGameReady As Boolean = False

        Public Sub New(isClient As Boolean, dnsModel As DNSModel)
            Me.isClient = isClient

            If isClient Then
                client = New Client(dnsModel)
            Else
                server = New Server(dnsModel)
            End If
        End Sub

        Public Function GetIsClient() As Boolean
            Return isClient
        End Function

        Public Sub SetClientGameReady(cgr As Boolean)
            clientGameReady = cgr
        End Sub
        Public Function GetClientGameReady() As Boolean
            Return clientGameReady
        End Function

        Public Function StartServer() As Boolean
            If server IsNot Nothing Then
                Return server.StartServer()
            End If
            Return False
        End Function

        Public Function Connect(ServerIP As String) As Boolean
            If client IsNot Nothing Then
                Return client.Connect(ServerIP)
            End If
            Return False
        End Function

        Public Sub Reconnect()
            If client IsNot Nothing Then client.Reconnect()
        End Sub

        Public Sub SendToServer(data As String)
            If Not isClient Then Exit Sub
            If client Is Nothing Then Exit Sub

            client.SendToServer(data)
        End Sub

        Public Sub SendToClients(data As String)
            If isClient Then Exit Sub
            If server Is Nothing Then Exit Sub
            server.SendToClients(data)
        End Sub

        Public Function GetIsConnected() As Boolean
            If client IsNot Nothing Then Return client.GetIsConnected
            Return False
        End Function

        Public Sub UpdateClientUsername(client As TcpClient, username As String)
            If isClient Then Exit Sub
            If server Is Nothing Then Exit Sub
            server.UpdateUsername(client, username)
        End Sub

        Public Function GetClientUsernames() As String
            If isClient Then Return ""
            If server Is Nothing Then Return ""
            Return server.GetClientUsernames()
        End Function
    End Class

    Private Class Client
        Private Client As TcpClient
        Private RX As StreamReader
        Private TX As StreamWriter
        Private ServerIP As String
        Private dnsModel As DNSModel
        Private isConnected As Boolean = False

        Public Sub New(dnsModel As DNSModel)
            Me.dnsModel = dnsModel
        End Sub

        Private Sub WriteToLog(msg As String)
            WTL(dnsModel, msg, True)
        End Sub

        Public Function GetIsConnected() As Boolean
            Return isConnected
        End Function

        Public Function Connect(ServerIP As String) As Boolean
            Me.ServerIP = ServerIP
            Try
                Client = New TcpClient(ServerIP, 65535)
                If Client.GetStream.CanRead Then
                    RX = New StreamReader(Client.GetStream)
                    TX = New StreamWriter(Client.GetStream)

                    Threading.ThreadPool.QueueUserWorkItem(AddressOf Connected)
                    Return True
                End If
            Catch ex As Exception
            End Try
            Return False
        End Function

        Public Sub Reconnect()
            RX = New StreamReader(Client.GetStream)
            TX = New StreamWriter(Client.GetStream)
            If RX.BaseStream.CanRead Then
                Try
                    While RX.BaseStream.CanRead
                        Dim RawData As String = RX.ReadLine
                        dnsModel.HandleIncomingMessage(Client, RawData)
                    End While
                Catch ex As Exception
                    Client.Close()
                End Try
            End If
        End Sub

        Private Sub Connected()
            isConnected = True
            WriteToLog("Connected to " & ServerIP & ":65535")
            If RX.BaseStream.CanRead Then
                Try
                    While RX.BaseStream.CanRead
                        Dim RawData As String = RX.ReadLine
                        WriteToLog("Server >> " & RawData)
                        dnsModel.HandleIncomingMessage(Client, RawData)
                        If RawData = "START:" Then Exit Sub
                    End While
                Catch ex As Exception
                    Client.Close()
                End Try
            End If
        End Sub

        Public Sub SendToServer(data As String)
            Try
                TX.WriteLine(data)
                TX.Flush()
            Catch ex As Exception

            End Try
        End Sub
    End Class

    Private Class Server
        Private ServerStatus As Boolean = False
        Private ServerTrying As Boolean = False
        Private Server As TcpListener
        Private Clients As New List(Of TcpClient)
        Private Usernames As New Dictionary(Of String, String)
        Private dnsModel As DNSModel

        Public Sub New(dnsModel As DNSModel)
            Me.dnsModel = dnsModel
        End Sub

        Private Sub WriteToLog(msg As String)
            WTL(dnsModel, msg, False)
        End Sub

        Public Function GetClientUsernames() As String
            Dim res As String = ""
            For Each val As String In Usernames.Values
                res &= val & ","
            Next
            Return res
        End Function

        Public Sub UpdateUsername(client As TcpClient, newUsername As String)
            Usernames(client.Client.RemoteEndPoint.ToString) = newUsername
        End Sub

        Public Function StartServer() As Boolean
            If ServerStatus = False Then
                ServerTrying = True
                Try
                    Server = New TcpListener(IPAddress.Any, 65535)
                    Server.Start()
                    ServerStatus = True
                    Threading.ThreadPool.QueueUserWorkItem(AddressOf ClientHandler)
                Catch ex As Exception
                    ServerStatus = False
                    Return False
                End Try
                ServerTrying = False
            End If
            Return True
        End Function

        Private Sub StopServer()
            If ServerStatus Then
                ServerTrying = True
                Try
                    For Each Client As TcpClient In Clients
                        Client.Close()
                    Next
                    Server.Stop()
                    ServerStatus = False

                Catch ex As Exception
                    StopServer()
                End Try
            End If
        End Sub

        Public Sub SendToClients(data As String)
            If ServerStatus Then
                If Clients.Count > 0 Then
                    Try
                        For Each Client As TcpClient In Clients
                            Dim TX_C As New StreamWriter(Client.GetStream)
                            Dim RX_C As New StreamReader(Client.GetStream)

                            TX_C.WriteLine(data)
                            TX_C.Flush()

                        Next
                    Catch ex As Exception
                        SendToClients(data)
                    End Try
                End If
            End If
        End Sub

        Private Sub RemoveClient(Client As TcpClient)
            WriteToLog("Client removed: " & Client.Client.RemoteEndPoint.ToString)
            Usernames.Remove(Client.Client.RemoteEndPoint.ToString)
            Client.Close()
            Clients.Remove(Client)
            dnsModel.HandleIncomingMessage(Nothing, "REMOVED:")
        End Sub

        Private Sub ClientHandler()
            Try
                Dim Client As TcpClient = Server.AcceptTcpClient
                If Not ServerTrying Then
                    Threading.ThreadPool.QueueUserWorkItem(AddressOf ClientHandler)
                End If
                If Clients.Count >= 3 Then
                    MsgBox("Lobby overflow")
                    Dim TX_C As New StreamWriter(Client.GetStream)
                    TX_C.WriteLine("Lobby is full.")
                    TX_C.Flush()
                    Client.Close()
                    Exit Sub
                End If

                Clients.Add(Client)
                Usernames.Add(Client.Client.RemoteEndPoint.ToString, "_UNNAMED_")
                WriteToLog("Client added: " & Client.Client.RemoteEndPoint.ToString)

                Dim TX As New StreamWriter(Client.GetStream)
                Dim RX As New StreamReader(Client.GetStream)

                If RX.BaseStream.CanRead Then
                    While RX.BaseStream.CanRead
                        Dim RawData As String = RX.ReadLine
                        WriteToLog(Client.Client.RemoteEndPoint.ToString & " >> " & RawData)
                        dnsModel.HandleIncomingMessage(Client, RawData)
                    End While
                End If

                If Not RX.BaseStream.CanRead Then
                    RemoveClient(Client)
                End If

            Catch ex As Exception
                For i As Integer = 0 To Clients.Count - 1
                    If Not Clients(i).Client.Connected Then
                        RemoveClient(Clients(i))
                        Exit For
                    End If
                Next

            End Try

        End Sub

    End Class

End Module