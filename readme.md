# System Logger
System Logger (SysLog) is a companion app to Open Hardware Monitor that enhances its built-in logging features. Given a 
user's list of applications, SysLog will store and calculate CPU and GPU max temperatures, average temperatures, and 
average loads using the values observed during the monitored processes' lifecycle. Upon an application's closure, SysLog 
will write the appropriate data to log files unique to each application.

## Getting Started

Download the latest version [here](https://github.com/jeffreykramey/SysLog-Release/raw/main/SystemLogger%201.0-beta.zip) 
and unzip the package in your desired directory. To conduct the initial set-up you'll need to do a few things:
1. Open `appsToWatch.txt` and add each process you would like to monitor while SysLog is running.
2. Run `SystemLogger.exe`
   
    * Complete the configuration pop-up, this will tell SysLog where to locate the various files it needs to operate. Most
    entries should be pre-populated with default locations, only change these if you want your log files to be saved elsewhere
      or want to run a different version of Open Hardware Monitor. If you would like to automate the launching/closing of
      NiceHash, click the applicable checkbox and provide it the location of application.
    * Open Hardware Monitor should be running at this point. Under the `Options` menu, ensure `Log Sensors` is checked
    and choose your logging interval, I suggest 5 or 10 seconds.
    * Next, right click the menu bar of the SysLog terminal, click `Properties`, and make sure the `Quick Edit Mode` is
    **not** selected.
3. Finally, close and relaunch SysLog which will complete the configuration. If you need to change any of your filepaths
in the future, delete the `config.txt` file from the `LogFiles` folder. Next time you launch SysLog, the configuration window
   will reappear so can make the desired changes.
      

## License


## Known Issues
* Readme
* Currently only works with 8-core CPUs
* App list builder added to configuration

## To-Do
