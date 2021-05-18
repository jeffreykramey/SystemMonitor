# System Logger
System Logger (SysLog) is a companion app to Open Hardware Monitor that enhances its built-in logging features. Given a 
user's list of applications, SysLog will store and calculate CPU and GPU max temperatures, average temperatures, and 
average loads using the values observed during the monitored processes' lifecycle. Upon an application's closure, SysLog 
will write the appropriate data to log files unique to each application.

## Getting Started

Download the latest version from the Release page [here](https://github.com/jeffreykramey/SystemMonitor/releases)
and unzip the package in your desired directory. To conduct the initial set-up you'll need to do a few things:
1. Open `appsToWatch.txt` and add each process you would like to monitor while SysLog is running. If you are unsure of
   the name the app uses, you can run SysLog and watch the console as you launch the desired app to find exact syntax.
2. Run `SystemLogger.exe` and complete the configuration pop-up. This will tell SysLog where to locate the various files it needs to operate. Most
    entries should be pre-populated with default locations, only change these if you want your log files to be saved elsewhere
    or want to run a different version of Open Hardware Monitor. If you would like to automate the launching/closing of
    NiceHash, click the applicable checkbox and provide it the location of the application.
3. If you need to change any of your filepaths in the future, delete the `config.txt` file from the `LogFiles` folder. 
   Next time you launch SysLog, the configuration window will reappear so can make the desired changes. Alternately, you
   can manually edit the `config.txt` file in the SysLog directory.
      

## Versions
**SystemLogger 0.3-beta**
* Fixed logic in `checkCurrentlyRunningProcesses()` so that NiceHash is only launched when other target processes aren't running.

**SystemLogger 0.2-beta**
* `appsToWatch` key is now a `uint` to track process IDs instead of the process name which was
causing missed detections of process closures.
* `TargetProcess` class updated to hold its process name.
* `runningApps` is now populated with any currently running apps during start up.
* File path errors when selecting a custom output directory for log files resolved.
* Corrected tracking and logging of multiple process instances of the same monitored application.
* Removed `instanceCount` and related methods from `TargetProcess` class in order to conform to revamped multi-instance
  tracking.
* Manually closing SysLog automatically triggers the logging of any actively monitored applications.
* `CsvTranslator` class added to hold the correct column positions within the generated .csv file depending on their 
  hardware.
* Console's quick edit mode is now automatically disabled on launch.
* Open Hardware Monitor now comes with pre-edited config file so logging is on by default.


**SystemLogger 0.1-beta**
* Original release


## Known Issues
* Readme


## To-Do
* Add functionality to build `appToWatch.txt` from configuration menu
* Migrate away from Open Hardware Monitor

