Imports System
Imports System.Net

Module ClientHostFramework

    Public Class Host
        <Obsolete>
        Sub Main(args As String())
            Dim hostName = Dns.GetHostName()
            Dim addresses As IPAddress() = Dns.GetHostEntry(hostName).AddressList()
            For Each hostAdr As IPAddress In addresses
                Console.WriteLine("Name: " & hostName & " IP Address: " & hostAdr.ToString())
                If hostAdr.AddressFamily.ToString() = "InterNetwork" Then
                    Console.WriteLine("    [Potential host IP]")
                End If
            Next
            Console.WriteLine(vbCrLf & "Listening...")

            ' Obsolete line!
            Dim listener As New Sockets.TcpListener(65535)
            Dim client As Sockets.TcpClient

            listener.Start()
            Dim message As String = ""
            Do
                If listener.Pending = True Then
                    message = ""
                    client = listener.AcceptTcpClient()

                    Dim Reader As New IO.StreamReader(client.GetStream())

                    While Reader.Peek > -1
                        message &= Convert.ToChar(Reader.Read()).ToString
                    End While

                    Console.WriteLine("RECV: " & message)
                End If
            Loop


            Console.Read()
        End Sub
    End Class

    Class Client
        Sub Main(args As String())
            Dim hostName = Dns.GetHostName()
            Dim addresses As IPAddress() = Dns.GetHostEntry(hostName).AddressList()
            For Each hostAdr As IPAddress In addresses
                Console.WriteLine("Name: " & hostName & " IP Address: " & hostAdr.ToString())
                If hostAdr.AddressFamily.ToString() = "InterNetwork" Then
                    Console.WriteLine("    [Potential client IP]")
                End If
            Next
            Console.Write(vbCrLf & "Enter host IP: ")
            Dim HostIP = Console.ReadLine()

            Dim host As New Sockets.TcpClient(HostIP, 65535)
            Dim streamWriter As New IO.StreamWriter(host.GetStream())
            streamWriter.Write(hostName)
            streamWriter.Flush()

            Console.Read()
        End Sub
    End Class
End Module