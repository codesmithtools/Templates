---------------------------------------------------
  CSLA 2.x CodeSmith Templates for C#
  Version: 1.1.0 RC
  Released: October 4, 2006
  Author: Ricky A. Supit (rsupit@hotmail.com)
  Download Site: http://www.codeplex.com/Release/ProjectReleases.aspx?ProjectName=CSLAcontrib
  Workspace was moved from: http://workspaces.gotdotnet.com/cslacstemplate
  
---------------------------------------------------

This is a collection of CodeSmith templates to generate CSLA 2.x business object for C#.  I completely rewrote the 1.x version in order to support code generation from xml file.  In addition to support xml data source, there are numbers of new features that was added such as view as data source, and partial class generation. 

Unlike version 1.x, these templates no longer used TableSchema and ColumnSchema directly as data source to generate Csla business object. The current template uses custom classes called ObjectInfo and PropertyInfo. This implementation permits one template to have multiple data sources such as table, view, and xml file.

This collection templates is design to work with the freeware version (v2.6) of CodeSmith and has not been tested with other version. 

This set of templates and the template base support code are provided free charge. You're free to use and modify anyway you want it. The only request is for you to give back and share any improvement made to these templates if you feel many would benefit from it.  I reserve the right to include (or not include) any of your improvements in future version and in return give credit to you.

Enjoy them,
Ricky Supit

===================================================
 Contributor(s)
===================================================
- Tom Cooley (TEC)
  Stored Procedure support.
  
===================================================
 Known Issues
===================================================
- Split class by using partial classes requires Csla Framework version 2.0.1 or later.  Initialize() method was added in this version so hook for validation rules can be initialized before AddBusinessRules method is called.
- Split class by using base/abstract class requires Csla Framework version 2.0.1 or later.  The earlier version was not able to find DataPortal_xxx methods when it's in generic abstract class.

===================================================
 How to Use
===================================================
Method A: 
Generate single Business Object by directly open specific template for Csla object type.
1. Double click on template based on Csla object type.
2. Enter the following:
   - ClassNamespace: type namespace of your generated object.
   - ObjectName: type name of your generated object.
3. If you choose to generate your class based on a table
   - RootTable: select table from table picker.
   If you choose to generate your class based on a view
   - RootView: select view from view picker.
   - UniqueColumnNames: type unique column(s) on selected view.
   If you choose to generate your class based on StoredProcedure (Fetch SP).
   - RootCommand: select stored procedure from stored procedure picker.
   - ResultSetIndex: change this value is the index is different from the result sets returned from sp. (this is usually the case for child object)
   - UniqueColumnNames: required for child objects; type unique columns of chosen (ResultSetIndex) result set.
4. Based on your generation method, enter the following:
   - GenerationMethod: Select one:
     Single: to generate once and modify it as needed.
     SplitBase: split classes using abstract class as base and implemented on user class.
     SplitPartial: split classes using partial classes.
   - ClassType: if you choose split method, select either 'generated' or 'user' class.
5. Security options on your generated object.
   - AuthorizationRules: 'True' to include access security in factory methods.
   - PropertyAuthorization: 
     To include property level access security. options are 'Both', 'Read', 'Write'
6. Click Generate Button to generate code.
7. Copy the generated template output and paste it into your project.

Method B: 
Generate single Business Object by open a master template
1. Double click on Csla.cst
2. Enter the following:
   - OutputToFile: 'True' to generate as file, or 'False' to generate on template output screen.
   - OutputDirectory: select folder where output file will be generated.
   - Template: select Csla object type.
3. Follow step 2 to 6 of Method A.
4. If you choose OutputToFile=False, Copy the generated template output and paste it into your project.

Method C:
Generate multiple Business Objects using xml source file.
1. Double click on CslaXml.cst
2. Enter the following:
   - OutputToFile: 'True' to generate as file, or 'False' to generate on template output screen.
   - OutputDirectory: select folder where output file will be generated.
   - XmlFileName: select xml file from file picker.
3. Click Generate Button to generate files.

===================================================
 Templates
===================================================
- EditableRoot (ER)
- EditableRootList (ERL)
- EditableChild (EC)
- EditableChildList (ECL)
- EditableSwitchable (ES)
- ReadOnlyRoot (ROR)
- ReadOnlyRootList (RORL)
- ReadOnlyChild (ROC)
- ReadOnlyChildList (ROCL)
- NameValueList (NVL)
- Csla
- CslaXml
  
===================================================
 Parameters
===================================================
--------------------------------------------------------------------------------------------
 Parameter               Description                                      Template
--------------------------------------------------------------------------------------------
 ClassNameSpace          Namespace of your generated object               All
 ObjectName              Name of your generated object.                   All
 ParentName              Name of your parent object.                      EC, ECL, ES
 ChildName               Name of your list item/child object.             ERL, ECL, RORL, ROCL
 RootTable               DB Table name to be used as data source.         All
 RootView                DB View  name to be used as data source.         All
 RootCommand             DB Stored Procedure to be used as data source.   All
 XmlFileName             Xml file contains metadata that describe         CslaXml
                         the objects.
 ResultSetIndex          Result set index (zero based) which result set   All
                         to use from the stored procedure.
 UniqueColumnNames       Unique column name(s) when RootView or           ER, EC, ROR, ROC, ES
                         RootCommand is the data source.
 FilterColumnNames       Filter column name(s) as retrieve criteria       ERL, RORL
                         when RootView is the data source.
 ChildCollectionNames    Child collection object name(s).                 ER, EC, ROR, ROC, ES
 ChildPropertyNames      Property name(s) accessing the child             ER, EC, ROR, ROC, ES
                         collection object(s).
 AuthorizationRules      Option to include/exclude access security        All
                         in factory methods.
 PropertyAuthrization    Option to include/exclude property level         All
                         access security.
                         - Read : Access security on property getter.
                         - Write : Access security on property setter.
                         - Both : Access security on both getter and 
                                  setter
 TransactionalType       Data access transactional type.                  All
                         - None : Do not use transaction.
                         - Ado : Use ado transaction.
                         - EnterpriseService : 
                           Use Enterprise Service transaction.
                         - TransactionScope : 
                           Use TransactionScope object.
 GenerationMethod        Generation method.                               All
                         - Single : Generate as single class.
                         - SplitBase : Split generated code as base class 
                                       and modifiable code as user class.
                         - SplitPartial : Split generated and modifiable 
                                        code on separate partial classes.
 ClassType               Class type when select SplitBase or SplitPartial All
                         - Generated : class contains generated code.
                         - User : class contains user code.
 OutputToFile            Option to generate as file or to generate        Csla, CslaXml
                         on screen.
 OutputDirectory         Folder name where file(s) will be generated.     Csla, CslaXml
--------------------------------------------------------------------------------------------
=================================================== 
 Next
===================================================
There are some features in the following list that I wish to include/build in order to improve this template collection. There is no timeframe on when any of these will be implemented. They might even just stay there as a wish list or be removed from the list.  I reserved the right to change my mind. ;)
If anybody is interested in helping out, please let me know.
Template:
- NUnit
- Frequently used combo objects
- Frequently used command object
- Solution/Project
- Stored Procedure
Template Base
- Support to load entire object tree.
Metadata:
- Support objects define as object tree.
- Add more xml attributes to improve coverage on Csla object definition
Object Designer:
- Create application that produce xml file to be generated by this template. 
  This application can be either a new designer or an add-on to existing designer app.

=================================================== 
 Links
===================================================
If you have not heard about Component-based Scalable Logical Architecture (CSLA), here are some useful links.
Book:           http://www.amazon.com/gp/product/1590596323/sr=8-1/qid=1144015081/ref=pd_bbs_1/103-0015541-1570234?%5Fencoding=UTF8
                http://www.amazon.com/gp/product/1590596315/sr=8-3/qid=1144015081/ref=pd_bbs_3/103-0015541-1570234?%5Fencoding=UTF8
Rocky Lhotka:   http://www.lhotka.net/
CSLA Reference: http://www.onelittlevictory.com/
                http://www.primos.com.au/primos/Articles/CSLAversion2whatsinitforme/tabid/67/Default.aspx
Community:      http://forums.lhotka.net/forums/default.aspx

CodeSmith is a template based code generation tool. Here are some useful links.
Website:        http://www.codesmithtools.com/
Community:      http://community.codesmithtools.com/forums/12/ShowForum.aspx
Download:       http://www.codesmithtools.com/freeware.aspx
