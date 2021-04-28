using System;
using System.Collections.Generic;

namespace BaseSim2021
{
    /// <summary>
    /// This contains a few numeric functions used to model costs and effects.
    /// Linear and Sigmoid are from -1 - +1 to -1 - +1, NoPositive is to -1 - 0,
    /// while NoNegative and the Policy functions are to 0 - +1. The first should
    /// be used for indicators or groups (in order to reverse the effect when the
    /// value is less than the average point), the others for policies, perks and
    /// crises (in order to have an effect from the first active values onwards).
    /// </summary>
    public static class FunLibrary
    {
        #region the numeric functions, approximately from and to the -1 : +1 domain
        public static Func<double, double> Linear { get { return (a => a); } }
        public static Func<double, double> Sigmoid { get { return (a => 2.0 / (1 + Math.Exp(a * -5.0)) - 1.0); } }
        public static Func<double, double> NoPositive { get { return (a => a > 0 ? 0 : a); } }
        public static Func<double, double> NoNegative { get { return (a => a > 0 ? a : 0); } }
        public static Func<double, double> PolicyLinear { get { return (a => (a + 1.0) / 2.0); } }
        public static Func<double, double> PolicyLog { get { return (a => Math.Log10((a + 1.22) * 4.5)); } }
        public static Func<double, double> PolicyExp { get { return (a => (Math.Pow(2.0, (a + 1)) - 1.0) / 3.0); } }
        #endregion
        public static Dictionary<string, Func<double, double>> Functions { get; } = new Dictionary<string, Func<double, double>>()
        {
            {"linear", Linear },
            {"sigmoid", Sigmoid },
            {"noPositive", NoPositive },
            {"noNegative", NoNegative },
            {"policyLinear", PolicyLinear },
            {"policyLog", PolicyLog },
            {"policyExp", PolicyExp }
        };
        public static void DisplayTest()
        {
            foreach (string s in Functions.Keys)
            {
                Console.WriteLine(s + ": -1=" + Functions[s](-1) + ", 0=" + Functions[s](0) + ", 1=" + Functions[s](1));
            }
        }
    }
}