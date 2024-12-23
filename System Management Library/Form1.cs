using System;
using System.Data;
using System.Windows.Forms;

namespace System_Management_Library
{
    public partial class Form1 : Form
    {
        public static DatabaseHelper databaseHelper = new DatabaseHelper();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            databaseHelper.CreateTablesIfNotExists();
            // تحميل البيانات عند فتح النموذج
            LoadDataToGrid();
        }

        // دالة لتحميل البيانات وعرضها في DataGridView
        private void LoadDataToGrid(string query = null)
        {
            try
            {
                if (query == null)
                {
                    // كتابة استعلام لجلب البيانات
                    string queryBooks = "SELECT * FROM Books";

                    // استدعاء GetData لجلب البيانات كـ DataTable
                    DataTable data = databaseHelper.GetData(queryBooks);

                    // ربط البيانات بـ DataGridView
                    dataGridView1.DataSource = data;

                    // تحسين مظهر الشبكة
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
                else
                {

                    // استدعاء GetData لجلب البيانات كـ DataTable
                    DataTable data = databaseHelper.GetData(query);

                    // ربط البيانات بـ DataGridView
                    dataGridView1.DataSource = data;

                    // تحسين مظهر الشبكة
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل البيانات: {ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // اختبار الاتصال بقاعدة البيانات
            bool IsConnection = databaseHelper.TestConnection();
        }


        private void button5_Click(object sender, EventArgs e)
        {
            LoadDataToGrid("SELECT * FROM Customers");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadDataToGrid();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form2 newForm = new Form2();
            newForm.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BookControlForm newForm2 = new BookControlForm();
            newForm2.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            LoadDataToGrid("SELECT * FROM Borrowings;");
            
        }

    }
}
