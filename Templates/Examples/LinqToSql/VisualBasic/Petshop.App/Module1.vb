Imports Petshop.Data

Module Module1

    Sub Main()

        GetTopFiveProducts()
        GetPenguin()

        Console.Out.WriteLine("Press enter to exit.")
        Console.In.ReadLine()

    End Sub

    Sub GetTopFiveProducts()

        Dim context As New PetshopDataContext()
        Dim products As List(Of Product) = context.Product.Take(5).ToList()

        Console.Out.WriteLine("Top 5 Products")
        Dim x As Integer = 0
        While x < 5
            Console.Out.WriteLine("{0}: {1}", x + 1, products(x).Name)
            System.Math.Max(System.Threading.Interlocked.Increment(x), x - 1)
        End While
        Console.Out.WriteLine()

    End Sub

    Sub GetPenguin()
        Dim key As String = "BD-02"

        Dim context As New PetshopDataContext()
        Dim product As Product = context.Manager.Product.GetByKey(key)

        If product Is Nothing Then
            Console.Out.WriteLine("No entry found for key {0}.")
        Else
            Console.Out.WriteLine("Found one entry for key {0}:", key)
            Console.Out.WriteLine("{0} - {1}", product.Name, product.Descn)
        End If
        Console.Out.WriteLine()
    End Sub

End Module
