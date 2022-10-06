Imports System
Imports System.Data
Imports System.Globalization
Imports System.Text.RegularExpressions

Module Program

    ReadOnly Operations As String() = {
                                       "Pi",
                                       "*",
                                       "/",
                                       "+",
                                       "-",
                                       "Abs",
                                       "Acos",
                                       "Acosh",
                                       "Asin",
                                       "Asinh",
                                       "Atan",
                                       "Atan2",
                                       "Atanh",
                                       "Cbrt",
                                       "Ceil",
                                       "Cos",
                                       "Cosh",
                                       "Exp",
                                       "Floor",
                                       "Log",
                                       "Ln",
                                       "Log10",
                                       "Log2",
                                       "Pow",
                                       "Sign",
                                       "Sin",
                                       "Sinh",
                                       "Sqrt",
                                       "Tan",
                                       "Tanh"
                                      }

    ReadOnly Pattern As String = "[+-]?([0-9]+\.?[0-9]*|\.[0-9]+)(E-([0-9]+))?"

    Function Digest(Expression As String) As String
        Dim DigestedExpression As String = Expression.Replace(" ", String.Empty)
        Dim OldDigestedExpression As String = String.Empty
        While DigestedExpression <> OldDigestedExpression
            OldDigestedExpression = DigestedExpression
            For Each Operation As String In Operations
                If Operation = "Pi" Then
                    For Each Match As Match In Regex.Matches(DigestedExpression, Operation, RegexOptions.IgnoreCase)
                        DigestedExpression = DigestedExpression.Replace(Match.Value, CalculateOperation(Operation).ToString())
                    Next
                ElseIf Operation = "*" Then
                    For Each Match As Match In Regex.Matches(DigestedExpression, Pattern + "\*" + Pattern, RegexOptions.IgnoreCase)
                        DigestedExpression = DigestedExpression.Replace(Match.Value, "+" + CalculateExpression(Match.Value))
                    Next
                ElseIf Operation = "/" Then
                    For Each Match As Match In Regex.Matches(DigestedExpression, Pattern + "/" + Pattern, RegexOptions.IgnoreCase)
                        DigestedExpression = DigestedExpression.Replace(Match.Value, "+" + CalculateExpression(Match.Value))
                    Next
                ElseIf Operation = "+" Then
                    For Each Match As Match In Regex.Matches(DigestedExpression, Pattern + "\+" + Pattern, RegexOptions.IgnoreCase)
                        DigestedExpression = DigestedExpression.Replace(Match.Value, "+" + CalculateExpression(Match.Value))
                    Next
                ElseIf Operation = "-" Then
                    For Each Match As Match In Regex.Matches(DigestedExpression, Pattern + "-" + Pattern, RegexOptions.IgnoreCase)
                        DigestedExpression = DigestedExpression.Replace(Match.Value, "+" + CalculateExpression(Match.Value))
                    Next
                ElseIf Operation = "Pow" OrElse Operation = "Atan2" Then
                    For Each GlobalMatch As Match In Regex.Matches(DigestedExpression, Operation + "\(" + Pattern + "," + Pattern + "\)", RegexOptions.IgnoreCase)
                        For Each NumMatch As Match In Regex.Matches(GlobalMatch.Value, Pattern, RegexOptions.IgnoreCase)
                            If NumMatch.NextMatch().Value.Trim() = String.Empty Then Continue For
                            DigestedExpression = DigestedExpression.Replace(GlobalMatch.Value, CalculateOperation(Operation, Convert.ToDouble(NumMatch.Value), Convert.ToDouble(NumMatch.NextMatch().Value)).ToString())
                        Next
                    Next
                Else
                    For Each GlobalMatch As Match In Regex.Matches(DigestedExpression, Operation + "\(" + Pattern + "\)", RegexOptions.IgnoreCase)
                        For Each NumMatch As Match In Regex.Matches(GlobalMatch.Value, Pattern, RegexOptions.IgnoreCase)
                            DigestedExpression = DigestedExpression.Replace(GlobalMatch.Value, CalculateOperation(Operation, Convert.ToDouble(NumMatch.Value)).ToString())
                        Next
                    Next
                End If
            Next
        End While
        Return DigestedExpression
    End Function

    Function CalculateOperation(Operation As String, Optional X As Double = 1, Optional Y As Double = 1) As Double
        Select Case Operation
            Case "+"
                Return X + Y
            Case "-"
                Return X - Y
            Case "*"
                Return X * Y
            Case "/"
                Return X / Y
            Case "Abs"
                Return Math.Abs(X)
            Case "Acos"
                Return Math.Acos(X)
            Case "Acosh"
                Return Math.Acosh(X)
            Case "Asin"
                Return Math.Asin(X)
            Case "Asinh"
                Return Math.Asinh(X)
            Case "Atan"
                Return Math.Atan(X)
            Case "Atan2"
                Return Math.Atan2(X, Y)
            Case "Atanh"
                Return Math.Atanh(X)
            Case "Cbrt"
                Return Math.Cbrt(X)
            Case "Ceil"
                Return Math.Ceiling(X)
            Case "Cos"
                Return Math.Cos(X)
            Case "Cosh"
                Return Math.Cosh(X)
            Case "Exp"
                Return Math.Exp(X)
            Case "Floor"
                Return Math.Floor(X)
            Case "Log", "Ln"
                Return Math.Log(X)
            Case "Log10"
                Return Math.Log10(X)
            Case "Log2"
                Return Math.Log2(X)
            Case "Pi"
                Return Math.PI
            Case "Pow"
                Return Math.Pow(X, Y)
            Case "Sign"
                Return Math.Sign(X)
            Case "Sin"
                Return Math.Sin(X)
            Case "Sinh"
                Return Math.Sinh(X)
            Case "Sqrt"
                Return Math.Sqrt(X)
            Case "Tan"
                Return Math.Tan(X)
            Case "Tanh"
                Return Math.Tanh(X)
            Case Else
                Return 0
        End Select
    End Function

    Function CalculateExpression(Expression As String) As String
        Try
            Dim Result As String = New DataTable().Compute(Expression, Nothing).ToString()
            Return Result
        Catch
            Return "Invalid expression"
        End Try
    End Function

    Sub Main(Args As String())
        CultureInfo.DefaultThreadCurrentCulture = New CultureInfo("en-US")
        CultureInfo.DefaultThreadCurrentUICulture = New CultureInfo("en-US")
        If Args.Length = 1 Then
            Console.WriteLine(CalculateExpression(Digest(Args(0))))
            Return
        End If
        Console.WriteLine("Expression Calculator v1.0.0")
        While True
            Console.Write("> ")
            Console.WriteLine(CalculateExpression(Digest(Console.ReadLine())))
        End While
    End Sub

End Module
