using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace System_Management_Library
{
    public partial class Form2 : Form
    {
        public static DatabaseHelper DatabaseHelper = new DatabaseHelper();
        public Form2()
        {
            InitializeComponent();
        }



        private void Form2_Load(object sender, EventArgs e)
        {
            // كتابة استعلام لجلب البيانات
            string queryBooks = "SELECT * FROM Customers";

            // استدعاء GetData لجلب البيانات كـ DataTable
            DataTable data = DatabaseHelper.GetData(queryBooks);

            // ربط البيانات بـ DataGridView
            //this.dataGridView1.DataSource = data;

            //// تحسين مظهر الشبكة
            //this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 newForm = new Form1();
            newForm.Show();
            this.Hide(); 
        }




        private void buttonAddDataBase_Click(object sender, EventArgs e)
        {

            // الحصول على القيم من حقول الإدخال
            string customerName = textBoxCustomerName.Text;
            string email = textBoxEmail.Text;
            string phone = textBoxPhone.Text;

            // التحقق من أن الحقول ليست فارغة
            if (string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone))
            {
                
                MessageBox.Show("يرجى ملء جميع الحقول قبل إضافة العميل.");
                return;
            }

            if (!email.Contains("@gmail.com"))
            {
                MessageBox.Show("يجب أن يحتوي البريد الإلكتروني على @gmail.com.");
                return;
            }

            try
            {
                // استدعاء الدالة لإضافة العميل
                DatabaseHelper.AddCustomer(customerName, email, phone);

                // تحديث DataGridView بعد الإضافة
                //string query = "SELECT * FROM Customers";
                //DataTable data = DatabaseHelper.GetData(query);
                //dataGridView1.DataSource = data;

                // إفراغ الحقول بعد الإضافة
                textBoxCustomerName.Text = "";
                textBoxEmail.Text = "";
                textBoxPhone.Text = "";

                MessageBox.Show("تمت إضافة العميل بنجاح!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء إضافة العميل: {ex.Message}");
            }

        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string email = textBoxEmail.Text;
                string phone = textBoxPhone.Text;
                // بناء استعلام الحذف بناءً على القيم المدخلة
                string query = "DELETE FROM Customers WHERE ";

                if (!string.IsNullOrEmpty(email))
                {
                    query += $"Email = '{email}'";
                }
                else if (!string.IsNullOrEmpty(phone))
                {
                    query += $"Phone = '{phone}'";
                }
                else
                {
                    MessageBox.Show("يرجى إدخال Email أو Phone لحذف العميل.");
                    return;
                }

                // استخدام ExecuteNonQuery لتنفيذ استعلام الحذف
                DatabaseHelper.ExecuteNonQuery(query);

                // إبلاغ المستخدم بالنجاح
                MessageBox.Show("تم حذف العميل بنجاح!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء الحذف: {ex.Message}");
            }

        }

        private void buttonedit_Click(object sender, EventArgs e)
        {
            EditCustomerForm editForm = new EditCustomerForm();
            editForm.ShowDialog();
        }
    }
}
