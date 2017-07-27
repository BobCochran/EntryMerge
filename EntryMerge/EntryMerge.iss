#define MyAppName "EntryMerge"
#define MyAppVersion "1.0"
#define MyAppPublisher "Jonathan Duncan"
#define MyAppURL "http://www.example.com/"
#define MyAppExeName "EntryMerge.exe"

[Setup]
AppId={{5380AF81-9AAD-47A7-82F8-08C48DB5FD41}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DisableProgramGroupPage=yes
LicenseFile=D:\dev\FSharp\EntryMerge\EntryMerge\bin\Release\Licence.txt
OutputBaseFilename=EntryMergeSetup {#MyAppVersion}
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "D:\dev\FSharp\EntryMerge\EntryMerge\bin\Release\EntryMerge.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\dev\FSharp\EntryMerge\EntryMerge\bin\Release\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{commonprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\DotNetRuntime461-Web.exe"; Parameters: "/passive /promprestart"; StatusMsg: "Installing .Net Runtime"; Flags: nowait

Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

