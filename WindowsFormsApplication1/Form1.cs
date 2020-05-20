using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Bitmap bPic;
        BinaryReader bText;
        List<byte> bList;
        int CountText;
        public Form1()
        {
            InitializeComponent();
        }

        private BitArray ByteToBit(byte src)
        {
            BitArray bitArray = new BitArray(8);
            bool st = false;
            for (int i = 0; i < 8; i++)
            {
                if ((src >> i & 1) == 1)
                {
                    st = true;
                }
                else st = false;
                bitArray[i] = st;
            }
            return bitArray;
        }

        private byte BitToByte(BitArray scr)
        {
            byte num = 0;
            for (int i = 0; i < scr.Count; i++)
                if (scr[i] == true)
                    num += (byte)Math.Pow(2, i);
            return num;
        }

        /*Записыает количество символов для шифрования в первые биты картинки */
        private void WriteCountText(int count, Bitmap src)
        {
            byte[] CountSymbols = Encoding.GetEncoding(1251).GetBytes(count.ToString());
            for (int i = 0; i < count.ToString().Length; ++i)
            {
                BitArray bitCount = ByteToBit(CountSymbols[i]);
                //биты количества символов
                Color pColor = src.GetPixel(0, i + 1); //1, 2, 3 пиксели
                BitArray bitsCurColor = ByteToBit(pColor.R); //бит цветов текущего пикселя
                bitsCurColor[0] = bitCount[0];
                bitsCurColor[1] = bitCount[1];
                byte nR = BitToByte(bitsCurColor); //новый бит цвета пиксея

                bitsCurColor = ByteToBit(pColor.G);//бит бит цветов текущего пикселя
                bitsCurColor[0] = bitCount[2];
                bitsCurColor[1] = bitCount[3];
                bitsCurColor[2] = bitCount[4];
                byte nG = BitToByte(bitsCurColor);//новый цвет пиксея

                bitsCurColor = ByteToBit(pColor.B);//бит бит цветов текущего пикселя
                bitsCurColor[0] = bitCount[5];
                bitsCurColor[1] = bitCount[6];
                bitsCurColor[2] = bitCount[7];
                byte nB = BitToByte(bitsCurColor);//новый цвет пиксея

                Color nColor = Color.FromArgb(nR, nG, nB); //новый цвет из полученных битов
                src.SetPixel(0, i + 1, nColor); //записали полученный цвет в картинку
            }
            MessageBox.Show("Ваш ключ для прочтения: " + count.ToString().Length + "\nЗапишите или запомните", "Информация");
            toolStripStatusLabel1.Text = "Загружен текст длинной " + count + " символов";
        }

        private int ReadCountText(Bitmap src)
         {
             byte[] rez = new byte[Convert.ToInt32(textBox2.Text)]; //массив на 3 элемент, т.е. максимум 999 символов шифруется
             for (int i = 0; i < Convert.ToInt32(textBox2.Text); i++)
             {
                 Color color = src.GetPixel(0, i + 1); //цвет 1, 2, 3 пикселей 
                 BitArray colorArray = ByteToBit(color.R); //биты цвета
                 BitArray bitCount = ByteToBit(color.R); ; //инициализация результирующего массива бит
                 bitCount[0] = colorArray[0];
                 bitCount[1] = colorArray[1];

                 colorArray = ByteToBit(color.G);
                 bitCount[2] = colorArray[0];
                 bitCount[3] = colorArray[1];
                 bitCount[4] = colorArray[2];

                 colorArray = ByteToBit(color.B);
                 bitCount[5] = colorArray[0];
                 bitCount[6] = colorArray[1];
                 bitCount[7] = colorArray[2];
                 rez[i] = BitToByte(bitCount);
             }
            string m = Encoding.GetEncoding(1251).GetString(rez);
             return Convert.ToInt32(m, 10);
         }

        private bool isEncryption(Bitmap scr)
        {
            byte[] rez = new byte[1];
            Color color = scr.GetPixel(0, 0);
            BitArray colorArray = ByteToBit(color.R); //получаем байт цвета и преобразуем в массив бит
            BitArray messageArray = ByteToBit(color.R); ;//инициализируем результирующий массив бит
            messageArray[0] = colorArray[0];
            messageArray[1] = colorArray[1];

            colorArray = ByteToBit(color.G);//получаем байт цвета и преобразуем в массив бит
            messageArray[2] = colorArray[0];
            messageArray[3] = colorArray[1];
            messageArray[4] = colorArray[2];

            colorArray = ByteToBit(color.B);//получаем байт цвета и преобразуем в массив бит
            messageArray[5] = colorArray[0];
            messageArray[6] = colorArray[1];
            messageArray[7] = colorArray[2];
            rez[0] = BitToByte(messageArray); //получаем байт символа, записанного в 1 пикселе
            string m = Encoding.GetEncoding(1251).GetString(rez);
            if (m == "/")
            {
                return true;
            }
            else return false;
         }

        private void button1_Click(object sender, EventArgs e)
        {
            string FilePic;
            OpenFileDialog dPic = new OpenFileDialog();
            dPic.Filter = "Файлы изображений (*.bmp)|*.bmp|Все файлы (*.*)|*.*";
            if (dPic.ShowDialog() == DialogResult.OK)
            {
                FilePic = dPic.FileName;
            }
            else
            {
                FilePic = null;
                return;
            }

            FileStream rFile;
            try
            {
                rFile = new FileStream(FilePic, FileMode.Open); //открываем поток
            }
            catch (IOException)
            {
                MessageBox.Show("Ошибка открытия файла", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            bPic = new Bitmap(rFile);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Image = bPic;
            toolStripStatusLabel1.Text = "Загружено изображение " + Path.GetFileName(rFile.Name);
            rFile.Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked) { button2.Text = "Скрыть сообщение"; button3.Enabled = true; textBox2.Text = "Значение ключа"; textBox2.Enabled = false; }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void открытьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string FilePic;
            OpenFileDialog dPic = new OpenFileDialog();
            dPic.Filter = "Файлы изображений (*.bmp)|*.bmp|Все файлы (*.*)|*.*";
            if (dPic.ShowDialog() == DialogResult.OK)
            {
                FilePic = dPic.FileName;
            }
            else
            {
                FilePic = null;
                return;
            }

            FileStream rFile;
            try
            {
                rFile = new FileStream(FilePic, FileMode.Open); //открываем поток
            }
            catch (IOException)
            {
                MessageBox.Show("Ошибка открытия файла", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            bPic = new Bitmap(rFile);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Image = bPic;
            toolStripStatusLabel1.Text = "Загружено изображение " + Path.GetFileName(rFile.Name);
            rFile.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(bPic != null)
            {
            if (radioButton1.Checked)
            {
                if (!isEncryption(bPic))
                {
                    if (CountText > ((bPic.Width * bPic.Height)) - 4)
                    {
                        MessageBox.Show("Выбранная картинка мала для размещения выбранного текста", "Информация");
                        return;
                    }

                    //проверяем, может быть картинка уже зашифрована
                    if (isEncryption(bPic))
                    {
                        MessageBox.Show("Файл уже зашифрован", "Информация", MessageBoxButtons.OK);
                        return;
                    }

                    byte[] Symbol = Encoding.GetEncoding(1251).GetBytes("/");
                    BitArray ArrBeginSymbol = ByteToBit(Symbol[0]);
                    Color curColor = bPic.GetPixel(0, 0);
                    BitArray tempArray = ByteToBit(curColor.R);
                    tempArray[0] = ArrBeginSymbol[0];
                    tempArray[1] = ArrBeginSymbol[1];
                    byte nR = BitToByte(tempArray);

                    tempArray = ByteToBit(curColor.G);
                    tempArray[0] = ArrBeginSymbol[2];
                    tempArray[1] = ArrBeginSymbol[3];
                    tempArray[2] = ArrBeginSymbol[4];
                    byte nG = BitToByte(tempArray);

                    tempArray = ByteToBit(curColor.B);
                    tempArray[0] = ArrBeginSymbol[5];
                    tempArray[1] = ArrBeginSymbol[6];
                    tempArray[2] = ArrBeginSymbol[7];
                    byte nB = BitToByte(tempArray);

                    Color nColor = Color.FromArgb(nR, nG, nB);
                    bPic.SetPixel(0, 0, nColor);
                    //то есть в первом пикселе будет символ /, который говорит о том, что картика зашифрована
                    WriteCountText(CountText, bPic); //записываем количество символов для шифрования
                    
                    int index = 0;
                     bool st = false;
                     for (int i = 4; i < bPic.Width; i++)
                     {
                         for (int j = 0; j < bPic.Height; j++)
                         {
                             Color pixelColor = bPic.GetPixel(i, j);
                             if (index == bList.Count)
                             {
                                 st = true;
                                 break;
                             }
                             BitArray colorArray = ByteToBit(pixelColor.R);
                             BitArray messageArray = ByteToBit(bList[index]);
                             colorArray[0] = messageArray[0]; //меняем
                             colorArray[1] = messageArray[1]; // в нашем цвете биты
                             byte newR = BitToByte(colorArray);

                             colorArray = ByteToBit(pixelColor.G);
                             colorArray[0] = messageArray[2];
                             colorArray[1] = messageArray[3];
                             colorArray[2] = messageArray[4];
                             byte newG = BitToByte(colorArray);

                             colorArray = ByteToBit(pixelColor.B);
                             colorArray[0] = messageArray[5];
                             colorArray[1] = messageArray[6];
                             colorArray[2] = messageArray[7];
                             byte newB = BitToByte(colorArray);

                             Color newColor = Color.FromArgb(newR, newG, newB);
                             bPic.SetPixel(i, j, newColor);
                             index++;
                         }
                         if (st)
                         {
                             break;
                         }
                     }
                     String sFilePic;
                     SaveFileDialog dSavePic = new SaveFileDialog();
                     dSavePic.Filter = "Файлы изображений (*.bmp)|*.bmp|Все файлы (*.*)|*.*";
                     if (dSavePic.ShowDialog() == DialogResult.OK)
                     {
                         sFilePic = dSavePic.FileName;
                     }
                     else
                     {
                         sFilePic = "";
                         return;
                     };

                     FileStream wFile;
                     try
                     {
                         wFile = new FileStream(sFilePic, FileMode.Create); //открываем поток на запись результатов
                     }
                     catch (IOException)
                     {
                         MessageBox.Show("Ошибка открытия файла на запись", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                         return;
                     }

                     bPic.Save(wFile, System.Drawing.Imaging.ImageFormat.Bmp);
                     toolStripStatusLabel1.Text = "Файл " + Path.GetFileName(sFilePic) + " успешно записан";
                     wFile.Close(); //закрываем поток
                }
                else MessageBox.Show("В изображении уже записано сообщение.", "Информация");
            }
            if (radioButton2.Checked)
            {
                int countSymbol = ReadCountText(bPic); //считали количество зашифрованных символов
                byte[] message = new byte[countSymbol];
                int index = 0;
                bool st = false;
                for (int i = 4; i < bPic.Width; i++)
                {
                    for (int j = 0; j < bPic.Height; j++)
                    {
                        Color pixelColor = bPic.GetPixel(i, j);
                        if (index == message.Length)
                        {
                            st = true;
                            break;
                        }
                        BitArray colorArray = ByteToBit(pixelColor.R);
                        BitArray messageArray = ByteToBit(pixelColor.R); ;
                        messageArray[0] = colorArray[0];
                        messageArray[1] = colorArray[1];

                        colorArray = ByteToBit(pixelColor.G);
                        messageArray[2] = colorArray[0];
                        messageArray[3] = colorArray[1];
                        messageArray[4] = colorArray[2];

                        colorArray = ByteToBit(pixelColor.B);
                        messageArray[5] = colorArray[0];
                        messageArray[6] = colorArray[1];
                        messageArray[7] = colorArray[2];
                        message[index] = BitToByte(messageArray);
                        index++;
                    }
                    if (st)
                    {
                        break;
                    }
                }
                string strMessage = Encoding.GetEncoding(1251).GetString(message);
                textBox1.Text = strMessage;
                string sFileText;
                SaveFileDialog dSaveText = new SaveFileDialog();
                dSaveText.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                if (dSaveText.ShowDialog() == DialogResult.OK)
                {
                    sFileText = dSaveText.FileName;
                }
                else
                {
                    sFileText = "";
                    return;
                };

                FileStream wFile;
                try
                {
                    wFile = new FileStream(sFileText, FileMode.Create); //открываем поток на запись результатов
                }
                catch (IOException)
                {
                    MessageBox.Show("Ошибка открытия файла на запись", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                StreamWriter wText = new StreamWriter(wFile, Encoding.Default);
                wText.Write(strMessage);
                toolStripStatusLabel1.Text = "Текст записан в файл " + Path.GetFileName(sFileText);
                wText.Close();
                wFile.Close(); //закрываем поток
            }
            }
            else MessageBox.Show("Пожалуйста, сначала загрузите изображение", "Информация");
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked) { button2.Text = "Прочитать сообщение"; button3.Enabled = false; textBox2.Enabled = true; textBox2.Text = null; } 
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string FileText;
            OpenFileDialog dText = new OpenFileDialog();
            dText.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            if (dText.ShowDialog() == DialogResult.OK)
            {
                FileText = dText.FileName;
            }
            else
            {
                FileText = "";
                return;
            }

            FileStream rText;
            try
            {
                rText = new FileStream(FileText, FileMode.Open); //открываем поток
            }
            catch (IOException)
            {
                MessageBox.Show("Ошибка открытия файла", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            bText = new BinaryReader(rText, Encoding.ASCII);

            bList = new List<byte>();
            while (bText.PeekChar() != -1)
            { //считали весь текстовый файл для шифрования в лист байт
                bList.Add(bText.ReadByte());
            }
            CountText = bList.Count; // в CountText - количество в байтах текста, который нужно закодировать
            toolStripStatusLabel1.Text = "Текст успешно загружен из файла " + Path.GetFileName(FileText);
            bText.Close();

            
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && (number > '1' && number < '3')) 
            {
                e.Handled = true;
            }
        }
    }
}
