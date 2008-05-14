-------------------------------------------------------------
CodeSmith DBDocumenter Templates v3.0
Author:  Jason Alexander (jason@ngallery.org), Eric J. Smith
Website: http://www.ngallery.org/dbdocumenter/
-------------------------------------------------------------

Requirements:
  * CodeSmith v3.0 or higher
  * Actipro's CodeHighlighter component (ActiproSoftware.CodeHighlighter) (included)

ActiproSoftware.CodeHighlighter is a great syntax highlighting component that is highlighly customizable, fast, and (best of all) FREE! Please see http://www.codehighlighter.com/ for more details.

Setup & Running:

1.) Open dbdocs_properties.xml in your favorite text editor and edit your properties for your specific environment:
  
  - OutputDirectory
  This is the literal path that you want your documentation to generate to.  If this value is empty it will default to a sub-directorty called "output" in the directory that the template is located. 
  
  - connectionString
  This is the connection string to the target database that you will be creating documentation for. Keep in mind that you must run this in the context of a user that has permissions to the Master database on the target server.
  
  - ServerName
  Simply the name of the target server. Unfortunately, at this time we cannot pull this information from the connection string.
  
  - DocumentationTitle
  This is the title of your documentation. This is placed as the literal title of each HTML page, and in a header on each page.

2.) Click build.bat and let it run! Keep in mind, the batch file relies on the CodeSmith console application (cs.exe), and hardcodes the assumed path to this executable (c:\Program Files\CodeSmith\v3.0\). If this differs, please edit the build.bat accordingly.

3.) Check your output directory, and you should have a ton of newly built documentation!


  
