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
    public partial class createNewEmail : Form
    {
        /// <summary>
        /// этот конструктор дефолтом делает окно для создание нового письма
        /// </summary>
        public createNewEmail()
        {
            InitializeComponent();
            textBox1.Text = "glebPNZ@gmail.com";
            textBox1.Enabled = false;
        }

        bool draft = false;
        ArrayList list = new ArrayList();
        int indexRow = 0;

        public ArrayList List { get => list; set => list = value; }
        public int IndexRow { get => indexRow; set => indexRow = value; }

        /// <summary>
        /// конструктор для редактирования сообщения
        /// </summary>
        /// <param name="textNameForm"> наименование формы </param>
        /// <param name="textWhom"> кому сообщение предназначено </param>
        /// <param name="textThemes"> тема сообщения </param>
        /// <param name="textEmails"> само сообщение </param>
        public createNewEmail(String textNameForm,String textWhom, String textThemes, String textEmails)
        {
            InitializeComponent();
            this.Text = textNameForm;
            if (textWhom == "glebPNZ@gmail.com")
            {
                textBox1.Enabled = false;
                textBox2.Text = textWhom;
                textBox3.Text = textThemes;
                richTextBox1.Text = textEmails;
            }
            else
            {
                textBox1.Text = "glebPNZ@gmail.com";
                textBox1.Enabled = false;
                textBox2.Text = textWhom;
                textBox3.Text = textThemes;
                richTextBox1.Text = textEmails;
            }
            draft = true;
        }

        ArrayList allEmails = new ArrayList();


        private void button1_Click(object sender, EventArgs e)
        {
            String FileName = "Черновики.txt";
            StreamReader reader = new StreamReader(FileName); // открытие файла с базой данных
                                                              // и считывание информации из него
            int count = 1;
            while (!reader.EndOfStream)
            {
                string str = reader.ReadLine().ToString();
                allEmails.Add(str);
            }
            reader.Close(); // ОБЯЗАТЕЛЬНОЕ ЗАКРЫТИЕ ФАЙЛА
            
            string newStr = textBox2.Text + "###" + textBox3.Text + "###" + richTextBox1.Text;
            allEmails.Add(newStr);

            FileStream file = new FileStream(FileName, FileMode.Create); //создаем файловый поток
            StreamWriter writer = new StreamWriter(file); //создаем «потоковый писатель» и связываем его с файловым потоком
            for (int i = 0; i < allEmails.Count; i++)
            {
                writer.WriteLine(allEmails[i]);
            }
            writer.Close();
            MessageBox.Show("Ваш черновик сохранён","Напоминание");
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String FileName = "Отправленные.txt";
            StreamReader reader = new StreamReader(FileName); // открытие файла с базой данных
                                                              // и считывание информации из него
            int count = 1;
            while (!reader.EndOfStream)
            {
                string str = reader.ReadLine().ToString();
                allEmails.Add(str);
            }
            reader.Close(); // ОБЯЗАТЕЛЬНОЕ ЗАКРЫТИЕ ФАЙЛА

            string newStr = textBox2.Text + "###" + textBox3.Text + "###" + richTextBox1.Text;
            allEmails.Add(newStr);

            FileStream file = new FileStream(FileName, FileMode.Create); //создаем файловый поток
            StreamWriter writer = new StreamWriter(file); //создаем «потоковый писатель» и связываем его с файловым потоком
            for (int i = 0; i < allEmails.Count; i++)
            {
                writer.WriteLine(allEmails[i]);
            }
            writer.Close();
            if (draft)
            {
                list.RemoveAt(IndexRow);
                rewriteDBEmails("Черновики.txt");
            }
            MessageBox.Show("Ваше письмо отправлено", "Напоминание");
            Close();
        }

        /// <summary>
        /// перезапись БД
        /// </summary>
        /// <param name="FileName"></param>
        private void rewriteDBEmails(String FileName)
        {
            FileStream file = new FileStream(FileName, FileMode.Create); //создаем файловый поток
            StreamWriter writer = new StreamWriter(file); //создаем «потоковый писатель» и связываем его с файловым потоком
            for (int i = 0; i < list.Count; i++)
            {
                Tuple<String, String, String> emailsDB_table = (Tuple<String, String, String>)list[i];
                String whom = emailsDB_table.Item1;
                String themes = emailsDB_table.Item2;
                String emails = emailsDB_table.Item3;
                string newStr = whom + "###" + themes + "###" + emails;
                writer.WriteLine(newStr);
            }
            writer.Close();

            //=====================================================================================

            if (this.Owner is Form1 owner)
            {
                owner.dataGridViewRowsClear();
                owner.openDraft();
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
