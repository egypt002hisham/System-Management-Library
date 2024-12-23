using System;
using System.Data;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace System_Management_Library
{
    public partial class BookControlForm : Form
    {
        private DatabaseHelper databaseHelper;

        public BookControlForm()
        {
            InitializeComponent();
            databaseHelper = new DatabaseHelper(); // تأكد من تهيئة DatabaseHelper هنا
            panel2.Visible = true;
            panel2.Enabled = true;
            panel4.Visible = false;
            panel4.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!panel2.Visible)
            {
                panel2.Visible=true;
                panel2.Enabled = true;
                panel4.Visible=false;
                panel4.Enabled=false;
                return;
            }
            string TitleBook = TitleBook1.Text;
            string Author = AuthorText1.Text;
            double Price = (double)PriceUpDown1.Value;
            int AvailableCopies = (int)AvailableCopiesUpDown1.Value;

            // التحقق من أن الحقول ليست فارغة
            if (string.IsNullOrWhiteSpace(TitleBook) || string.IsNullOrWhiteSpace(Author) || Price <= 0 || AvailableCopies <= 0)
            {
                MessageBox.Show("يرجى ملء جميع الحقول وإدخال قيم صحيحة قبل إضافة الكتاب.");
                return;
            }

            try
            {
                // استدعاء الدالة لإضافة الكتاب
                databaseHelper.AddBook(TitleBook, AvailableCopies, Author, Price);

                // تحديث DataGridView بعد الإضافة (يمكنك إضافة كود لتحميل البيانات هنا)
                //string query = "SELECT * FROM Books";
                //DataTable data = databaseHelper.GetData(query);
                //dataGridView1.DataSource = data;

                // إفراغ الحقول بعد الإضافة
                TitleBook1.Text = "";
                AuthorText1.Text = "";
                PriceUpDown1.Value = 0;
                AvailableCopiesUpDown1.Value = 0;

                MessageBox.Show("تمت إضافة الكتاب بنجاح!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show($"حدث خطأ أثناء إضافة الكتاب: {ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // الحصول على البيانات من الحقول
                int bookId = (int)numericUpDown5.Value;
                string newBookName = textBox4.Text;
                string newAuthor = textBox3.Text;
                double? newPrice = (double)numericUpDown3.Value;
                int? newAvailableCopies = (int)numericUpDown4.Value;

                // التحقق من صحة الكتاب ID
                if (bookId <= 0)
                {
                    MessageBox.Show("يرجى إدخال ID صحيح للكتاب!");
                    return;
                }
                if (newPrice <= 0)
                {
                    MessageBox.Show("يرجى إدخال سعر صحيح للكتاب!");
                    return;
                }
                if (newAvailableCopies <= 0)
                {
                    MessageBox.Show("يرجى إدخال عدد النسخ المتاحة للكتاب!");
                    return;
                }

                // تحديث بيانات الكتاب
                databaseHelper.UpdateBook(bookId, newBookName, newAuthor, newPrice, newAvailableCopies);

                // تحديث DataGridView بعد التعديل

                // إفراغ الحقول بعد التحديث
                numericUpDown5.Value = 0;
                textBox4.Text = "";
                textBox3.Text = "";
                numericUpDown3.Value = 0;
                numericUpDown4.Value = 0;

                MessageBox.Show("تم تعديل بيانات الكتاب بنجاح!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء تعديل البيانات: {ex.Message}");
            }
        }



        private void button3_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel2.Enabled = false;
            panel4.Visible = true;
            panel4.Enabled = true;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            // جمع البيانات من واجهة المستخدم
            string NameCustmer = textBox1.Text;
            string NameBook = textBox6.Text;
            string Email = textBox2.Text;
            string Phone = textBox5.Text;
            DateTime borrowDate = dateTimePicker1.Value;

            // التحقق من صحة البريد الإلكتروني
            if (!Email.Contains("@gmail.com"))
            {
                MessageBox.Show("يجب أن يحتوي البريد الإلكتروني على @gmail.com.");
                return;
            }

            // استدعاء الدالة BorrowBook
            try
            {
                // تحديد تاريخ الإرجاع كقيمة اختيارية
                DateTime? returnDate = null; // يمكن تعديلها لاحقًا حسب الحاجة

                databaseHelper.BorrowBook(NameCustmer, Email, Phone, NameBook, borrowDate, returnDate);

                // رسالة تأكيد بعد النجاح
                MessageBox.Show("تم تسجيل البيانات بنجاح!");
            }
            catch (Exception ex)
            {
                // معالجة الأخطاء وعرضها للمستخدم
                MessageBox.Show($"حدث خطأ: {ex.Message}");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form1 newForm = new Form1();
            newForm.Show();
            this.Hide();
        }
    }
}
