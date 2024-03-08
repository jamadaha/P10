﻿using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;

namespace P10.RefinementStrategies
{
    public interface IRefinementStrategy
    {
        public ActionDecl? Refine(DomainDecl domain, List<ProblemDecl> problems, ActionDecl currentMetaAction, ActionDecl originalMetaAction, string workingDir);
    }
}