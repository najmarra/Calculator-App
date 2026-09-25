using System;
using System.Globalization;
using System.Windows.Forms;

namespace CalculatorApp
{
    public partial class Form1 : Form
    {
        // Logika perhitungan dipisah ke class CalculatorService
        private readonly CalculatorService kalkulator = new CalculatorService();

        double firstNumber = 0;
        double secondNumber = 0;
        double result = 0;
        string operation = "";

        // currentInput = angka yang sedang diketik
        // fullExpression = teks ekspresi yang sudah "terkunci", misal "20 + "
        string currentInput = "0";
        string fullExpression = "";

        public Form1()
        {
            InitializeComponent();
        }

        // ==========================================
        // TOMBOL ANGKA (0-9)
        // ==========================================
        private void NumberButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            if (currentInput == "0")
                currentInput = button.Text;
            else
                currentInput += button.Text;

            txtDisplay.Text = fullExpression + currentInput;
        }

        private void btn7_Click(object sender, EventArgs e) => NumberButton_Click(sender, e);
        private void btn4_Click(object sender, EventArgs e) => NumberButton_Click(sender, e);
        private void btn1_Click(object sender, EventArgs e) => NumberButton_Click(sender, e);
        private void btn8_Click(object sender, EventArgs e) => NumberButton_Click(sender, e);
        private void btn5_Click(object sender, EventArgs e) => NumberButton_Click(sender, e);
        private void btn2_Click(object sender, EventArgs e) => NumberButton_Click(sender, e);
        private void btn9_Click(object sender, EventArgs e) => NumberButton_Click(sender, e);
        private void btn6_Click(object sender, EventArgs e) => NumberButton_Click(sender, e);
        private void btn3_Click(object sender, EventArgs e) => NumberButton_Click(sender, e);
        private void btn0_Click(object sender, EventArgs e) => NumberButton_Click(sender, e);

        // ==========================================
        // TOMBOL OPERATOR (+, −, ×, ÷)
        // ==========================================
        private void OperatorButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            firstNumber = double.Parse(currentInput.Replace(",", "."), CultureInfo.InvariantCulture);
            operation = button.Text;

            fullExpression = currentInput + " " + operation + " ";
            currentInput = "0";

            txtDisplay.Text = fullExpression;
        }

        private void btnDivide_Click(object sender, EventArgs e) => OperatorButton_Click(sender, e);
        private void btnMultiply_Click(object sender, EventArgs e) => OperatorButton_Click(sender, e);
        private void btnMinus_Click(object sender, EventArgs e) => OperatorButton_Click(sender, e);
        private void btnPlus_Click(object sender, EventArgs e) => OperatorButton_Click(sender, e);

        // ==========================================
        // TOMBOL SAMA DENGAN (=)
        // ==========================================
        private void btnEquals_Click(object sender, EventArgs e)
        {
            try
            {
                secondNumber = double.Parse(currentInput.Replace(",", "."), CultureInfo.InvariantCulture);
                result = kalkulator.Hitung(firstNumber, secondNumber, operation);

                string entriLengkap = fullExpression + currentInput + " = " + result;
                txtDisplay.Text = entriLengkap;

                kalkulator.TambahRiwayat(entriLengkap);
                RefreshRiwayat();

                currentInput = result.ToString(CultureInfo.InvariantCulture).Replace(".", ",");
                fullExpression = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                currentInput = "0";
                fullExpression = "";
            }
        }

        // ==========================================
        // TOMBOL CLEAR (C)
        // ==========================================
        private void btnClear_Click(object sender, EventArgs e)
        {
            firstNumber = 0;
            secondNumber = 0;
            result = 0;
            operation = "";
            currentInput = "0";
            fullExpression = "";
            txtDisplay.Text = "0";
        }

        // ==========================================
        // TOMBOL DESIMAL (.)
        // ==========================================
        private void btnDecimal_Click(object sender, EventArgs e)
        {
            if (!currentInput.Contains(","))
            {
                currentInput += ",";
                txtDisplay.Text = fullExpression + currentInput;
            }
        }

        // ==========================================
        // TOMBOL ± (PLUS MINUS)
        // ==========================================
        private void btnPlusMinus_Click(object sender, EventArgs e)
        {
            if (double.TryParse(currentInput.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double angka))
            {
                currentInput = kalkulator.Negasi(angka).ToString(CultureInfo.InvariantCulture).Replace(".", ",");
                txtDisplay.Text = fullExpression + currentInput;
            }
        }

        // ==========================================
        // TOMBOL % (PERSEN)
        // ==========================================
        private void btnPercent_Click(object sender, EventArgs e)
        {
            if (double.TryParse(currentInput.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double angka))
            {
                currentInput = kalkulator.Persen(angka).ToString(CultureInfo.InvariantCulture).Replace(".", ",");
                txtDisplay.Text = fullExpression + currentInput;
            }
        }

        // ==========================================
        // TOMBOL BACKSPACE (HAPUS SATU KARAKTER TERAKHIR)
        // ==========================================
        private void btnBackspace_Click(object sender, EventArgs e)
        {
            if (currentInput.Length > 1)
                currentInput = currentInput.Substring(0, currentInput.Length - 1);
            else
                currentInput = "0";

            txtDisplay.Text = fullExpression + currentInput;
        }

        // ==========================================
        // TOMBOL SCIENTIFIC: √, x², sin, cos, tan, log
        // ==========================================
        private void btnSqrt_Click(object sender, EventArgs e) => HitungUnary(kalkulator.AkarKuadrat, "√");
        private void btnSquare_Click(object sender, EventArgs e) => HitungUnary(kalkulator.Kuadrat, "sqr");
        private void btnSin_Click(object sender, EventArgs e) => HitungUnary(kalkulator.Sin, "sin");
        private void btnCos_Click(object sender, EventArgs e) => HitungUnary(kalkulator.Cos, "cos");
        private void btnTan_Click(object sender, EventArgs e) => HitungUnary(kalkulator.Tan, "tan");
        private void btnLog_Click(object sender, EventArgs e) => HitungUnary(kalkulator.Log, "log");

        // Method bantu supaya ke-6 tombol scientific di atas tidak duplikasi kode
        private void HitungUnary(Func<double, double> operasi, string namaFungsi)
        {
            try
            {
                double angka = double.Parse(currentInput.Replace(",", "."), CultureInfo.InvariantCulture);
                double hasil = operasi(angka);

                string entriLengkap = $"{namaFungsi}({angka}) = {hasil}";
                txtDisplay.Text = hasil.ToString(CultureInfo.InvariantCulture).Replace(".", ",");

                kalkulator.TambahRiwayat(entriLengkap);
                RefreshRiwayat();

                currentInput = hasil.ToString(CultureInfo.InvariantCulture).Replace(".", ",");
                fullExpression = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        // ==========================================
        // RIWAYAT PERHITUNGAN
        // ==========================================
        private void RefreshRiwayat()
        {
            lstHistory.Items.Clear();
            foreach (string entri in kalkulator.Riwayat)
            {
                lstHistory.Items.Add(entri);
            }
        }
    }
}