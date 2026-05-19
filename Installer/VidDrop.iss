; VidDrop Installer — Inno Setup 6.x
; Build first: dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

#define AppName      "VidDrop"
#define AppVersion   "1.0.0"
#define AppPublisher "VidDrop"
#define AppExe       "VidDrop.exe"
#define PublishDir   "..\bin\Release\net8.0-windows\win-x64\publish"

[Setup]
AppId={{6F3A2D1E-84BC-4E97-A21F-D0C5B9F8E43A}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={localappdata}\{#AppName}
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
OutputBaseFilename=VidDrop_Setup
OutputDir=..\dist
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayIcon={app}\{#AppExe}
MinVersion=10.0

[Languages]
Name: "portuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
Name: "english";    MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Criar atalho na área de trabalho"; Flags: unchecked

[Files]
Source: "{#PublishDir}\{#AppExe}";        DestDir: "{app}"; Flags: ignoreversion
Source: "{#PublishDir}\Tools\yt-dlp.exe"; DestDir: "{app}\Tools"; Flags: ignoreversion
Source: "{#PublishDir}\Tools\ffmpeg.exe"; DestDir: "{app}\Tools"; Flags: ignoreversion

[Icons]
Name: "{group}\{#AppName}";        Filename: "{app}\{#AppExe}"
Name: "{autodesktop}\{#AppName}";  Filename: "{app}\{#AppExe}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExe}"; Description: "Iniciar {#AppName}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{app}"
