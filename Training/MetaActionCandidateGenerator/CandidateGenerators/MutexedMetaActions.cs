﻿using PDDLSharp.Contextualisers.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Tools;
using PDDLSharp.Translators.StaticPredicateDetectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Toolkit.MutexDetectors;
using System.Threading;

namespace MetaActionCandidateGenerator.CandidateGenerators
{
    /// <summary>
    /// Assumed every predicate can be a mutex, and constructs meta actions out of them
    /// </summary>
    public class MutexedMetaActions : BaseCandidateGenerator
    {
        public override List<ActionDecl> GenerateCandidates(PDDLDecl pddlDecl)
        {
            if (pddlDecl.Domain.Predicates == null)
                throw new Exception("No predicates defined in domain!");
            Initialize(pddlDecl);
            ContextualizeIfNotAlready(pddlDecl);

            var candidates = new List<ActionDecl>();
            foreach (var predicate in pddlDecl.Domain.Predicates.Predicates)
            {
                if (!Statics.Any(x => x.Name.ToUpper() == predicate.Name.ToUpper()))
                {
                    int counter = 0;
                    foreach (var action in pddlDecl.Domain.Actions)
                    {
                        candidates.Add(GenerateMetaAction(
                            $"meta_{predicate.Name}_{counter++}",
                            new List<IExp>() { new NotExp(predicate) },
                            new List<IExp>() { predicate },
                            action));
                        candidates.Add(GenerateMetaAction(
                            $"meta_{predicate.Name}_{counter++}",
                            new List<IExp>() { predicate },
                            new List<IExp>() { new NotExp(predicate) },
                            action));
                    }
                }
            }

            return candidates;
        }
    }
}
