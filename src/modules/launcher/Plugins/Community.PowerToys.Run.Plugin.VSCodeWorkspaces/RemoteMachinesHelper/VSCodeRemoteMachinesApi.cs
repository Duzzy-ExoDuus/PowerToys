﻿using Community.PowerToys.Run.Plugin.VSCodeWorkspaces.SshConfigParser;
using Community.PowerToys.Run.Plugin.VSCodeWorkspaces.VSCodeHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Wox.Plugin.Logger;

namespace Community.PowerToys.Run.Plugin.VSCodeWorkspaces.RemoteMachinesHelper
{
    public class VSCodeRemoteMachinesApi
    {
        public VSCodeRemoteMachinesApi() { }

        public List<VSCodeRemoteMachine> Machines
        {
            get
            {
                var results = new List<VSCodeRemoteMachine>();

                foreach (var vscodeInstance in VSCodeInstances.instances)
                {
                    // settings.json contains path of ssh_config
                    var vscode_settings = Path.Combine(vscodeInstance.AppData, "User\\settings.json");

                    if (File.Exists(vscode_settings))
                    {
                        var fileContent = File.ReadAllText(vscode_settings);

                        try
                        {
                            JsonElement vscodeSettingsFile = JsonSerializer.Deserialize<JsonElement>(fileContent);


                            if (vscodeSettingsFile.TryGetProperty("remote.SSH.configFile", out var pathElement))
                            {
                                var path = pathElement.GetString();

                                if (File.Exists(path))
                                {
                                    foreach (SshHost h in SshConfig.ParseFile(path))
                                    {
                                        var machine = new VSCodeRemoteMachine();
                                        machine.Host = h.Host;
                                        machine.VSCodeInstance = vscodeInstance;
                                        machine.HostName = h.HostName != null ? h.HostName : String.Empty;
                                        machine.User = h.User != null ? h.User : String.Empty;

                                        results.Add(machine);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var message = $"Failed to deserialize ${vscode_settings}";
                            Log.Exception(message, ex, GetType());
                        }
                    }
                }

                return results;

            }
        }
    }
}
