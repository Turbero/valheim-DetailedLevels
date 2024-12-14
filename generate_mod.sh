set next_version="1.3.4"
set folder_name="MODGEN"

set FILETOZIP="%~dp0%folder_name%"

set TEMPDIR="%~dp0temp"
rd /q /s %TEMPDIR%
mkdir %TEMPDIR%
xcopy /s %FILETOZIP% %TEMPDIR%

echo Set objArgs = WScript.Arguments > _zipIt.vbs
echo InputFolder = objArgs(0) >> _zipIt.vbs
echo ZipFile = objArgs(1) >> _zipIt.vbs
echo CreateObject("Scripting.FileSystemObject").CreateTextFile(ZipFile, True).Write "PK" ^& Chr(5) ^& Chr(6) ^& String(18, vbNullChar) >> _zipIt.vbs
echo Set objShell = CreateObject("Shell.Application") >> _zipIt.vbs
echo Set source = objShell.NameSpace(InputFolder).Items >> _zipIt.vbs
echo objShell.NameSpace(ZipFile).CopyHere(source) >> _zipIt.vbs
echo wScript.Sleep 2000 >> _zipIt.vbs

CScript  _zipIt.vbs  %TEMPDIR%  "%~dp0%folder_name%\Turbero-DetailedLevels-%next_version%.zip"

rd /q /s %TEMPDIR%
del _zipIt.vbs