﻿Imports System
Imports System.IO
Imports System.Net
Imports System.Net.Sockets

Module WebHandler
    Private Sub WTL(dnsModel As DNSModel, msg As String, isClient As Boolean)
        dnsModel.WriteToLog(msg, isClient)
    End Sub

    Public Class WebController
        Private client As Client
        Private server As Server
        Private isClient As Boolean

        Public Sub New(isClient As Boolean, dnsModel As DNSModel)
            Me.isClient = isClient

            If isClient Then
                client = New Client(dnsModel)
            Else
                server = New Server(dnsModel)
            End If
        End Sub

        Public Function StartServer() As Boolean
            If server IsNot Nothing Then
                Return server.StartServer()
            End If
            Return False
        End Function
    End Class

    Private Class Client
        Private Client As TcpClient
        Private RX As StreamReader
        Private TX As StreamWriter
        Private ServerIP As String
        Private dnsModel As DNSModel

        Public Sub New(dnsModel As DNSModel)
            Me.dnsModel = dnsModel
            Connect()
        End Sub

        Private Sub WriteToLog(msg As String)
            WTL(dnsModel, msg, True)
        End Sub

        'Private Sub ClientLoop()
        '    While True
        '        Console.Write("$ ")
        '        Dim str As String = Console.ReadLine

        '        If str.ToUpper = "DC" Then Disconnect()
        '        If str.ToUpper.StartsWith("MSG") Then
        '            Console.Write("Enter message to the server: ")
        '            Dim data As String = Console.ReadLine
        '            Threading.ThreadPool.QueueUserWorkItem(AddressOf SendToServer, data)
        '        End If
        '    End While
        'End Sub

        Private Sub Connect()
            Console.Write("Enter Server IP: ")
            ServerIP = Console.ReadLine
            Console.WriteLine("Trying to connect...")
            Try
                Client = New TcpClient(ServerIP, 65535)
                If Client.GetStream.CanRead Then
                    RX = New StreamReader(Client.GetStream)
                    TX = New StreamWriter(Client.GetStream)

                    Threading.ThreadPool.QueueUserWorkItem(AddressOf Connected)
                End If
            Catch ex As Exception
                Console.WriteLine(ex.Message)
            End Try
        End Sub

        Private Sub Connected()
            Console.Write("Connected." & vbCrLf & "$ ")

            If RX.BaseStream.CanRead Then
                Try
                    While RX.BaseStream.CanRead
                        Dim RawData As String = RX.ReadLine
                        Console.Write("Server >> " & RawData & vbCrLf & "$ ")
                    End While
                Catch ex As Exception
                    Client.Close()
                End Try
            End If
        End Sub

        Private Sub Disconnect()
            Try
                Client.Close()
                Console.WriteLine("Connection ended.")
            Catch ex As Exception
            End Try
        End Sub

        Private Sub SendToServer(data As String)
            Try
                TX.WriteLine(data)
                TX.Flush()
            Catch ex As Exception

            End Try
        End Sub

        Private Sub MSG(data As String)
            Console.WriteLine("MSG: " & data)
        End Sub
    End Class

    Private Class Server
        Private ServerStatus As Boolean = False
        Private ServerTrying As Boolean = False
        Private Server As TcpListener
        Private Clients As New List(Of TcpClient)
        Private dnsModel As DNSModel

        Public Sub New(dnsModel As DNSModel)
            Me.dnsModel = dnsModel
            StartServer()
        End Sub

        Private Sub WriteToLog(msg As String)
            WTL(dnsModel, msg, False)
        End Sub
        'Private Sub ServerLoop()
        '    While (ServerStatus)
        '        Console.Write("$ ")
        '        Dim str As String = Console.ReadLine
        '        If str.ToUpper = "STOP" Then StopServer()
        '        If str.ToUpper.StartsWith("MSG") Then
        '            Console.Write("Enter message to clients: ")
        '            Dim data As String = Console.ReadLine
        '            Threading.ThreadPool.QueueUserWorkItem(AddressOf SendToClients, data)
        '        End If
        '    End While
        'End Sub

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

        Private Sub SendToClients(data As String)
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

        Private Sub ClientHandler()
            Try
                Dim Client As TcpClient = Server.AcceptTcpClient
                If Not ServerTrying Then
                    Threading.ThreadPool.QueueUserWorkItem(AddressOf ClientHandler)
                End If
                Clients.Add(Client)
                Console.Write("Client added: " & Client.Client.RemoteEndPoint.ToString & vbCrLf & "$ ")

                Dim TX As New StreamWriter(Client.GetStream)
                Dim RX As New StreamReader(Client.GetStream)

                If RX.BaseStream.CanRead Then
                    While RX.BaseStream.CanRead
                        Dim RawData As String = RX.ReadLine
                        Console.Write(Client.Client.RemoteEndPoint.ToString & " >> " & RawData & vbCrLf & "$ ")
                    End While
                End If

                If Not RX.BaseStream.CanRead Then
                    Client.Close()
                    Clients.Remove(Client)
                    Console.Write("Client removed: " & Client.Client.RemoteEndPoint.ToString & vbCrLf & "$ ")
                End If

            Catch ex As Exception
                For i As Integer = 0 To Clients.Count
                    If Not Clients(i).Connected Then
                        Clients(i).Close()
                        Console.Write("Client removed." & vbCrLf & "$ " & vbCrLf)
                        Clients.Remove(Clients(i))
                        Exit For
                    End If
                Next

            End Try

        End Sub

    End Class

End Module