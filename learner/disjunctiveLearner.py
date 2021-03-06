import subprocess
import sys
import re
import os
import time
import argparse
import collections
import pprint
import csv
import json
import time
import shutil
import io
import itertools
import random
from os import sys, path
import reviewData
import numpy as np
import math

from learner import Learner
from houdiniExtended import HoudiniExtended
from houdini import Houdini

import z3simplify
import logging




logger = logging.getLogger("Framework.DisjunctLearner")
# TODO: calling learner.SetDataPoints changes list of list to list of tuples.

class DisjunctiveLearner(Learner):


    def __init__(self, name, binary, parameters, tempLocation):
        Learner.__init__(self, name, binary, parameters, tempLocation)
        self.entropy = True
        self.numerical = True
        self.allPredicates = True

    def generateFiles(self):
        pass

    def readResults(self):
        pass

    def splitSamples(self, predicate, houdiniEx, datapoints):
        allInputVariables = self.intVariables + self.boolVariables
        pos = []
        neg = []
        for dp in datapoints:
            state = houdiniEx.createStateInformation(
                allInputVariables, dp[0:-1])
            if eval(predicate, state):
                pos.append(dp)
            else:
                neg.append(dp)

        return pos, neg

    def learn(self, dataPoints, simplify=True):
        # Intuition: Only need HoudiniExt to call createAllPredicates()
        # Need Houdini to Learn conjunction
        self.setDataPoints(dataPoints)
        #logger.info("learner "+ str(self.entropy) +str(self.numerical)+ str(self.allPredicates))
        if len(self.dataPoints) == 0:
            return "true"

        houdiniEx = HoudiniExtended("HoudiniExtended", "", "", "")
        houdiniEx.setVariables(self.intVariables, self.boolVariables)
        houdiniEx.setDataPoints(self.dataPoints)
        # for debugging
        houdiniEx.numerical = self.numerical
        
        if len(self.dataPoints) == 1:
            return houdiniEx.learn(self.dataPoints, simplify=True)

        # createAllPredicates() returns
        #logger.info("houdiniExt: numerical: "+str(houdiniEx.numerical))
        allSynthesizedPredicatesPrefix, allSynthesizedPredicatesInfix = houdiniEx.createAllPredicates()
        booleanData = []
        # iterating over rows
        for point in self.dataPoints:
            allInputVariables = self.intVariables + self.boolVariables
            state = houdiniEx.createStateInformation(
                allInputVariables, point[0:-1])
            # only give houdiniEx boolean because at this point no more integers
            booleanData.append(houdiniEx.evalauteDataPoint(
                allSynthesizedPredicatesInfix, state) + [point[-1]])
            # the infix form of the predicates are used to evalute them (into true or false)

        # print booleanData
        # Call Houdini directly
        # Compute All True predicates
        listAllSynthesizePredInfix = list(allSynthesizedPredicatesInfix)
        houd = Houdini("Houdini", "", "", "")
        houd.setVariables([], listAllSynthesizePredInfix)
        houd.learn(booleanData, simplify=False)

        alwaysTruePredicateInfix = []
        #TODO: if houd.LearntConjunction is true, then either we cannot express post condition
        #or postcondition requires disjunction; In case it requires disjunction we need to change the format
        # of output formula at the end of this code
        assert(not (len(houd.learntConjuction) == 1 and "true" in houd.learntConjuction) )
        assert(len(houd.learntConjuction) > 0)
        alwaysTruePredicateInfix = houd.learntConjuction

        # TODO: compute with prefix otherwie z3 throws error
        remainingPredicatesInfix = list(set(
            listAllSynthesizePredInfix).symmetric_difference(set(alwaysTruePredicateInfix)))
        # remainingPredicatesPrefix = list(set(listAllSynthesizePredPrefix).symmetric_difference(set(alwaysTruePredicateInfix)))
        # for computing disjunctions, we only need to considr p or not p both not both
        score = []
        sortedScore = []

        predicatesToSplitOn = remainingPredicatesInfix
        for i in xrange(0, len(predicatesToSplitOn)):
            predicateSplitP = predicatesToSplitOn[i]
            positiveP = []
            negativeP = []
            positiveP, negativeP = self.splitSamples(
                predicateSplitP, houdiniEx, self.dataPoints)
            
            assert( len(positiveP) + len(negativeP) == len(self.dataPoints) )
            if len(positiveP) == 0 and len(negativeP) == 0:
                # a predicate can only be true or false  
                assert(False)

            elif len(positiveP) > 0 and len(negativeP) == 0:
                #this should never happen otherwise houidini learner is wrong
                assert(False)
            elif len(positiveP) == 0 and len(negativeP) > 0:
                #predicate is always false:
                continue
            
            elif len(positiveP) > 0 and len(negativeP) > 0:
                pass
            #print positiveP
            #print negativeP
            boolPDatapoints = []
            boolNegPDatapoints = []
            for posPoint in positiveP:
                allInputVariables = self.intVariables + self.boolVariables
                state = houdiniEx.createStateInformation(
                    allInputVariables, posPoint[0:-1])
                boolPDatapoints.append(houdiniEx.evalauteDataPoint(
                    predicatesToSplitOn, state) + [posPoint[-1]])

            for negPoint in negativeP:
                allInputVariables = self.intVariables + self.boolVariables
                state = houdiniEx.createStateInformation(
                    allInputVariables, negPoint[0:-1])
                boolNegPDatapoints.append(houdiniEx.evalauteDataPoint(
                    predicatesToSplitOn, state) + [negPoint[-1]])

            houd.setVariables([], predicatesToSplitOn)
            conjP = houd.learn(boolPDatapoints, simplify=False)
            conjPList = houd.learntConjuction
            conjN = houd.learn(boolNegPDatapoints, simplify=False)
            conjNList = houd.learntConjuction

            posMultiplier = len(conjPList)
            negMultiplier = len(conjNList)
            if len(conjPList) == 1 and 'true' in conjPList:
                posMultiplier = 0
            if len(conjNList) == 1 and 'true' in conjPList:
                negMultiplier = 0

            plusLabel = ['+'] * posMultiplier
            minusLabel = ['-'] * negMultiplier
            entropyR = 0
            if self.entropy:
                entropyR = self.shannonsEntropy(plusLabel+minusLabel)
            else:
                entropyR = self.scoreByLen(conjPList,conjNList) 
            #score.append({'predicate': predicateSplitP,
            # 'score':self.scoreByLen(conjPList, conjNList) , 'left': conjPList, 'right': conjNList})
            score.append({'predicate': predicateSplitP,
                          'score': entropyR, 'left': conjPList, 'right': conjNList})

            # sortedScore = sorted(score.iteritems(), key=lambda (k,v): v['score'])
        if len(score) == 0:
            alwaysTruePrefix = self.findPrefixForm(alwaysTruePredicateInfix,
                                               allSynthesizedPredicatesInfix, allSynthesizedPredicatesPrefix)
            z3StringFormula = "(and " +' '.join(alwaysTruePrefix)+")"
            z3StringFormula = z3simplify.simplify(self.symbolicIntVariables, self.symbolicBoolVariables, z3StringFormula)
            return z3StringFormula
                         
        sortedScore = sorted(score, key=lambda x: x['score'])
        leftDisjunct = []
        rightDisjunct = []
        choosePtoSplitOn = ""
        if not self.entropy:
            choosePtoSplitOn = sortedScore[-1]['predicate']
            leftDisjunct = sortedScore[-1]['left']
            rightDisjunct = sortedScore[-1]['right']
        else:
            for pred in sortedScore:
                if pred['score'] != 0:
                    print "predicate:"
                    print pred['predicate']
                    choosePtoSplitOn = pred['predicate']
                    print "left:"
                    print pred['left']
                    leftDisjunct = pred['left']
                    print "right:"
                    print pred['right']
                    rightDisjunct = pred['right']
                    break

        print "always true: "
        print alwaysTruePredicateInfix
        #logger.info(' '.join(alwaysTruePredicateInfix))
        alwaysTruePrefix = self.findPrefixForm(alwaysTruePredicateInfix,
                                               allSynthesizedPredicatesInfix, allSynthesizedPredicatesPrefix)
        
        logger.info("predicate to split on:")
        logger.info(choosePtoSplitOn)
        
        #print "or"
        #print leftDisjunct
        #logger.info(' '.join(leftDisjunct))

        leftDisjunctPrefix = self.findPrefixForm(leftDisjunct,
                                                 allSynthesizedPredicatesInfix, allSynthesizedPredicatesPrefix)
        #print rightDisjunct
        #logger.info(' '.join(rightDisjunct))

        rightDisjunctPrefix = self.findPrefixForm(
            rightDisjunct, allSynthesizedPredicatesInfix, allSynthesizedPredicatesPrefix)

        if self.allPredicates:
            z3StringFormula = "(and " +' '.join(alwaysTruePrefix) + "(or " + "(and " + ' '.join(leftDisjunctPrefix) + ") " +"(and "+ ' '.join(rightDisjunctPrefix) +")))"
            z3FormulaInfix = ' && '.join(alwaysTruePredicateInfix)  + " && (" +' && '.join(leftDisjunct) +" || " +' && '.join(rightDisjunct)+ ")"             
        else:
            z3StringFormula = "(or " + "(and " + ' '.join(leftDisjunctPrefix) + ") " +"(and "+ ' '.join(rightDisjunctPrefix) +"))"
            z3FormulaInfix = "("+ ' && '.join(leftDisjunct) +" || " +' && '.join(rightDisjunct)+ ")"             

        #logger.info("unsimplified z3 formula: "+ z3StringFormula)
        logger.info("unsimplified Z3: ")
        logger.info(z3FormulaInfix)

        z3StringFormula = z3simplify.simplify(self.symbolicIntVariables, self.symbolicBoolVariables, z3StringFormula)
       
        #logger.info("simplified z3 formula: "+z3StringFormula)
        logger.info("simplified Z3 Final formula: ")
        logger.info(z3StringFormula)
        #print z3StringFormula
        return z3StringFormula
        # return "(Old_s1Count != New_s1Count )"

    def findPrefixForm(self, infixForm, allInFixPredicateList, allPrefixPredicateList):
        prefixForm = []

        for predToConvert in infixForm:
            assert(not("false" == predToConvert))
            if "true" == predToConvert:
                return ["true"]
            indexToConvert = allInFixPredicateList.index(predToConvert)
            prefixForm.append(allPrefixPredicateList[indexToConvert])

        return prefixForm

    def scoreByLen(self, conjPList, conjNList):
        return len(conjPList)+len(conjNList)

    def scoreByEntropy(self, conjPList, conjNlist):
        pass

    def shannonsEntropy(self, labels, base=None):
        value, counts = np.unique(labels, return_counts=True)
        norm_counts = np.true_divide(counts, counts.sum())
        base = math.e if base is None else base
        return - (norm_counts * np.log(norm_counts) / np.log(base)).sum()


if __name__ == '__main__':

    learner = DisjunctiveLearner("disjunctiveLearner", "", "", "")

    # intVariables = ['oldCount', 's1.Count', 'oldTop', 's1.Peek()', 'oldx', 'x']
    intVariables = ['Old_s1.Count', 'New_s1.Count',
                    'Old_s1.Peek()', 'New_s1.Peek()', 'Old_x', 'New_x']

    boolVariables = ["Old_s1.Contains(x)"]

    learner.setVariables(intVariables, boolVariables)

    dataPoints = [
        ['1', '2', '10', '0', '0', '0', 'true', 'true'],
        ['2', '3', '10', '0', '0', '0', 'true', 'true'],
        ['1', '2', '0', '0', '0', '0', 'true', 'true'],
        ['1', '2', '-5', '2', '2', '2', 'true', 'true'],
        ['1', '2', '0', '1', '1', '1', 'true', 'true'],
        ['1', '2', '1', '0', '0', '0', 'true', 'true'],
        ['1', '2', '2', '0', '0', '0', 'true', 'true']
    ]

    print learner.learn(dataPoints)
