param($rootPath, $toolsPath, $package, $project)

# When the package is installed into a project we need to perform the following steps:
# 1. Create a .wpp.targets file if it doesn't already exist
# 2. If the .wpp.targets doesn't have an import for aspnet_merge.targets then insert one

if ($project -eq $null) {
    $project = Get-Project
}

$pwMsbuildLabel = "AspNetMergeNuGetTarget"

function WriteParamsToFile {
    param([string]$filePath)
    
    $strToWrite="rootPath={0}`r`ntoolsPath={1}`r`npackage={2}`r`nproject path={3}`r`n" -f $rootPath, $toolsPath, $package, $project.FullName
    
    Write-Debug ("params: {0}" -f $strToWrite)
    
    $strToWrite | Out-File $filePath
}

function CreateWppTargetsFile($project, $toolsPath) {    
    $projName = $project.Name
    $projDirectory = $project.Properties.Item('FullPath').Value
    
    $wppTargetsPath = Join-Path $projDirectory -ChildPath ("{0}.wpp.targets" -f $projName)
    $wppTargetsExists = Test-Path $wppTargetsPath
    
    
    $msbuildProj = $null
    $projCollection = New-Object Microsoft.Build.Evaluation.ProjectCollection
    if(!($wppTargetsExists)) {
        Write-Debug ("    Creating MSBuild file at {0}" -f $wppTargetsPath) | Out-Null
        # create a new file there        
        $msbuildProj = (New-Object Microsoft.Build.Evaluation.Project -ArgumentList $projCollection)
        $msbuildProj.Save($wppTargetsPath)
    }
    else {
        # file already exists let's load 
        Write-Debug ("    MSBuild file already exists at {0}" -f $wppTargetsPath) | Out-Null
        $projCollection.LoadProject($wppTargetsPath) | Out-Null
    }
    
    $projRoot = [Microsoft.Build.Construction.ProjectRootElement]::Open($wppTargetsPath)
    # now we need to see if the file has the import that we are looking to add
    $wppTargetsHasImport = DoesProjectHaveImport -projRoot $projRoot
    
	Write-Debug ("    wpp.targets Has Import: {0}" -f $wppTargetsHasImport) | Out-Null
    if(!($wppTargetsHasImport)) {
        # we need to add an import to that file now
        AddImportToWppTargets $projRoot $toolsPath
    }
    
    # add the .wpp.targets file to the project so that it gets checked in
    $project.ProjectItems.AddFromFile($wppTargetsPath) | Out-Null
    $project.Save() | Out-Null
	$projCollection.UnloadProject($projRoot)
}

function AddImportToWppTargets($projRoot, $toolsPath) {

	Write-Debug ("    Adding import to WppTargets") | Out-Null
   
    $targetsPropertyName = "AspNetMergeNuGetTargetsPath"
    $importFileName = "aspnet_merge.targets"
    $importPath = ("{0}\{1}" -f $toolsPath, $importFileName)
    $importCondition = " '`$(AspNetMergeNuGetTargetsPath)'=='' "
    
    # add the property for the import location 
    $propGroup = $projRoot.AddPropertyGroup()
    $ppe = $propGroup.AddProperty($targetsPropertyName,$importPath)
    $e = $ppe.Condition = (" '`$({0})'=='' " -f $ppe.Name)
    $propGroup.Label = $pwMsbuildLabel
    
    # add the import itself
	$importGroup = $projRoot.AddImportGroup()
	$importGroup.Label = $pwMsbuildLabel
    $importStr = ("`$({0})" -f $targetsPropertyName)
    $importElement = $importGroup.AddImport($importStr)
    $importElement.Condition= ("Exists('{0}')" -f $importStr)
    $projRoot.Save() | Out-Null
}

function DoesProjectHaveImport {
    param($projRoot)
        
    $hasImport = $false
    foreach($pie in $projRoot.ImportGroups) {
        # see if it has the expected label
		Write-Debug ("    Checking import {0}" -f $pie) | Out-Null
        if($pie -ne $null -and $pie.Label -ne $null -and $pie.Label.Trim() -ceq $pwMsbuildLabel) {
		    Write-Debug ("    Required import exists: {0}" -f $pie) | Out-Null
            $hasImport = $true
            break
        }               
    }
    
    return $hasImport
}

#WriteParamsToFile -filePath "D:\temp\log-ps.txt"

CreateWppTargetsFile $project $toolsPath
# SIG # Begin signature block
# MIIgSgYJKoZIhvcNAQcCoIIgOzCCIDcCAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQUQ6t00iDPe83XT7rwZrxjCcCa
# G8CgghsfMIIEwzCCA6ugAwIBAgITMwAAAHeHqQZ/e88wOQAAAAAAdzANBgkqhkiG
# 9w0BAQUFADB3MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4G
# A1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSEw
# HwYDVQQDExhNaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0EwHhcNMTUwNjA0MTc0MDQ4
# WhcNMTYwOTA0MTc0MDQ4WjCBszELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hp
# bmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jw
# b3JhdGlvbjENMAsGA1UECxMETU9QUjEnMCUGA1UECxMebkNpcGhlciBEU0UgRVNO
# Ojk4RkQtQzYxRS1FNjQxMSUwIwYDVQQDExxNaWNyb3NvZnQgVGltZS1TdGFtcCBT
# ZXJ2aWNlMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAuB0SjjU3FVee
# I4WMHnSMEDy8zmwDG6eABx9SVkiczwIuYQvt6G/VnG+B+QIVIN0W5+X3zmUpdoQx
# 6sin25cHNlcU3MBZFTYbduA0UrOAeNJRrSD/ps1NxnoPLl0rij7OZ6qrQT5JYRYE
# el8zMo/bkDA6gL+t7/1TmwQJd25YX7w+dREpOnrM4d11CcElVAR1mG9WaiIt4Irm
# NheXuFSd7uP85rZSUSaJG/+EsFQqgq8CgwvoIovBdht32Uqci1Hm6ju9XEUekY8M
# 4/lgctv/LIo3e48O6ml4gBRr9kb7TqolF1hfF73wCpPDANEStjEyoPs3tRpe9Znl
# Q/j5iyscDQIDAQABo4IBCTCCAQUwHQYDVR0OBBYEFOYH/Cr4bOMrDsemu3XomLER
# 1369MB8GA1UdIwQYMBaAFCM0+NlSRnAK7UD7dvuzK7DDNbMPMFQGA1UdHwRNMEsw
# SaBHoEWGQ2h0dHA6Ly9jcmwubWljcm9zb2Z0LmNvbS9wa2kvY3JsL3Byb2R1Y3Rz
# L01pY3Jvc29mdFRpbWVTdGFtcFBDQS5jcmwwWAYIKwYBBQUHAQEETDBKMEgGCCsG
# AQUFBzAChjxodHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtpL2NlcnRzL01pY3Jv
# c29mdFRpbWVTdGFtcFBDQS5jcnQwEwYDVR0lBAwwCgYIKwYBBQUHAwgwDQYJKoZI
# hvcNAQEFBQADggEBAGH8WtqccWHi3ExFFAC8wVo7kpdIoQuTqeuIU4yd9laTM8s8
# iWESm8DXR6MCdNSoLauq+cXCZrVuqYvPxi9TC2R9NvIMhpFm2ZQvVNZgq07Uwmum
# BM6Hc74Q66UHebK/BKyAtUP8DcZO1IQdxqQQP4D7aMDNdFdqailllIfv3rh9Wsw/
# Vdu/DYJTxC7djMqbbZGHtzgTdpjWA/zjQb0aniyD2JQWM2RgfMhjNTzFDxFDVCnY
# IlAuRomWNvcPE7r5N7VIgOeEhwSSyJXKazJTTbupYWRioxbSA4q/KnV4k0L9cydI
# 6zhS3BBTmsAVyevaYKQJ82js/wSgdJBln//09+wwggTsMIID1KADAgECAhMzAAAB
# Cix5rtd5e6asAAEAAAEKMA0GCSqGSIb3DQEBBQUAMHkxCzAJBgNVBAYTAlVTMRMw
# EQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVN
# aWNyb3NvZnQgQ29ycG9yYXRpb24xIzAhBgNVBAMTGk1pY3Jvc29mdCBDb2RlIFNp
# Z25pbmcgUENBMB4XDTE1MDYwNDE3NDI0NVoXDTE2MDkwNDE3NDI0NVowgYMxCzAJ
# BgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25k
# MR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xDTALBgNVBAsTBE1PUFIx
# HjAcBgNVBAMTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjCCASIwDQYJKoZIhvcNAQEB
# BQADggEPADCCAQoCggEBAJL8bza74QO5KNZG0aJhuqVG+2MWPi75R9LH7O3HmbEm
# UXW92swPBhQRpGwZnsBfTVSJ5E1Q2I3NoWGldxOaHKftDXT3p1Z56Cj3U9KxemPg
# 9ZSXt+zZR/hsPfMliLO8CsUEp458hUh2HGFGqhnEemKLwcI1qvtYb8VjC5NJMIEb
# e99/fE+0R21feByvtveWE1LvudFNOeVz3khOPBSqlw05zItR4VzRO/COZ+owYKlN
# Wp1DvdsjusAP10sQnZxN8FGihKrknKc91qPvChhIqPqxTqWYDku/8BTzAMiwSNZb
# /jjXiREtBbpDAk8iAJYlrX01boRoqyAYOCj+HKIQsaUCAwEAAaOCAWAwggFcMBMG
# A1UdJQQMMAoGCCsGAQUFBwMDMB0GA1UdDgQWBBSJ/gox6ibN5m3HkZG5lIyiGGE3
# NDBRBgNVHREESjBIpEYwRDENMAsGA1UECxMETU9QUjEzMDEGA1UEBRMqMzE1OTUr
# MDQwNzkzNTAtMTZmYS00YzYwLWI2YmYtOWQyYjFjZDA1OTg0MB8GA1UdIwQYMBaA
# FMsR6MrStBZYAck3LjMWFrlMmgofMFYGA1UdHwRPME0wS6BJoEeGRWh0dHA6Ly9j
# cmwubWljcm9zb2Z0LmNvbS9wa2kvY3JsL3Byb2R1Y3RzL01pY0NvZFNpZ1BDQV8w
# OC0zMS0yMDEwLmNybDBaBggrBgEFBQcBAQROMEwwSgYIKwYBBQUHMAKGPmh0dHA6
# Ly93d3cubWljcm9zb2Z0LmNvbS9wa2kvY2VydHMvTWljQ29kU2lnUENBXzA4LTMx
# LTIwMTAuY3J0MA0GCSqGSIb3DQEBBQUAA4IBAQCmqFOR3zsB/mFdBlrrZvAM2PfZ
# hNMAUQ4Q0aTRFyjnjDM4K9hDxgOLdeszkvSp4mf9AtulHU5DRV0bSePgTxbwfo/w
# iBHKgq2k+6apX/WXYMh7xL98m2ntH4LB8c2OeEti9dcNHNdTEtaWUu81vRmOoECT
# oQqlLRacwkZ0COvb9NilSTZUEhFVA7N7FvtH/vto/MBFXOI/Enkzou+Cxd5AGQfu
# FcUKm1kFQanQl56BngNb/ErjGi4FrFBHL4z6edgeIPgF+ylrGBT6cgS3C6eaZOwR
# XU9FSY0pGi370LYJU180lOAWxLnqczXoV+/h6xbDGMcGszvPYYTitkSJlKOGMIIF
# mTCCA4GgAwIBAgIQea0WoUqgpa1Mc1j0BxMuZTANBgkqhkiG9w0BAQUFADBfMRMw
# EQYKCZImiZPyLGQBGRYDY29tMRkwFwYKCZImiZPyLGQBGRYJbWljcm9zb2Z0MS0w
# KwYDVQQDEyRNaWNyb3NvZnQgUm9vdCBDZXJ0aWZpY2F0ZSBBdXRob3JpdHkwHhcN
# MDEwNTA5MjMxOTIyWhcNMjEwNTA5MjMyODEzWjBfMRMwEQYKCZImiZPyLGQBGRYD
# Y29tMRkwFwYKCZImiZPyLGQBGRYJbWljcm9zb2Z0MS0wKwYDVQQDEyRNaWNyb3Nv
# ZnQgUm9vdCBDZXJ0aWZpY2F0ZSBBdXRob3JpdHkwggIiMA0GCSqGSIb3DQEBAQUA
# A4ICDwAwggIKAoICAQDzXfqAZ9Rap6kMLJAg0DUIPHWEzbcHiZyJ2t7Ow2D6kWha
# npRxKRh2fMLgyCV2lA5Y+gQ0Nubfr/eAuulYCyuT5Z0F43cikfc0ZDwikR1e4QmQ
# vBT+/HVYGeF5tweSo66IWQjYnwfKA1j8aCltMtfSqMtL/OELSDJP5uu4rU/kXG8T
# lJnbldV126gat5SRtHdb9UgMj2p5fRRwBH1tr5D12nDYR7e/my9s5wW34RFgrHmR
# FHzF1qbk4X7Vw37lktI8ALU2gt554W3ztW74nzPJy1J9c5g224uha6KVl5uj3sJN
# Jv8GlmclBsjnrOTuEjOVMZnINQhONMp5U9W1vmMyWUA2wKVOBE0921sHM+RYv+8/
# U2TYQlk1V/0PRXwkBE2e1jh0EZcikM5oRHSSb9VLb7CG48c2QqDQ/MHAWvmjYbkw
# R3GWChawkcBCle8Qfyhq4yofseTNAz93cQTHIPxJDx1FiKTXy36IrY4t7EXbxFEE
# ySr87IaemhGXW97OU4jm4rf9rJXCKEDb7wSQ34EzOdmyRaUjhwalVYkxuwYtYA5B
# GH0fLrWXyxHrFdUkpZTvFRSJ/Utz+jJb/NEzAPlZYnAHMuouq0Ate8rdIWcbMJmP
# FqojqEHRsG4RmzbE3kB0nOFYZcFgHnpbOMiPuwQmfNQWQOW2a2yqhv0Av87BNQID
# AQABo1EwTzALBgNVHQ8EBAMCAcYwDwYDVR0TAQH/BAUwAwEB/zAdBgNVHQ4EFgQU
# DqyCYEBWJ5flJRP8KuEKU5VZ5KQwEAYJKwYBBAGCNxUBBAMCAQAwDQYJKoZIhvcN
# AQEFBQADggIBAMURTQM6YN1dUhF3j7K7NsiyBb+0t6jYIJ1cEwO2HCL6BhM1tshj
# 1JpHbyZX0lXxBLEmX9apUGigvNK4bszD6azfGc14rFl0rGY0NsQbPmw4TDMOMBIN
# oyb+UVMA/69aToQNDx/kbQUuToVLjWwzb1TSZKu/UK99ejmgN+1jAw/8EwbOFjbU
# VDuVG1FiOuVNF9QFOZKaJ6hbqr3su77jIIlgcWxWs6UT0G0OI36VA+1oPfLYY7hr
# TbboMLXhypRL96KqXZkwsj2nwlFsKCABJCcrSwC3nRFrcL6yEIK8DJto0I07JIeq
# mShynTNfWZC99d6TnjpiWjQ54ohVHbkGsMGJay3XacMZEjaE0Mmg2v8vaXiy5Xra
# 69cMwPe9Yxe4ORM4ojZbe/KFVmodZGLBOOKqv1FmopT1EpxmIhBr8rcwki3yKfA9
# OxRDaKLxnCk3y844ICVtfGfzfiQSJAMIgUfspZ6X9RjXz7vV73aW7/3O21adlaBC
# +ZdY4dcxItNfWeY+biIA6kOEtiXb2fMIVmjAZGsdfOy2k6JiV24u2OdYj8QxSSbd
# 3ik1h/UwcXBbFDxpvYkSfesuo/7Yf56CWlIKK8FDK9kwiJ/IEPuJjeahhXUzfmye
# 23MTZGJppS99ypZtn/gETTCSPW4hFCHJPeDD/YprnUr90aGdmUN3P7DaMIIFvDCC
# A6SgAwIBAgIKYTMmGgAAAAAAMTANBgkqhkiG9w0BAQUFADBfMRMwEQYKCZImiZPy
# LGQBGRYDY29tMRkwFwYKCZImiZPyLGQBGRYJbWljcm9zb2Z0MS0wKwYDVQQDEyRN
# aWNyb3NvZnQgUm9vdCBDZXJ0aWZpY2F0ZSBBdXRob3JpdHkwHhcNMTAwODMxMjIx
# OTMyWhcNMjAwODMxMjIyOTMyWjB5MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2Fz
# aGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENv
# cnBvcmF0aW9uMSMwIQYDVQQDExpNaWNyb3NvZnQgQ29kZSBTaWduaW5nIFBDQTCC
# ASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBALJyWVwZMGS/HZpgICBCmXZT
# bD4b1m/My/Hqa/6XFhDg3zp0gxq3L6Ay7P/ewkJOI9VyANs1VwqJyq4gSfTwaKxN
# S42lvXlLcZtHB9r9Jd+ddYjPqnNEf9eB2/O98jakyVxF3K+tPeAoaJcap6Vyc1bx
# F5Tk/TWUcqDWdl8ed0WDhTgW0HNbBbpnUo2lsmkv2hkL/pJ0KeJ2L1TdFDBZ+NKN
# Yv3LyV9GMVC5JxPkQDDPcikQKCLHN049oDI9kM2hOAaFXE5WgigqBTK3S9dPY+fS
# LWLxRT3nrAgA9kahntFbjCZT6HqqSvJGzzc8OJ60d1ylF56NyxGPVjzBrAlfA9MC
# AwEAAaOCAV4wggFaMA8GA1UdEwEB/wQFMAMBAf8wHQYDVR0OBBYEFMsR6MrStBZY
# Ack3LjMWFrlMmgofMAsGA1UdDwQEAwIBhjASBgkrBgEEAYI3FQEEBQIDAQABMCMG
# CSsGAQQBgjcVAgQWBBT90TFO0yaKleGYYDuoMW+mPLzYLTAZBgkrBgEEAYI3FAIE
# DB4KAFMAdQBiAEMAQTAfBgNVHSMEGDAWgBQOrIJgQFYnl+UlE/wq4QpTlVnkpDBQ
# BgNVHR8ESTBHMEWgQ6BBhj9odHRwOi8vY3JsLm1pY3Jvc29mdC5jb20vcGtpL2Ny
# bC9wcm9kdWN0cy9taWNyb3NvZnRyb290Y2VydC5jcmwwVAYIKwYBBQUHAQEESDBG
# MEQGCCsGAQUFBzAChjhodHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtpL2NlcnRz
# L01pY3Jvc29mdFJvb3RDZXJ0LmNydDANBgkqhkiG9w0BAQUFAAOCAgEAWTk+fyZG
# r+tvQLEytWrrDi9uqEn361917Uw7LddDrQv+y+ktMaMjzHxQmIAhXaw9L0y6oqhW
# nONwu7i0+Hm1SXL3PupBf8rhDBdpy6WcIC36C1DEVs0t40rSvHDnqA2iA6VW4LiK
# S1fylUKc8fPv7uOGHzQ8uFaa8FMjhSqkghyT4pQHHfLiTviMocroE6WRTsgb0o9y
# lSpxbZsa+BzwU9ZnzCL/XB3Nooy9J7J5Y1ZEolHN+emjWFbdmwJFRC9f9Nqu1IIy
# bvyklRPk62nnqaIsvsgrEA5ljpnb9aL6EiYJZTiU8XofSrvR4Vbo0HiWGFzJNRZf
# 3ZMdSY4tvq00RBzuEBUaAF3dNVshzpjHCe6FDoxPbQ4TTj18KUicctHzbMrB7HCj
# V5JXfZSNoBtIA1r3z6NnCnSlNu0tLxfI5nI3EvRvsTxngvlSso0zFmUeDordEN5k
# 9G/ORtTTF+l5xAS00/ss3x+KnqwK+xMnQK3k+eGpf0a7B2BHZWBATrBC7E7ts3Z5
# 2Ao0CW0cgDEf4g5U3eWh++VHEK1kmP9QFi58vwUheuKVQSdpw5OPlcmN2Jshrg1c
# nPCiroZogwxqLbt2awAdlq3yFnv2FoMkuYjPaqhHMS+a3ONxPdcAfmJH0c6IybgY
# +g5yjcGjPa8CQGr/aZuW4hCoELQ3UAjWwz0wggYHMIID76ADAgECAgphFmg0AAAA
# AAAcMA0GCSqGSIb3DQEBBQUAMF8xEzARBgoJkiaJk/IsZAEZFgNjb20xGTAXBgoJ
# kiaJk/IsZAEZFgltaWNyb3NvZnQxLTArBgNVBAMTJE1pY3Jvc29mdCBSb290IENl
# cnRpZmljYXRlIEF1dGhvcml0eTAeFw0wNzA0MDMxMjUzMDlaFw0yMTA0MDMxMzAz
# MDlaMHcxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQH
# EwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xITAfBgNV
# BAMTGE1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQTCCASIwDQYJKoZIhvcNAQEBBQAD
# ggEPADCCAQoCggEBAJ+hbLHf20iSKnxrLhnhveLjxZlRI1Ctzt0YTiQP7tGn0Uyt
# dDAgEesH1VSVFUmUG0KSrphcMCbaAGvoe73siQcP9w4EmPCJzB/LMySHnfL0Zxws
# /HvniB3q506jocEjU8qN+kXPCdBer9CwQgSi+aZsk2fXKNxGU7CG0OUoRi4nrIZP
# VVIM5AMs+2qQkDBuh/NZMJ36ftaXs+ghl3740hPzCLdTbVK0RZCfSABKR2YRJylm
# qJfk0waBSqL5hKcRRxQJgp+E7VV4/gGaHVAIhQAQMEbtt94jRrvELVSfrx54QTF3
# zJvfO4OToWECtR0Nsfz3m7IBziJLVP/5BcPCIAsCAwEAAaOCAaswggGnMA8GA1Ud
# EwEB/wQFMAMBAf8wHQYDVR0OBBYEFCM0+NlSRnAK7UD7dvuzK7DDNbMPMAsGA1Ud
# DwQEAwIBhjAQBgkrBgEEAYI3FQEEAwIBADCBmAYDVR0jBIGQMIGNgBQOrIJgQFYn
# l+UlE/wq4QpTlVnkpKFjpGEwXzETMBEGCgmSJomT8ixkARkWA2NvbTEZMBcGCgmS
# JomT8ixkARkWCW1pY3Jvc29mdDEtMCsGA1UEAxMkTWljcm9zb2Z0IFJvb3QgQ2Vy
# dGlmaWNhdGUgQXV0aG9yaXR5ghB5rRahSqClrUxzWPQHEy5lMFAGA1UdHwRJMEcw
# RaBDoEGGP2h0dHA6Ly9jcmwubWljcm9zb2Z0LmNvbS9wa2kvY3JsL3Byb2R1Y3Rz
# L21pY3Jvc29mdHJvb3RjZXJ0LmNybDBUBggrBgEFBQcBAQRIMEYwRAYIKwYBBQUH
# MAKGOGh0dHA6Ly93d3cubWljcm9zb2Z0LmNvbS9wa2kvY2VydHMvTWljcm9zb2Z0
# Um9vdENlcnQuY3J0MBMGA1UdJQQMMAoGCCsGAQUFBwMIMA0GCSqGSIb3DQEBBQUA
# A4ICAQAQl4rDXANENt3ptK132855UU0BsS50cVttDBOrzr57j7gu1BKijG1iuFcC
# y04gE1CZ3XpA4le7r1iaHOEdAYasu3jyi9DsOwHu4r6PCgXIjUji8FMV3U+rkuTn
# jWrVgMHmlPIGL4UD6ZEqJCJw+/b85HiZLg33B+JwvBhOnY5rCnKVuKE5nGctxVEO
# 6mJcPxaYiyA/4gcaMvnMMUp2MT0rcgvI6nA9/4UKE9/CCmGO8Ne4F+tOi3/FNSte
# o7/rvH0LQnvUU3Ih7jDKu3hlXFsBFwoUDtLaFJj1PLlmWLMtL+f5hYbMUVbonXCU
# bKw5TNT2eb+qGHpiKe+imyk0BncaYsk9Hm0fgvALxyy7z0Oz5fnsfbXjpKh0NbhO
# xXEjEiZ2CzxSjHFaRkMUvLOzsE1nyJ9C/4B5IYCeFTBm6EISXhrIniIh0EPpK+m7
# 9EjMLNTYMoBMJipIJF9a6lbvpt6Znco6b72BJ3QGEe52Ib+bgsEnVLaxaj2JoXZh
# tG6hE6a/qkfwEm/9ijJssv7fUciMI8lmvZ0dhxJkAj0tr1mPuOQh5bWwymO0eFQF
# 1EEuUKyUsKV4q7OglnUa2ZKHE3UiLzKoCG6gW4wlv6DvhMoh1useT8ma7kng9wFl
# b4kLfchpyOZu6qeXzjEp/w7FW1zYTRuh2Povnj8uVRZryROj/TGCBJUwggSRAgEB
# MIGQMHkxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQH
# EwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xIzAhBgNV
# BAMTGk1pY3Jvc29mdCBDb2RlIFNpZ25pbmcgUENBAhMzAAABCix5rtd5e6asAAEA
# AAEKMAkGBSsOAwIaBQCgga4wGQYJKoZIhvcNAQkDMQwGCisGAQQBgjcCAQQwHAYK
# KwYBBAGCNwIBCzEOMAwGCisGAQQBgjcCARUwIwYJKoZIhvcNAQkEMRYEFAdz9Ijk
# uKV5uTs7vWI4pljPedlLME4GCisGAQQBgjcCAQwxQDA+oCSAIgBNAGkAYwByAG8A
# cwBvAGYAdAAgAEEAUwBQAC4ATgBFAFShFoAUaHR0cDovL3d3dy5hc3AubmV0LyAw
# DQYJKoZIhvcNAQEBBQAEggEAL5H2xxrGd4Yw+Zu6E2uP5WYKmVttNFAs5dK59vHr
# VM99XhW5cgYxdqqNjvDYRB7D+De4kAFTmr58yVp+ZYi4ufia0F433gxDCQWPIaBE
# HxrL8dDVL1+BY/e4JNA21yHoo3ZSF9JJCp916rGoKhmTa9t1Dv0uAMD/7sVmMPUm
# sMpkDt77eQIbGXiEf+hUi6+VCVyIMklqidcyg2GIUQAZl4r3RrKYxwwVs95Ob8UF
# XoRcwu131zEUazZ8hRZRbrTUju2S2G9WK00rAQeDCn5bdAu4w2aY6Wu0EeykaRNK
# oGUVvfU3f8MhSHlnD+BRPeLuzvF65fic9nxq9R0hvdO6cqGCAigwggIkBgkqhkiG
# 9w0BCQYxggIVMIICEQIBATCBjjB3MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2Fz
# aGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENv
# cnBvcmF0aW9uMSEwHwYDVQQDExhNaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0ECEzMA
# AAB3h6kGf3vPMDkAAAAAAHcwCQYFKw4DAhoFAKBdMBgGCSqGSIb3DQEJAzELBgkq
# hkiG9w0BBwEwHAYJKoZIhvcNAQkFMQ8XDTE1MDgyNjIwMzgyOFowIwYJKoZIhvcN
# AQkEMRYEFB14f/lGlHyilgiYlvXCnzh16EpFMA0GCSqGSIb3DQEBBQUABIIBAIsg
# 8CV6u5MJCv7I00RYScGPFwD0NwXOO8U8UwjNSlf89O3yltviqTBBVy37jfK+YgrO
# 7KxQuDFtOpGLuvk8BkwBUPbRds5orivcGlFa7Wo7S++HfNhDbMzskATkz30JkPcs
# F1PZF/61JfeGUQa5WqhNujq45nsqPWSy7ixeaTsS50rWzpSNpGI6fHOBi7a8tByc
# l1fVTO3TFmlbbvoLBGZE5Y1ym9a3VQfhXQ5/aVV3lRWkLrCHOY3tFsl+wWDNhMv2
# 6GzCopjNltT5cTjcPGJUSgpZOfkudBxSoKaKr7Kbcxh+/41pciOKlSDB/8+rbmoD
# CGLDS6nTBovnHlIdV+c=
# SIG # End signature block
