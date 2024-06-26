﻿using CommandLine;
using static MetaActionGenerators.MetaGeneratorBuilder;

namespace FocusedMetaActions.Train
{
    public class Options
    {
        [Flags]
        public enum UsefulnessStrategies
        {
            None,
            UsedInPlans,
            ReducesMetaSearchTime,
            ReducesMetaSearchTimeTop1,
            ReducesMetaSearchTimeTop2,
            ReducesMetaSearchTimeTop5,
            ReducesMetaSearchTimeTop10,
            ReducesPlanLength,
            ReducesPlanLengthTop1,
            ReducesPlanLengthTop2,
            ReducesPlanLengthTop5,
            ReducesPlanLengthTop10,
        }
        [Option("output", Required = false, HelpText = "Where to output the meta actions", Default = "output")]
        public string OutputPath { get; set; } = "output";
        [Option("temp", Required = false, HelpText = "Where to put temporary files", Default = "temp")]
        public string TempPath { get; set; } = "temp";
        [Option("domain", Required = true, HelpText = "Path to the domain file")]
        public string DomainPath { get; set; } = "";
        [Option("problems", Required = true, HelpText = "Path to the problem files")]
        public IEnumerable<string> ProblemsPath { get; set; } = new List<string>();
        [Option("generator", Required = true, HelpText = $"The generator strategies")]
        public GeneratorOptions GeneratorOption { get; set; }
        [Option("args", Required = false, HelpText = "Optional arguments for the generator. Some generators require specific arguments, others do not. The arguments are in key-pairs, in the format key;value")]
        public IEnumerable<string> Args { get; set; } = new List<string>();
        [Option("pre-usefulness-strategy", Required = false, HelpText = "The usefulness strategy for the pre-usefulness check", Default = UsefulnessStrategies.None)]
        public UsefulnessStrategies PreUsefulnessStrategy { get; set; } = UsefulnessStrategies.None;
        [Option("post-usefulness-strategy", Required = false, HelpText = "The usefulness strategy for the post-usefulness check", Default = UsefulnessStrategies.None)]
        public UsefulnessStrategies PostUsefulnessStrategy { get; set; } = UsefulnessStrategies.None;
        [Option("last-n-usefulness", Required = false, HelpText = "How many of the training problems, in reverse, should be used for the usefulness checks (-1 is all)", Default = -1)]
        public int LastNUsefulness { get; set; } = -1;

        [Option("fast-downward-path", Required = false, HelpText = "Path to Fast Downward")]
        public string FastDownwardPath { get; set; } = "";
        [Option("stackelberg-path", Required = false, HelpText = "Path to the Stackelberg Planner")]
        public string StackelbergPath { get; set; } = "";

        [Option("validation-time-limit", Required = false, HelpText = "Time limit in seconds that each validation step is allowed to take. (-1 for no time limit)", Default = -1)]
        public int ValidationTimeLimitS { get; set; } = -1;
        [Option("exploration-time-limit", Required = false, HelpText = "Time limit in seconds that each state exploration step is allowed to take. (-1 for no time limit)", Default = 999999)]
        public int ExplorationTimeLimitS { get; set; } = 999999;
        [Option("refinement-time-limit", Required = false, HelpText = "Time limit in seconds that each refinement step is allowed to take. (-1 for no time limit)", Default = -1)]
        public int RefinementTimeLimitS { get; set; } = -1;
        [Option("total-refinement-time-limit", Required = false, HelpText = "Time limit in seconds that each refinement step is allowed to take. (-1 for no time limit)", Default = -1)]
        public int TotalRefinementTimeLimitS { get; set; } = -1;
        [Option("usefulness-time-limit", Required = false, HelpText = "Time limit in seconds that each usefulness step is allowed to take. (-1 for no time limit)", Default = -1)]
        public int UsefulnessTimeLimitS { get; set; } = -1;
        [Option("cache-generation-time-limit", Required = false, HelpText = "Time limit in seconds that each cache generation step is allowed to take. (-1 for no time limit)", Default = -1)]
        public int CacheGenerationTimeLimitS { get; set; } = -1;

        [Option("stackelberg-debug", Required = false, HelpText = "Show the stdout of the Stackelberg Planner", Default = false)]
        public bool StackelbergDebug { get; set; } = false;

        [Option("max-precondition-combinations", Required = false, HelpText = "How many precondition combinations to try", Default = 10)]
        public int MaxPreconditionCombinations { get; set; } = 10;
        [Option("max-added-parameters", Required = false, HelpText = "How many additional parameters are allowed to add", Default = 0)]
        public int MaxAddedParameters { get; set; } = 0;
        [Option("skip-refinement", Required = false, HelpText = "Optionally skip refinement step and assume the initial meta actions are correct (useful for debugging)", Default = false)]
        public bool SkipRefinement { get; set; } = false;
        [Option("skip-macro-cache", Required = false, HelpText = "Optionally skip macro cache generation (useful for debugging)", Default = false)]
        public bool SkipMacroCache { get; set; } = false;
        [Option("remove-temp-on-finish", Required = false, HelpText = "If the temp folder should be removed after a run", Default = false)]
        public bool RemoveTempOnFinish { get; set; } = false;
        [Option("post-validity-check", Required = false, HelpText = "If an additional check of validity should be done before finishing the program. Useful for sanity checking.", Default = false)]
        public bool PostValidityCheck { get; set; } = false;
        [Option("macro-cache-check-delay", Required = false, HelpText = "How long the program should wait to check replacement folder before continuing (in seconds)", Default = 1)]
        public int MacroCacheCheckDelay { get; set; } = 1;
        [Option("macro-cache-max-free-params", Required = false, HelpText = "Maximum amount of free parameters is allowed in a macro.", Default = 2)]
        public int MaxMacroCacheFreeParams { get; set; } = 2;
    }
}
