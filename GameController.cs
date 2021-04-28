using System;
using System.Collections.Generic;

namespace BaseSim2021
{
    /// <summary>
    /// Controler constructed for routing player interaction to the view.
    /// The current view used here is the Console.
    /// </summary>
    public static class GameController
    {
        #region attributes and methods for text interaction
        private static GameView theView;
        private static WorldState theWorld;
        private static readonly Dictionary<string, Action<string>> commands = new Dictionary<string, Action<string>>();
        /// <summary>
        /// Sets the global view
        /// </summary>
        /// <param name="view">The main window</param>
        public static void SetView(GameView view)
        {
            theView = view;
        }

        private static string DefaultCommand { get; set; } = "aide";
        private static void MakeCommands()
        {
            commands.Add("aide", DisplayHelp);
            commands.Add("etat", DisplayStatus);
            commands.Add("historique", DisplayHistory);
            commands.Add("liste", DisplayList);
            commands.Add("politique", ApplyPolicyChanges);
            commands.Add("suivant", s => theWorld.NextTurn());
            commands.Add("quitter", s => Environment.Exit(0));
        }
        /// <summary>
        /// Interprets a string command and performs the required actions
        /// </summary>
        /// <param name="input">The string to interpret</param>
        public static void Interpret(string input)
        {
            if (theWorld.Finished())
            {
                theView.WriteLine("Partie terminée.");
                theView.Refresh();
                return;
            }
            string command = input.Split(' ')[0];
            string arguments = input.Contains(" ") ? input.Substring(input.IndexOf(' ') + 1) : "";
            if (input == "")
            {
                commands[DefaultCommand]("");
                switch (DefaultCommand)
                {
                    case "aide": DefaultCommand = "etat"; break;
                    case "etat": DefaultCommand = "liste"; break;
                    case "suivant": DefaultCommand = "etat"; break;
                    default: DefaultCommand = "suivant"; break;
                }
            }
            else if (commands.ContainsKey(command))
            {
                commands[command](arguments);
            }
            else
            {
                theView.WriteLine("Commande inconnue : " + input);
                DisplayHelp("");
            }
            theView.WriteLine(DefaultCommand + ">");
            theView.Refresh();
        }
        private static void DisplayList(string arg)
        {
            if (arg != null && arg != "")
            {
                switch (arg.ToLower())
                {
                    case "groupes":
                        theView.WriteLine("-Groupes- : ");
                        theWorld.Groups.FindAll(g => g.Active != false).ForEach(g => theView.WriteLine(g.ToString()));
                        break;
                    case "indicateurs":
                        theView.WriteLine("-Indicateurs- : ");
                        theWorld.Indicators.FindAll(i => i.Active != false).ForEach(i => theView.WriteLine(i.ToString()));
                        break;
                    case "politiques":
                        theView.WriteLine("-Politiques actives (sauf taxes et quetes)- : ");
                        theWorld.Expenses.FindAll(p => p.Active != false).ForEach(p => theView.WriteLine(p.ToString()));
                        theView.WriteLine("-Politiques disponibles (sauf taxes et quetes)- : ");
                        theWorld.Expenses.FindAll(p => p.Active == false && p.AvailableAt <= theWorld.Turns).ForEach(p => theView.WriteLine(p.ToString()));
                        break;
                    case "taxes":
                        theView.WriteLine("-Taxes actives- : ");
                        theWorld.Taxes.FindAll(p => p.Active != false).ForEach(p => theView.WriteLine(p.ToString()));
                        theView.WriteLine("-Taxes disponibles- :");
                        theWorld.Taxes.FindAll(p => p.Active == false && p.AvailableAt <= theWorld.Turns).ForEach(p => theView.WriteLine(p.ToString()));
                        break;
                    case "quetes":
                        theView.WriteLine("-Quêtes actives- : ");
                        theWorld.Quests.FindAll(p => p.Active != false).ForEach(p => theView.WriteLine(p.ToString()));
                        theView.WriteLine("-Quêtes disponibles- :");
                        theWorld.Quests.FindAll(p => p.Active == false && p.AvailableAt <= theWorld.Turns).ForEach(p => theView.WriteLine(p.ToString()));
                        break;
                    case "benefices":
                        theView.WriteLine("-Bénéfices- : ");
                        theWorld.Perks.FindAll(p => p.Active == true).ForEach(p => theView.WriteLine(p.ToString()));
                        break;
                    case "problemes":
                        theView.WriteLine("-Problèmes- : ");
                        theWorld.Crises.FindAll(c => c.Active == true).ForEach(c => theView.WriteLine(c.ToString()));
                        break;
                    default: break;
                }
            }
            else
            {
                DisplayList("groupes");
                DisplayList("indicateurs");
                DisplayList("politiques");
                DisplayList("taxes");
                DisplayList("quetes");
                DisplayList("benefices");
                DisplayList("problemes");
            }
            theView.Refresh();
        }
        private static void DisplayStatus(string arg)
        {
            if (arg != null && arg != "")
            {
                theWorld.Values.FindAll(val => val.Name.ToLower().Equals(arg.ToLower())).ForEach(val => theView.WriteLine(val.CompletePresentation()));
            }
            else
            {
                string s = "Etat actuel : Tour " + (theWorld.Turns + 1) + " ";
                if (theWorld.TurnsLeft >= 0)
                {
                    s+="(" + theWorld.TurnsLeft + " restant) ";
                }
                theView.WriteLine(s+"Finances : " + theWorld.Money + " pièces d'or, gloire : " + theWorld.Glory);
            }
            theView.Refresh();
        }
        private static void DisplayHelp(string s)
        {
            switch (s)
            {
                case "":
                    string str = "Actions possibles : ";
                    foreach (string k in commands.Keys)
                    {
                        str += k + " ";
                    }
                    theView.WriteLine(str);
                    break;
                case "aide":
                    theView.WriteLine("Affiche l'aide. Sans argument : liste les commandes, avec : explique une commande.");
                    break;
                case "etat":
                    theView.WriteLine("Affiche une valeur. Sans argument : état du monde, avec : examine une valeur.");
                    break;
                case "historique":
                    theView.WriteLine("Affiche l'historique d'une valeur. Argument : nom de la valeur.");
                    break;
                case "liste":
                    theView.WriteLine("Liste des: valeurs (toutes), groupes, indicateurs, politiques, taxes, quêtes, bénéfices, problèmes.");
                    break;
                case "politique":
                    theView.WriteLine("Met en place, modifie ou revoque une politique. Argument 1 : nom de la politique. Argument 2 : degre (0 pour revoquer).");
                    break;
                case "suivant":
                    theView.WriteLine("Passe au tour suivant.");
                    break;
                case "quitter":
                    theView.WriteLine("Met fin a la partie (sans sauvegarde)");
                    break;
                default:
                    theView.WriteLine("Mot-clé inconnu : " + s);
                    break;
            }
            theView.Refresh();
        }
        private static void DisplayHistory(string s)
        {
            if (s != null && s != "")
            {
                theWorld.ValuesLog.ForEach(vl => vl.FindAll(v => v.Split(':')[0].ToLower().Equals(s.ToLower())).ForEach(v => theView.WriteLine(v + ", ")));
                theWorld.Values.FindAll(val => val.Name.ToLower().Equals(s.ToLower())).ForEach(val => theView.WriteLine(val.ToString()));
                if (s.ToLower().Equals("gloire"))
                {
                    theView.WriteLine("gloire:" + theWorld.Glory);
                }
                if (s.ToLower().Equals("finances"))
                {
                    theView.WriteLine("finances:" + theWorld.Money);
                }
            }
            else
            {
                theView.WriteLine("Quel historique ?");
            }
            theView.Refresh();
        }
        /// <summary>
        /// Sets the global state of the simulation
        /// </summary>
        /// <param name="world">The main world-state object</param>
        public static void SetWorld(WorldState world)
        {
            theWorld = world;
            MakeCommands();
            theView.WriteLine(DefaultCommand + ">");
            theView.Refresh();
        }
        #endregion
        #region public anchor methods
        /// <summary>
        /// Method called when a value is activated
        /// </summary>
        /// <param name="indexedValue">The newly active value</param>
        public static void Activate(IndexedValue indexedValue)
        {
            theView.WriteLine("Nouvel effet actif : " + indexedValue.ToString());
            theView.Refresh();
        }
        /// <summary>
        /// Method called when a value is deactivated
        /// </summary>
        /// <param name="indexedValue">The newly inactive value</param>
        public static void Deactivate(IndexedValue indexedValue)
        {
            theView.WriteLine("Fin de l'action de : " + indexedValue.ToString());
            theView.Refresh();
        }
        /// <summary>
        /// Method called when a yes/no dialog should be displayed
        /// </summary>
        /// <returns>Yes iff confirmation is made</returns>
        public static bool ConfirmDialog()
        {
            return theView.ConfirmDialog();
        }
        /// <summary>
        /// Method called whenever the game is lost
        /// </summary>
        /// <param name="indexedValue">The value responsible or null for debt</param>
        public static void LoseDialog(IndexedValue indexedValue)
        {
            theView.WriteLine("Partie perdue." +
                (indexedValue==null ? " Dette insurmontable." : indexedValue.CompletePresentation()));
            theView.Refresh();
        }
        /// <summary>
        /// Method called whenever the game is won
        /// </summary>
        public static void WinDialog()
        {
            theView.WriteLine("Partie gagnée.");
            theView.Refresh();
        }
        /// <summary>
        /// Method called to change a policy
        /// </summary>
        /// <param name="arg">The arguments to the "policy" command</param>
        public static void ApplyPolicyChanges(string arg)
        {
            if (!arg.Contains(" "))
            {
                theView.WriteLine("erreur texte application politique : " + arg);
                return;
            }
            string pol = arg.Split(' ')[0];
            int.TryParse(arg.Split(' ')[1], out int amount);
            if (amount < 0)
            {
                theView.WriteLine("erreur nombre application politique : " + arg);
                return;
            }
            IndexedValue val = theWorld.FindPolicyOrDefault(pol);
            if (val==null)
            {
                theView.WriteLine("erreur nom application politique : " + arg);
                return;
            }

            int mCost;

            int gCost;
            if (amount == 0)
            {
                theWorld.DeactivatePolicy(val, out mCost, out gCost);
                theView.Refresh();
                return;
            }
            val.PreviewPolicyChange(ref amount, out mCost, out gCost);
            theView.WriteLine("Estimation : " + mCost + " pièces (par tour) et " + gCost + " gloire.");
            if (!ConfirmDialog())
            {
                return;
            }
            if (gCost < 0)
            {
                if (theWorld.CostGlory(gCost))
                {
                    theView.WriteLine("Changement effectue pour " + gCost + " gloire.");
                    val.ChangeTo(amount, out _, out _);
                    theView.Refresh();
                    return;
                }
                else
                {
                    theView.WriteLine("Gloire insuffisante : " + theWorld.Glory);
                    return;
                }
            }
            theView.WriteLine("Changement effectué.");
            val.ChangeTo(amount, out _, out _);
            theView.Refresh();
        }
        /// <summary>
        /// Method called when a policy is deactivated by the player
        /// </summary>
        /// <param name="gCost">The cost (glory)</param>
        public static void DeactivatePolicyDialog(int gCost)
        {
            theView.WriteLine("Desactivation effectuée pour " + gCost + " gloire.");
            theView.Refresh();
        }
        #endregion
    }
}