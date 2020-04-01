# Digitizeit.QuartzHostedService
Goal of this project is to make it easy to get up and running with https://www.quartz-scheduler.net/ in dotnet core

## This project is based on the work of 
https://github.com/ErikXu/Quartz.HostedService

## Getting started using in memory scheduler .
Refrence Digitizeit.QuartzHostedService from youre application.

 appsettings.json  

```json

 "quartz": {
    "plugin": {
      "jobInitializer": {
        "type": "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz.Plugins",
        "fileNames": "quartz_jobs.xml"
      }
    }
  }

```
*  "type": "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz.Plugins",  => Instruct Quartz to use job settings from xml.
*  "fileNames": "quartz_jobs.xml" : say that a file name quartz_jobs.xml  => instruct  Quartz where to find  job instructions.

quartz_job.xml the xml file can hold one to meny job instructions 
```xml
<?xml version="1.0" encoding="UTF-8"?>

<job-scheduling-data xmlns="http://quartznet.sourceforge.net/JobSchedulingData"
                     xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                     version="2.0">

  <processing-directives>
    <overwrite-existing-data>true</overwrite-existing-data>
    <schedule-trigger-relative-to-replaced-trigger>true</schedule-trigger-relative-to-replaced-trigger>
  </processing-directives>

  <schedule>
    <job>
      <name>TestJob</name>
      <group>TestGroup</group> 
      <description>Test Job</description> 
      <job-type>QuartzCore.TestJob, QuartzCore</job-type> <!-- projectname.classname of jobb to get fired -->
      <durable>true</durable> 
      <recover>true</recover>
    </job>    
    <trigger>
      <cron>
        <name>TestTrigger</name> 
        <group>TestGroup</group> 
        <description>Test Trigger</description> 
        <job-name>TestJob</job-name> 
        <job-group>TestGroup</job-group>
        <cron-expression>0/2 * * * * ?</cron-expression>
      </cron>
    </trigger>
  </schedule>
</job-scheduling-data>

```
Refrence for more information on whats goes in to xml file https://github.com/quartznet/quartznet/blob/master/src/Quartz/Xml/job_scheduling_data_2_0.xsd

## Configur Quartz to use MsSqlserver to store job schedule and triggers

minimum settings in application.json 

```json 

"quartz": {    
    "plugin": {
      "jobInitializer": {
        "type": "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz.Plugins",
        "fileNames": "quartz_jobs.xml"
      }
    },
    "jobStore": {         
      "connectionString": "Server=localhost;Database=Quartz;User Id=sa;Password=Secret123!%;MultipleActiveResultSets=true",
      "provider": "SqlServer"    
    }

```

Using MsSqlServer will validate if sqlServer have the database configured in connectionString, If database can´t be found it will try to create it. 
Sql for creating Database can be found here https://github.com/quartznet/quartznet/tree/master/database
"provider sqlserver" will default to the following settings:

```txt
["quartz.jobStore.misfireThreshold"] =  "600000",
["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
["quartz.jobStore.useProperties"] = "true",
["quartz.jobStore.dataSource"] = "default",
["quartz.jobStore.tablePrefix"] =  "QRTZ_",
["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.SqlServerDelegate,Quartz"

```
To override in application.json

```json
"jobStore": {
      "misfireThreshold": "60000",
      "type": "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
      "useProperties": "true",
      "dataSource": "default",
      "tablePrefix": "QRTZ_",
      "lockHandler": "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz",
      "connectionString": "Server=localhost;Database=Quartz;User Id=sa;Password=Secret123!%;MultipleActiveResultSets=true",
      "provider": "SqlServer",
      "driverdelegate": "Quartz.Impl.AdoJobStore.StdAdoDelegate ,Quartz"
    }
```

## Configure Quartz to use Sqlite database 

minimum settings in application.json 
```json 

"quartz": {    
    "plugin": {
      "jobInitializer": {
        "type": "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz.Plugins",
        "fileNames": "quartz_jobs.xml"
      }
    },
    "jobStore": {         
      "connectionString": "c:\\temp\\Quartz.DB",
      "provider": "sqlite-custom"    
    }

```

Difference from SqlServer is, ConnectionString is a "**file Path**" and provder change to "**sqlite-custom**"

