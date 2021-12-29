using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace postalSystem
{
    public partial class Form1 : Form
    {
        //DataTable dt = new DataTable();

        public Form1()
        {
            InitializeComponent();
            // Создаем таблицу
            dataGridView1.ColumnCount = 3;
            // Добавляем столбцы, с именами
            dataGridView1.Columns[0].HeaderText = "Номер";
            dataGridView1.Columns[0].Width = 50;
            dataGridView1.Columns[1].HeaderText = "Почта";
            dataGridView1.Columns[2].HeaderText = "Тема";
            dataGridView1.Columns[2].Width = 160;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;
            //dataGridView1.ColumnHeadersVisible = false;
        }

        createNewEmail cNE1;
        private void button6_Click(object sender, EventArgs e)
        {
            if (indexRowBool) 
            {
                Tuple<String, String, String> emailsDB_table = (Tuple<String, String, String>)emailsDB_tableList[indexRow];
                String whom = emailsDB_table.Item1;
                String themes = emailsDB_table.Item2;
                String emails = emailsDB_table.Item3;

                if (cNE1 == null)
                {
                    cNE1 = new createNewEmail("Чтение письма", whom, themes, emails);
                    cNE1.FormClosed += (x, y) => { cNE1 = null; }; //для избежания проблем с повторным открытием после закрытия
                }
                cNE1.Owner = this;
                if (draft)
                {
                    cNE1.List = emailsDB_tableList;
                    cNE1.IndexRow = indexRow;
                }
                cNE1.Show();
            }
           
        }

        public void dataGridViewRowsClear() 
        {
            dataGridView1.Rows.Clear();
        }

        bool incoming = false;
        private void button1_Click(object sender, EventArgs e) // тап на входящие
        {
            dataGridView1.Rows.Clear();

            StreamReader reader = new StreamReader("Входящие.txt"); // открытие файла с базой данных
                                                                    // и считывание информации из него
            int i = 1;
            while (!reader.EndOfStream)
            {
                    string str = reader.ReadLine().ToString();
                    //dataGridView1.Rows[i].Cells[0].Value = i;
                    //dataGridView1.Rows[i].Cells[1].Value = stringScissors(str, 0, spacePars(str, ' '));
                    //dataGridView1.Rows[i].Cells[2].Value = stringScissors(str, spacePars(str, ' '), str.Length);
                    dataGridView1.Rows.Add(i, stringScissors(str, 0, spacePars(str, "###",1)),
                                    stringScissors(str, spacePars(str, "###",1)+3, spacePars(str, "###", 2)));
                    i++;         
            }
            reader.Close(); // ОБЯЗАТЕЛЬНОЕ ЗАКРЫТИЕ ФАЙЛА
            themeSize(i);
            emailsDB_tableList.Clear();
            parseEmailsDB("Входящие.txt");
            incoming = true;
            draft = false;
            sent = false;
            basket = false;
            indexRowBool = false;
        }

        /// <summary>
        /// Подсчет строк в файле (м/б ещё понадобится)
        /// </summary>
        /// <param name="FileName"> Имя файла </param>
        /// <returns></returns>
        private int rowCounter(String FileName) 
        {
            StreamReader reader = new StreamReader(FileName); // открытие файла с базой данных
                                                                    // и считывание информации из него
            int count = 1;
            while (!reader.EndOfStream)
            {
                if (reader.ReadLine().ToString() != "") 
                {
                    count++;
                }
            }
            reader.Close(); // ОБЯЗАТЕЛЬНОЕ ЗАКРЫТИЕ ФАЙЛА
            return count;
        }

        /// <summary>
        /// Определение местоположения символа
        /// </summary>
        /// <param name="str"> строка </param>
        /// <param name="ch"> символ </param>
        /// <returns></returns>
        private int spacePars(String str, String ch, int count) 
        {
            int num = 0;
            int checkingCount = 0;
            for (int i = 0; i < str.Length - 2; i++)
            {
                if (str[i] == ch[0] && str[i+1] == ch[1] && str[i + 2] == ch[2]) 
                {
                    num = i;
                    checkingCount++;
                    if (checkingCount == count)
                    {
                        return num; // сделать так чтобы ещё останавливался на 1 2 или 3 найденом символе
                    }
                }
            }
            return num;
        }

        /// <summary>
        /// Это ножницы для строк 
        /// </summary>
        /// <param name="str"> строка </param>
        /// <param name="beginning"> начало </param>
        /// <param name="end"> конец </param>
        /// <returns></returns>
        private string stringScissors(String str, int beginning, int end) 
        {
            String ret = "";
            for (int i = beginning; i < end; i++)
            {
                ret += str[i];
            }
            return ret;
        }

        bool sent = false;
        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();

            StreamReader reader = new StreamReader("Отправленные.txt"); // открытие файла с базой данных
                                                                    // и считывание информации из него
            int i = 1;
            while (!reader.EndOfStream)
            {
                string str = reader.ReadLine().ToString();
                //dataGridView1.Rows[i].Cells[0].Value = i;
                //dataGridView1.Rows[i].Cells[1].Value = stringScissors(str, 0, spacePars(str, ' '));
                //dataGridView1.Rows[i].Cells[2].Value = stringScissors(str, spacePars(str, ' '), str.Length);
                dataGridView1.Rows.Add(i, stringScissors(str, 0, spacePars(str, "###", 1)),
                                   stringScissors(str, spacePars(str, "###", 1) + 3, spacePars(str, "###", 2)));
                i++;
            }
            reader.Close(); // ОБЯЗАТЕЛЬНОЕ ЗАКРЫТИЕ ФАЙЛА
            themeSize(i);
            emailsDB_tableList.Clear();
            parseEmailsDB("Отправленные.txt");
            incoming = false;
            draft = false;
            sent = true;
            basket = false;
            indexRowBool = false;
        }

        bool draft = false;
        private void button3_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();

            StreamReader reader = new StreamReader("Черновики.txt"); // открытие файла с базой данных
                                                                     // и считывание информации из него
            int i = 1;
            while (!reader.EndOfStream)
            {
                string str = reader.ReadLine().ToString();
                //dataGridView1.Rows[i].Cells[0].Value = i;
                //dataGridView1.Rows[i].Cells[1].Value = stringScissors(str, 0, spacePars(str, ' '));
                //dataGridView1.Rows[i].Cells[2].Value = stringScissors(str, spacePars(str, ' '), str.Length);
                dataGridView1.Rows.Add(i, stringScissors(str, 0, spacePars(str, "###", 1)),
                                    stringScissors(str, spacePars(str, "###", 1) + 3, spacePars(str, "###", 2)));
                i++;
            }
            reader.Close(); // ОБЯЗАТЕЛЬНОЕ ЗАКРЫТИЕ ФАЙЛА
            themeSize(i);
            emailsDB_tableList.Clear();
            parseEmailsDB("Черновики.txt");
            incoming = false;
            draft = true;
            sent = false;
            basket = false;
            indexRowBool = false;
        }

        bool rightСlick = false;
        /// <summary>
        /// Для динамизирования размера полей таблицы
        /// </summary>
        /// <param name="i"></param>
        private void themeSize(int i)
        {
            if (i > 8)
            {
                dataGridView1.Columns[2].Width = 150;
            }
            if (i > 1)
            {
                rightСlick = true;
            }
            else {
                rightСlick = false;
            }
        }

        bool basket = false;
        private void button4_Click(object sender, EventArgs e)
        {

            dataGridView1.Rows.Clear();

            StreamReader reader = new StreamReader("Корзина.txt"); // открытие файла с базой данных
                                                                     // и считывание информации из него
            int i = 1;
            while (!reader.EndOfStream)
            {
                string str = reader.ReadLine().ToString();
                //dataGridView1.Rows[i].Cells[0].Value = i;
                //dataGridView1.Rows[i].Cells[1].Value = stringScissors(str, 0, spacePars(str, ' '));
                //dataGridView1.Rows[i].Cells[2].Value = stringScissors(str, spacePars(str, ' '), str.Length);
                dataGridView1.Rows.Add(i, stringScissors(str, 0, spacePars(str, "###", 1)),
                                   stringScissors(str, spacePars(str, "###", 1) + 3, spacePars(str, "###", 2)));
                i++;
            }
            reader.Close(); // ОБЯЗАТЕЛЬНОЕ ЗАКРЫТИЕ ФАЙЛА
            themeSize(i);
            emailsDB_tableList.Clear();
            parseEmailsDB("Корзина.txt");
            incoming = false;
            draft = false;
            sent = false;
            basket = true;
            indexRowBool = false;
        }

        createNewEmail cNE;
        private void button5_Click(object sender, EventArgs e)
        {
            if (cNE == null)
            {
                cNE = new createNewEmail();
                cNE.FormClosed += (x, y) => { cNE = null; }; //для избежания проблем с повторным открытием после закрытия
            }
            cNE.Owner = this;
            cNE.Show();
        }

        int indexRow = 0;
        bool indexRowBool = false;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // тут нунжы данные о том, какая строка была нажата
            if (rightСlick)
            {
                indexRow = dataGridView1.CurrentCell.RowIndex;
            }
            indexRowBool = true;
        }

        static String whom  = "";
        static String themes = "";
        static String emails = "";

        
        ArrayList emailsDB_tableList = new ArrayList();

        /// <summary>
        /// Запись всех писел в коллекцию
        /// </summary>
        /// <param name="FileName"></param>
        private void parseEmailsDB(String FileName)
        {
            StreamReader reader = new StreamReader(FileName); // открытие файла с базой данных
                                                                   // и считывание информации из него
            while (!reader.EndOfStream)
            {
                string str = reader.ReadLine().ToString();
                whom = stringScissors(str, 0, spacePars(str, "###", 1));
                themes = stringScissors(str, spacePars(str, "###", 1) + 3, spacePars(str, "###", 2));
                emails = stringScissors(str, spacePars(str, "###", 2) + 3, str.Length);
                Tuple<String, String, String> emailsDB_table = Tuple.Create(whom, themes, emails); // для передачи кортежа входных данных
                emailsDB_tableList.Add(emailsDB_table);
            }
            reader.Close(); // ОБЯЗАТЕЛЬНОЕ ЗАКРЫТИЕ ФАЙЛА
        }

        private void справкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Программа предназначена для имитации работы почтовой системы. Разработал программу студент группы 18ВО1 Олейников Глеб",
                "Справка");
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        String[] deletedElem = new String[3];
        private void button7_Click(object sender, EventArgs e)
        {
            if (indexRowBool)
            {
                Tuple<String, String, String> emailsDB_table = (Tuple<String, String, String>)emailsDB_tableList[indexRow];
                String whom = emailsDB_table.Item1;
                String themes = emailsDB_table.Item2;
                String emails = emailsDB_table.Item3;
                deletedElem[0] = whom;
                deletedElem[1] = themes;
                deletedElem[2] = emails;
                //======================================
                emailsDB_tableList.RemoveAt(indexRow);
                if (incoming)
                {
                    rewriteDBEmails("Входящие.txt");
                }
                if (draft)
                {
                    rewriteDBEmails("Черновики.txt");
                }
                if (sent)
                {
                    rewriteDBEmails("Отправленные.txt");
                }
                if (basket)
                {
                    rewriteDBEmails("Корзина.txt");
                }
            }
        }

        private void rewriteDBEmails(String FileName) 
        {
            FileStream file = new FileStream(FileName, FileMode.Create); //создаем файловый поток
            StreamWriter writer = new StreamWriter(file); //создаем «потоковый писатель» и связываем его с файловым потоком
            for (int i = 0; i < emailsDB_tableList.Count; i++)
            {
                Tuple<String, String, String> emailsDB_table = (Tuple<String, String, String>)emailsDB_tableList[i];
                String whom = emailsDB_table.Item1;
                String themes = emailsDB_table.Item2;
                String emails = emailsDB_table.Item3;
                string newStr = whom + "###" + themes + "###" + emails;
                writer.WriteLine(newStr);
            }
            writer.Close();

            //=====================================================================================

            dataGridView1.Rows.Clear();
            StreamReader reader = new StreamReader(FileName); // открытие файла с базой данных
                                                                    // и считывание информации из него
            int inc = 1;
            while (!reader.EndOfStream)
            {
                string str = reader.ReadLine().ToString();
                dataGridView1.Rows.Add(inc, stringScissors(str, 0, spacePars(str, "###", 1)),
                                stringScissors(str, spacePars(str, "###", 1) + 3, spacePars(str, "###", 2)));
                inc++;
            }
            reader.Close(); // ОБЯЗАТЕЛЬНОЕ ЗАКРЫТИЕ ФАЙЛА
            themeSize(inc);

            //=====================================================================================

            if (!basket)
            {
                sendingCart();
            }
        }

        private void sendingCart()
        {
            ArrayList allEmails = new ArrayList();
            StreamReader reader = new StreamReader("Корзина.txt"); // открытие файла с базой данных
                                                                   // и считывание информации из него
            int count = 1;
            while (!reader.EndOfStream)
            {
                string str = reader.ReadLine().ToString();
                allEmails.Add(str);
            }
            reader.Close(); // ОБЯЗАТЕЛЬНОЕ ЗАКРЫТИЕ ФАЙЛА

            string newStr = deletedElem[0] + "###" + deletedElem[1] + "###" + deletedElem[2];
            allEmails.Add(newStr);

            FileStream file = new FileStream("Корзина.txt", FileMode.Create); //создаем файловый поток
            StreamWriter writer = new StreamWriter(file); //создаем «потоковый писатель» и связываем его с файловым потоком
            for (int i = 0; i < allEmails.Count; i++)
            {
                writer.WriteLine(allEmails[i]);
            }
            writer.Close();
            MessageBox.Show("Удаленное письмо теперь находится в корзине...", "Напоминание");
        }

        public void openDraft() 
        {
            StreamReader reader = new StreamReader("Черновики.txt"); // открытие файла с базой данных
                                                                     // и считывание информации из него
            int i = 1;
            while (!reader.EndOfStream)
            {
                string str = reader.ReadLine().ToString();
                //dataGridView1.Rows[i].Cells[0].Value = i;
                //dataGridView1.Rows[i].Cells[1].Value = stringScissors(str, 0, spacePars(str, ' '));
                //dataGridView1.Rows[i].Cells[2].Value = stringScissors(str, spacePars(str, ' '), str.Length);
                dataGridView1.Rows.Add(i, stringScissors(str, 0, spacePars(str, "###", 1)),
                                    stringScissors(str, spacePars(str, "###", 1) + 3, spacePars(str, "###", 2)));
                i++;
            }
            reader.Close(); // ОБЯЗАТЕЛЬНОЕ ЗАКРЫТИЕ ФАЙЛА
            themeSize(i);
            emailsDB_tableList.Clear();
            parseEmailsDB("Черновики.txt");
            incoming = false;
            draft = true;
            sent = false;
            basket = false;
            indexRowBool = false;
        }
    }
}
