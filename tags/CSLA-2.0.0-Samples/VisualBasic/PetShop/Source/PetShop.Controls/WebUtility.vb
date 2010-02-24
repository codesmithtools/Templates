Imports System.Web
Imports System.Web.Caching
Imports System.Configuration
Imports System.Text.RegularExpressions

Imports PetShop.Business

''' <summary>
''' Collection of utility methods for web tier
''' </summary>
Public Class WebUtility

    Private Const CATEGORY_NAME_KEY As String = "category_name_{0}"
    Private Const PRODUCT_NAME_KEY As String = "product_name_{0}"
    Private Const REDIRECT_URL As String = "~/Search.aspx?keywords={0}"
    Private Shared ReadOnly enableCaching As Boolean = Boolean.Parse(ConfigurationManager.AppSettings("EnableCaching"))

    ''' <summary>
    ''' Method to make sure that user's inputs are not malicious
    ''' </summary>
    ''' <param name="text">User's Input</param>
    ''' <param name="maxLength">Maximum length of input</param>
    ''' <returns>The cleaned up version of the input</returns>
    Public Shared Function InputText(ByVal text As String, ByVal maxLength As Integer) As String
        text = text.Trim()
        If String.IsNullOrEmpty(text) Then
            Return String.Empty
        End If
        If text.Length > maxLength Then
            text = text.Substring(0, maxLength)
        End If
        text = Regex.Replace(text, "[\s]{2,}", " ")
        'two or more spaces
        text = Regex.Replace(text, "(<[b|B][r|R]/*>)+|(<[p|P](.|\n)*?>)", vbLf)
        '<br>
        text = Regex.Replace(text, "(\s*&[n|N][b|B][s|S][p|P];\s*)+", " ")
        '&nbsp;
        text = Regex.Replace(text, "<(.|\n)*?>", String.Empty)
        'any other tags
        text = text.Replace("'", "''")
        Return text
    End Function

    ''' <summary>
    ''' Method to check whether input has other characters than numbers
    ''' </summary>
    Public Shared Function CleanNonWord(ByVal text As String) As String
        Return Regex.Replace(text, "\W", "")
    End Function

    ''' <summary>
    ''' Method to redirect user to search page
    ''' </summary>
    ''' <param name="key">Search keyword</param> 
    Public Shared Sub SearchRedirect(ByVal key As String)
        HttpContext.Current.Response.Redirect(String.Format(REDIRECT_URL, InputText(key, 255)))
    End Sub

    ''' <summary>
    ''' Method to retrieve and cache category name by its ID
    ''' </summary>
    ''' <param name="categoryId">Category id</param>
    ''' <returns>Category name</returns>
    Public Shared Function GetCategoryName(ByVal categoryId As String) As String
        If Not enableCaching Then
            Return Category.GetByCategoryId(categoryId).Name
        End If

        Dim cacheKey As String = String.Format(CATEGORY_NAME_KEY, categoryId)

        ' Check if the data exists in the data cache
        Dim data As String = DirectCast(HttpRuntime.Cache(cacheKey), String)
        If data = Nothing Then
            ' Caching duration from Web.config
            Dim cacheDuration As Integer = Integer.Parse(ConfigurationManager.AppSettings("CategoryCacheDuration"))

            ' If the data is not in the cache then fetch the data from the business logic tier
            data = Category.GetByCategoryId(categoryId).Name

            ' Store the output in the data cache, and Add the necessary AggregateCacheDependency object
            HttpRuntime.Cache.Add(cacheKey, data, Nothing, DateTime.Now.AddHours(cacheDuration), Cache.NoSlidingExpiration, CacheItemPriority.High, _
             Nothing)
        End If

        Return data
    End Function

    ''' <summary>
    ''' Method to retrieve and cache product name by its ID
    ''' </summary>
    ''' <param name="productId">Product id</param>
    ''' <returns>Product name</returns>
    Public Shared Function GetProductName(ByVal productId As String) As String
        If Not enableCaching Then
            Return Product.GetByProductId(productId).Name
        End If

        Dim cacheKey As String = String.Format(PRODUCT_NAME_KEY, productId)

        ' Check if the data exists in the data cache
        Dim data As String = DirectCast(HttpRuntime.Cache(cacheKey), String)

        If data = Nothing Then
            ' Caching duration from Web.config
            Dim cacheDuration As Integer = Integer.Parse(ConfigurationManager.AppSettings("ProductCacheDuration"))

            ' If the data is not in the cache then fetch the data from the business logic tier
            data = Product.GetByProductId(productId).Name

            ' Store the output in the data cache, and Add the necessary AggregateCacheDependency object
            HttpRuntime.Cache.Add(cacheKey, data, Nothing, DateTime.Now.AddHours(cacheDuration), Cache.NoSlidingExpiration, CacheItemPriority.High, _
             Nothing)
        End If

        Return data
    End Function
End Class