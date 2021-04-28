using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace BaseSim2021
{
    /// <summary>
    /// This class represents an indexed (i. e., normalised between a min and max value) value
    /// for indicators (a population statistic), groups (specific population groups, the value
    /// indicating their happiness on a 0-100 scale), policies (spending, taxes, and quests,
    /// valued between 0-100, the only values set directly by the player), perks (positive situations
    /// giving benefits when active, scale is 0-1000), and crises (negative situations giving 
    /// problems when active, scale is 0-1000). Indexed values are constructed from XML.
    /// </summary>
    public class IndexedValue
    {
        public enum ValueType { Indicator=0, Group=1, Policy=2, Perk=3, Crisis=4 };
        public enum ResultType { None=0, Win=1, Lose=2};
        #region attributes and encapsulated properties
        public ResultType Result { get; private set; } = ResultType.None; 
        public ValueType Type { get; private set; } = ValueType.Indicator;
        public bool? Active { get; private set; } = null;
        private readonly int? activationThreshold = null;
        private readonly int? deactivationThreshold = null;
        public int? AvailableAt { get; private set; } = null;
        private readonly int minValue=0;
        private readonly int maxValue=0;
        private double actualValue;
        private double currentInfluence = 0;
        public int? MoneyAmount { get; private set; } = null;
        public int? GloryAmount { get; private set; } = null;
        private readonly Func<double, double> cost = FunLibrary.Linear;
        private readonly Func<double, double> effect = FunLibrary.Linear;
        public Func<double, double> Cost { get { return cost; } }
        public Func<double, double> Effect { get { return effect; } }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Dictionary<IndexedValue, double> OutputWeights { get; private set; } = new Dictionary<IndexedValue, double>();
        private readonly Dictionary<string, double> tmpOutputs = new Dictionary<string, double>();
        private readonly string tmpDifficulty = null;
        #endregion
        #region computed properties / read-only
        public int Value { get { return (int)Math.Max(minValue, Math.Min(maxValue, actualValue)); } }
        public double Impact { get { return Math.Max(-1.0, Math.Min(1.0, effect((((actualValue - minValue) * 2.0) / (maxValue - minValue)) - 1.0))); } }

        public int MoneyImpacted { get { return (int)Math.Abs(MoneyAmount.GetValueOrDefault(0) * Impact); } }
        public int GloryImpacted { get { return (int)Math.Abs(GloryAmount.GetValueOrDefault(0) * Impact); } }
        public int MinValue { get { return minValue; } }
        public int MaxValue { get { return maxValue; } }
        #endregion
        #region co-modification methods
        /// <summary>
        /// Updates a value, and also checks for its possible activation
        /// </summary>
        public void Update()
        {
            if (currentInfluence != 0)
            {
                actualValue += currentInfluence;
                actualValue = Math.Max(minValue, Math.Min(maxValue, actualValue));
                CheckActivation();
                currentInfluence = 0;
            }
        }
        /// <summary>
        /// Updates the influence on this value (to serve as a modifier for the value at the next step)
        /// </summary>
        /// <param name="modifier">The update for this influence, -1.0 to 1.0</param>
        public void Influence(double modifier)
        {
            double realMod = Math.Max(Math.Min(modifier, 1.0), -1.0);
            currentInfluence += realMod * (maxValue - minValue);
        }
        /// <summary>
        /// Checks if the value becomes active or inactive, and reacts accordingly.
        /// </summary>
        public void CheckActivation()
        {
            if (!Active.GetValueOrDefault(false) && activationThreshold.HasValue && actualValue >= activationThreshold.Value)
            {
                Active = true;
                GameController.Activate(this);
            }
            else if (Active.GetValueOrDefault(false) && deactivationThreshold.HasValue && actualValue <= deactivationThreshold.Value)
            {
                Active = false;
                GameController.Deactivate(this);
            }
        }
        /// <summary>
        /// Propagates the current value as influence to all other values
        /// </summary>
        public void Propagate()
        {
            foreach (IndexedValue iv in OutputWeights.Keys)
            {
                iv.Influence(Impact * OutputWeights[iv]);
            }
        }
        #endregion
        #region XML-based constructor
        /// <summary>
        /// XML constructor for an indexed value
        /// </summary>
        /// <param name="element">The Xelement loaded from an XML file describing the value</param>
        public IndexedValue(XElement element)
        {
            if (element.Name.ToString() != "iValue") return;
            foreach (XElement e in element.Elements())
            {
                switch (e.Name.ToString())
                {
                    case "name":
                        Name = e.Value;
                        break;
                    case "desc":
                        Description = e.Value;
                        break;
                    case "type":
                        int a = -1;
                        _ = int.TryParse(e.Value.ToString(), out a);
                        Type = (ValueType)(a >= 0 && a <= 4 ? a : 0);
                        break;
                    case "result":
                        int z = -1;
                        int.TryParse(e.Value.ToString(), out z);
                        Result = (ResultType)(z >= 0 && z <= 2 ? z : 0);
                        break;
                    case "cost":
                        if (FunLibrary.Functions.ContainsKey(e.Value.ToString()))
                        {
                            cost = FunLibrary.Functions[e.Value.ToString()];
                        }
                        break;
                    case "effect":
                        if (FunLibrary.Functions.ContainsKey(e.Value.ToString()))
                        {
                            effect = FunLibrary.Functions[e.Value.ToString()];
                        }
                        break;
                    case "active":
                        bool act;
                        try
                        {
                            act = bool.Parse(e.Value.ToString());
                        }
                        catch (Exception)
                        {
                            Active = null;
                            break;
                        }
                        Active = act;
                        break;
                    case "actThres":
                        int b = -1;
                        int.TryParse(e.Value.ToString(), out b);
                        activationThreshold = b;
                        if (activationThreshold == -1)
                        {
                            activationThreshold = null;
                        }
                        break;
                    case "deactThres":
                        int c = -1;
                        _ = int.TryParse(e.Value.ToString(), out c);
                        deactivationThreshold = c;
                        if (deactivationThreshold == -1)
                        {
                            deactivationThreshold = null;
                        }
                        break;
                    case "availableAt":
                        int d = -1;
                        _ = int.TryParse(e.Value.ToString(), out d);
                        AvailableAt = d;
                        if (AvailableAt == -1)
                        {
                            AvailableAt = null;
                        }
                        break;
                    case "min":
                        int f = 0;
                        _ = int.TryParse(e.Value.ToString(), out f);
                        minValue = f;
                        break;
                    case "max":
                        int g = 0;
                        _ = int.TryParse(e.Value.ToString(), out g);
                        maxValue = g;
                        break;
                    case "value":
                        double h = 0;
                        _ = double.TryParse(e.Value.ToString(), out h);
                        actualValue = h;
                        break;
                    case "money":
                        int i = -1;
                        _ = int.TryParse(e.Value.ToString(), out i);
                        MoneyAmount = i;
                        if (MoneyAmount == -1)
                        {
                            MoneyAmount = null;
                        }
                        break;
                    case "glory":
                        int j = -1;
                        _ = int.TryParse(e.Value.ToString(), out j);
                        GloryAmount = j;
                        if (GloryAmount == -1)
                        {
                            GloryAmount = null;
                        }
                        break;
                    case "difficulty":
                        tmpDifficulty = e.Value;
                        break;
                    case "outputs":
                        foreach (XElement w in e.Nodes())
                        {
                            if (w.Name.ToString() == "value" && w.HasAttributes)
                            {
                                tmpOutputs.Add(w.Attribute("val").Value, double.Parse(w.Attribute("weight").Value.ToString()));
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion
        #region external modification methods
        /// <summary>
        /// Change the policies for some cost
        /// </summary>
        /// <param name="amount">Amount to be set (min to max)</param>
        /// <param name="mCost">Cost in money (relative)</param>
        /// <param name="gCost">Cost in glory (relative)</param>
        public void ChangeTo(int amount, out int mCost, out int gCost)
        {
            PreviewPolicyChange(ref amount, out mCost, out gCost);
            actualValue = amount;
            if (actualValue>=minValue)
            {
                Active = true;
            }
            actualValue = Math.Min(maxValue, Math.Max(minValue, actualValue));
        }
        /// <summary>
        /// Computes the cost of a policy change according to a given amount
        /// </summary>
        /// <param name="amount">Amount to be set (min to max)</param>
        /// <param name="mCost">Cost in money (relative)</param>
        /// <param name="gCost">Cost in glory (relative)</param>
        public void PreviewPolicyChange(ref int amount, out int mCost, out int gCost)
        {
            amount = Math.Min(Math.Max(amount, minValue), maxValue);
            double delta = Math.Abs(actualValue - amount);
            delta /= maxValue - minValue;
            delta = cost(delta);
            mCost = (int)(MoneyAmount.GetValueOrDefault(0) * delta);
            gCost = (int)(GloryAmount.GetValueOrDefault(0) * delta);
        }
        /// <summary>
        /// Deactivates a policy
        /// </summary>
        /// <param name="mCost">Cost of the deactivation in money (relative)</param>
        /// <param name="gCost">Cost of the deactivation in glory (relative)</param>
        internal void Deactivate(out int mCost, out int gCost)
        {
            mCost = MoneyAmount.GetValueOrDefault(0);
            gCost = GloryAmount.GetValueOrDefault(0);
            actualValue = minValue;
            Active = false;
        }
        #endregion
        #region additional initialisation methods
        /// <summary>
        /// Used after all values have been loaded to link that value to all outputs
        /// </summary>
        /// <param name="values">The list of all values</param>
        internal void LinkTo(List<IndexedValue> values)
        {
            foreach (string s in tmpOutputs.Keys)
            {
                IndexedValue output = values.Exists(v=>v.Name.Equals(s))?values.Find(v => v.Name.Equals(s)):null;
                if (output != null)
                {
                    OutputWeights.Add(output, tmpOutputs[s]);
                }
            }
        }
        /// <summary>
        /// Called after loading to activate the difficulty-dependent values
        /// </summary>
        /// <param name="diff">The selected difficulty</param>
        internal void ActivateForDiff(WorldState.Difficulty diff)
        {
            if (Active==false && tmpDifficulty!=null)
            {
                switch (tmpDifficulty.ToLower())
                {
                    case "easy":
                        Active = diff == WorldState.Difficulty.Easy;
                        break;
                    case "medium":
                        Active = diff < WorldState.Difficulty.Hard;
                        break;
                    case "hard":
                        Active = true;
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion
        #region text-display methods
        /// <summary>
        /// Basic ToString()
        /// </summary>
        /// <returns>A short description</returns>
        public override string ToString()
        {
            return (Name + ":" + Value);
        }
        /// <summary>
        /// Computes a full description of the value
        /// </summary>
        /// <returns>A long description</returns>
        public string CompletePresentation()
        {
            string pres = "==" + Name + "==";
            pres += Environment.NewLine;
            pres += Description + Environment.NewLine;
            pres += "Valeur : <" + minValue + " :" + Value + ": " + maxValue + ">" + Environment.NewLine;
            if (MoneyAmount.HasValue && MoneyAmount.GetValueOrDefault(0)<0)
            {
                pres += "Coûte " + MoneyImpacted + " pièces d'or par tour" + Environment.NewLine;
            }
            if (MoneyAmount.HasValue && MoneyAmount.GetValueOrDefault(0) > 0)
            {
                pres += "Rapporte " + MoneyImpacted + " pièces d'or par tour" + Environment.NewLine;
            }
            if (GloryAmount.HasValue && GloryAmount.GetValueOrDefault(0) < 0)
            {
                pres += "Coûte " + Math.Abs(GloryAmount.Value) + " gloire" + Environment.NewLine;
            }
            if (GloryAmount.HasValue && GloryAmount.GetValueOrDefault(0) > 0)
            {
                pres += "Rapporte " + GloryImpacted + " gloire par tour" + Environment.NewLine;
            }
            pres += "Actuellement " + ((Active != false) ? "en action" : "hors action");
            return pres;
        }
        #endregion
    }
}