using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.CmdRunner
{
    public class CmdRunner : IPlugin, IContextMenu, IPluginI18n, ISettingProvider
    {
        public const string IconPath = "Images\\CmdRunner-Icon.png";

        private PluginInitContext _context;
        private Settings _settings;
        private Query _query;

        public void Init(PluginInitContext context)
        {
            _context = context;
            _settings = context.API.LoadSettingJsonStorage<Settings>();
        }

        public List<Result> Query(Query query)
        {
            _query = query;
            return _convertToResult(query);
        }


        private List<Result> _convertToResult(Query query)
        {
            var searchTerms = query.SearchTerms;
            var termCount = searchTerms.Length;
            if (termCount == 0)
            {
                return _settings.Commands
                    .Select(c =>
                    {
                        return new Result
                        {
                            IcoPath = IconPath,
                            Title = c.Name,
                            SubTitle = c.Description,
                            ContextData = c,
                            AutoCompleteText = $"{query.ActionKeyword} {c.Name} ",
                            Action = _ =>
                            {
                                _context.API.ChangeQuery($"{query.ActionKeyword} {c.Name} ");
                                return false;
                            }
                        };
                    })
                    .ToList();
            }

            var inputName = searchTerms[0];
            var paramCount = termCount - 1;

            var fuzzyMatchCmdList = new List<Command>();

            var result = new List<Result>();

            foreach (var command in _settings.Commands)
            {
                if (command.Name.Equals(inputName))
                {
                    if (command.ArgumentNames.Count == paramCount)
                    {
                        result.Add(new Result
                        {
                            IcoPath = IconPath,
                            Title = command.Name,
                            SubTitle = string.Format(_context.API.GetTranslation("cmd_runner_enter_run_command"),
                                command.Description),
                            ContextData = command,
                            Action = _ => _runCmdAction(command, _query)
                        });
                    }
                    else
                    {
                        var completeText = $"{query.ActionKeyword} {command.Name} ";
                        if (paramCount > command.ArgumentNames.Count)
                        {
                            result.Add(new Result
                            {
                                IcoPath = IconPath,
                                Title = command.Name,
                                SubTitle = string.Format(
                                    _context.API.GetTranslation("cmd_runner_arguments_too_many"),
                                    $"{paramCount}", $"{command.ArgumentNames.Count}"),
                                Action = _ => false
                            });
                            continue;
                        }

                        result.Add(new Result
                        {
                            IcoPath = IconPath,
                            Title = command.Name,
                            SubTitle = _getNotEnoughArgumentsMessage(paramCount, command.ArgumentNames.Count,
                                command.ArgumentNames),
                            ContextData = command,
                            AutoCompleteText = completeText,
                            Action = _ =>
                            {
                                _context.API.ChangeQuery(completeText);
                                return false;
                            }
                        });
                    }
                }
                else
                {
                    var mr = _context.API.FuzzySearch(inputName, command.Name);
                    if (mr.Success)
                    {
                        // for order , frzzy match should be last
                        fuzzyMatchCmdList.Add(command);
                    }
                }
            }

            // add latest
            foreach (var command in fuzzyMatchCmdList)
            {
                result.Add(new Result
                {
                    IcoPath = IconPath,
                    Title = command.Name,
                    SubTitle = command.Description,
                    ContextData = command,
                    AutoCompleteText = $"{query.ActionKeyword} {command.Name} ",
                    Action = _ =>
                    {
                        _context.API.ChangeQuery($"{query.ActionKeyword} {command.Name} ");
                        return false;
                    }
                });
            }

            return result;
        }

        private bool _runCmdAction(Command command, Query latestQuery)
        {
            var searchTerms = latestQuery.SearchTerms;
            if (searchTerms.Length == 0) return true;

            var paramsList = new List<string>();
            for (var i = 1; i < searchTerms.Length; i++)
                paramsList.Add(searchTerms[i]);

            CmdUtil.ExecuteCmd(command, paramsList, _settings.InternalArguments);
            return true;
        }

        private string _getNotEnoughArgumentsMessage(int currentCount, int needCount, List<string> argumentNames)
        {
            var miss = new List<string>();

            for (var i = currentCount; i < needCount; i++)
            {
                miss.Add(argumentNames[i]);
            }

            return string.Format(
                _context.API.GetTranslation("cmd_runner_arguments_not_enough"),
                $"{currentCount}", $"{needCount}",
                string.Join(",", miss));
        }

        public List<Result> LoadContextMenus(Result selectedResult)
        {
            return new List<Result>()
            {
            };
        }


        public string GetTranslatedPluginTitle()
        {
            return _context.API.GetTranslation("cmd_runner_plugin_title");
        }

        public string GetTranslatedPluginDescription()
        {
            return _context.API.GetTranslation("cmd_runner_plugin_description");
        }

        public Control CreateSettingPanel()
        {
            return new SettingsControlPanel(_context, _settings);
        }
    }
}