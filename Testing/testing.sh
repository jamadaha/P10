#!/bin/bash

OUT="$1"
LEARNER_DIRECTORY="$2"
ALIAS="$3"
DOMAIN="$4"
PROBLEM="$5"

# Assumes that fast-downward.py is aliased as downward
../Dependencies/fast-downward/fast-downward.py --plan-file ${OUT} --alias ${ALIAS} ${LEARNER_DIRECTORY}/output/enhancedDomain.pddl ${PROBLEM}