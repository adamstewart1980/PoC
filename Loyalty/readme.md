Loyalty
===

This is the technical spike to get COM interface working and then call out to 3rd party endpoints

Instructions

To install
Copy all dlls and config files including the INTEROP dll to the server in the c:\bootdrv\aloha\bin folder

In Data/aloha.ini set the value 

ACTIVITYCLSID5=ncl.app.Loyalty.Aloha.COMIntegration.AlohaEventIntercept

In NewData/aloha.ini set the value

ACTIVITYCLSID5=ncl.app.Loyalty.Aloha.COMIntegration.AlohaEventIntercept

These should all then get copied to the individual tills.

VNC on to each till and in an Administrator launched command window

cd c:\Windows\Microsoft.Net\Framework\v4.0.30319

regasm /codebase "c:\bootdrv\aloha\bin\ncl.app.Loyalty.Aloha.COMIntegration.dll"

NOTE: ignore the warning

Now you need to reboot the till

This should be the installation complete. 
To check, perform and order on the till and then VNC to it and check the contents of the following log file
c:\bootdrv\aloha\tmp\debout.Intercept-Log.log

This file should contain lines in the format

02/13/20 14:26:31.006 LogAlways: 
MM/dd/yy HH:mm:ss.mmm LogAlways: PostCloseCheck:: SourceTermId:{SourceTermId},EmployeeId:{EmployeeId},QueueId:{QueueId},TableId:{TableId},CheckId:{CheckId}, Result was :: {True/False}

True - indicates success
False - indicates failure

There is also the possibility that exceptions will be logged in here also
