param($installPath, $toolsPath, $package, $project)

function Delete-Temporary-File 
{
    Write-Host "Delete temporary file"

    $project.ProjectItems | Where-Object { $_.Name -eq 'readme.md' } | Foreach-Object {
        Remove-Item ( $_.FileNames(0) )
        $_.Remove() 
    }
}

function Update-Project-Files
{
    Write-Host "Update CodeSmith Project files"

    $project.ProjectItems | Foreach-Object {
		 Update-Project-File $_
    }
}

function Update-Project-File ( $projectItem )
{
	if ($projectItem.Kind -eq "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}") {
		$projectItem.ProjectItems | Foreach-Object {
			Update-Project-File $_
		}
	}
	elseif ($projectItem.Name.endswith(".csp", $true, $null)) {
		 Update-Project $projectItem.FileNames(0) $toolsPath
    }
}

function Update-Project ( $projectFile, $toolPath )
{
    Write-Host "Update Project File: " $projectFile 

    $projectDirectory = [IO.Path]::GetDirectoryName($projectFile)
    $newPath = [System.IO.Path]::Combine($toolPath, "CSharp\Entity.cst")        
    $newPath = Get-RelativePath $projectDirectory $newPath 
    
    Write-Host "New Template Path: " $newPath
    
    $xml = New-Object xml 
    $xml.Load($projectFile)

    $xml.codeSmith.propertySets.propertySet | where { $_.template.endswith("Entity.cst") } | foreach {
        $_.template = [string]$newPath
        $xml.Save( $projectFile )
    }
}

function Get-RelativePath ($folder, $filePath)
{
    $from = $folder = Split-Path $folder -NoQualifier -Resolve:$Resolve
    $to = $filePath = Split-Path $filePath -NoQualifier -Resolve:$Resolve

    while($from -and $to -and ($from -ne $to)) {
        if($from.Length -gt $to.Length) {
            $from = Split-Path $from
        } else {
            $to = Split-Path $to
        }
    }

    $filepath = $filepath -replace "^"+[regex]::Escape($to)+"\\"
    $from = $folder
    
    while($from -and $to -and $from -gt $to ) {
        $from = Split-Path $from
        $filepath = Join-Path ".." $filepath
    }
    
    return $filepath
}


Write-Host "Running install.ps1"
Update-Project-Files
Delete-Temporary-File