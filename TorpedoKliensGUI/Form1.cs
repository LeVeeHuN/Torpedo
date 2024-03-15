using System.Diagnostics;
using Torpedo;

namespace TorpedoKliensGUI
{
    public partial class Form1 : Form
    {
        Button[,] buttons;
        string serverip;
        string username;
        string roomcode;
        TorpedoKliens.Communicator communicator;

        bool isGame;
        List<LocationVector> triedLocations;
        List<List<LocationVector>> ships;
        List<LocationVector> tmpShipLocs;
        int currentShipLen;
        bool acceptYouNext;

        bool isSelfCheck;
        List<LocationVector> opponentShotLocations;
        Color[,] gameButtonColors;

        bool youNext;

        ThreadStart ts;
        Thread infoThread;

        public Form1()
        {
            InitializeComponent();
            buttons = new Button[10, 10];

            #region gombok arraybe rakása
            buttons[0, 0] = button3;
            buttons[0, 1] = button4;
            buttons[0, 2] = button5;
            buttons[0, 3] = button6;
            buttons[0, 4] = button7;
            buttons[0, 5] = button8;
            buttons[0, 6] = button9;
            buttons[0, 7] = button10;
            buttons[0, 8] = button11;
            buttons[0, 9] = button12;

            buttons[1, 0] = button13;
            buttons[1, 1] = button14;
            buttons[1, 2] = button15;
            buttons[1, 3] = button16;
            buttons[1, 4] = button17;
            buttons[1, 5] = button18;
            buttons[1, 6] = button19;
            buttons[1, 7] = button20;
            buttons[1, 8] = button21;
            buttons[1, 9] = button22;

            buttons[2, 0] = button23;
            buttons[2, 1] = button24;
            buttons[2, 2] = button25;
            buttons[2, 3] = button26;
            buttons[2, 4] = button27;
            buttons[2, 5] = button28;
            buttons[2, 6] = button29;
            buttons[2, 7] = button30;
            buttons[2, 8] = button31;
            buttons[2, 9] = button32;

            buttons[3, 0] = button33;
            buttons[3, 1] = button34;
            buttons[3, 2] = button35;
            buttons[3, 3] = button36;
            buttons[3, 4] = button37;
            buttons[3, 5] = button38;
            buttons[3, 6] = button39;
            buttons[3, 7] = button40;
            buttons[3, 8] = button41;
            buttons[3, 9] = button42;

            buttons[4, 0] = button43;
            buttons[4, 1] = button44;
            buttons[4, 2] = button45;
            buttons[4, 3] = button46;
            buttons[4, 4] = button47;
            buttons[4, 5] = button48;
            buttons[4, 6] = button49;
            buttons[4, 7] = button50;
            buttons[4, 8] = button51;
            buttons[4, 9] = button52;

            buttons[5, 0] = button53;
            buttons[5, 1] = button54;
            buttons[5, 2] = button55;
            buttons[5, 3] = button56;
            buttons[5, 4] = button57;
            buttons[5, 5] = button58;
            buttons[5, 6] = button59;
            buttons[5, 7] = button60;
            buttons[5, 8] = button61;
            buttons[5, 9] = button62;

            buttons[6, 0] = button63;
            buttons[6, 1] = button64;
            buttons[6, 2] = button65;
            buttons[6, 3] = button66;
            buttons[6, 4] = button67;
            buttons[6, 5] = button68;
            buttons[6, 6] = button69;
            buttons[6, 7] = button70;
            buttons[6, 8] = button71;
            buttons[6, 9] = button72;

            buttons[7, 0] = button73;
            buttons[7, 1] = button74;
            buttons[7, 2] = button75;
            buttons[7, 3] = button76;
            buttons[7, 4] = button77;
            buttons[7, 5] = button78;
            buttons[7, 6] = button79;
            buttons[7, 7] = button80;
            buttons[7, 8] = button81;
            buttons[7, 9] = button82;

            buttons[8, 0] = button83;
            buttons[8, 1] = button84;
            buttons[8, 2] = button85;
            buttons[8, 3] = button86;
            buttons[8, 4] = button87;
            buttons[8, 5] = button88;
            buttons[8, 6] = button89;
            buttons[8, 7] = button90;
            buttons[8, 8] = button91;
            buttons[8, 9] = button92;

            buttons[9, 0] = button93;
            buttons[9, 1] = button94;
            buttons[9, 2] = button95;
            buttons[9, 3] = button96;
            buttons[9, 4] = button97;
            buttons[9, 5] = button98;
            buttons[9, 6] = button99;
            buttons[9, 7] = button100;
            buttons[9, 8] = button101;
            buttons[9, 9] = button102;
            #endregion

            #region disable every control
            ActivateDeactivateButtons(false);
            ActivateDeactivateRoomControls(false);
            #endregion

            DefaultAllButtons();

            isGame = false;
            triedLocations = new List<LocationVector>();
            ships = new List<List<LocationVector>>();
            tmpShipLocs = new List<LocationVector>();
            currentShipLen = 1;
            acceptYouNext = true;

            isSelfCheck = false;
            opponentShotLocations = new List<LocationVector>();
            gameButtonColors = new Color[10, 10];

            selfCheck_button.Enabled = false;

            youNext = false;
        }

        // Hajó lerakás
        private void PlaceShip(LocationVector location)
        {
            // Safety checks
            foreach (List<LocationVector> shipLocs in ships)
            {
                foreach (LocationVector shipLoc in shipLocs)
                {
                    if (shipLoc.IsSame(location))
                    {
                        MessageBox.Show("Ide nem rakhatsz le hajót!", "Hajó lerakás", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (shipLoc.IsInRange(location))
                    {
                        MessageBox.Show("Ide nem rakhatsz le hajót!", "Hajó lerakás", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            foreach (LocationVector safetyLocation in tmpShipLocs)
            {
                if (safetyLocation.IsSame(location))
                {
                    MessageBox.Show("Ide nem rakhatsz le hajót!", "Hajó lerakás", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Check, hogy a sajátja mellé rakjuk
            if (tmpShipLocs.Count != 0)
            {
                //      Melyik tengelyre terjeszkedik x vagy y?
                if (tmpShipLocs.Count >= 2)
                {
                    int xDiff = tmpShipLocs[0].X - tmpShipLocs[1].X;
                    int yDiff = tmpShipLocs[0].Y - tmpShipLocs[1].Y;

                    if (xDiff == 0)
                    {
                        // X tengelyen húzódik
                        if (location.X != tmpShipLocs[0].X)
                        {
                            MessageBox.Show("Ide nem rakhatsz le hajót!", "Hajó lerakás", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else if (yDiff == 0)
                    {
                        // Y tengelyen húzódik
                        if (location.Y != tmpShipLocs[0].Y)
                        {
                            MessageBox.Show("Ide nem rakhatsz le hajót!", "Hajó lerakás", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                bool valid = false;
                foreach (LocationVector thisShipLocation in tmpShipLocs)
                {
                    if (thisShipLocation.IsNextTo(location))
                    {
                        valid = true;
                    }
                }
                if (!valid)
                {
                    MessageBox.Show("Ide nem rakhatsz le hajót!", "Hajó lerakás", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }



            // Hajó valid
            tmpShipLocs.Add(location);
            buttons[location.X, location.Y].BackColor = Color.Green;
            if (tmpShipLocs.Count == currentShipLen)
            {
                ships.Add(tmpShipLocs);
                tmpShipLocs = new List<LocationVector>();
                if (currentShipLen == 5)
                {
                    // adatok elküldése a szervernek
                    StartGame();
                }
                currentShipLen++;
            }
        }

        // Játék indítása
        private void StartGame()
        {
            isGame = true;

            // Kordináták elküldése és joinolás a játékba
            string shipLocationsStr = string.Empty;
            for (int i = 0; i < ships.Count; i++)
            {
                List<LocationVector> currentShipLocations = ships[i];
                shipLocationsStr = $"{shipLocationsStr}{i + 1}{{";

                string xs = string.Empty;
                string ys = string.Empty;
                foreach (LocationVector location in currentShipLocations)
                {
                    xs = xs + location.X + ".";
                    ys = ys + location.Y + ".";
                }
                xs = xs.Remove(xs.Length - 1);
                ys = ys.Remove(ys.Length - 1);
                shipLocationsStr = shipLocationsStr + xs + "|" + ys + ",";
            }
            shipLocationsStr = shipLocationsStr.Remove(shipLocationsStr.Length - 1);

            string newPlayerStr = $"new_player;{shipLocationsStr};{username};{roomcode}";
            communicator.Communicate(newPlayerStr);

            // Dolgok felsetupolása
            DefaultAllButtons();
            MessageBox.Show("Kezdődhet a játék. A bal alsó sarokban találsz információt arról, hogy mikor következel te, és milyen lépést tett az ellenfeled.", "LevTorpedó", MessageBoxButtons.OK, MessageBoxIcon.Information);
            infobox.Text = "Várakozás a másik játékosra...";
            nextMoveLabel.ForeColor = Color.Yellow;
            nextMoveLabel.Text = "Varakozas";
            acceptYouNext = true;
            selfCheck_button.Enabled = true;

            // Info thread elindítása
            InfoGetter.username = username;
            InfoGetter.roomcode = roomcode;
            InfoGetter.communicator = communicator;
            InfoGetter.form1 = this;
            ts = InfoGetter.GetInfo;
            infoThread = new Thread(ts);
            infoThread.Start();
            selfCheck_button.Enabled = true;
        }

        // Shoot
        private void Shoot(LocationVector location)
        {
            foreach (LocationVector triedLoc in triedLocations)
            {
                if (triedLoc.IsSame(location))
                {
                    MessageBox.Show("Ide már lőttél. Próbáld újra!", "Lövés", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            triedLocations.Add(location);
            string shootString = $"shot;{location.X},{location.Y};{username};{roomcode}";
            string response = communicator.Communicate(shootString);
            string[] respComponents = response.Split(";");
            string[] respCords = respComponents[1].Split(",");
            int x = int.Parse(respCords[0]);
            int y = int.Parse(respCords[1]);
            if (respComponents[0].Equals("empty"))
            {
                buttons[x, y].BackColor = Color.Red;
            }
            else if (respComponents[0].Equals("nice"))
            {
                buttons[x, y].BackColor = Color.Yellow;
            }
            else if (respComponents[0].Equals("shrank"))
            {
                buttons[x, y].BackColor = Color.Yellow;
                // Call function to make every ship place green
                int shipLen = int.Parse(respComponents[2]);
                MakeOpponentShipShrank(shipLen);
            }
            infobox.Text = "Várakozás";
            OpponentNext();
            acceptYouNext = true;
        }

        private void MakeOpponentShipShrank(int shipLen)
        {
            string requestString = $"ship_coords;{username};{shipLen};{roomcode}";
            string response = communicator.Communicate(requestString);
            string data = response.Split("{")[1];
            string[] xs = data.Split("|")[0].Split(".");
            string[] ys = data.Split("|")[1].Split(".");

            for (int i = 0; i < xs.Length; i++)
            {
                buttons[int.Parse(xs[i]), int.Parse(ys[i])].BackColor = Color.Green;
            }
        }


        delegate void ProcessInfoCallback(string info);
        public void ProcessInfoResponse(string response)
        {
            if (this.infobox.InvokeRequired)
            {
                ProcessInfoCallback d = new ProcessInfoCallback(ProcessInfoResponse);
                this.Invoke(d, new object[] { response });
            }
            else
            {
                string[] infoParts = response.Split(";");
                if (infoParts[0].Equals("you_next") && acceptYouNext)
                {
                    YouNext();
                    if (infoParts[1].Equals("nothing"))
                    {
                        infobox.Text = $"Nem találtak el.\nx: {infoParts[2].Split(",")[0]}, y: {infoParts[2].Split(",")[1]}";
                    }
                    else if (infoParts[1].Equals("dmg"))
                    {
                        infobox.Text = $"Eltaláltak!\nx: {infoParts[2].Split(",")[0]}, y: {infoParts[2].Split(",")[1]}";
                    }
                    else if (infoParts[1].Equals("died_ship"))
                    {
                        infobox.Text = $"Elsüllyedt a {infoParts[3]} hosszú hajód :(\nx: {infoParts[2].Split(",")[0]}, y: {infoParts[2].Split(",")[1]}";
                    }
                    else if (infoParts[1].Equals("first"))
                    {
                        infobox.Text = "Te vagy az első!";
                    }
                    acceptYouNext = false;

                    if (!infoParts[1].Equals("first"))
                    {
                        string[] cords = infoParts[2].Split(",");
                        int x = int.Parse(cords[0]);
                        int y = int.Parse(cords[1]);
                        opponentShotLocations.Add(new LocationVector(x, y));
                    }
                }
                else if (infoParts[0].Equals("wait"))
                {
                    OpponentNext();
                    infobox.Text = "Várakozás...";
                    acceptYouNext = true;
                }
                else if (infoParts[0].Equals("game_over"))
                {
                    string winnerName = infoParts[1];
                    ActivateDeactivateButtons(false);
                    if (winnerName.Equals(username))
                    {
                        infobox.Text = "Gratulálok nyertél! :)";
                        nextMoveLabel.ForeColor = Color.Green;
                        nextMoveLabel.Text = "Nyertél :)";
                        MessageBox.Show("NYERTÉL :)", "Játék vége", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        infobox.Text = $"Sajnos vesztettél :( {winnerName} nyert!";
                        nextMoveLabel.ForeColor = Color.Red;
                        nextMoveLabel.Text = "Vesztettél :(";
                        MessageBox.Show("VESZTETTÉL :(", "Játék vége", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    // Uj jatek inditasa
                    selfCheck_button.Enabled = false;
                    newGame_button.Visible = true;
                }
                else if (infoParts[0].Equals("error"))
                {
                    // Szoba megszunt
                    MessageBox.Show("A szoba megszunt", "Szoba hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Uj jatek inditasa
                    selfCheck_button.Enabled = false;
                    newGame_button.Visible = true;
                }
            }
        }

        private void InitializeNewGame()
        {
            newGame_button.Visible = false;
            DefaultAllButtons();
            ActivateDeactivateButtons(false);
            ActivateDeactivateRoomControls(true);
            isGame = false;
            triedLocations = new List<LocationVector>();
            ships = new List<List<LocationVector>>();
            tmpShipLocs = new List<LocationVector>();
            currentShipLen = 1;
            acceptYouNext = true;
            isSelfCheck = false;
            opponentShotLocations = new List<LocationVector>();
            gameButtonColors = new Color[10, 10];
        }

        private void YouNext()
        {
            youNext = true;
            nextMoveLabel.ForeColor = Color.Green;
            nextMoveLabel.Text = "Te kovetkezel";
            if (!isSelfCheck )
            {
                ActivateDeactivateButtons(true);
            }
        }

        private void OpponentNext()
        {
            youNext = false;
            nextMoveLabel.ForeColor = Color.Red;
            nextMoveLabel.Text = "Az ellenfel kovetkezik";
            ActivateDeactivateButtons(false);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void DefaultAllButtons()
        {
            for (int i = 0; i < buttons.GetLength(0); i++)
            {
                for (int j = 0; j < buttons.GetLength(1); j++)
                {
                    buttons[i, j].BackColor = Color.LightGray;
                }
            }
        }

        public void ActivateDeactivateButtons(bool activated)
        {
            for (int i = 0; i < buttons.GetLength(0); i++)
            {
                for (int j = 0; j < buttons.GetLength(1); j++)
                {
                    buttons[i, j].Enabled = activated;
                }
            }
        }

        public void ActivateDeactivateServerControls(bool activated)
        {
            textBox_username.Enabled = activated;
            textBox_serverip.Enabled = activated;
            button1.Enabled = activated;
        }

        public void ActivateDeactivateRoomControls(bool activated)
        {
            textBox_roomcode.Enabled = activated;
            radioButton1.Enabled = activated;
            radioButton2.Enabled = activated;
            button2.Enabled = activated;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox_username.Text) || string.IsNullOrWhiteSpace(textBox_serverip.Text))
            {
                MessageBox.Show("Adj meg felhasználó nevet és szerver ip-t is!", "Érvénytelen adatok", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            username = textBox_username.Text;
            serverip = textBox_serverip.Text;
            ActivateDeactivateServerControls(false);
            ActivateDeactivateRoomControls(true);

            communicator = new TorpedoKliens.Communicator(serverip);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox_roomcode.Text))
            {
                MessageBox.Show("Adj meg szobakódot", "Érvénytelen adatok", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            roomcode = textBox_roomcode.Text;

            // csatlakozás szobához
            if (radioButton1.Checked)
            {
                // Szoba ellenőrzése
                string checkResp = communicator.Communicate($"check_room;{roomcode}");

                if (checkResp.Equals("ok"))
                {
                    // Játék indítása
                    isGame = false;
                    ActivateDeactivateRoomControls(false);
                    ActivateDeactivateButtons(true);
                    MessageBox.Show("A csatlakozás sikeres. Leteheted a hajóidat.", "LevTorpedó", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (checkResp.Equals("full"))
                {
                    MessageBox.Show("A szoba tele van!", "Szoba hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (checkResp.Equals("non_existent"))
                {
                    MessageBox.Show("A szoba nem létezik!", "Szoba hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    MessageBox.Show("A KURVA ANYÁD", "Szoba hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            // új szoba létrehozása
            else if (radioButton2.Checked)
            {
                string newGameCommand = $"new_game;{roomcode}";
                string newGameResponse = communicator.Communicate(newGameCommand);

                if (newGameResponse.Equals("success"))
                {
                    // játék indítása
                    isGame = false;
                    ActivateDeactivateRoomControls(false);
                    ActivateDeactivateButtons(true);
                    MessageBox.Show("A csatlakozás sikeres. Leteheted a hajóidat.", "LevTorpedó", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (newGameResponse.Equals("fail"))
                {
                    MessageBox.Show("A szoba már létezik!", "Szoba hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    MessageBox.Show("A KURVA ANYÁD", "Szoba hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        // valtas a jatek es a sajat hajoid kozott
        private void SelfCheck(bool isSelfCheck)
        {
            if (isSelfCheck)
            {
                ActivateDeactivateButtons(false);
                // regi szinek elmentese
                for (int i = 0; i < buttons.GetLength(0); i++)
                {
                    for (int j = 0; j < buttons.GetLength(1); j++)
                    {
                        gameButtonColors[i, j] = buttons[i, j].BackColor;
                    }
                }
                DefaultAllButtons();

                // sajat hajok jelolese
                foreach (List<LocationVector> ship in ships)
                {
                    foreach (LocationVector shipLoc in ship)
                    {
                        buttons[shipLoc.X, shipLoc.Y].BackColor = Color.Green;
                    }
                }

                // ellenfel loveseinek feldolgozasa
                List<LocationVector> shipsShot = new List<LocationVector>();
                // Hajo lovesek ellenorzese
                foreach (LocationVector oppShotLoc in opponentShotLocations)
                {
                    foreach (List<LocationVector> ship in ships)
                    {
                        foreach (LocationVector shipLoc in ship)
                        {
                            if (shipLoc.IsSame(oppShotLoc))
                            {
                                buttons[oppShotLoc.X, oppShotLoc.Y].BackColor = Color.Red;
                                shipsShot.Add(oppShotLoc);
                            }
                        }
                    }
                }

                // Minden mas loves jelolese
                foreach (LocationVector oppShotLoc in opponentShotLocations)
                {
                    if (!shipsShot.Contains(oppShotLoc))
                    {
                        buttons[oppShotLoc.X, oppShotLoc.Y].BackColor = Color.Yellow;
                    }
                }
            }
            else
            {
                // regi szinek betoltese
                for (int i = 0; i < buttons.GetLength(0); i++)
                {
                    for (int j = 0; j < buttons.GetLength(1); j++)
                    {
                        buttons[i, j].BackColor = gameButtonColors[i, j];
                    }
                }
                if (youNext)
                {
                    ActivateDeactivateButtons(true);
                }
            }
        }

        #region gomb gecik

        private void button3_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(0, 0);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(0, 1);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(0, 2);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(0, 3);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(0, 4);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(0, 5);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(0, 6);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(0, 7);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(0, 8);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(0, 9);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(1, 0);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(1, 1);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(1, 2);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(1, 3);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(1, 4);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(1, 5);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(1, 6);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(1, 7);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(1, 8);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(1, 9);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(2, 0);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button24_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(2, 1);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(2, 2);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button26_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(2, 3);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button27_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(2, 4);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button28_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(2, 5);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button29_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(2, 6);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button30_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(2, 7);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button31_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(2, 8);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button32_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(2, 9);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button33_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(3, 0);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button34_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(3, 1);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button35_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(3, 2);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button36_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(3, 3);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button37_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(3, 4);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button38_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(3, 5);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button39_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(3, 6);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button40_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(3, 7);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button41_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(3, 8);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button42_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(3, 9);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button43_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(4, 0);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button44_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(4, 1);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button45_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(4, 2);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button46_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(4, 3);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button47_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(4, 4);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button48_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(4, 5);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button49_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(4, 6);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button50_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(4, 7);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button51_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(4, 8);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button52_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(4, 9);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button53_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(5, 0);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button54_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(5, 1);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button55_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(5, 2);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button56_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(5, 3);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button57_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(5, 4);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button58_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(5, 5);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button59_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(5, 6);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button60_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(5, 7);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button61_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(5, 8);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button62_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(5, 9);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button63_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(6, 0);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button64_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(6, 1);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button65_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(6, 2);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button66_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(6, 3);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button67_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(6, 4);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button68_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(6, 5);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button69_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(6, 6);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button70_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(6, 7);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button71_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(6, 8);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button72_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(6, 9);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button73_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(7, 0);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button74_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(7, 1);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button75_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(7, 2);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button76_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(7, 3);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button77_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(7, 4);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button78_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(7, 5);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button79_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(7, 6);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button80_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(7, 7);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button81_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(7, 8);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button82_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(7, 9);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button83_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(8, 0);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button84_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(8, 1);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button85_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(8, 2);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button86_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(8, 3);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button87_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(8, 4);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button88_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(8, 5);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button89_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(8, 6);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button90_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(8, 7);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button91_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(8, 8);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button92_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(8, 9);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button93_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(9, 0);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button94_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(9, 1);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button95_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(9, 2);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button96_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(9, 3);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button97_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(9, 4);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button98_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(9, 5);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button99_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(9, 6);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button100_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(9, 7);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button101_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(9, 8);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        private void button102_Click(object sender, EventArgs e)
        {
            LocationVector thisLocation = new LocationVector(9, 9);
            if (isGame)
            {
                Shoot(thisLocation);
            }
            else
            {
                PlaceShip(thisLocation);
            }
        }

        #endregion

        private void newGame_button_Click(object sender, EventArgs e)
        {
            InitializeNewGame();
        }

        private void selfCheck_button_Click(object sender, EventArgs e)
        {
            if (!isSelfCheck)
            {
                selfCheck_button.BackColor = Color.Red;
                selfCheck_button.Text = "Vissza";
            }
            else
            {
                selfCheck_button.BackColor = Color.LightGray;
                selfCheck_button.Text = "Sajat";
            }
            isSelfCheck = !isSelfCheck;
            SelfCheck(isSelfCheck);
        }
    }
}
