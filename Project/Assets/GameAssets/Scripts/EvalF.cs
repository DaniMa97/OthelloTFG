using System.Globalization;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EvalF : MonoBehaviour
{
    const float INTERNAL_STAB_COEF = 36;
    const float POTENTIAL_MOV_COEF = 99;
    const float ESAC_INTERCEPT = 312;
    const float ESAC_COEF = 6.24f;
    const float CMAC_INTERCEPT_1 = 50;
    const float CMAC_COEF_1 = 2;
    const int CMAC_SPLIT = 25;
    const float CMAC_INTERCEPT_2 = 75;
    const float CMAC_COEF_2 = 1;


    public TextAsset text;

    EvalFAspects evalF;
    double[] dArr;

    void Start()
    {
        evalF = new EvalFAspects();
        //StreamReader sr = new StreamReader(PATH);
        //string s = sr.ReadLine();
        string s = text.text;
        string[] sArr = s.Split(',');
        dArr = new double[sArr.Length];
        for (int i = 0; i < sArr.Length; i++)
        {
            dArr[i] = double.Parse(sArr[i], CultureInfo.InvariantCulture);
        }
    }

    float ComputeEsac(int turn)
    {
        return ESAC_INTERCEPT + ESAC_COEF * turn;
    }

    float ComputeCmac(int turn)
    {
        if (turn <= CMAC_SPLIT)
            return CMAC_INTERCEPT_1 + CMAC_COEF_1 * turn;
        return CMAC_INTERCEPT_2 + CMAC_COEF_2 * turn;
    }

    public double EvaluationFunction(byte[,] board, byte[,] moveBoard, bool isP1Evaluated, bool isP1Turn, int turnCounter)
    {
        double eval = ComputeEsac(turnCounter) * evalF.EdgeStability(board, isP1Evaluated, isP1Turn, dArr) +
            INTERNAL_STAB_COEF * evalF.InternalStability(board, isP1Evaluated) +
            ComputeCmac(turnCounter) * evalF.CurrentMobility(moveBoard, isP1Evaluated) +
            POTENTIAL_MOV_COEF * evalF.PotentialMobility(board, isP1Evaluated);
        return eval;
    }
}
