# Digitizeit.QuartzHostedService
Goal of this project is to make it easy to get up and running with https://www.quartz-scheduler.net/ in dotnet core

## This project is based on the work of 
https://github.com/ErikXu/Quartz.HostedService

## Implemented database support 
* InMemory 
* SqLite
* MySql
* Postgres
* Ms SQL server
* Firebird **Comming soon**
* Oracle **Comming soon**

## Getting started Digitizeit.QuartzHostedService with  in memory scheduler .

### Create a dotnet core project 

### Add References 
``` 
PM> Install-Package Digitizeit.Quartz.HostedService -Version 0.1.6
PM> Install-Package Quartz -Version 3.0.7
PM> Install-Package Microsoft.Extensions.Hosting -Version 3.1.3
```

### Logging 
To get loginformation from Digitizeit.QuartzHostedService add refrence to a dotnet logger.
Set log level to debug,  

```
PM> Install-Package Serilog.Extensions.Logging -Version 3.0.1
PM> Install-Package Microsoft.Extensions.Logging -Version 3.1.3
```
### Create a Job to get executed by quartz 

```Csharp
using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;

namespace HostedServiceImplementation.Jobs
{
    public class FirstJob : IJob
    {
        private readonly ILogger _logger;

        public FirstJob(ILogger<FirstJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("FirstJob is running...");

            return Task.CompletedTask;
        }
    }
}
```

### Add job and Digitizeit.QuartzHostedService to hostbuilder or Startup ConfigureServices

This is a HostBuilder from a console application 

```Csharp
 private static IHostBuilder BuildHost()
        {
            var hostBuilder = new HostBuilder();

            hostBuilder.UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Debug")              
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddJsonFile("appsettings.json", true);
                })              
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddQuartzHostedService(hostContext.Configuration);
                    services.AddSingleton<FirstJob, FirstJob>();
                })              
                .UseConsoleLifetime()
                .UseSerilog();
            return hostBuilder;
        }

```
Dotnet core Asp project Startup.cs

```Csharp
public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {           
                  services.AddQuartzHostedService(configuration);
                  services.AddSingleton<FirstJob, FirstJob>();
        }

```

### Add quartz to appsettings.json
**Note.** if no provider is provided in the appsettings  Digitizit.QuartzHostedService will default to in memory database

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

* ` "type": "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz.Plugins" `  Instruct Quartz to use job settings from xml.
*  ` "fileNames": "quartz_jobs.xml" `  Tell Quartz  a file named quartz_jobs.xml is where job information is stored.

### Create a quartz_job.xml 

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
      <name>FirstJob</name>
      <group>FirstGroup</group>
      <description>FirstJob</description>
      <job-type>HostedServiceImplementation.Jobs.FirstJob, HostedServiceImplementation</job-type>
      <durable>true</durable>
      <recover>true</recover>
    </job>   
    <trigger>
      <cron>
        <name>FirstJobTrigger</name>
        <group>FirstGroup</group>
        <description>FirstJob Trigger</description>
        <job-name>FirstJob</job-name>
        <job-group>FirstGroup</job-group>
        <cron-expression>0/1 * * * * ?</cron-expression>
      </cron>
    </trigger>
  </schedule>
</job-scheduling-data>

```

### Example Xml containing two jobs 

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
      <name>FirstJob</name>
      <group>FirstGroup</group>
      <description>FirstJob</description>
      <job-type>HostedServiceImplementation.Jobs.FirstJob, HostedServiceImplementation</job-type>
      <durable>true</durable>
      <recover>true</recover>
    </job>
    <job>
      <name>SecondJob</name>
      <group>SecondGroup</group>
      <description>FirstJob</description>
      <job-type>HostedServiceImplementation.Jobs.SecondJob, HostedServiceImplementation</job-type>
      <durable>true</durable>
      <recover>true</recover>
    </job>
    <trigger>
      <simple>
        <name>TestTrigger</name>
        <group>SecondGroup</group>
        <description>Test Trigger</description>
        <job-name>SecondJob</job-name>
        <job-group>SecondGroup</job-group>
        <repeat-count>-1</repeat-count>
        <repeat-interval>2000</repeat-interval>
      </simple>
    </trigger>
    <trigger>
      <cron>
        <name>FirstJobTrigger</name>
        <group>FirstGroup</group>
        <description>FirstJob Trigger</description>
        <job-name>FirstJob</job-name>
        <job-group>FirstGroup</job-group>
        <cron-expression>0/1 * * * * ?</cron-expression>
      </cron>
    </trigger>
  </schedule>
</job-scheduling-data>

```
Refrence for more information on whats goes in to xml file https://github.com/quartznet/quartznet/blob/master/src/Quartz/Xml/job_scheduling_data_2_0.xsd

## Configur Quartz to use MsSqlserver to store job schedule and triggers

minimum settings in appsettings.json 

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

Using a database provider  will validate if server have the database configured in connectionString, If database can´t be found it will try to create it. 
 SQL scripts used in Digitizeit.QuartzHostedService to create database and tables can be found here https://github.com/quartznet/quartznet/tree/master/database   
"provider sqlserver" will default to the following settings:

```txt
["quartz.jobStore.misfireThreshold"] =  "600000",
["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
["quartz.jobStore.useProperties"] = "true",
["quartz.jobStore.dataSource"] = "default",
["quartz.jobStore.tablePrefix"] =  "QRTZ_",
["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.SqlServerDelegate,Quartz"

```
To override in appsettings.json

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

minimum settings in appsettings.json 
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

Difference from SqlServer is, ConnectionString is a "**file Path**" and provider change to "**sqlite-custom**"

## Configure Quartz to use MySql database 

 appsettings.json 

```json 
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  },
  "quartz": {
    "scheduler": {
      "instanceName": "QuartzCore",
      "instanceId": "QuartzCore"
    },
    "threadPool": {
      "type": "Quartz.Simpl.SimpleThreadPool, Quartz",
      "threadPriority": "Normal",
      "threadCount": 10
    },
    "plugin": {
      "jobInitializer": {
        "type": "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz.Plugins",
        "fileNames": "quartz_jobs.xml"
      }
    },
    "jobStore": {
      "misfireThreshold": "60000",
      "type": "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
      "useProperties": "true",
      "dataSource": "default",
      "tablePrefix": "QRTZ_",
      "connectionString": "server=localhost;Database=Quartz;uid=root;pwd=root;",
      "provider": "MySql-50"
    },
    "serializer": {
      "type": "json"
    }
  }
}

```
## Configure Quartz to use PostgresSql database 

appsettings.json

```json 
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  },
  "quartz": {
    "scheduler": {
      "instanceName": "Quartz-implpementation",
      "instanceId": "Quartz-implementation"
    },
    "threadPool": {
      "type": "Quartz.Simpl.SimpleThreadPool, Quartz",
      "threadPriority": "Normal",
      "threadCount": 10
    },
    "plugin": {
      "jobInitializer": {
        "type": "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz.Plugins",
        "fileNames": "quartz_jobs.xml"
      }
    },
    "jobStore": {
      "misfireThreshold": "60000",
      "type": "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
      "useProperties": "true",
      "dataSource": "default",
      "tablePrefix": "QRTZ_",
      "connectionString": "User ID=postgres;Password=Secret123!%;Host=localhost;Port=5432;Database=quartz;Pooling=true;",
      "provider": "Npgsql-20"
    },
    "serializer": {
      "type": "json"
    }
  }
}
```
