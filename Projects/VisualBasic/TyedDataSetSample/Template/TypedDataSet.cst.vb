Imports System 
Imports System.Text 
Imports System.ComponentModel 
Imports CodeSmith.Engine 
Imports System.Data 
Imports SchemaExplorer
Imports Microsoft.VisualBasic

Namespace CodeSmith.BaseTemplates
    Public Class SqlCodeTemplate
        Inherits CodeTemplate
        Public Function GetSqlParameterStatements(ByVal statementPrefix As String, ByVal column As ColumnSchema) As String
            Return GetSqlParameterStatements(statementPrefix, column, "sql")
        End Function

        Public Function GetSqlParameterStatements(ByVal statementPrefix As String, ByVal column As ColumnSchema, ByVal sqlObjectName As String) As String
            Dim statements As String = ControlChars.NewLine + statementPrefix & sqlObjectName & ".AddParameter(""@" & column.Name & """"", SqlDbType." & GetSqlDbType(column) & ", this." & GetPropertyName(column) & GetSqlParameterExtraParams(statementPrefix, column)
            Return statements.Substring(statementPrefix.Length + 2)
        End Function

        Public Function GetCamelCaseName(ByVal value As String) As String
            Return value.Substring(0, 1).ToLower() + value.Substring(1)
        End Function

        Public Function GetSpacedName(ByVal value As String) As String
            Dim spacedName As New StringBuilder()
            For i As Integer = 0 To value.Length - 1

                If i > 0 AndAlso i < value.Length - 1 AndAlso value.Substring(i, 1).ToUpper() = value.Substring(i, 1) Then
                    spacedName.Append(" ")
                End If
                spacedName.Append(value(i))
            Next

            Return spacedName.ToString()
        End Function

        Public Function GetClassName(ByVal value As String) As String
            Return value.Replace(" ", "")
        End Function

        Public Function GetFillByIndexName(ByVal index As IndexSchema) As String
            Dim fillByIndexName As New StringBuilder()

            fillByIndexName.Append("FillBy")
            For i As Integer = 0 To index.MemberColumns.Count - 1

                fillByIndexName.Append(index.MemberColumns(i).Name.Replace(" ", ""))
                If i < index.MemberColumns.Count - 1 Then
                    fillByIndexName.Append("And")
                End If
            Next

            Return fillByIndexName.ToString()
        End Function

        Public Function GetFillByIndexParameters(ByVal index As IndexSchema) As String
            Dim fillByIndexParameters As New StringBuilder()
            For i As Integer = 0 To index.MemberColumns.Count - 1

				fillByIndexParameters.Append(GetCamelCaseName(index.MemberColumns(i).Name))
				fillByIndexParameters.Append(" AS ")
                fillByIndexParameters.Append(GetCSharpVariableType(index.MemberColumns(i)))              

                If i < index.MemberColumns.Count - 1 Then
                    fillByIndexParameters.Append(", ")
                End If
            Next

            Return fillByIndexParameters.ToString()
        End Function

        Public Function GetMemberVariableName(ByVal value As String) As String
            Dim memberVariableName As String = "_" + GetCamelCaseName(value)

            Return memberVariableName
        End Function

        Public Function GetSqlParameterExtraParams(ByVal statementPrefix As String, ByVal column As ColumnSchema) As String
            If SizeMatters(column) AndAlso PrecisionMatters(column) Then
                Return ");" & Chr(13) & "" & Chr(10) & "" + statementPrefix + "prm.Scale = " + column.Scale + ";" & Chr(13) & "" & Chr(10) & "" + statementPrefix + "prm.Precision = " + column.Precision + ";"
            ElseIf SizeMatters(column) Then
                Return ", " + column.Size + ");"
            Else
                Return ");"
            End If
        End Function

        Public Function SizeMatters(ByVal column As ColumnSchema) As Boolean
            Select Case column.DataType
                Case DbType.[String], DbType.AnsiString, DbType.AnsiStringFixedLength, DbType.[Decimal]
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Public Function PrecisionMatters(ByVal column As ColumnSchema) As Boolean
            Select Case column.DataType
                Case DbType.[Decimal]
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Public Function GetMemberVariableDeclarationStatement(ByVal column As ColumnSchema) As String
            Return GetMemberVariableDeclarationStatement("protected", column)
        End Function

        Public Function GetMemberVariableDeclarationStatement(ByVal protectionLevel As String, ByVal column As ColumnSchema) As String
            Dim statement As String = protectionLevel + " "
            statement += GetCSharpVariableType(column) + " " + GetMemberVariableName(column.Name)

            Dim defaultValue As String = GetMemberVariableDefaultValue(column)
            If defaultValue <> "" Then
                statement += " = " + defaultValue
            End If

            statement += ";"

            Return statement
        End Function

        Public Function GetSqlReaderAssignmentStatement(ByVal column As ColumnSchema, ByVal index As Integer) As String
            Dim statement As String = "if (!reader.IsDBNull(" + index.ToString() + ")) "
            statement += GetMemberVariableName(column.Name) + " = "

            If column.Name.EndsWith("TypeCode") Then
                statement += "(" + column.Name + ")"
            End If

            statement += "reader." + GetReaderMethod(column) + "(" + index.ToString() + ");"

            Return statement
        End Function

        Public Function GetValidateStatements(ByVal table As TableSchema, ByVal statementPrefix As String) As String
            Dim statements As String = ""

            For Each column As ColumnSchema In table.Columns
                If IncludeEmptyCheck(column) Then
                    statements += "" & Chr(13) & "" & Chr(10) & "" + statementPrefix + "if (" + GetMemberVariableName(column.Name) + " == " + GetMemberVariableDefaultValue(column) + ") this.ValidationErrors.Add(new ValidationError(ValidationTypeCode.Required, """ + table.Name + """, """ + column.Name + """, """ + column.Name + " is required.""));"
                End If
                If IncludeMaxLengthCheck(column) Then
                    statements += "" & Chr(13) & "" & Chr(10) & "" + statementPrefix + "if (" + GetMemberVariableName(column.Name) + ".Length > " + column.Size.ToString() + ") this.ValidationErrors.Add(new ValidationError(ValidationTypeCode.MaxLength, """ + table.Name + """, """ + column.Name + """, """ + column.Name + " is too long.""));"
                End If
            Next

            Return statements.Substring(statementPrefix.Length + 2)
        End Function

        Public Function GetPropertyName(ByVal column As ColumnSchema) As String
            Dim propertyName As String = column.Name

            If propertyName = column.Table.Name + "Name" Then
                Return "Name"
            End If
            If propertyName = column.Table.Name + "Description" Then
                Return "Description"
            End If

            If propertyName.EndsWith("TypeCode") Then
                propertyName = propertyName.Substring(0, propertyName.Length - 4)
            End If

            Return propertyName
        End Function

        Public Function GetCSharpVariableType(ByVal column As ColumnSchema) As String
            If column.Name.EndsWith("TypeCode") Then
                Return column.Name
            End If

            Select Case column.DataType
                Case DbType.AnsiString
                    Return "String"
                Case DbType.AnsiStringFixedLength
                    Return "String"
                Case DbType.Binary
                    Return "Byte[]"
                Case DbType.[Boolean]
                    Return "Boolean"
                Case DbType.[Byte]
                    Return "Byte"
                Case DbType.Currency
                    Return "Decimal"
                Case DbType.[Date]
                    Return "DateTime"
                Case DbType.DateTime
                    Return "DateTime"
                Case DbType.[Decimal]
                    Return "Decimal"
                Case DbType.[Double]
                    Return "Double"
                Case DbType.Guid
                    Return "Guid"
                Case DbType.Int16
                    Return "Short"
                Case DbType.Int32
                    Return "Integer"
                Case DbType.Int64
                    Return "Long"
                Case DbType.[Object]
                    Return "Object"
                Case DbType.[SByte]
                    Return "SByte"
                Case DbType.[Single]
                    Return "Float"
                Case DbType.[String]
                    Return "String"
                Case DbType.StringFixedLength
                    Return "String"
                Case DbType.Time
                    Return "TimeSpan"
                Case DbType.UInt16
                    Return "UShort"
                Case DbType.UInt32
                    Return "UInt"
                Case DbType.UInt64
                    Return "ULong"
                Case DbType.VarNumeric
                    Return "Decimal"
                Case Else
                    Return "__UNKNOWN__" + column.NativeType
            End Select
        End Function

        Public Function GetReaderMethod(ByVal column As ColumnSchema) As String
            Select Case column.DataType
                Case DbType.[Byte]
                    Return "GetByte"
                Case DbType.Int16
                    Return "GetInt16"
                Case DbType.Int32
                    Return "GetInt32"
                Case DbType.Int64
                    Return "GetInt64"
                Case DbType.AnsiStringFixedLength, DbType.AnsiString, DbType.[String], DbType.StringFixedLength
                    Return "GetString"
                Case DbType.[Boolean]
                    Return "GetBoolean"
                Case DbType.Guid
                    Return "GetGuid"
                Case DbType.Currency, DbType.[Decimal]
                    Return "GetDecimal"
                Case DbType.DateTime, DbType.[Date]
                    Return "GetDateTime"
                Case Else
                    Return "__SQL__" + column.DataType
            End Select
        End Function

        Public Function GetSqlDbType(ByVal column As ColumnSchema) As String
            Select Case column.NativeType
                Case "bigint"
                    Return "BigInt"
                Case "binary"
                    Return "Binary"
                Case "bit"
                    Return "Bit"
                Case "char"
                    Return "Char"
                Case "datetime"
                    Return "DateTime"
                Case "decimal"
                    Return "Decimal"
                Case "float"
                    Return "Float"
                Case "image"
                    Return "Image"
                Case "int"
                    Return "Int"
                Case "money"
                    Return "Money"
                Case "nchar"
                    Return "NChar"
                Case "ntext"
                    Return "NText"
                Case "numeric"
                    Return "Decimal"
                Case "nvarchar"
                    Return "NVarChar"
                Case "real"
                    Return "Real"
                Case "smalldatetime"
                    Return "SmallDateTime"
                Case "smallint"
                    Return "SmallInt"
                Case "smallmoney"
                    Return "SmallMoney"
                Case "sql_variant"
                    Return "Variant"
                Case "sysname"
                    Return "NChar"
                Case "text"
                    Return "Text"
                Case "timestamp"
                    Return "Timestamp"
                Case "tinyint"
                    Return "TinyInt"
                Case "uniqueidentifier"
                    Return "UniqueIdentifier"
                Case "varbinary"
                    Return "VarBinary"
                Case "varchar"
                    Return "VarChar"
                Case Else
                    Return "__UNKNOWN__" + column.NativeType
            End Select
        End Function

        Public Function GetMemberVariableDefaultValue(ByVal column As ColumnSchema) As String
            Select Case column.DataType
                Case DbType.Guid
                    Return "Guid.Empty"
                Case DbType.AnsiString, DbType.AnsiStringFixedLength, DbType.[String], DbType.StringFixedLength
                    Return "String.Empty"
                Case Else
                    Return ""
            End Select
        End Function

        Public Function IncludeMaxLengthCheck(ByVal column As ColumnSchema) As Boolean
            Select Case column.DataType
                Case DbType.AnsiString, DbType.AnsiStringFixedLength, DbType.[String], DbType.StringFixedLength
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Public Function IncludeEmptyCheck(ByVal column As ColumnSchema) As Boolean
            If column.IsPrimaryKeyMember OrElse column.AllowDBNull OrElse column.Name.EndsWith("TypeCode") Then
                Return False
            End If

            Select Case column.DataType
                Case DbType.Guid
                    Return True
                Case DbType.AnsiString, DbType.AnsiStringFixedLength, DbType.[String], DbType.StringFixedLength
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Public Function GetSqlParameterStatement(ByVal column As ColumnSchema) As String
            Return GetSqlParameterStatement(column, False)
        End Function

        Public Function GetSqlParameterStatement(ByVal column As ColumnSchema, ByVal isOutput As Boolean) As String
            Dim param As String = "@" + column.Name + " " + column.NativeType

            Select Case column.DataType
                Case DbType.[Decimal]
                    param += "(" + column.Precision + ", " + column.Scale + ")"
                    Exit Select
                Case DbType.AnsiString, DbType.AnsiStringFixedLength, DbType.[String], DbType.StringFixedLength
                    If column.Size > 0 Then
                        param += "(" + column.Size + ")"
                    End If
                    Exit Select
            End Select

            If isOutput Then
                param += " OUTPUT"
            End If

            Return param
        End Function
    End Class
End Namespace