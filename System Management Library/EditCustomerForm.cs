using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace System_Management_Library
{
    public partial class EditCustomerForm : Form
    {
        public EditCustomerForm()
        {
            InitializeComponent();
        }



        private void ChaGeDataButtom_Click(object sender, EventArgs e)
        {
            int customerId = (int)numericUpDown1.Value;

            if (customerId <= 0)
            {
                MessageBox.Show("يرجى إدخال ID صحيح للعميل!");
                return;
            }

            try
            {
                // جمع البيانات المدخلة من المستخدم
                string newName = textBoxCustomerName.Text;
                string newEmail = textBoxEmail.Text;
                string newPhone = textBoxPhone.Text;

                // التحقق إذا كانت جميع الحقول فارغة
                if (string.IsNullOrWhiteSpace(newName) && string.IsNullOrWhiteSpace(newEmail) && string.IsNullOrWhiteSpace(newPhone))
                {
                    MessageBox.Show("يرجى إدخال بيانات لتعديل العميل!");
                    return;
                }

                // إنشاء قائمة لتخزين التعديلات المطلوبة
                List<string> updateFields = new List<string>();

                if (!string.IsNullOrWhiteSpace(newName))
                {
                    updateFields.Add($"CustomerName = '{newName}'");
                }
                if (!string.IsNullOrWhiteSpace(newEmail))
                {
                    updateFields.Add($"Email = '{newEmail}'");
                }
                if (!string.IsNullOrWhiteSpace(newPhone))
                {
                    updateFields.Add($"Phone = '{newPhone}'");
                }

                // دمج الحقول المُعدلة في استعلام SQL
                string updateQuery = $"UPDATE Customers SET {string.Join(", ", updateFields)} WHERE Id = {customerId}";

                // تنفيذ استعلام التحديث
                Form2.DatabaseHelper.ExecuteNonQuery(updateQuery);

                MessageBox.Show("تم تعديل بيانات العميل بنجاح!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء تعديل البيانات: {ex.Message}");
            }

        }
    }
}
