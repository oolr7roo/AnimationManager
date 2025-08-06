@echo off
setlocal enabledelayedexpansion

:: Change to the directory where the script is located
cd /d "%~dp0"

echo Current working directory: %CD%

if not exist "package.json" (
    echo Error: package.json file not found.
    echo Please check if package.json exists in the current directory.
    pause
    exit /b 1
)

:: Read current version from package.json
for /f "tokens=*" %%a in ('type package.json ^| findstr /C:"\"version\":"') do (
    set "version_line=%%a"
    set "version_line=!version_line: =!"
    set "version_line=!version_line:"=!"
    set "version_line=!version_line:version:=!"
    set "version_line=!version_line:,=!"
    set "current_version=!version_line!"
)

echo Current version: !current_version!

:: Split version number
for /f "tokens=1,2,3 delims=." %%a in ("!current_version!") do (
    set "major=%%a"
    set "minor=%%b"
    set "patch=%%c"
)

:: Increment patch version
set /a patch+=1
set "new_version=!major!.!minor!.!patch!"

echo New version will be: !new_version!

:: Create temporary file
type nul > package.json.tmp

:: Update version in file
set "found_version=0"
for /f "tokens=*" %%a in (package.json) do (
    set "line=%%a"
    echo Processing line: !line!
    echo !line! | findstr /C:\"version\" >nul
    if errorlevel 1 (
        echo !line! >> package.json.tmp
    ) else (
        set /a found_version+=1
        echo   "version": "!new_version!", >> package.json.tmp
        echo Found and updated version line
    )
)

if !found_version!==0 (
    echo Error: Could not find version line in package.json
    del package.json.tmp
    pause
    exit /b 1
)

:: Replace original file
move /y package.json.tmp package.json

:: Verify the change
echo.
echo Verifying the change...
type package.json | findstr /C:"\"version\":"

:: Get commit message
echo.
echo All Add And Commit Please Input Message
set /p commitMessage=

if "%commitMessage%"=="" (
    echo You must input message. Closing program.
    pause
    exit /b 1
)

:: Stage and commit changes
git add .
git commit -m "%commitMessage%"
git push

echo Version updated to !new_version!
echo Commit completed.
pause