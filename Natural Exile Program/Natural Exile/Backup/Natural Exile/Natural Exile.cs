using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System;
using System.Collections.Generic;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        CombatEngine CE = new CombatEngine();
        Random toHitRand = new Random();
        Random ttRand = new Random();

        Font myFont = new Font("Courier New", 14);
        Pen mypen = new Pen(Brushes.Black);

        string str;
        char[] chars;
        StreamReader tr;

        int pcPos = 67;
        int mobPos = 34 + (23*66);

        PlayerCharacter player = new PlayerCharacter();
        MonsterClass goblin = new MonsterClass("Goblin", 1, 51+(23 * 66));

        Timer wanderTimer = new Timer();

        int battleFlag = 0;
        int pcTurn = 0;
        int pcCanMove = 0;
        int pcMoved = 0;
        int pcActed = 0;
        int attackCommand = 0;
        int spellCommand = 0;

        int messageBS = 0;
        //int messagePT = 0;
        int messageTT = 0;

        Weapon dagger;
        Weapon staff;
        Weapon noWeapon;
        Weapon curWeapon;
        Armor clothRobes;
        Armor noArmor;
        Armor curArmor;
        OffensiveSpell waveOfPain;
        OffensiveSpell noSpell;

        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.Gray;

            textBox1.Font = myFont;
            textBox1.BorderStyle = BorderStyle.Fixed3D;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.WordWrap = true;
            textBox1.ReadOnly = true;
            textBox1.Text = "Welcome to Natural Exile.";

            textBox2.Font = myFont;
            textBox2.BorderStyle = BorderStyle.Fixed3D;
            textBox2.ScrollBars = ScrollBars.Vertical;
            textBox2.WordWrap = true;
            textBox2.ReadOnly = true;
            textBox2.Text = "You are safe.";

            pictureBox1.BorderStyle = BorderStyle.Fixed3D;
            pictureBox2.BorderStyle = BorderStyle.Fixed3D;

            label1.ForeColor = Color.White;
            label1.Text = "Current Weapon";
            label2.ForeColor = Color.White;
            label2.Text = "Current Armor";
            label3.ForeColor = Color.White;
            label3.Text = "Current Spell";
            label4.ForeColor = Color.White;
            label4.Text = "Environment Log";
            label5.ForeColor = Color.White;
            label5.Text = "Combat Log";
            label6.ForeColor = Color.White;
            label6.Text = "Exile Stats";
            label7.ForeColor = Color.White;
            label7.Text = "Consumables";

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Items.Add("None");
            comboBox1.Items.Add("Dagger");
            comboBox1.Items.Add("Staff");
            //comboBox1.Items.Add("Hand Crossbow");
            comboBox1.SelectedIndex = 1;

            dagger = new Weapon("Dagger", 3, 1, 20, 5, 1);
            staff = new Weapon("Staff", 2, 3, 15, 20, 1);
            noWeapon = new Weapon("None", 0, 0, 0, 0, 0);
            curWeapon = dagger;

            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.Items.Add("None");
            comboBox2.Items.Add("Cloth Robes");
            comboBox2.SelectedIndex = 1;

            clothRobes = new Armor("Cloth Robes", 2, 2);
            noArmor = new Armor("None", 0, 0);
            curArmor = clothRobes;

            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.Items.Add("None");
            comboBox3.Items.Add("Wave of Pain");
            comboBox3.SelectedIndex = 1;

            waveOfPain = new OffensiveSpell("Wave of Pain", 7, 4, 3, 0, 1, 0);
            noSpell = new OffensiveSpell("None", 0, 0, 0, 0, 0, 0);

            comboBox4.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox4.Items.Add("None");
            //comboBox4.Items.Add("");
            //comboBox4.Items.Add("");
            comboBox4.SelectedIndex = 0;

            button1.Text = "Use Consumable";

            this.KeyPreview = true;
            this.KeyPress += new KeyPressEventHandler(Form1_KeyPress);

            InitializePlayerItems();
            LM();

            wanderTimer.Tick += new EventHandler(WanderTimer_Tick);
            wanderTimer.Interval = (1000) * (3);
            wanderTimer.Enabled = true;
            wanderTimer.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CE.MobList[0] = goblin.mobID;

            this.Invalidate();
        }

        private void InitializePlayerItems()
        {
            player.meleeDmg2 += curWeapon.meleeDmg;
            if (player.meleeDmg2 >= 10)
            {
                player.meleeDmg1 += player.meleeDmg2 / 10;
                player.meleeDmg2 = player.meleeDmg2 % 10;
            }

            player.magicDmg2 += curWeapon.magicDmg;
            if (player.magicDmg2 >= 10)
            {
                player.magicDmg1 += player.magicDmg2 / 10;
                player.magicDmg2 = player.magicDmg2 % 10;
            }

            player.meleeHit2 += curWeapon.meleeHit;
            if (player.meleeHit2 >= 10)
            {
                player.meleeHit1 += player.meleeHit2 / 10;
                player.meleeHit2 = player.meleeHit2 % 10;
            }

            player.magicHit2 += curWeapon.magicHit;
            if (player.magicHit2 >= 10)
            {
                player.magicHit1 += player.magicHit2 / 10;
                player.magicHit2 = player.magicHit2 % 10;
            }

            player.armor2 += curArmor.prot;
            if (player.armor2 >= 10)
            {
                player.armor1 += player.armor2 / 10;
                player.armor2 = player.armor2 % 10;
            }

            player.magicResist2 += curArmor.mResist;
            if (player.magicResist2 >= 10)
            {
                player.magicResist1 += player.magicResist2 / 10;
                player.magicResist2 = player.magicResist2 % 10;
            }
        }

        private void LM()
        {
            tr = new StreamReader("C:\\Users\\Admin\\School\\Adrian's School Work\\Spring 2013\\Games and Virtual Environments\\Homework 2\\Goblin Caves.txt");
            str = tr.ReadToEnd();

            chars = str.ToCharArray();
        }

        private Weapon CurrentWeapon()
        {
            string selectedWeapon = comboBox1.Items[comboBox1.SelectedIndex].ToString();

            if(selectedWeapon.CompareTo("Dagger") == 0)
                return dagger;
            else if(selectedWeapon.CompareTo("Staff") == 0)
                return staff;
            else
                return noWeapon;
        }

        private Armor CurrentArmor()
        {
            string selectedArmor = comboBox2.Items[comboBox2.SelectedIndex].ToString();

            if(selectedArmor.CompareTo("Cloth Robes") == 0)
                return clothRobes;
            else
                return noArmor;
        }

        private void CurrentConsumable()
        {
            string selectedConsumable = comboBox3.Items[comboBox3.SelectedIndex].ToString();
        }

        private OffensiveSpell CurrentSpell()
        {
            string selectedSpell = comboBox3.Items[comboBox3.SelectedIndex].ToString();

            if (selectedSpell.CompareTo("Wave of Pain") == 0)
                return waveOfPain;
            else
                return noSpell;
        }

        private void RefreshStats()
        {
            player.meleeDmg2 += curWeapon.meleeDmg;
            if (player.meleeDmg2 >=  10)
            {
                player.meleeDmg1 -= player.meleeDmg2 / 10;
                player.meleeDmg2 = Math.Abs(player.meleeDmg2 % 10);
            }
            else
                player.meleeDmg2 -= 2*(curWeapon.meleeDmg);

            player.magicDmg2 += curWeapon.magicDmg;
            if (player.magicDmg2 >= 10)
            {
                player.magicDmg1 -= player.magicDmg2 / 10;
                player.magicDmg2 = Math.Abs(player.magicDmg2 % 10);
            }
            else
                player.magicDmg2 -= 2 * (curWeapon.magicDmg);

            player.meleeHit2 += curWeapon.meleeHit;
            if (player.meleeHit2 >= 10)
            {
                player.meleeHit1 -= player.meleeHit2 / 10;
                player.meleeHit2 = Math.Abs(player.meleeHit2 % 10);
            }
            else
                player.meleeHit2 -= 2 * (curWeapon.meleeHit);

            player.armor2 += curArmor.prot;
            if (player.armor2 >= 10)
            {
                player.armor1 -= player.armor2 / 10;
                player.armor2 = Math.Abs(player.armor2 % 10);
            }
            else
                player.armor2 -= 2 * (curArmor.prot);

            player.magicResist2 += curArmor.mResist;
            if (player.magicResist2 >= 10)
            {
                player.magicResist1 -= player.magicResist2 / 10;
                player.magicResist2 = Math.Abs(player.magicResist2 % 10);
            }
            else
                player.magicResist2 -= 2 * (curArmor.mResist);
        }

        private void CalcStats()
        {
            Weapon newWeapon = CurrentWeapon();
            Armor newArmor = CurrentArmor();

            //RefreshStats();
            player.ResetStats();

            player.meleeDmg2 += newWeapon.meleeDmg;
            if (player.meleeDmg2 >= 10)
            {
                player.meleeDmg1 += player.meleeDmg2 / 10;
                player.meleeDmg2 = player.meleeDmg2 % 10;
            }

            player.magicDmg2 += newWeapon.magicDmg;
            if (player.magicDmg2 >= 10)
            {
                player.magicDmg1 += player.magicDmg2 / 10;
                player.magicDmg2 = player.magicDmg2 % 10;
            }

            player.meleeHit2 += newWeapon.meleeHit;
            if (player.meleeHit2 >= 10)
            {
                player.meleeHit1 += player.meleeHit2 / 10;
                player.meleeHit2 = player.meleeHit2 % 10;
            }

            player.magicHit2 += curWeapon.magicHit;
            if (player.magicHit2 >= 10)
            {
                player.magicHit1 += player.magicHit2 / 10;
                player.magicHit2 = player.magicHit2 % 10;
            }

            curWeapon = newWeapon;

            player.armor2 += newArmor.prot;
            if (player.armor2 >= 10)
            {
                player.armor1 += player.armor2 / 10;
                player.armor2 = player.armor2 % 10;
            }

            player.magicResist2 += newArmor.mResist;
            if (player.magicResist2 >= 10)
            {
                player.magicResist1 += player.magicResist2 / 10;
                player.magicResist2 = player.magicResist2 % 10;
            }

            curArmor = newArmor;
        }

        private void PlaceStats(char[] statChars)
        {

            statChars[63] = GetChar(player.currentLevel1);
            statChars[64] = GetChar(player.currentLevel2);

            statChars[90] = GetChar(player.hitPoints1);
            statChars[91] = GetChar(player.hitPoints2);

            statChars[108] = GetChar(player.mana1);
            statChars[109] = GetChar(player.mana2);
            
            statChars[129] = GetChar(player.pcStrength1);
            statChars[130] = GetChar(player.pcStrength2);

            statChars[147] = GetChar(player.pcDexterity1);
            statChars[148] = GetChar(player.pcDexterity2);
            
            statChars[168] = GetChar(player.pcEndurance1);
            statChars[169] = GetChar(player.pcEndurance2);
            
            statChars[186] = GetChar(player.pcWillpower1);
            statChars[187] = GetChar(player.pcWillpower2);

            statChars[207] = GetChar(player.meleeDmg1);
            statChars[208] = GetChar(player.meleeDmg2);

            statChars[225] = GetChar(player.magicDmg1);
            statChars[226] = GetChar(player.magicDmg2);

            statChars[246] = GetChar(player.armor1);
            statChars[247] = GetChar(player.armor2);

            statChars[264] = GetChar(player.magicResist1);
            statChars[265] = GetChar(player.magicResist2);

            statChars[285] = GetChar(player.meleeHit1);
            statChars[286] = GetChar(player.meleeHit2);

            statChars[303] = GetChar(player.magicHit1);
            statChars[304] = GetChar(player.magicHit2);

            statChars[324] = GetChar(player.exp1);
            statChars[325] = GetChar(player.exp2);

            statChars[342] = GetChar(player.toLevel1);
            statChars[343] = GetChar(player.toLevel2);
        }

        private void LoadStatPage()
        {
            Bitmap DrawArea_StatsPage;
            Graphics statG;
            DrawArea_StatsPage = new Bitmap(pictureBox2.Size.Width, pictureBox2.Size.Height);
            pictureBox2.Image = DrawArea_StatsPage;

            statG = Graphics.FromImage(DrawArea_StatsPage);
            statG.Clear(Color.White);

            StreamReader statR = new StreamReader("C:\\Users\\Admin\\School\\Adrian's School Work\\Spring 2013\\Games and Virtual Environments\\Homework 2\\StatsPage.txt");
            string statStr = statR.ReadToEnd();
            char[] statChars = statStr.ToCharArray();

            CalcStats();
            PlaceStats(statChars);

            statStr = new string(statChars);
            statG.DrawString(statStr, myFont, Brushes.Black, new Point(0, 0));

            statR.Close();
            statG.Dispose();
        }

        private void BattleMode()
        {
            if (!(CE.MetVictory()))
            {
                if (CE.FocusedActor() == 0)
                {
                    pcTurn = 0;
                    pcCanMove = 1;
                }
                else
                    pcTurn = 1;

                if (pcTurn == 0)
                {
                    /*textBox1.AppendText(" PC Turn ");
                    string tempstr = mobPos.ToString();
                    textBox1.AppendText(tempstr);
                    textBox1.AppendText("/");
                    tempstr = pcPos.ToString();
                    textBox1.AppendText(tempstr);
                    textBox1.AppendText(",");*/

                    //CombatText();
                    //messagePT = 1;
                    System.Threading.Thread.Sleep(1000);

                    CombatTurn();
                }
                else if(pcTurn == 1)
                {
                    //textBox1.AppendText(" Mob Turn,");
                    //pcTurn = 1;
                    RunMobTurn(CE.FocusedActor());
                    CE.NextTurn();
                    pcMoved = 0;
                    //messagePT = 0;
                }

                //messagePT = 0;
            }
            else
            {
                battleFlag = 0;
                messageBS = 0;
            }
        }

        public void CombatTurn()
        {
            if (pcMoved == 1 && pcActed == 1)
            {
                CE.NextTurn();
            }
        }

        public void RunMobTurn(int mobNum)
        {
            //string tempstr = goblin.pos.ToString();
            //textBox1.AppendText(tempstr);
            //CE.MobList[0] = 0;
            //MoveMob(goblin, 's');

            MobMoveToPC();
            MobTakeAction();
        }

        private void MobMoveToPC()
        {
            int pcRow = pcPos / 66;
            int pcCol = pcPos % 66;
            int mobRow = mobPos / 66;
            int mobCol = mobPos % 66;
            int rowDiff = pcRow - mobRow;
            int colDiff = pcCol - mobCol;

            if (Math.Abs(rowDiff) > Math.Abs(colDiff))
            {
                if (rowDiff > 0)
                {
                    MoveMob(goblin, 's');
                }
                else
                {
                    MoveMob(goblin, 'n');
                }
            }
            else
            {
                if (colDiff > 0)
                {
                    MoveMob(goblin, 'e');
                }
                else
                {
                    MoveMob(goblin, 'w');
                }
            }
        }

        private void MobTakeAction()
        {
            if((pcPos - 1) == mobPos || (pcPos + 1) == mobPos || (pcPos - 66) == mobPos || (pcPos + 66) == mobPos)
            {
                MobMeleeAttack();
            }
        }

        private void MobMeleeAttack()
        {
            int truePCHP = (player.hitPoints1 * 10) + player.hitPoints2;
            int truePCArmor = (player.armor1 * 10) + player.armor2;
            int finalMobHit = goblin.meleeHit - (2 * truePCArmor);
            int isHit = toHitRand.Next(1, 100);

            if (finalMobHit >= isHit)
            {
                truePCHP -= goblin.meleeDmg;

                if (truePCHP <= 0)
                {
                    textBox1.AppendText(Environment.NewLine);
                    textBox1.AppendText("You have perished...");
                    System.Threading.Thread.Sleep(10000);
                    this.Close();
                }
                else
                {
                    player.hitPoints1 = truePCHP / 10;
                    player.hitPoints2 = truePCHP % 10;
                }
            }
        }

        private void CombatText()
        {
            if (messageBS == 0)
            {
                textBox2.AppendText(Environment.NewLine);
                textBox2.AppendText("A hostile creature has spotted you!");
            }
            else if (messageTT == 0)
            {
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText("Goblin: \"Dirty human!\"");
            }
            //else if (messagePT == 0)
            //{
            //    textBox2.Clear();
            //    textBox2.AppendText(Environment.NewLine);
            //    textBox2.AppendText("Player's turn...");
            //}
        }

        private char GetChar(int num)
        {
            if (num == 0)
                return '0';
            else if (num == 1)
                return '1';
            else if (num == 2)
                return '2';
            else if (num == 3)
                return '3';
            else if (num == 4)
                return '4';
            else if (num == 5)
                return '5';
            else if (num == 6)
                return '6';
            else if (num == 7)
                return '7';
            else if (num == 8)
                return '8';
            else
                return '9';
        }

        private void GameFlow()
        {
            if (battleFlag == 0)// || pcCanMove == 1)
            {
                LoadMap();
                //pcCanMove = 0;
            }
            //else if (pcCanMove == 1)
            //{
            //    LoadMap();
            //    pcMoved = 1;
            //}
            else
            {
                //textBox1.AppendText(Environment.NewLine);
                //textBox1.AppendText("A hostile creature has spotted you!");
                CombatText();
                messageBS = 1;
                CombatText();
                messageTT = 1;
                BattleMode();
            }
        }

        private void LoadMap()
        {
            Bitmap DrawArea;
            Graphics g;

            DrawArea = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            pictureBox1.Image = DrawArea;

            g = Graphics.FromImage(DrawArea);

            g.Clear(Color.White);

            LM();

            mobPos = goblin.pos;
            player.pos = pcPos;

            if (goblin.dead == 1)
            {
                chars[mobPos] = 'X';
            }
            else
                chars[mobPos] = '%';

            chars[pcPos] = '@';

            char[] newChars = new char[chars.Length];
            CenterCamera(chars, newChars);

            str = new string(newChars);

            g.DrawString(str, myFont, Brushes.Black, new Point(0, 0));

            tr.Close();
            g.Dispose();

            CheckAggro();
        }

        private void CheckAggro()
        {
            if (goblin.dead != 1 && CheckAggroRad(goblin.pos, goblin.aggroRad))
            {
                battleFlag = 1;
            }
        }

        private bool CheckAggroRad(int mobPos, int aggroRad)
        {
            int i = aggroRad*66, j = mobPos-i, k = mobPos+i;
            int a = mobPos - aggroRad, s = mobPos + aggroRad;
            int g = 66;

            if (a == pcPos || s == pcPos)
            {
                return true;
            }
            else if ((s - g) == pcPos || (s - 2 * g) == pcPos || (s + g) == pcPos || (s + 2 * g) == pcPos)
            {
                return true;
            }
            else if ((a - g) == pcPos || (a - 2 * g) == pcPos || (a + g) == pcPos || (a + 2 * g) == pcPos)
            {
                return true;
            }
            else if(j == pcPos || k == pcPos)
            {
                return true;
            }
            else if ((j - 1) == pcPos || (j - 2) == pcPos)
            {
                return true;
            }
            else if((j + 1) == pcPos || (j + 2) == pcPos)
            {
                return true;
            }
            else if ((k - 1) == pcPos || (k - 2) == pcPos)
            {
                return true;
            }
            else if ((k + 1) == pcPos || (k + 2) == pcPos)
            {
                return true;
            }
            else
                return false;
        }

        private void CenterCamera(char[] chars, char[] newChars)
        {
            int pcRow = pcPos / 66;
            int pcEdge = pcPos % 66;
            int maxRows = chars.Length / 66;

            int surpN=0, surpS=0, surpW=0, surpE=0;

            char[,] viewSegment = new char[27,56];


            if (pcEdge < 28)
            {
                surpW = 28 - pcEdge;
            }
            else if (pcEdge >= 37)
            {
                surpE = pcEdge - 36;
            }
            
            if(pcRow < 13)
            {
                surpN = 13 - pcRow;
            }
            else if(maxRows < (pcRow + 13))
            {
                surpS = (pcRow + 13) - maxRows;
            }

            if (surpN == 0 && surpS == 0 && surpE == 0 && surpW == 0)
            {
                int zeros = 0;

                for (; zeros < chars.Length ; zeros++)
                {
                    newChars[zeros] = chars[zeros];
                }
            }

            int i, j, l, k;
            int rowMod;
            int nDiff = 13 - surpN + surpS;
            int sDiff = 13 - surpS + surpN;
            int eDiff = 28 - surpE + surpW;
            int wDiff = 28 - surpW + surpE;

            for (i = nDiff, l = 0; i > 0; i--, l++)
            {
                rowMod = pcPos - (i * 66);
                for (j = rowMod - wDiff, k = 0; j < (rowMod + eDiff); j++, k++)
                {
                    viewSegment[l, k] = chars[j];
                }
            }

            for (i = pcPos - wDiff, l = 0; i < (pcPos + eDiff); i++, l++)
            {
                viewSegment[nDiff, l] = chars[i];
            }

            for (i = 1, l = nDiff + 1; i <= sDiff; i++, l++)
            {
                rowMod = pcPos + (i * 66);
                for (j = rowMod - wDiff, k = 0; j < (rowMod + eDiff); j++, k++)
                {
                    viewSegment[l, k] = chars[j];
                }
            }

            for (i = 0, l = 0; i < 27; i++, l++)
            {
                for (j = 0; j < 56; j++, l++)
                {
                    newChars[l] = viewSegment[i, j];
                }
                l++;
                newChars[l] = '\n';
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            LoadStatPage();

            GameFlow();

            base.OnPaint(e);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (battleFlag == 0)
            {
                KeyPressHandler(sender, e);
            }
            else
            {
                if (pcTurn == 0)
                {
                    KeyPressHandler(sender, e);
                    pcMoved = 1;

                    BattleKeyHandler(sender, e);
                }
                else
                {
                    textBox2.AppendText(Environment.NewLine);
                    textBox2.AppendText("Not your turn...");
                }
            }
        }

        public void BattleKeyHandler(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'e')
            {
                textBox2.AppendText(Environment.NewLine);
                textBox2.AppendText("Attack north, east, south, or west?");

                System.Threading.Thread.Sleep(1000);

                pcActed = 1;
                attackCommand = 1;
            }
            else if (e.KeyChar == 'q')
            {
                textBox2.AppendText(Environment.NewLine);
                textBox2.AppendText("Cast north, east, south, or west?");

                System.Threading.Thread.Sleep(1000);

                pcActed = 1;
                spellCommand = 1;
            }
            else if (e.KeyChar == 'x')
            {
                textBox2.AppendText(Environment.NewLine);
                textBox2.AppendText("Action held...");

                pcActed = 1;
            }

            e.Handled = true;
        }

        public void ResolveMeleeAttack()
        {
            int pcTrueHit = (player.meleeHit1 * 10) + player.meleeHit2;
            int pcTrueDmg = (player.meleeDmg1 * 10) + player.meleeDmg2;
            int finalHit = pcTrueHit - (goblin.armor * 2);
            int isHit = toHitRand.Next(1, 100);

            if (isHit <= finalHit)
            {
                goblin.hitPoints -= pcTrueDmg;

                if (goblin.hitPoints <= 0)
                {
                    goblin.dead = 1;
                    CE.MobList[0] = 0;

                    player.exp2 += goblin.exp;
                }

                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText("You score a solid hit with your weapon.");

                TrashTalk();
            }
            else
            {
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText("Your enemy skillfully dodges your attack.");
            }
        }

        public void ResolveMagicAttack()
        {
            int pcTrueHit = (player.magicHit1 * 10) + player.magicHit2;
            int pcTrueDmg = CurrentSpell().damage;
            pcTrueDmg += ((player.magicDmg1 * 10) + player.magicDmg2) / 2;

            int finalHit = pcTrueHit - (goblin.mResist * 2);
            int isHit = toHitRand.Next(1, 100);

            if (isHit <= finalHit)
            {
                goblin.hitPoints -= pcTrueDmg;

                if (goblin.hitPoints <= 0)
                {
                    goblin.dead = 1;
                    CE.MobList[0] = 0;

                    player.exp2 += goblin.exp;
                }

                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText("Magical energies burn away at your enemy's life force.");
            }
            else
            {
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText("Unphased by the attack your enemy continues on.");
            }
        }

        public void TrashTalk()
        {
            int tt = ttRand.Next(1, 10);

            if (tt == 1 || tt == 2)
            {
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText("Goblin: \"Owie, owie, owie!\"");
            }
            else if (tt == 4 || tt == 5)
            {
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText("Goblin: \"I'm gunna eat your eyes in a soup...\"");
            }
            else if (tt == 7 || tt == 8)
            {
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText("The goblin snarls, baring his stained teeth at you.");
            }
        }

        public void PerformAction(object sender, KeyPressEventArgs e)
        {
            int dirOffset = 0;

            if (e.KeyChar == 'w')
            {
                dirOffset = -66;
            }
            else if (e.KeyChar == 'd')
            {
                dirOffset = 1;
            }
            else if (e.KeyChar == 's')
            {
                dirOffset = 66;
            }
            else if (e.KeyChar == 'a')
            {
                dirOffset = -1;
            }

            System.Threading.Thread.Sleep(1000);

            if (attackCommand == 1)
            {
                if ((pcPos + dirOffset) != mobPos)
                {
                    textBox2.AppendText(Environment.NewLine);
                    textBox2.AppendText("You swing wildly, hitting nothing but air.");
                }
                else
                {
                    ResolveMeleeAttack();
                }
            }
            else
            {
                int inSpell = 0, tempPos, i, j;

                for (i = 1; i <= CurrentSpell().range; i++)
                {
                    tempPos = pcPos + (dirOffset * i);

                    for (j = i - 1; j > 0; j--)
                    {
                        if (Math.Abs(dirOffset) == 1)
                        {
                            if((tempPos + (j * 66)) == mobPos || (tempPos - (j * 66)) == mobPos)
                            {
                                inSpell = 1;
                                break;
                            }
                        }
                        else
                        {
                            if ((tempPos + 1) == mobPos || (tempPos - 1) == mobPos)
                            {
                                inSpell = 1;
                                break;
                            }
                        }
                    }

                    if(tempPos == mobPos || inSpell == 1)
                    {
                        inSpell = 1;
                        break;
                    }
                }

                if (inSpell == 1)
                {
                    int m1 = player.mana1, m2 = player.mana2;
                    int trueMana = (m1 * 10) + m2;
                    
                    if(trueMana >= CurrentSpell().cost)
                    {
                        player.mana2 -= CurrentSpell().cost;
                        if (player.mana2 < 0)
                        {
                            player.mana1--;
                            player.mana2 += 10;
                        }

                        ResolveMagicAttack();
                    }
                    else
                    {
                        textBox1.AppendText(Environment.NewLine);
                        textBox1.AppendText("You are unable to gather the energy for your spell.");
                    }
                }
                else
                {
                    textBox2.AppendText(Environment.NewLine);
                    textBox2.AppendText("Your spell has no effect.");
                }
            }

            attackCommand = 0;
            spellCommand = 0;
        }

        //A key handler for WASD movement only
        public void KeyPressHandler(object sender, KeyPressEventArgs e)
        {
            int tempPcPos = pcPos;

            if (attackCommand == 1 || spellCommand == 1)
            {
                PerformAction(sender, e);
            }
            else
            {
                if (e.KeyChar == 'w')
                {
                    tempPcPos -= 66;
                    if (CanMove(tempPcPos))
                    {
                        pcPos = tempPcPos;
                        this.Invalidate();
                    }
                }
                else if (e.KeyChar == 's')
                {
                    tempPcPos += 66;
                    if (CanMove(tempPcPos))
                    {
                        pcPos = tempPcPos;
                        this.Invalidate();
                    }
                }
                else if (e.KeyChar == 'a')
                {
                    tempPcPos -= 1;
                    if (CanMove(tempPcPos))
                    {
                        pcPos = tempPcPos;
                        this.Invalidate();
                    }
                }
                else if (e.KeyChar == 'd')
                {
                    tempPcPos += 1;
                    if (CanMove(tempPcPos))
                    {
                        pcPos = tempPcPos;
                        this.Invalidate();
                    }
                }
            }

            e.Handled = true;
        }

        //Moves a mob in some cardinal direction on the map
        void MoveMob(MonsterClass mob, char dir)
        {
            int tempMobPos = mob.pos;

            if (dir == 'n')
            {
                tempMobPos -= 66;
                if (CanMove(tempMobPos))
                {
                    mob.pos = tempMobPos;
                }
            }
            else if (dir == 's')
            {
                tempMobPos += 66;
                if (CanMove(tempMobPos))
                {
                    mob.pos = tempMobPos;
                }
            }
            else if (dir == 'e')
            {
                tempMobPos += 1;
                if (CanMove(tempMobPos))
                {
                    mob.pos = tempMobPos;
                }
            }
            else if (dir == 'w')
            {
                tempMobPos -= 1;
                if (CanMove(tempMobPos))
                {
                    mob.pos = tempMobPos;
                }
            }

            LoadMap();
        }

        //Checks if a new position is a valid space for
        //a player or mob
        private bool CanMove(int newPos)
        {
            char key = chars[newPos];

            if (key == '-' || key == '|' || key == '+' || key == '@' || key == '#' || key == '\\' || key == '/' || key == '%')
                return false;
            else
                return true;
        }

        //Invalidates (re-paints) the form every time the timer "ticks"
        void WanderTimer_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        //Function for performing an action when the form closes
        //Want to implement a goodbye message
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }

    //Class for controlling combat
    //Currently little more than a glorified turn tracker
    public class CombatEngine
    {
        public int[] MobList { get; set; }
        public int[] TurnOrder { get; set; }
        public int[] TurnHistory { get; set; }
        public int CurrentTurn { get; set; }

        public CombatEngine()
        {
            TurnHistory = new int[20];
            MobList = new int[20];
            TurnOrder = new int[2];

            CombatOrder();
        }

        public void CombatOrder()
        {
            TurnOrder[0] = 0;
            TurnOrder[1] = 1;

            CurrentTurn = 0;
        }

        public int NextTurn()
        {
            if (CurrentTurn == 1)
                return CurrentTurn = 0;
            else
                return CurrentTurn++;
        }

        public int FocusedActor()
        {
            return TurnOrder[CurrentTurn];
        }

        public bool MetVictory()
        {
            int allDead = 1;
            int i;

            for (i = 0; i < MobList.Length; i++)
            {
                if (MobList[i] != 0)
                    allDead = 0;
            }

            if(allDead == 1)
            {
                return true;
            }
            else
                return false;
        }
    }

    //Class for the player's character
    public class PlayerCharacter
    {
        public int currentLevel1 { get; set; }
        public int currentLevel2 { get; set; }
        
        public int hitPoints1 { get; set; }
        public int hitPoints2 { get; set; }

        public int mana1 { get; set; }
        public int mana2 { get; set; }

        public int pcStrength1 { get; set; }
        public int pcStrength2 { get; set; }

        public int pcDexterity1 { get; set; }
        public int pcDexterity2 { get; set; }

        public int pcEndurance1 { get; set; }
        public int pcEndurance2 { get; set; }

        public int pcWillpower1 { get; set; }
        public int pcWillpower2 { get; set; }

        public int meleeDmg1 { get; set; }
        public int meleeDmg2 { get; set; }

        public int magicDmg1 { get; set; }
        public int magicDmg2 { get; set; }

        public int armor1 { get; set; }
        public int armor2 { get; set; }

        public int magicResist1 { get; set; }
        public int magicResist2 { get; set; }

        public int meleeHit1 { get; set; }
        public int meleeHit2 { get; set; }

        public int magicHit1 { get; set; }
        public int magicHit2 { get; set; }

        public int exp1 { get; set; }
        public int exp2 { get; set; }

        public int toLevel1 { get; set; }
        public int toLevel2 { get; set; }

        public int pos { get; set; }

        public PlayerCharacter()
        {
            currentLevel1 = 0;
            currentLevel2 = 1;

            hitPoints1 = 1;
            hitPoints2 = 5;

            mana1 = 2;
            mana2 = 0;

            pcStrength1 = 1;
            pcStrength2 = 0;

            pcDexterity1 = 1;
            pcDexterity2 = 2;

            pcEndurance1 = 0;
            pcEndurance2 = 7;

            pcWillpower1 = 1;
            pcWillpower2 = 5;

            meleeDmg1 = 0;
            meleeDmg2 = 2;

            magicDmg1 = 0;
            magicDmg2 = 3;

            armor1 = 0;
            armor2 = 1;

            magicResist1 = 0;
            magicResist2 = 1;

            meleeHit1 = 5;
            meleeHit2 = 1;

            magicHit1 = 5;
            magicHit2 = 1;

            exp1 = 0;
            exp2 = 0;

            toLevel1 = 1;
            toLevel2 = 0;
        }

        //This function resets the character's stats to the current base
        public void ResetStats()
        {
            pcStrength1 = 1;
            pcStrength2 = 0;

            pcDexterity1 = 1;
            pcDexterity2 = 2;

            pcEndurance1 = 0;
            pcEndurance2 = 7;

            pcWillpower1 = 1;
            pcWillpower2 = 5;

            meleeDmg1 = 0;
            meleeDmg2 = 2;

            magicDmg1 = 0;
            magicDmg2 = 3;

            armor1 = 0;
            armor2 = 1;

            magicResist1 = 0;
            magicResist2 = 1;

            meleeHit1 = 5;
            meleeHit2 = 1;

            magicHit1 = 5;
            magicHit2 = 1;
        }
    }

    //Base class for the various monsters (mobs)
    public class MonsterClass
    {
        Random wanderRand = new Random();

        public string Name { get; set; }
        public int mobID { get; set; }
        public int pos { get; set; }
        public int aggroRad { get; set; }
        public int hitPoints { get; set; }
        public int armor { get; set; }
        public int mResist { get; set; }
        public int meleeDmg { get; set; }
        public int meleeHit { get; set; }
        public int exp { get; set; }
        public int dead { get; set; }
        
        public MonsterClass(string name, int ID, int initPos)
        {
            Name = name;
            mobID = ID; 
            pos = initPos;
            aggroRad = 6;
            hitPoints = 10;
            armor = 3;
            mResist = 1;
            meleeDmg = 4;
            meleeHit = 70;
            exp = 2;
            dead = 0;
        }

        //Function for determing a random direction to wander in
        public char Wander()
        {
            int numRand = wanderRand.Next(1, 5);

            if (numRand == 1)
            {
                return 'n';
            }
            else if (numRand == 2)
            {
                return 's';
            }
            else if (numRand == 3)
            {
                return 'w';
            }
            else
            {
                return 'e';
            }
        }
    }

    //Base class for the items in the game
    public class BaseItem
    {
        public string name { get; set; }

        public string type { get; set; }

        public BaseItem(string n, string t)
        {
            name = n;
            type = t;
        }
    }

    //Class for a player's weapon
    public class Weapon : BaseItem
    {
        public int meleeDmg { get; set; }
        public int magicDmg { get; set; }
        public int meleeHit { get; set; }
        public int magicHit { get; set; }
        public int range { get; set; }

        public Weapon(string n, int meD, int maD, int melH, int magH, int r) : base (n, "Weapon")
        {
            //BaseItem(n, "Weapon");

            meleeDmg = meD;
            magicDmg = maD;
            meleeHit = melH;
            magicHit = magH;
            range = r;
        }
    }

    //Class for a player's armor
    public class Armor : BaseItem
    {
        public int prot { get; set; }
        public int mResist { get; set; }

        public Armor(string n, int p, int mR) : base (n, "Armor")
        {
            //BaseItem(n, "Armor");

            prot = p;
            mResist = mR;
        }
    }

    //Class for consumable items
    //Not yet fully implemented
    public class Consumable : BaseItem
    {
        public Consumable(string n) : base (n, "Consumable")
        {
            //BaseItem(n, "Consumable");
        }
    }

    //Base class for various spells
    public class Spell
    {
        public string name { get; set; }
        public int cost { get; set; }
        public string type { get; set; }

        public Spell(string n, int c, string t)
        {
            name = n;
            cost = c;
            type = t;
        }
    }

    //Class for combat based spells
    public class OffensiveSpell : Spell
    {
        //Variables for damage, range, AOE and cone flags,
        //as well as where the spells effect takes place.
        public int damage { get; set; }
        public int range { get; set; }
        public int AOE { get; set; }
        public int cone { get; set; }
        public int centerEffect { get; set; }

        public OffensiveSpell(string n, int c, int d, int r, int aoe, int co, int cE) : base(n, c, "Offensive")
        {
            damage = d;
            range = r;
            AOE = aoe;
            cone = co;
            centerEffect = cE;
        }
    }
}