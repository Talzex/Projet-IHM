using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace BaseSim2021
{
    /// <summary>
    /// This class contains the status of a single game.
    /// Methods are invoked from, or make use of, the GameController
    /// static methods.
    /// </summary>
    public class WorldState
    {
        public enum Difficulty { Easy, Medium, Hard };
        #region world-status variables for a single game
        private int turnsLeft;
        private int turns = 0;

        private readonly List<IndexedValue> values = new List<IndexedValue>();
        private readonly List<IndexedValue> groups = new List<IndexedValue>();
        private readonly List<IndexedValue> indicators = new List<IndexedValue>();
        private readonly List<IndexedValue> policies = new List<IndexedValue>();
        private readonly List<IndexedValue> perks = new List<IndexedValue>();
        private readonly List<IndexedValue> crises = new List<IndexedValue>();
        private readonly List<List<string>> valuesLog = new List<List<string>>();
        private int glory = 0;
        private int money = 0;
        #endregion
        #region read-only accessor properties
        public Difficulty TheDifficulty { get; private set; }
        public int TurnsLeft { get { return turnsLeft; } }
        public int Turns { get { return turns; } }
        public List<IndexedValue> Values { get { return values; } }
        public List<IndexedValue> Groups { get { return groups; } }
        public List<IndexedValue> Indicators { get { return indicators; } }
        public List<IndexedValue> Policies { get { return policies; } }
        public List<IndexedValue> Expenses { get {
                return Policies.FindAll(p=>p.MoneyAmount < 0 && p.GloryAmount < 0);
            }
        }
        public List<IndexedValue> Taxes {  get
            {
                return Policies.FindAll(p => p.MoneyAmount > 0 && p.GloryAmount < 0);
            }
        }
        public List<IndexedValue> Quests {  get
            {
                return Policies.FindAll(p=>p.GloryAmount > 0);
            }
        }
        public List<IndexedValue> Perks { get { return perks; } }
        public List<IndexedValue> Crises { get { return crises; } }
        public List<List<string>> ValuesLog { get { return valuesLog; } }
        public int Glory { get { return glory; } }
        public int Money { get { return money; } }

        #endregion
        #region external game manipulation handlers
        /// <summary>
        /// Method called to get to the next turn and update all indexes
        /// </summary>
        public void NextTurn()
        {
            List<string> valuesThisTurn = new List<string>();
            values.ForEach(v => valuesThisTurn.Add(v.ToString()));
            valuesThisTurn.Add("gloire:" + glory);
            valuesThisTurn.Add("finances:" + money);
            valuesLog.Add(valuesThisTurn);
            values.FindAll(v => v.Active != false).ForEach(p => p.Propagate());
            values.ForEach(v => v.Update());
            indicators.FindAll(i => i.Active != false && i.GloryAmount != null).ForEach(i => { money += i.MoneyImpacted; glory += i.GloryImpacted; });
            policies.FindAll(p => p.Active != false && p.MoneyAmount < 0).ForEach(p => money -= p.MoneyImpacted);
            policies.FindAll(p => p.Active != false && p.MoneyAmount > 0).ForEach(p => money += p.MoneyImpacted);
            policies.FindAll(p => p.Active != false && p.GloryAmount > 0).ForEach(p => glory += p.GloryImpacted);
            perks.FindAll(p => p.Active == true).ForEach(p => { money += p.MoneyImpacted; glory += p.GloryImpacted; });
            perks.FindAll(p => p.Active == false).ForEach(p => p.CheckActivation());
            crises.FindAll(c => c.Active == true).ForEach(c => { money -= c.MoneyImpacted; glory -= c.GloryImpacted; });
            crises.FindAll(c => c.Active == false).ForEach(c => c.CheckActivation());
            turns++;
            if (turnsLeft > 0)
            {
                turnsLeft--;
            }
            if (crises.Exists(c => c.Result == IndexedValue.ResultType.Lose && c.Active == true) || money < 0)
            {
                GameController.LoseDialog(crises.Find(c => c.Result == IndexedValue.ResultType.Lose && c.Active == true));
                turnsLeft = 0;
            }
            else if (perks.Exists(p => p.Result == IndexedValue.ResultType.Win && p.Active == true)) {
                GameController.WinDialog();
                turnsLeft = 0;
            }
        }
        /// <summary>
        /// Method to look a policy up by name
        /// </summary>
        /// <param name="pol">The name to search for</param>
        /// <returns>The policy, null if not found and unique</returns>
        public IndexedValue FindPolicyOrDefault(string pol)
        {
            List<IndexedValue> iVs = policies.FindAll(p => p.Name.ToLower().Equals(pol.ToLower()));
            if (iVs.Count==1)
            {
                return iVs[0];
            }
            return null;
        }
        /// <summary>
        /// Deactivate a policy and display the appropriate results
        /// </summary>
        /// <param name="val">The policy to deactivate</param>
        /// <param name="mCost">The money cost (output)</param>
        /// <param name="gCost">The glory cost (output)</param>
        public void DeactivatePolicy(IndexedValue val, out int mCost, out int gCost)
        {
            val.Deactivate(out mCost, out gCost);
            GameController.DeactivatePolicyDialog(gCost);
            glory -= Math.Abs(gCost);
        }
        /// <summary>
        /// Method called when adding or substracting glory
        /// </summary>
        /// <param name="gCost">Glory update (relative)</param>
        /// <returns>true iff possible and done</returns>
        public bool CostGlory(int gCost)
        {
            if (glory+gCost<0)
            {
                return false;
            }
            glory += gCost;
            return true;
        }
        #endregion
        #region constructor and initialisation methods
        /// <summary>
        /// Parameter constructor
        /// </summary>
        /// <param name="diff">The difficulty for the game</param>
        /// <param name="file">The XML file to load for the world</param>
        public WorldState(Difficulty diff, string file, int turnsLeft = -1)
        {
            this.turnsLeft = turnsLeft;
            TheDifficulty = diff;
            LoadWorldFrom(file, diff);
            switch(diff)
            {
                case Difficulty.Easy:
                    glory = 50;
                    money = 10000000;
                    break;
                case Difficulty.Medium:
                    glory = 30;
                    money = 1000000;
                    break;
                case Difficulty.Hard:
                    glory = 25;
                    money = 500000;
                    break;
            }
        }
        /// <summary>
        /// Method called for initialisation
        /// </summary>
        /// <param name="file">The path to the XML file defining the world</param>
        /// <param name="diff">The difficulty for the game (used for activations)</param>
        public void LoadWorldFrom(string file, Difficulty diff)
        {
            try
            {
                XDocument doc = XDocument.Load(file);
                foreach (XElement el in doc.Root.Nodes())
                {
                    values.Add(new IndexedValue(el));
                }
                values.ForEach(val => val.ActivateForDiff(diff));
                values.ForEach(val=>val.LinkTo(values));
                groups.AddRange(values.FindAll(val => val.Type == IndexedValue.ValueType.Group));
                indicators.AddRange(values.FindAll(val => val.Type == IndexedValue.ValueType.Indicator));
                crises.AddRange(values.FindAll(val => val.Type == IndexedValue.ValueType.Crisis));
                perks.AddRange(values.FindAll(val => val.Type == IndexedValue.ValueType.Perk));
                policies.AddRange(values.FindAll(val => val.Type == IndexedValue.ValueType.Policy));
            }
            catch (IOException e)
            {
                Console.Write("Erreur fichier : " + file + " => " + e);
                Environment.Exit(-1);
            }
        }

        #endregion
        #region control flow methods
        /// <summary>
        /// Indicates if the game is over (won or lost)
        /// </summary>
        /// <returns>True iff the game is finished</returns>
        public bool Finished()
        {
            return turnsLeft == 0;
        }
        #endregion
    }
}