Imports System
Imports System.IO
Imports System.Net
Imports System.Net.Sockets

Module ClientHostFramework

    Private Class Controller
        Public Sub New()

            Dim hostName As String = System.Net.Dns.GetHostName()
            Dim addresses As IPAddress() = Dns.GetHostEntry(hostName).AddressList()

            For Each hostAdr As IPAddress In addresses
                Console.WriteLine("Name: " & hostName & " IP Address: " & hostAdr.ToString())
                If hostAdr.AddressFamily.ToString() = "InterNetwork" Then
                    Console.WriteLine("    [Potential host IP]")
                End If
            Next

            Console.Write(vbCrLf & "Server/Client [S/C]: ")
            Dim res As String = Console.ReadLine()

            Select Case Char.ToUpper(res(0))
                Case "S"
                    Console.WriteLine("Server Selected!")
                    Dim server As New Server
                Case "C"
                    Console.WriteLine("Client Selected!")
                    Dim client As New Client
            End Select

            Console.ReadLine()
        End Sub
    End Class

    Private Class Client
        Dim Client As TcpClient
        Dim RX As StreamReader
        Dim TX As StreamWriter
        Dim ServerIP As String

        Public Sub New()
            Connect()
        End Sub

        Sub ClientLoop()
            While True
                Console.Write("$ ")
                Dim str As String = Console.ReadLine

                If str.ToUpper = "DC" Then Disconnect()
                If str.ToUpper.StartsWith("MSG") Then
                    Console.Write("Enter message to the server: ")
                    Dim data As String = Console.ReadLine
                    Threading.ThreadPool.QueueUserWorkItem(AddressOf SendToServer, data)
                End If
            End While
        End Sub

        Sub Connect()
            Console.Write("Enter Server IP: ")
            ServerIP = Console.ReadLine
            Console.WriteLine("Trying to connect...")
            Try
                Client = New TcpClient(ServerIP, 65535)
                If Client.GetStream.CanRead Then
                    RX = New StreamReader(Client.GetStream)
                    TX = New StreamWriter(Client.GetStream)

                    Threading.ThreadPool.QueueUserWorkItem(AddressOf Connected)
                    ClientLoop()
                End If
            Catch ex As Exception
                Console.WriteLine(ex.Message)
            End Try
        End Sub

        Sub Connected()
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

        Sub Disconnect()
            Try
                Client.Close()
                Console.WriteLine("Connection ended.")
            Catch ex As Exception
            End Try
        End Sub

        Sub SendToServer(data As String)
            Try
                TX.WriteLine(data)
                TX.Flush()
            Catch ex As Exception

            End Try
        End Sub

        Sub MSG(data As String)
            Console.WriteLine("MSG: " & data)
        End Sub
    End Class

    Private Class Server
        Dim ServerStatus As Boolean = False
        Dim ServerTrying As Boolean = False
        Dim Server As TcpListener
        Dim Clients As New List(Of TcpClient)

        Public Sub New()
            StartServer()
            ServerLoop()

            Console.WriteLine("Finished server executing.")
            Console.ReadLine()
        End Sub

        Sub ServerLoop()
            While (ServerStatus)
                Console.Write("$ ")
                Dim str As String = Console.ReadLine
                If str.ToUpper = "STOP" Then StopServer()
                If str.ToUpper.StartsWith("MSG") Then
                    Console.Write("Enter message to clients: ")
                    Dim data As String = Console.ReadLine
                    Threading.ThreadPool.QueueUserWorkItem(AddressOf SendToClients, data)
                End If
            End While
        End Sub

        Sub StartServer()
            If ServerStatus = False Then
                ServerTrying = True
                Try
                    Server = New TcpListener(IPAddress.Any, 65535)
                    Server.Start()
                    ServerStatus = True
                    Threading.ThreadPool.QueueUserWorkItem(AddressOf ClientHandler)
                    Console.WriteLine("Server Started...")
                Catch ex As Exception
                    ServerStatus = False
                End Try

                ServerTrying = False
            End If
        End Sub

        Sub StopServer()
            If ServerStatus Then
                ServerTrying = True

                Try
                    For Each Client As TcpClient In Clients
                        Client.Close()
                    Next
                    Server.Stop()
                    ServerStatus = False
                    Console.WriteLine("Server Stopped")
                Catch ex As Exception
                    StopServer()
                End Try
            End If
        End Sub

        Sub SendToClients(data As String)
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

        Sub ClientHandler()
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