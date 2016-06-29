# ServicePointManagerExtensions

## Sample code

Here's how the code can be used in a Windows Azure project; reporting high connection counts to a 3rd party service.

    // Get config values using CloudConfigurationManager, so the settings can be changed in the Azure portal    
    var loggingInterval = CloudConfigurationManager.GetSetting("ServicePointMonitor.Interval");
    var loggingTreshold = CloudConfigurationManager.GetSetting("ServicePointMonitor.Treshold");

    int treshold;
    if (!int.TryParse(loggingTreshold, out treshold))
        treshold = 0;

    // Only start when the config option is set
    if (!string.IsNullOrEmpty(loggingInterval))
    {
        ServicePointMonitor.Start(TimeSpan.Parse(loggingInterval),
                                  status =>
                                  {
                                      if (status.TotalConnectionsOverall > treshold)
                                      {
                                          // log to 3rd party logging tool
                                          ExceptionlessClient.Default
                                                             .CreateLog("ServicePointMonitor", $"ServicePoint count: {status.ServicePointCount}, " +
                                                                                               $"TotalConnections: {status.TotalConnectionsOverall} " +
                                                                                               $"DefaultConnectionLimit: {status.DefaultConnectionLimit}")
                                                             .AddObject(status)
                                                             .Submit();
                                      }
                                  });
    }

## References

ServicePointMonitor was based on the [blog post by Freek Paans][1] about starving outgoing connections in Windows Azure Web Sites.

[1]: http://www.freekpaans.nl/2015/08/starving-outgoing-connections-on-windows-azure-web-sites/
