adb root
@echo off
for /F "usebackq tokens=1,2 delims==" %%i in (`wmic os get LocalDateTime /VALUE 2^>NUL`) do if '.%%i.'=='.LocalDateTime.' set ldt=%%j
set ldt=%ldt:~4,2%%ldt:~6,2%%ldt:~8,2%%ldt:~10,2%%ldt:~0,4%.%ldt:~12,2%
echo Local date is [%ldt%]
adb shell "date %ldt%"
adb shell setprop sys.usb.config "rndis,adb"


