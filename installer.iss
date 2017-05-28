; Script generated by the Inno Script Studio Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Engineers Calculator"
#define MyAppVersion "2.1"
#define MyAppPublisher "webmaster442"
#define MyAppURL "https://github.com/webmaster442/ECalc"
#define MyAppExeName "ECalc.exe"
#define AdminPath "{pf}"
#define NonAdminPath "{localappdata}"
#include <idp.iss>

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{75746EDF-9C9E-40D6-9F1B-F6496895D0E5}
DisableWelcomePage=no
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={code:GetDirName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputDir=bin\setup
OutputBaseFilename=Ecalc-{#MyAppVersion}-setup
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64
MinVersion=0,6.1
LicenseFile=LICENSE
InfoBeforeFile=installer\readme.rtf
PrivilegesRequired=lowest
WizardImageFile=installer\Side.bmp
WizardSmallImageFile=installer\SmallImage.bmp

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
Name: "catalan"; MessagesFile: "compiler:Languages\Catalan.isl"
Name: "corsican"; MessagesFile: "compiler:Languages\Corsican.isl"
Name: "czech"; MessagesFile: "compiler:Languages\Czech.isl"
Name: "danish"; MessagesFile: "compiler:Languages\Danish.isl"
Name: "dutch"; MessagesFile: "compiler:Languages\Dutch.isl"
Name: "finnish"; MessagesFile: "compiler:Languages\Finnish.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "greek"; MessagesFile: "compiler:Languages\Greek.isl"
Name: "hebrew"; MessagesFile: "compiler:Languages\Hebrew.isl"
Name: "hungarian"; MessagesFile: "compiler:Languages\Hungarian.isl"
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"
Name: "norwegian"; MessagesFile: "compiler:Languages\Norwegian.isl"
Name: "polish"; MessagesFile: "compiler:Languages\Polish.isl"
Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "scottishgaelic"; MessagesFile: "compiler:Languages\ScottishGaelic.isl"
Name: "serbiancyrillic"; MessagesFile: "compiler:Languages\SerbianCyrillic.isl"
Name: "serbianlatin"; MessagesFile: "compiler:Languages\SerbianLatin.isl"
Name: "slovenian"; MessagesFile: "compiler:Languages\Slovenian.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "bin\Release\ECalc.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\AppLib.Common.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\AppLib.Common.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\AppLib.WPF.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\AppLib.WPF.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\ECalc.Api.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\ECalc.Api.XML"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\ECalc.Docs.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\ECalc.ExcelInterop.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\ECalc.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\ICSharpCode.AvalonEdit.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\IronPython.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\IronPython.Modules.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\IronPython.SQLite.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\IronPython.Wpf.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\MahApps.Metro.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\Microsoft.Dynamic.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\Microsoft.Scripting.AspNet.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\Microsoft.Scripting.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\Microsoft.Scripting.Metadata.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\System.Windows.Interactivity.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\UserFunctions.py"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
function Framework45IsNotInstalled(): Boolean;
var
  bSuccess: Boolean;
  regVersion: Cardinal;
begin
  Result := True;

  bSuccess := RegQueryDWordValue(HKLM, 'Software\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', regVersion);
  if (True = bSuccess) and (regVersion >= 378389) then begin
    Result := False;
  end;
end;

procedure InstallFramework;
var
  StatusText: string;
  ResultCode: Integer;
begin
  StatusText := WizardForm.StatusLabel.Caption;
  WizardForm.StatusLabel.Caption := 'Installing .NET Framework 4.5.2. This might take a few minutes�';
  WizardForm.ProgressGauge.Style := npbstMarquee;
  try
    if not Exec(ExpandConstant('{tmp}\NetFrameworkInstaller.exe'), '/passive /norestart', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then
    begin
      MsgBox('.NET installation failed with code: ' + IntToStr(ResultCode) + '.', mbError, MB_OK);
    end;
  finally
    WizardForm.StatusLabel.Caption := StatusText;
    WizardForm.ProgressGauge.Style := npbstNormal;

    DeleteFile(ExpandConstant('{tmp}\NetFrameworkInstaller.exe'));
  end;
end; 

function GetDirName(Param: string): string;
begin
  if IsAdminLoggedOn or IsPowerUserLoggedOn then
    Result := ExpandConstant('{#AdminPath}\{#MyAppName}')
  else
    Result := ExpandConstant('{#NonAdminPath}\{#MyAppName}');
end;

function GetUninstallString(): String;
var
  sUnInstPath: String;
  sUnInstallString: String;
begin
  sUnInstPath := ExpandConstant('Software\Microsoft\Windows\CurrentVersion\Uninstall\{#emit SetupSetting("AppId")}_is1');
  sUnInstallString := '';
  if not RegQueryStringValue(HKLM, sUnInstPath, 'UninstallString', sUnInstallString) then
    RegQueryStringValue(HKCU, sUnInstPath, 'UninstallString', sUnInstallString);
  Result := sUnInstallString;
end;


/////////////////////////////////////////////////////////////////////
function IsUpgrade(): Boolean;
begin
  Result := (GetUninstallString() <> '');
end;


/////////////////////////////////////////////////////////////////////
function UnInstallOldVersion(): Integer;
var
  sUnInstallString: String;
  iResultCode: Integer;
begin
// Return Values:
// 1 - uninstall string is empty
// 2 - error executing the UnInstallString
// 3 - successfully executed the UnInstallString

  // default return value
  Result := 0;

  // get the uninstall string of the old app
  sUnInstallString := GetUninstallString();
  if sUnInstallString <> '' then begin
    sUnInstallString := RemoveQuotes(sUnInstallString);
    if Exec(sUnInstallString, '/SILENT /NORESTART /SUPPRESSMSGBOXES','', SW_HIDE, ewWaitUntilTerminated, iResultCode) then
      Result := 3
    else
      Result := 2;
  end else
    Result := 1;
end;

/////////////////////////////////////////////////////////////////////
procedure CurStepChanged(CurStep: TSetupStep);
begin
  case CurStep of 
    ssInstall:
    begin
      if IsUpgrade() then UnInstallOldVersion(); 
    end;
    ssPostInstall:
    begin
      if Framework45IsNotInstalled() then InstallFramework();
    end;
  end;
end;

procedure InitializeWizard;
begin
  if Framework45IsNotInstalled() then
  begin
    idpAddFile('http://go.microsoft.com/fwlink/?LinkId=397707', ExpandConstant('{tmp}\NetFrameworkInstaller.exe'));
    idpDownloadAfter(wpReady);
  end;
end;
