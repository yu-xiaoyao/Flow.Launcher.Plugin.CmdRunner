using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Flow.Launcher.Plugin.CmdRunner
{
    public class CmdRunner : IPlugin, IContextMenu
    {
        public const string IconPath = "Images\\CmdRunner-Icon.png";

        private PluginInitContext _context;
        private Settings _settings;

        public void Init(PluginInitContext context)
        {
            _context = context;
            _settings = context.API.LoadSettingJsonStorage<Settings>();
        }

        public List<Result> Query(Query query)
        {
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
            var inputParams = new List<string>();
            for (var i = 1; i < searchTerms.Length; i++)
                inputParams.Add(searchTerms[i]);

            _context.API.LogInfo("Input",
                $"{inputName}. searchTerms.Length = {searchTerms.Length}. size = {inputParams.Count}. PARAM: {string.Join(',', inputParams)}");

            var fullMatchCmdList = new List<Command>();
            var fuzzyMatchCmdList = new List<Command>();

            foreach (var command in _settings.Commands)
            {
                if (command.Name.Equals(inputName))
                {
                    fullMatchCmdList.Add(command);
                    continue;
                }

                if (!inputParams.Any())
                {
                    var mr = _context.API.FuzzySearch(inputName, command.Name);
                    if (mr.Success)
                    {
                        fuzzyMatchCmdList.Add(command);
                    }
                }
            }

            var result = new List<Result>();

            if (fullMatchCmdList.Any())
            {
                var tipList = new List<Result>();
                foreach (var command in fullMatchCmdList)
                {
                    if (command.ArgumentList.Count == inputParams.Count)
                    {
                        var ew = new ExecuteParamWrapper(command, inputParams);
                        result.Add(new Result
                        {
                            IcoPath = IconPath,
                            Title = command.Name,
                            SubTitle = "回车: " + command.Description,
                            ContextData = ew,
                            Action = ctx =>
                            {
                                _context.API.LogInfo("Select",
                                    $"ARGS = {command.Arguments} ... {string.Join(',', ew.InputParams)}");
                                _context.API.LogInfo("Select", $"P1: {string.Join(';', command.ArgumentList)}");
                                _context.API.LogInfo("Select", $"P2: {string.Join(';', query.SearchTerms)}");
                                _context.API.LogInfo("Select", $"Search: {string.Join(';', query.RawQuery)}");
                                // CmdUtil.ExecuteCmd(command, inputParams);
                                return false;
                            }
                        });
                    }
                    else
                    {
                        tipList.Add(new Result
                        {
                            IcoPath = IconPath,
                            Title = command.Name,
                            SubTitle = "参数: " + command.Description,
                            ContextData = command,
                            AutoCompleteText = $"{query.ActionKeyword} {command.Name} ",
                            Action = _ =>
                            {
                                _context.API.ChangeQuery($"{query.ActionKeyword} {command.Name} ");
                                return false;
                            }
                        });
                    }
                }

                result.AddRange(tipList);
            }

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

        public List<Result> LoadContextMenus(Result selectedResult)
        {
            var ew = selectedResult.ContextData as ExecuteParamWrapper;
            return new List<Result>()
            {
                new Result()
                {
                    Title = ew.Command.Name,
                    SubTitle = string.Join(',', ew.InputParams),
                }
            };
        }
    }
}