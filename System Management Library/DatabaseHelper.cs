using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

public class DatabaseHelper
{
    private string SQLConnationString;

    public DatabaseHelper()
    {
        // تحديد المسار النسبي للقاعدة بيانات داخل مجلد Database
        string databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "Library.db");

        // تأكد من أن المجلد موجود، وإذا لم يكن موجودًا، أنشئه
        string directory = Path.GetDirectoryName(databasePath);
        if (!Directory.Exists(directory))   
        {
            Directory.CreateDirectory(directory);
        }

        // تأكد من أن الملف لا يوجد، إذا لم يكن موجودًا، أنشئه
        if (!File.Exists(databasePath))
        {
            SQLiteConnection.CreateFile(databasePath);
        }

        // إعداد الاتصال بقاعدة البيانات
        SQLConnationString = $"Data Source={databasePath};Version=3;";
    }

    // Method to Test Connection
    public bool TestConnection()
    {
        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(SQLConnationString))
            {
                connection.Open();
                MessageBox.Show("تم الاتصال بقاعدة البيانات بنجاح");
                return true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"فشل الاتصال بقاعدة البيانات: {ex.Message}");
            return false;
        }
    }

    // Method to Execute Non-Query (INSERT, UPDATE, DELETE)
    public void ExecuteNonQuery(string query)
    {
        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(SQLConnationString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"خطأ في تنفيذ الاستعلام: {ex.Message}");
        }
    }

    // General Method to Fetch Data (SELECT) from any table based on the query
    public DataTable GetData(string query)
    {
        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(SQLConnationString))
            {
                connection.Open();
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    return table;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"خطأ في الحصول على البيانات: {ex.Message}");
            return null;
        }
    }

    // Method to Create All Tables if not exists
    public void CreateTablesIfNotExists()
    {
        string createBooksTableQuery = @"
            CREATE TABLE IF NOT EXISTS Books (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                BookName TEXT NOT NULL,
                AvailableCopies INTEGER NOT NULL,
                Author TEXT NOT NULL,
                Price REAL NOT NULL
            );";

        string createCustomersTableQuery = @"
            CREATE TABLE IF NOT EXISTS Customers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CustomerName TEXT NOT NULL,
                Email TEXT NOT NULL,
                Phone TEXT NOT NULL
            );";

        string createBorrowingsTableQuery = @"
            CREATE TABLE IF NOT EXISTS Borrowings (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CustomerName TEXT NOT NULL,
                Email TEXT NOT NULL,
                Phone TEXT NOT NULL,
                BookName TEXT NOT NULL,
                BorrowedCopies INTEGER NOT NULL,
                BorrowDate TEXT NOT NULL,
                ReturnDate TEXT,
                Price REAL NOT NULL,
                FOREIGN KEY (BookName) REFERENCES Books (BookName)
            );";

        // تنفيذ الاستعلامات لإنشاء الجداول إذا كانت غير موجودة
        ExecuteNonQuery(createBooksTableQuery);
        ExecuteNonQuery(createCustomersTableQuery);
        ExecuteNonQuery(createBorrowingsTableQuery);


    }

    // Example method to add data to the database
    // Method to Add Book to Books Table
    public void AddBook(string bookName, int availableCopies, string author, double price)
    {
        string query = $"INSERT INTO Books (BookName, AvailableCopies, Author, Price) VALUES ('{bookName}', {availableCopies}, '{author}', {price});";
        ExecuteNonQuery(query);
        MessageBox.Show($"تم إضافة الكتاب '{bookName}' للمؤلف '{author}' إلى قاعدة البيانات!");
    }

    // Method to Add Customer to Customers Table
    public void AddCustomer(string customerName, string email, string phone)
    {
        string query = $"INSERT INTO Customers (CustomerName, Email, Phone) VALUES ('{customerName}', '{email}', '{phone}');";
        ExecuteNonQuery(query);
        MessageBox.Show($"تم إضافة العميل '{customerName}' إلى قاعدة البيانات!");
    }

    // Method to Update Book Data
    public void UpdateBook(int bookId, string bookName, string author, double? price, int? availableCopies)
    {
        // إعداد قائمة الحقول التي سيتم تحديثها
        List<string> updateFields = new List<string>();

        if (!string.IsNullOrWhiteSpace(bookName))
        {
            updateFields.Add($"BookName = '{bookName}'");
        }
        if (!string.IsNullOrWhiteSpace(author))
        {
            updateFields.Add($"Author = '{author}'");
        }
        if (price.HasValue)
        {
            updateFields.Add($"Price = {price.Value}");
        }
        if (availableCopies.HasValue)
        {
            updateFields.Add($"AvailableCopies = {availableCopies.Value}");
        }

        // إذا لم يتم إدخال أي بيانات، إعطاء تنبيه
        if (updateFields.Count == 0)
        {
            throw new ArgumentException("يرجى إدخال بيانات لتحديث الكتاب!");
        }

        // إنشاء استعلام التحديث
        string updateQuery = $"UPDATE Books SET {string.Join(", ", updateFields)} WHERE Id = {bookId};";

        // تنفيذ الاستعلام
        ExecuteNonQuery(updateQuery);
        MessageBox.Show("تم تحديث بيانات الكتاب بنجاح!");
    }

    // دالة لإجراء استعارة كتاب
    public void BorrowBook(string customerName, string email, string phone, string bookName, DateTime borrowDate, DateTime? returnDate)
    {
        // تحقق من توفر عدد كافٍ من النسخ والسعر
        string checkAvailabilityAndPriceQuery = $"SELECT AvailableCopies, Price FROM Books WHERE BookName = '{bookName}';";
        DataTable result = GetData(checkAvailabilityAndPriceQuery);

        if (result.Rows.Count == 0)
        {
            MessageBox.Show("هذا الكتاب غير موجود في المكتبة.");
            return;
        }

        int availableCopies = Convert.ToInt32(result.Rows[0]["AvailableCopies"]);
        double price = Convert.ToDouble(result.Rows[0]["Price"]); // جلب السعر من قاعدة البيانات

        if (availableCopies <= 0)
        {
            MessageBox.Show("هذا الكتاب غير متوفر للاستعارة.");
            return;
        }

        // تحديث عدد النسخ المتوفرة: استعار نسخة واحدة فقط
        string updateCopiesQuery = $"UPDATE Books SET AvailableCopies = AvailableCopies - 1 WHERE BookName = '{bookName}';";
        ExecuteNonQuery(updateCopiesQuery);

        // إضافة عملية الاستعارة إلى جدول Borrowings
        string borrowDataQuery = $@"
        INSERT INTO Borrowings (CustomerName, Email, Phone, BookName, BorrowedCopies, BorrowDate, ReturnDate, Price)
        VALUES ('{customerName}', '{email}', '{phone}', '{bookName}', 1, '{borrowDate:yyyy-MM-dd}', '{returnDate?.ToString("yyyy-MM-dd")}', {price});";

        ExecuteNonQuery(borrowDataQuery);

        MessageBox.Show("تم تسجيل استعارة الكتاب بنجاح.");
    }



    // دالة لاسترجاع بيانات الاستعارة
    public DataTable GetBorrowings()
    {
        string query = "SELECT * FROM Borrowings;";
        return GetData(query);
    }


}
