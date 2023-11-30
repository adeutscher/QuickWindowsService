# QuickWindowsService

## Setup

1. Set the following environment variables at the machine level:

* `QUICK_WINDOWS_SERVICE__LOG_PATH` : Directory to create logs in
* `QUICK_WINDOWS_SERVICE__LOG_MINIMUM_LEVEL` : Minimum log level
* `QUICK_WINDOWS_SERVICE__PROGRAM_PATH` : Path to program to be run
* `QUICK_WINDOWS_SERVICE__PROGRAM_ARGUMENTS` : Arguments to be passed to program (optional)
* `QUICK_WINDOWS_SERVICE__REST_DELAY_SECONDS` : Sets the delay between attempts to run the program
* `QUICK_WINDOWS_SERVICE__EXECUTION_DIRECTORY` : Run program from this directory (optional)

2. The following PowerShell commands declare a service named `QuickService` under a system account.

```
$sid = [System.Security.Principal.WellKnownSidType]::LocalServiceSid	
$account = new-object system.security.principal.securityidentifier($sid, $null)
$serviceUser = $account.Translate([system.security.principal.ntaccount]).value
sc.exe create QuickService binPath='<path-to-quick-windows-service.exe' obj= $serviceUser            
```

3. Start the service:

```
Start-Service QuickService
```