![](https://raw.githubusercontent.com/jaysonragasa/3DPrinterStepperController/master/ss/2020-04-27_0843.png)![](https://raw.githubusercontent.com/jaysonragasa/3DPrinterStepperController/master/ss/2020-04-27_0844.png)
# 3D Printer Stepper Motor Controller
Just a simple WPF app for controlling the stepper motors on 3D printers.  
Also provides a help for calibrating the E stepper motor.
  
# USAGE
Make sure to configure the port name first in `App.config` file  
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <add key="PortName" value="COM5"/>
    <add key="BaudRate" value="250000"/>
  </appSettings>
  
  <startup> 
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.7.2" />
  </startup>
</configuration>
```

# Third Party Licenses
SerialPortStream  
[https://github.com/jcurl/serialportstream](https://github.com/jcurl/serialportstream)
