﻿using PDDLSharp.CodeGenerators.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers.FastDownward.Plans;
using System.Text.RegularExpressions;
using Tools;

namespace P10.UsefulnessCheckers
{
    public class ReducesMetaSearchTimeUsefulness : UsedInPlansUsefulness
    {
        public static int Rounds { get; set; } = 2;
        private readonly Regex _searchTime = new Regex("Search time: ([0-9.]*)", RegexOptions.Compiled);

        public ReducesMetaSearchTimeUsefulness(string workingDir, int timeLimitS) : base(workingDir, timeLimitS)
        {
        }

        public override List<ActionDecl> GetUsefulCandidates(DomainDecl domain, List<ProblemDecl> problems, List<ActionDecl> candidates)
        {
            if (candidates.Count == 0)
                return new List<ActionDecl>();
            var usefulCandidates = new List<ActionDecl>();
            var searchTimes = GetDefaultSearchTimes(domain, problems);

            ConsoleHelper.WriteLineColor($"\tAverage base search time: {searchTimes.Average()}s", ConsoleColor.Magenta);

            var count = 1;
            foreach (var candidate in candidates)
            {
                ConsoleHelper.WriteLineColor($"\tChecking candidate {count++} out of {candidates.Count}", ConsoleColor.Magenta);
                if (IsMetaActionUseful(domain, problems, candidate) &&
                    DoesMetaActionReduceSearchTime(domain, problems, candidate, searchTimes))
                {
                    usefulCandidates.Add(candidate);
                }
            }

            return usefulCandidates;
        }

        private List<double> GetDefaultSearchTimes(DomainDecl domain, List<ProblemDecl> problems)
        {
            var returnList = new List<double>();
            var errorListener = new ErrorListener();
            var codeGenerator = new PDDLCodeGenerator(errorListener);
            var planParser = new FDPlanParser(errorListener);

            var testDomain = domain.Copy();
            var domainFile = new FileInfo(Path.Combine(WorkingDir, "usefulCheckDomain.pddl"));
            codeGenerator.Generate(testDomain, domainFile.FullName);

            int count = 1;
            foreach (var problem in problems)
            {
                ConsoleHelper.WriteLineColor($"\t\tGetting base search time in problem {count} out of {problems.Count}", ConsoleColor.Magenta);
                var problemFile = new FileInfo(Path.Combine(WorkingDir, "usefulCheckProblem.pddl"));
                codeGenerator.Generate(problem, problemFile.FullName);

                var times = new List<double>();
                for (int i = 0; i < Rounds; i++)
                {
                    using (ArgsCaller fdCaller = new ArgsCaller("python3"))
                    {
                        var log = "";
                        fdCaller.StdOut += (s, o) =>
                        {
                            log += o.Data;
                        };
                        fdCaller.StdErr += (s, o) => { };
                        fdCaller.Arguments.Add(ExternalPaths.FastDownwardPath, "");
                        fdCaller.Arguments.Add("--alias", "lama-first");
                        fdCaller.Arguments.Add("--overall-time-limit", $"{TimeLimitS}s");
                        fdCaller.Arguments.Add("--plan-file", "plan.plan");
                        fdCaller.Arguments.Add(domainFile.FullName, "");
                        fdCaller.Arguments.Add(problemFile.FullName, "");
                        fdCaller.Process.StartInfo.WorkingDirectory = WorkingDir;
                        fdCaller.Run();
                        var matches = _searchTime.Match(log);
                        if (matches == null)
                            throw new Exception("No search time for problem???");
                        if (matches.Groups[1].Value == "")
                            times.Add(TimeSpan.FromMinutes(30).TotalSeconds);
                        else
                            times.Add(double.Parse(matches.Groups[1].Value));
                    }
                }
                returnList.Add(times.Average());
                count++;
            }

            return returnList;
        }

        private bool DoesMetaActionReduceSearchTime(DomainDecl domain, List<ProblemDecl> problems, ActionDecl candidate, List<double> searchTimes)
        {
            var errorListener = new ErrorListener();
            var codeGenerator = new PDDLCodeGenerator(errorListener);
            var planParser = new FDPlanParser(errorListener);

            var testDomain = domain.Copy();
            testDomain.Actions.Add(candidate);

            var domainFile = new FileInfo(Path.Combine(WorkingDir, "usefulCheckDomain.pddl"));
            codeGenerator.Generate(testDomain, domainFile.FullName);

            int count = 1;
            foreach (var problem in problems)
            {
                ConsoleHelper.WriteLineColor($"\t\tChecking if meta action reduces search time in problem {count} out of {problems.Count}", ConsoleColor.Magenta);
                var problemFile = new FileInfo(Path.Combine(WorkingDir, "usefulCheckProblem.pddl"));
                codeGenerator.Generate(problem, problemFile.FullName);

                var times = new List<double>();
                for (int i = 0; i < Rounds; i++)
                {
                    using (ArgsCaller fdCaller = new ArgsCaller("python3"))
                    {
                        var log = "";
                        fdCaller.StdOut += (s, o) =>
                        {
                            log += o.Data;
                        };
                        fdCaller.StdErr += (s, o) => { };
                        fdCaller.Arguments.Add(ExternalPaths.FastDownwardPath, "");
                        fdCaller.Arguments.Add("--alias", "lama-first");
                        fdCaller.Arguments.Add("--overall-time-limit", $"{TimeLimitS}s");
                        fdCaller.Arguments.Add("--plan-file", "plan.plan");
                        fdCaller.Arguments.Add(domainFile.FullName, "");
                        fdCaller.Arguments.Add(problemFile.FullName, "");
                        fdCaller.Process.StartInfo.WorkingDirectory = WorkingDir;
                        fdCaller.Run();
                        var matches = _searchTime.Match(log);
                        if (matches == null)
                            throw new Exception("No search time for problem???");
                        if (matches.Groups[1].Value == "")
                            times.Add(TimeLimitS);
                        else
                            times.Add(double.Parse(matches.Groups[1].Value));
                    }
                }

                ConsoleHelper.WriteLineColor($"\t\tAverage search time: {times.Average()}s", ConsoleColor.Magenta);
                if (times.Average() < searchTimes[count - 1])
                {
                    ConsoleHelper.WriteLineColor($"\t\tMeta action reduced search time in problem {count}!", ConsoleColor.Green);
                    return true;
                }
                count++;
            }

            ConsoleHelper.WriteLineColor($"\t\tMeta action does not appear useful...", ConsoleColor.Red);
            return false;
        }
    }
}
