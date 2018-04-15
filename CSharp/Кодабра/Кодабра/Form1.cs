using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Кодабра
{
    public partial class Form1 : Form
    {
        [DataContract]
        public class DictElem
        {
            public String key;
            [DataMember]
            public String value;
        }

        MainForm mf;
        public Form1(MainForm mf)
        {
            InitializeComponent();
            this.mf = mf;
            dict = new Dictionary<string, string>();
            //dict.Add("txt", "notepad.exe");
            buffer = new Bitmap(600, 600);//(Bitmap)Bitmap.FromFile("start.bmp");
            g = Graphics.FromImage(buffer);
            b = new SolidBrush(Color.Black);
            b1 = new SolidBrush(Color.Red);
            b2 = new SolidBrush(Color.White);
            b3 = new SolidBrush(Color.Gray);
            b4 = new SolidBrush(Color.Gold);
            p = new Pen(b);
            KeyDown += OnKeyPress;
            player = new Player(1, 1, 2);
            doors = new List<byte[]>();
            quests = new List<byte[]>();
            textBox1.Enabled = false;
            button1.Enabled = false;
            LoadMaze("lab1");
            LoadDict();
        }

        private bool DoorIsOpened(int x, int y)
        {
            byte[] temp = doors.Find(
            delegate (byte[] door)
            { return (door[0] == x & door[1] == y); });
            if (temp == null)
                return true;
            //TODO
            if (player.Key)
            {
                doors.Remove(temp);
                player.Key = false;
                return true;
            }
            return false;
        }

        private void OnKeyPress(object sender, KeyEventArgs e)
        {
            if (button1.Enabled)
                return;
            int tempDir = player.Direction;
            switch (e.KeyCode)
            {
                case Keys.W:
                    player.Direction = 1;
                    break;
                case Keys.A:
                    player.Direction = 0;
                    break;
                case Keys.S:
                    player.Direction = 3;
                    break;
                case Keys.D:
                    player.Direction = 2;
                    break;
            }

            int x = player.X + ((player.Direction + 1) % 2) * (player.Direction - 1),
                y = player.Y + (player.Direction % 2) * (player.Direction - 2);

            
                //TODO

            if (!DoorIsOpened(x, y))
            {
                MessageBox.Show("Для того, чтобы пройти через дверь, найдите вопрос");
                return;
            }

            if (maze[y, x] != 1 & CheckQuest(x, y))
            {
                g.FillRectangle(b2, player.X * 20, player.Y * 20, 20, 20);
                player.X = x;
                player.Y = y;
                g.FillEllipse(b1, player.X * 20, player.Y * 20, 20, 20);
                pictureBox1.Image = buffer;
            }
            else
                player.Direction = tempDir;
        }

        private bool CheckQuest(int x, int y)
        {
            byte[] temp = quests.Find(
            delegate (byte[] door)
            { return (door[0] == x & door[1] == y); });
            if (temp == null)
                return true;
            GetQuest();
            return false;
            
            //TODO
        }

        private void CheckAnswer()
        {
            if (answer.Equals(textBox1.Text))
            {
                MessageBox.Show("Верно! Проход открыт");
                player.Key = true;
            }
            else
                MessageBox.Show("Неверно! Попробуйте снова");
        }

        private void GetQuest()
        {
            button1.Enabled = true;
            textBox1.Enabled = true;
            List<String> temp = Enumerable.ToList(dict.Keys);
            Random r = new Random(DateTime.Now.Millisecond);
            answer = temp[r.Next(0, temp.Count)];
            label1.Text = dict[answer];
            //TODO
        }

        Graphics g;
        Bitmap buffer;
        Pen p;
        Brush b, b1, b2, b3, b4;
        Player player;
        String answer;

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Show();
        }

        public byte[,] maze = new byte[30, 30];

        private void button1_Click(object sender, EventArgs e)
        {
            if (!textBox1.Enabled)
                return;
            CheckAnswer();
            textBox1.Enabled = false;
            button1.Enabled = false;
        }

        public List<byte[]> doors;
        private List<byte[]> quests;
        private Dictionary<String, String> dict;
        DataContractJsonSerializer jsonSerializer;

        public void LoadDict()
        {
            jsonSerializer = new DataContractJsonSerializer(typeof(DictElem));
            using (FileStream fs = new FileStream("dict.json", FileMode.OpenOrCreate))
            {
                DictElem[] de = (DictElem[])jsonSerializer.ReadObject(fs);

                foreach (DictElem e in de)
                    dict.Add(e.key, e.value);
            }
        }

        public void LoadMaze(String name)
        {
            using (StreamReader file = new StreamReader(name + ".txt"))
                for (int i = 0; i < 30; i++)
                {
                    String str = file.ReadLine();
                    for (int j = 0; j < str.Length; j++)
                    {
                        if (str[j] == '@')
                        {
                            maze[i, j] = 2;
                            byte[] temp = { (byte)j, (byte)i };
                            doors.Add(temp);
                        }
                        if (str[j] == '#') maze[i, j] = 1;
                        if (str[j] == '*')
                        {
                            maze[i, j] = 3;
                            byte[] temp = { (byte)j, (byte)i };
                            quests.Add(temp);
                        }
                    }
                }
            g.Clear(Color.White);
            for (int i = 0; i < 30; i++)
                for (int j = 0; j < 30; j++)
                {
                    if (maze[i, j] == 1)
                        g.FillRectangle(b, new Rectangle(j * 20, i * 20, 20, 20));
                    if (maze[i, j] == 2)
                        g.FillRectangle(b3, new Rectangle(j * 20, i * 20, 20, 20));
                    if (maze[i, j] == 3)
                        g.FillRectangle(b4, new Rectangle(j * 20, i * 20, 20, 20));
                }
            pictureBox1.Image = buffer;
        }
    }   
}
