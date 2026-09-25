using System;
using System.Collections.Generic;

namespace CalculatorApp
{
    // ==========================================================
    // CLASS TERPISAH UNTUK LOGIKA PERHITUNGAN (bukan UI)
    // Form1 hanya perlu MEMANGGIL method di sini, tidak perlu
    // tahu bagaimana perhitungan dilakukan (pemisahan logika/UI)
    // ==========================================================
    public class CalculatorService
    {
        // Menyimpan riwayat perhitungan, terbaru di posisi paling atas
        public List<string> Riwayat { get; private set; } = new List<string>();

        // ---------- Operasi dasar ----------
        public double Tambah(double a, double b) => a + b;
        public double Kurang(double a, double b) => a - b;
        public double Kali(double a, double b) => a * b;

        public double Bagi(double a, double b)
        {
            if (b == 0)
                throw new DivideByZeroException("Tidak bisa membagi dengan nol.");
            return a / b;
        }

        // Menghitung operasi biner berdasarkan simbol operator (+, −, ×, ÷)
        public double Hitung(double a, double b, string operatorSimbol)
        {
            switch (operatorSimbol)
            {
                case "+": return Tambah(a, b);
                case "−": return Kurang(a, b);
                case "×": return Kali(a, b);
                case "÷": return Bagi(a, b);
                default: throw new InvalidOperationException("Operator tidak dikenali.");
            }
        }

        // ---------- Operasi satu-angka (unary) ----------
        public double Negasi(double a) => -a;                 // tombol ±
        public double Persen(double a) => a / 100.0;           // tombol %
        public double Kuadrat(double a) => a * a;              // x²

        public double AkarKuadrat(double a)
        {
            if (a < 0)
                throw new ArgumentException("Tidak bisa akar dari angka negatif.");
            return Math.Sqrt(a);
        }

        public double Log(double a)
        {
            if (a <= 0)
                throw new ArgumentException("Tidak bisa log dari angka nol atau negatif.");
            return Math.Log10(a);
        }

        // Fungsi trigonometri menerima input dalam DERAJAT (lebih familiar dari radian)
        public double Sin(double derajat) => Math.Sin(derajat * Math.PI / 180);
        public double Cos(double derajat) => Math.Cos(derajat * Math.PI / 180);
        public double Tan(double derajat) => Math.Tan(derajat * Math.PI / 180);

        // ---------- Riwayat ----------
        public void TambahRiwayat(string entri)
        {
            Riwayat.Insert(0, entri);
        }

        public void HapusRiwayat()
        {
            Riwayat.Clear();
        }
    }
}
