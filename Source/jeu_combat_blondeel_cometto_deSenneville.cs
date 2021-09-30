using System;
using System.Collections.Generic;


//Bon Courage, nous n'avons utilisé qu'un seul fichier qui contient toute les données (notre jeu est bien ceci dit) 
namespace Défi_Jeu_de_Combat
{
    class Program
    {
        //Dictionnaire contenant les couleurs à utiliser selon ce qu'on affiche
        static Dictionary<string, ConsoleColor> colorCode = new Dictionary<string, ConsoleColor>
            {
                {"system", ConsoleColor.Gray},
                {"player", ConsoleColor.Green},
                {"ia", ConsoleColor.DarkRed},
                {"attack", ConsoleColor.Red},
                {"special", ConsoleColor.Yellow},
                {"defend", ConsoleColor.Cyan},  
                {"easy", ConsoleColor.DarkCyan},
                {"medium", ConsoleColor.DarkYellow},
                {"hard", ConsoleColor.Magenta},
                {"damage", ConsoleColor.Yellow},
                {"health", ConsoleColor.Red},
                {"Mana", ConsoleColor.Blue},
                {"Endurance", ConsoleColor.DarkGreen},
                {"Rage", ConsoleColor.DarkYellow},
            };

        //Dictionnaire contenant les descriptions des différentes classes
        static Dictionary<string, string> classDescription = new Dictionary<string, string>{
                {"Combattant", "Le Combattant est très puissant, mais aussi très fragile.\nCependant, les imprudents qui osent l'attaquer le regrette aussitôt." +
                "\nSpécial : Dévie les attaques ennemies et renvoie les dégâts. Ne s'applique pas aux capacités spéciales."},

                {"Templier", "Le Templier est un solat robuste à l'armure impénétrable.\nFervent défenseur de l'Empire, il sacrifierait même sa propre vie pour écraser ses ennemis." +
                "\nSpécial : Sacrifie un point de vie pour infliger une attaque puissante, dont les dégâts bonus transperçent la défense ennemie."},

                {"Nécromancien", "Le Nécromancien est un sorcier lugubre.\nAgent de la Mort, il absorbe la vie de ses proies pour entretenir sa frêle carcasse."+
                "\nSpécial : Si la cible possède plus de vie que le Nécromancien, ce dernier inflige des dégâts malgré la défense, et récupère un point de vie."},

                {"Prêtre", "Abandonné de tous, le Prêtre soigne plaies et maladies.\nMalheureusement, un tel pouvoir n'est d'aucun secours dans un duel, et ne fait que prolonger le désespoir." +
                "\nSpécial : Récupère de la vie, tout simplement."},

                {"Berserker", "Guerrier redoutable, le Berserker est assoiffé de sang, y compris le sien.\nOn raconte que ses blessures ne font que déchaîner la rage qu'il accumule au combat." +
                "\nSpécial : Inflige une attaque plus puissante en fonction des points de vie qu'il lui manque."}
            };

        //Dictionnaire contenant les descriptions des différentes ressources
        static Dictionary<string, string> ressourceDescription = new Dictionary<string, string>{
            {"Endurance","\nLes personnages utilisant de l'endurance en consomme 1 par attaque et 2 par utilisation de spécial." +
                "\nElle se régénère de 1 à chaque tour, et 1 bonus peut être obtenu en bloquant une attaque."},
            {"Mana", "\nLes personnages utilisant du mana en consomme 2 par utilisation de spécial, et s'en régénère 2 lors d'une défense." },
            {"Rage", "\nLes personnages utilisant de la rage en consomme 3 par utilisation de spécial, et s'en régénère 1 lors d'une attaque."}
        };

        //Liste des classes disponibles
        static List<CharacterClass> listeClass = new List<CharacterClass>();

        //Array des actions possibles
        static string[] possibleAction = { "attack", "special", "defend" };

        //Initialisation des classes du joueur et de l'ordinateur
        static CharacterClass playerChar;
        static CharacterClass iaChar;

        //Variables diverses pour la gestion de la partie
        static bool endGame = false;
        static string playerName = "Godefroy de Montmirail";
        static int difficulty;
        static int tour = 0;
        static Random rand = new Random();

        static void Main()
        {
            //Création des classes et ajout dans la liste des classes. 
            CharacterClass Combattant = new CharacterClass(2, 9, "Combattant", "Endurance", 3);
            listeClass.Add(Combattant);

            CharacterClass Templier = new CharacterClass(1, 12, "Templier", "Endurance", 3);
            listeClass.Add(Templier);

            CharacterClass Prêtre = new CharacterClass(2, 10, "Prêtre", "Mana", 3);
            listeClass.Add(Prêtre);

            CharacterClass Nécromancien = new CharacterClass(2, 9, "Nécromancien", "Mana", 3);
            listeClass.Add(Nécromancien);

            CharacterClass berserker = new CharacterClass(1, 8, "Berserker", "Rage", 1);
            listeClass.Add(berserker);

            #region "Introduction"
            
            DisplayPanel("PANTHEON DES HEROS");
            Console.WriteLine("Bienvenue héros, quel est votre nom ?");
            playerName = Console.ReadLine();

            //Si le nom est plzHireMe, accéder au royaume de test
            if (playerName == "plzHireMe")
            {
                while (true)
                {
                    Ia_vs_Ia();
                    Console.WriteLine("Voulez-vous continuer les test ?\n o-Oui n-Non");
                    if (Console.ReadLine() != "o")
                        break;
                    Console.Clear();
                }
                return;
            }

            Console.ForegroundColor = colorCode["system"];
            Console.WriteLine("Très bien, un combattant de plus ! Dites-moi, dans quelle catégorie dois-je vous inscrire ?");

            //Choix de la difficulté
            difficulty = Difficulty();

            Console.WriteLine("\nBien, tout est en ordre. Suivez ce couloir et prenez la troisième sortie à droite, on vous équipera.\n[Entrée] Pour continuer...");
            Console.ReadLine();
            Console.Clear();
            
            //Choix de la classe
            playerChar = ChooseCharClass();

            //Retrait de la classe choisie par le joueur de la liste 
            listeClass.Remove(playerChar);

            Console.WriteLine("L'ordinateur choisit sa classe...");
            System.Threading.Thread.Sleep(700);

            //Choix d'une classe aléatoire parmi celles restantes
            int iaChoice = rand.Next(listeClass.Count);
            iaChar = listeClass[iaChoice];
            Console.WriteLine("L'ordinateur a choisi {0}.", iaChar.className);

            Console.WriteLine("\n[Entrée] Pour continuer...");
            Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Vous entrez dans l'arène et faites face à votre adversaire...");

            System.Threading.Thread.Sleep(2000);
            #endregion

            #region "Fight"
            while (!endGame)
            {
                ++tour;
                Console.ForegroundColor = colorCode["system"];
                Console.WriteLine("\n====TOUR {0}====", tour);
                

                //Affichage des stats (PV + ressources) des deux joueurs
                DisplayStats(playerName, playerChar);
                DisplayStats("Ordinateur", iaChar);

                //Choix de l'action
                playerChar.roundAction = PlayerTurn();

                //Selection de l'action de l'IA - ia Turn
                iaChar.roundAction = IaTurn(difficulty, iaChar, playerChar);

                //Execution des actions
                playerChar.Action(iaChar);
                iaChar.Action(playerChar);

                //Affichage des punchlines
                Punchline(playerName, playerChar.punchline);
                Punchline("Ordinateur", iaChar.punchline);
                

                //Condition de fin de jeu et de victoire/défaite
                Console.ForegroundColor = colorCode["system"];
                if (playerChar.health <= 0)
                {
                    //Si l'on n'a plus de vie...
                    Console.WriteLine("\nVous êtes mort, quel dommage.");
                    if (iaChar.health <= 0)
                        //Si l'adversaire n'a également plus de vie...
                        Console.WriteLine("Si ça peut vous rassurer, votre adversaire n'a pas survécu à ses blessures.");
                    else
                    {
                        System.Threading.Thread.Sleep(1800);
                        Console.WriteLine("Votre adversaire observe votre cadavre gisant et prononce ces mots : ");
                        System.Threading.Thread.Sleep(2000);
                        Console.ForegroundColor = colorCode["ia"];
                        Console.WriteLine("GIT");
                        System.Threading.Thread.Sleep(1000);
                        Console.WriteLine("GUD");
                        Console.ForegroundColor = colorCode["system"];
                    }
                    endGame = true;
                }

                else if (iaChar.health <= 0)
                {
                    //Sinon, si notre adversaire n'a plus de vie...
                    endGame = true;
                    Console.WriteLine("\nVotre adversaire a été réduit en poussière.");
                    System.Threading.Thread.Sleep(1500);
                    Console.WriteLine("\nLes autres participants n'ont plus trop envie de se battre, maintenant...");
                    System.Threading.Thread.Sleep(2600);
                    int gold = rand.Next(10000, 20000);
                    Console.WriteLine($"\nVous récupérez les {gold} pièces de la mise, puis sentez une pointe froide dans votre dos...");
                    System.Threading.Thread.Sleep(3000);
                    Console.WriteLine("\nVous êtes mort, quel dommage.");
                    System.Threading.Thread.Sleep(2000);

                    Console.WriteLine("\nVotre sang s'écoule autour de vous et dessine les mots suivants : ");
                    System.Threading.Thread.Sleep(2000);
                    Console.ForegroundColor = colorCode["ia"];
                    Console.WriteLine("GIT");
                    System.Threading.Thread.Sleep(1000);
                    Console.WriteLine("GUD");
                    Console.ForegroundColor = colorCode["system"];
                }
                else
                {
                    //Sinon, recommencer
                    Console.WriteLine("\nVous vous replacez en garde...");
                    System.Threading.Thread.Sleep(1000);
                }
            }
            #endregion
            Console.ReadLine();
        }

        #region "Affichage"
        static void DisplayPanel(string title)
        {
            //Crée un panneau contenant un titre en son centre
            Console.WriteLine($"+{new string('-', title.Length + 2)}+\n" +
            $"|{new string(' ', title.Length + 2)}|\n" +
            $"| {title} |\n" +
            $"|{new string(' ', title.Length + 2)}|\n" +
            $"+{new string('-', title.Length + 2)}+");
        }
        static void DisplayStats(string name, CharacterClass classToRender)
        {
            //Affiche le nom et la classe du personnage en couleur système (gris clair)
            Console.ForegroundColor = colorCode["system"];
            Console.Write($"\n{name} ({classToRender.className}) : ");

            //Affiche la vie actuelle du personnage en couleur vie (rouge)
            Console.ForegroundColor = colorCode["health"];
            Console.Write($"{new string('♥', classToRender.health)}");

            //Affiche la vie manquante du personnage, ainsi qu'une démaraquation à la fin, en couleur système
            Console.ForegroundColor = colorCode["system"];
            Console.Write($"{new string('♥', classToRender.maxHealth - classToRender.health)} | ");

            //Affiche les points de ressource du personnage, avec la couleur correspondant à cette dernière
            Console.ForegroundColor = colorCode[classToRender.ressource];
            Console.Write($"{new string('▼', classToRender.ressourceAmount)}");
        }
        static void Punchline(string name, string punchline)
        {
            //Paramètre la couleur en tant que couleur joueur
            Console.ForegroundColor = colorCode["player"];

            if (name == "Ordinateur")
            {
                //si le personnage est "Ordinateur", on utilise la couleur ia
                Console.ForegroundColor = colorCode["ia"];
            }
            Console.Write($"\n{name} : ");
            foreach (char letter in punchline)
            {
                //Affiche chaque caractère de la phrase 1 par 1, avec une légère latence
                Console.Write(letter);
                System.Threading.Thread.Sleep(28);
            }
            System.Threading.Thread.Sleep(200);
        }
        #endregion

        #region "Settings"
        static int Difficulty()
        {
            while (true)
            {
                //Affichage des différentes options de difficulté
                Console.ForegroundColor = colorCode["easy"];
                Console.WriteLine(" 1 - Débutant");
                Console.ForegroundColor = colorCode["medium"];
                Console.WriteLine(" 2 - Intermédiaire");
                Console.ForegroundColor = colorCode["hard"];
                Console.WriteLine(" 3 - Difficile");
                Console.ForegroundColor = colorCode["system"];
                try
                {
                    difficulty = int.Parse(Console.ReadLine());

                    if (difficulty >= 1 && difficulty <= 3)
                    {
                        //Si la difficulté est entre 1 et 3, fermer la boucle
                        break;
                    }
                    else
                    {
                        //Sinon, afficher...
                        Console.WriteLine("Doucement manant, choisis un niveau de difficulté qui existe.");
                    }
                }
                catch
                {
                    //En cas d'erreur (input incorrect), afficher...
                    Console.WriteLine("Doucement manant, choisis un niveau de difficulté qui existe.");
                }
            }
            return difficulty;
        }
        static CharacterClass ChooseCharClass()
        {
            CharacterClass chosenClass;
            Console.ForegroundColor = colorCode["system"];
            DisplayPanel("ARMURERIE");
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("Bien le bonjour dans l'armurerie. Quelle classe souhaitez vous jouer ?");

            while (true)
            {
                //Affiche chacune des classes présentes dans la liste des classes
                for (int i = 0; i < listeClass.Count; i++)
                {
                    //On affiche i+1 pour que la liste commence à 1 et non 0, contrairement aux index
                    Console.WriteLine(" {0} - {1}", i + 1, listeClass[i].className);
                }
                try
                {
                    //On retire 1 à l'input du joueur pour correspondre aux index (commencant par 0 et non 1)
                    int classIndex = int.Parse(Console.ReadLine()) - 1;

                    //Si l'index est valide...
                    if (classIndex >= 0 && classIndex < listeClass.Count)
                    {
                        chosenClass = listeClass[classIndex];

                        //Affiche les dégats de la classe
                        Console.ForegroundColor = colorCode["damage"];
                        Console.WriteLine($"Dégâts : {new string('♦', chosenClass.damage)}");

                        //Affiche la vie de la classe
                        Console.ForegroundColor = colorCode["health"];
                        Console.WriteLine($"Vie : {new string('♥', chosenClass.maxHealth)}");

                        //Affiche la ressource de la classe
                        Console.ForegroundColor = colorCode[chosenClass.ressource];
                        Console.WriteLine($"Ressource : {chosenClass.ressource}");

                        Console.ForegroundColor = colorCode["system"];
                        //Description de la classe
                        Console.WriteLine($"\n{classDescription[chosenClass.className]}" +
                            //Description de la ressource utilisée
                            $"\n{ressourceDescription[chosenClass.ressource]}" +
                            "\n\nCette classe vous convient-elle ?\n[o] Oui | [n] Non");

                        //Si oui, valider le choix
                        if (Console.ReadLine() == "o")
                        {
                            if (chosenClass.className == "Prêtre")
                                Console.WriteLine($"{chosenClass.className}, sérieusement ? Vous allez finir en bouillie.");
                            else
                                Console.WriteLine($"{chosenClass.className}, très bon choix.");
                            break;
                        }
                        //Autrement, Réinitialier l'interface (clear + réaffichage des éléments) 
                        Console.Clear();
                        DisplayPanel("ARMURERIE");
                        Console.WriteLine("Voulez-vous voir une autre classe ?");
                        continue;
                    }
                    //Si l'index n'est pas valide, réinitialiser l'interface
                    else
                    {
                        Console.Clear();
                        DisplayPanel("ARMURERIE");
                        Console.WriteLine("Choissisez une classe disponible.");
                    }

                }
                //En cas d'erreur, réinitialiser l'interface
                catch
                {
                    Console.Clear();
                    DisplayPanel("ARMURERIE");
                    Console.WriteLine("Choissisez une classe disponible.");
                }
            }
            return chosenClass;
        }
        static string PlayerTurn()
        {
            Console.ForegroundColor = colorCode["system"];

            string action;
            while (true)
            {
                Console.WriteLine("\nQue voulez faire ?");
                for (int i = 0; i < possibleAction.Length; i++)
                {
                    //Afficher toutes les actions parmis la liste d'actions possibles
                    Console.ForegroundColor = colorCode[possibleAction[i]];
                    Console.WriteLine("{0} - {1}", i + 1, possibleAction[i]);
                }
                Console.ForegroundColor = colorCode["system"];
                try
                {
                    int selectedAction = int.Parse(Console.ReadLine()) - 1;

                    //Si l'action choisie est valide...
                    if (selectedAction >= 0 && selectedAction < possibleAction.Length)
                    {
                        action = possibleAction[selectedAction];
                        //Si l'on choisit le spécial et qu'on est au tour 1, annuler
                        if (selectedAction == 1 && tour == 1)
                        {
                            Console.WriteLine("Il est trop tôt pour utiliser votre spécial");
                            continue;
                        }

                        //Si l'on peut faire l'action choisie, quitter la boucle
                        else if (playerChar.canDoAction[action])
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Vous n'avez pas assez d'énergie pour faire ça !");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Le dernier héros qui a tenté cette technique n'a " +
                            "plus jamais été revu. Contentez-vous de quelque chose que vous savez faire.");
                    }
                }
                catch
                {
                    Console.WriteLine("Le dernier héros qui a tenté cette technique n'a " +
                        "plus jamais été revu. Contentez-vous de quelque chose que vous savez faire.");
                }
            }

            //Selon l'action choisie, confirmer le choix par une phrase
            switch (action)
            {
                case "attack":
                    Console.WriteLine("Vous vous préparez à attaquer...");
                    break;

                case "defend":
                    Console.WriteLine("Vous vous préparez à défendre...");
                    break;

                case "special":
                    Console.WriteLine("Vous préparez votre spécial...");
                    break;
            }
            return action;
        }
        #endregion

        #region "IA Management"
        static string IaTurn(int difficulty, CharacterClass computer, CharacterClass player)
        {
            string action = "";
            Console.WriteLine("L'odinateur prépare son coup...");
            System.Threading.Thread.Sleep(500);
            //Selon la difficulté, appeler telle méthode pour choisir un coup
            switch (difficulty)
            {
                case 1:
                    action = possibleAction[IaDebutant(computer)];
                    break;
                case 2:
                    action = possibleAction[IaMedium(computer, player)];
                    break;
                case 3:
                    action = possibleAction[IaHard(computer, player)];
                    break;
            }

            return action;
        }
        public static int IaDebutant(CharacterClass computer)
        {
            int iaChoice;
            while (true)
            {
                //Choix aléatoire
                iaChoice = rand.Next(3);

                //Si elle choisit le spécial au premier tour, recommencer le tirage
                if (iaChoice == 1 && tour == 1)
                    continue;

                //Si l'action correspondante est possible, briser la boucle
                if (computer.canDoAction[possibleAction[iaChoice]])
                    break;

            }
            return iaChoice;
        }
        public static int IaMedium(CharacterClass computer, CharacterClass player)
        {
            int i = tour%2;

            int iaChoice = 0;
            //Alterne entre des choix Débutants et Réfléchis
            switch (i)
            {
                case 0:
                    iaChoice = IaDebutant(computer);
                    break;

                case 1:
                    iaChoice = IaHard(computer, player);
                    break;
            }
            return iaChoice;
        }
        public static int IaHard(CharacterClass computer, CharacterClass player)
        {
            int iaChoice;
            while (true)
            {
                iaChoice = rand.Next(3);
                //Si l'ia est un Berserker, que son spécial peut tuer le joueur et qu'elle peut utiliser son spécial, l'utiliser
                if (player.className == "Berserker" && player.health <= computer.damage + (computer.maxHealth - computer.health) && computer.canDoAction["special"])
                {
                    iaChoice = 1;
                }
                //Si l'ia a plus de dégâts que la vie restante et qu'elle peut attaquer, elle attaque forcément
                else if (player.health <= computer.damage && computer.canDoAction["attack"])
                {
                    iaChoice = 0;
                }
                //Si l'ia a 1 ou moins de ressource et que ce n'est pas de la rage, se défendre
                else if (computer.ressourceAmount <= 1 && computer.ressource != "Rage")
                {
                    iaChoice = 2;
                }
                //Si elle plus de 3 de ressource, attaquer ou utiliser le spécial
                else if (computer.ressourceAmount > 3)
                {
                    iaChoice = rand.Next(2);
                }
                //Si l'ia a choisi le spécial et qu'on est au premier tour, la faire attaquer plutôt
                if (iaChoice == 1 && tour == 1)
                    iaChoice = 0;

                //Si le choix est possible, briser la boucle
                if (computer.canDoAction[possibleAction[iaChoice]])
                    break;

            }
            return iaChoice;
        }
        static void Ia_vs_Ia()
        {
            Console.WriteLine("Bonjour. Tu es ici dans la salle de test, ou tu pourras regarder les chances de victoire d’une classe contre une autre .");

            //Création d'une liste des classe locale, afin de conserver la liste globale lors des retrait de classe
            List<CharacterClass> _listeClass = new List<CharacterClass>();

            while (true)
            {
                _listeClass.Clear();
                foreach(CharacterClass playableClass in listeClass)
                {
                    _listeClass.Add(playableClass);
                }

                int Compt_victory1 = 0;
                int Compt_victory2 = 0;
                int Compt_equality = 0;
                CharacterClass Ia1 = _listeClass[0];
                CharacterClass Ia2 = _listeClass[0];
                CharacterClass chosenClass;

                int difficulty;
                difficulty = Difficulty();

                for (int j = 1; j < 3; j++)
                {
                    Console.WriteLine("Quel classe voulez vous choisir pour l'ia {0}?", j);

                    for (int i = 0; i < _listeClass.Count; i++)
                    {
                        Console.WriteLine("{0} - {1}", i + 1, _listeClass[i].className);
                    }

                    while (true)
                    {
                        try
                        {
                            int classIndex = int.Parse(Console.ReadLine()) - 1;

                            if (classIndex >= 0 && classIndex < _listeClass.Count)
                            {
                                chosenClass = _listeClass[classIndex];
                                _listeClass.Remove(chosenClass);
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Choisissez une classe disponible.");
                            }

                        }
                        catch
                        {
                            Console.WriteLine("Choisissez une classe disponible.");
                        }
                    }

                    switch (j)
                    {
                        case 1:
                            Ia1 = chosenClass;
                            break;

                        case 2:
                            Ia2 = chosenClass;
                            break;
                    }
                }

                string action1 = "";
                string action2 = "";

                for (int i = 1; i <= 100; i++)
                {
                    while (true)
                    {
                        switch (difficulty)
                        {
                            case 1:
                                action1 = possibleAction[IaDebutant(Ia1)];
                                action2 = possibleAction[IaDebutant(Ia2)];
                                break;
                            case 2:
                                action1 = possibleAction[IaMedium(Ia1, Ia2)];
                                action2 = possibleAction[IaMedium(Ia2, Ia1)];
                                break;
                            case 3:
                                action1 = possibleAction[IaHard(Ia1, Ia2)];
                                action2 = possibleAction[IaHard(Ia2, Ia1)];
                                break;
                        }
                        Ia1.roundAction = action1;
                        Ia2.roundAction = action2;

                        Ia1.Action(Ia2);
                        Ia2.Action(Ia1);

                        //Si les deux sont morts, égalité
                        if (Ia1.health <= 0 && Ia2.health <= 0)
                        {
                            Compt_equality++;
                            break;
                        }

                        else if (Ia1.health <= 0)
                        {
                            Compt_victory2++;
                            break;
                        }

                        else if (Ia2.health <= 0)
                        {
                            Compt_victory1++;
                            break;
                        }
                    }
                    // Réinitialisation
                    Ia1.health = Ia1.maxHealth;
                    Ia2.health = Ia2.maxHealth;
                }
                // Présentation des résultats
                Console.WriteLine("L'ordinateur 1 ({0}) gagne {1}% des ses match contre l'ordinateur 2 ({2})", Ia1.className, Compt_victory1, Ia2.className);
                Console.WriteLine("L'ordinateur 2 ({0}) gagne {1}% des ses match contre l'ordinateur 1 ({2})", Ia2.className, Compt_victory2, Ia1.className);
                Console.WriteLine("Il y a eu {0}% de match nul entre l'ordinateur 1 ({1}) et l'ordinateur 2 ({2}) ", Compt_equality, Ia1.className, Ia2.className);
                break;
            }
        }

        #endregion
    }


    public class CharacterClass
    {
        public string className;
        public int damage;
        public int health;
        readonly public int maxHealth;
        public string ressource;
        public int ressourceAmount;
        public int ressourceNeeded;

        Random rand = new Random();

        string[] offPunchline = { "Tu vas mourir !", "Prends ça ! ", "Je vais te massacrer !" };
        string[] defPunchline = { "Tu frappes comme une fillette !", "C'est tout ce que tu sais faire ?" };

        public string punchline;

        public Dictionary<string, bool> canDoAction = new Dictionary<string, bool>{
            { "attack", true },
            { "special", true },
            { "defend", true }
        };
        public string roundAction;

        //Constructeur de classe
        public CharacterClass(int dmg, int hp, string name, string resrc, int resrcCount)
        {
            damage = dmg;
            maxHealth = health = hp;
            className = name;
            ressource = resrc;
            ressourceAmount = resrcCount;
        }
        public void Action(CharacterClass target)
        {
            //Gestion des ressources
            CombatRessource(target);

            //Appel des fonctions selon l'action choisie
            switch (roundAction)
            {
                case "attack":
                    Attack(target);
                    punchline = offPunchline[rand.Next(offPunchline.Length)];
                    break;

                case "special":
                    Special(target);
                    break;
                case "defend":
                    punchline = defPunchline[rand.Next(defPunchline.Length)];
                    break;
            }
        }
        public void Special(CharacterClass target)
        {
            switch (className)
            {
                case "Combattant":
                    punchline = "J'ai une surprise pour toi...";
                    if (target.roundAction == "attack")
                    {
                        //Si l'ennemi attaque, lui infliger ses propres dégâts
                        target.TakeDamage(target.damage);
                        punchline = "Mince alors, tu t'es fais bobo ? Ça t'apprendra.";
                        //Le damager ne subit plus les dégâts, on lui rend donc des pv équivalent au dégâts adverses
                        //Math.Clamp() permet de bloquer la valeur health+target.damage(valeur qu'on souhaite attribuer) entre 0 et maxHealth)
                        health = Math.Clamp(health + target.damage, 0, maxHealth);
                    }
                    break;

                case "Prêtre":
                    //Rend un pv sans dépasser la vie max
                    health = Math.Clamp(health + 1, 0, maxHealth); 
                    punchline = "L'acier ne peut rien contre mes pouvoirs !";
                    break;

                case "Templier":
                    //Le Templier subit un dégât
                    --health;
                    //Frappe
                    target.TakeDamage(damage);
                    //Et retire directement 1 pv(sans passer par la gestion de la défense)
                    --target.health;
                    punchline = "La fin justifie les moyens !";
                    break;

                case "Nécromancien":
                    punchline = "Tu es si maladif, tu mourras promptement.";
                    //S'il nous manque de la vie et que l'ennemi a plus de vie
                    if (target.health > health && health < maxHealth)
                    {
                        //retirer directement 1 pv
                        --target.health;
                        //Récupérer 1 pv
                        health = Math.Clamp(health + 1, 0, maxHealth);
                        punchline = "Mais tu es tout pâle, que t'arrives t'il ?";
                    }
                    break;
                case "Berserker":
                    punchline = "Je baigne dans le sang !!";
                    //Inflige des dégâts supplémentaires pour chaque pv manquants
                    target.TakeDamage(damage + (maxHealth - health));
                    break;
            }
        }
        public void Attack(CharacterClass target)
        {
            //Appel de la fonction TakeDamage ennemie
            target.TakeDamage(damage);
        }
        public void TakeDamage(int damage)
        {
            if (roundAction == "defend")
                //Si on se défend, subir 1 seul dégat
                --health;
            else
                //Sinon, subir tous les dégâts reçus
                health -= damage;
        }

        public void CombatRessource(CharacterClass target)
        {
            switch (ressource)
            {
                case "Mana":
                    ressourceNeeded = 2;
                    //Si on se défend, récupératin
                    if (roundAction == "defend")
                        ressourceAmount += 2;
                    //Si spécial, dépenser la valeur requise 
                    if (roundAction == "special")
                        ressourceAmount -= ressourceNeeded;
                    break;

                case "Endurance":
                    ressourceNeeded = 2;
                    if(roundAction == "attack")
                        //Dépenser si l'on attaque
                        --ressourceAmount;

                    if (roundAction == "special")
                        ressourceAmount -= ressourceNeeded;

                    //Récupère un point à chaque tour
                    ++ressourceAmount;
                    //Si l'on bloque une attaque, point supplémentaire
                    if (roundAction == "defend" && target.roundAction == "attack")
                        ++ressourceAmount;
                    //Les valeurs de canDoAction sont égales à la valeur de la condition
                    canDoAction["attack"] = (ressourceAmount >= 1);
                    break;

                case "Rage":
                    ressourceNeeded = 3;
                    if (roundAction == "attack")
                        ++ressourceAmount;

                    if (roundAction == "special")
                        ressourceAmount -= ressourceNeeded;

                    break;
            }
            //On limite les points de ressource à 7 
            ressourceAmount = Math.Clamp(ressourceAmount, 0, 7);
            //Si on a assez de ressource, la condition retourne vrai et on peut utiliser le spécial
            canDoAction["special"] = (ressourceAmount >= ressourceNeeded);
        }
    }
}
