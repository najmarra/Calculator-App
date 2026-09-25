# Laporan Praktikum: Kalkulator Desktop dengan C# Windows Forms

Laporan ini mendokumentasikan hasil pengerjaan Hand-on pembuatan aplikasi **Kalkulator Desktop** menggunakan C# Windows Forms App pada mata kuliah NET Programming dengan C#.

---

## Fitur Aplikasi

Aplikasi ini merupakan pengembangan dari kalkulator dasar pada modul lab, dengan tambahan fitur sesuai tantangan pengembangan yang diberikan:

1. **Operasi dasar**: penjumlahan, pengurangan, perkalian, pembagian
2. **Tombol ± (plus-minus)**: mengubah tanda angka menjadi positif/negatif
3. **Tombol %**: mengonversi angka menjadi bentuk persen (dibagi 100)
4. **Tombol Backspace (←)**: menghapus satu karakter terakhir yang diinput
5. **Riwayat perhitungan**: menampilkan histori seluruh perhitungan yang pernah dilakukan dalam satu sesi
6. **Scientific Calculator**: penambahan fungsi akar kuadrat (√), kuadrat (x²), sin, cos, tan, dan log
7. **Validasi input**: mencegah error saat pembagian dengan nol dan input yang tidak valid

---

## Struktur Project

```
CalculatorApp
│
├── Program.cs
├── Form1.cs                 # Menangani tampilan (UI) dan event tombol
├── Form1.Designer.cs        # Kode hasil generate dari Visual Designer
├── Form1.resx
└── CalculatorService.cs     # Menangani seluruh logika perhitungan
```

Struktur ini sengaja memisahkan antara **logika perhitungan** (`CalculatorService.cs`) dan **tampilan/interaksi pengguna** (`Form1.cs`), sesuai prinsip *separation of concerns* — supaya kode lebih rapi, mudah diuji, dan mudah dikembangkan tanpa saling mengganggu antara logika dan tampilan.

---

## Penjelasan Kode

### 1. Class `CalculatorService`

```csharp
public class CalculatorService
{
    public List<string> Riwayat { get; private set; } = new List<string>();

    public double Tambah(double a, double b) => a + b;
    public double Kurang(double a, double b) => a - b;
    public double Kali(double a, double b) => a * b;

    public double Bagi(double a, double b)
    {
        if (b == 0)
            throw new DivideByZeroException("Tidak bisa membagi dengan nol.");
        return a / b;
    }
    // ...
}
```

Class ini berisi seluruh method perhitungan (aritmatika dasar maupun scientific) dan daftar riwayat. Form1 tidak perlu tahu *bagaimana* perhitungan dilakukan — cukup memanggil method yang sesuai, misalnya `kalkulator.Hitung(firstNumber, secondNumber, operation)`.

### 2. Variabel Utama di `Form1.cs`

```csharp
double firstNumber = 0;
double secondNumber = 0;
double result = 0;
string operation = "";

string currentInput = "0";
string fullExpression = "";
```

- `firstNumber`, `secondNumber`, `result`: menyimpan nilai-nilai yang dipakai dalam perhitungan.
- `operation`: menyimpan operator yang dipilih (`+`, `−`, `×`, `÷`).
- `currentInput`: menyimpan angka yang sedang diketik pengguna saat ini.
- `fullExpression`: menyimpan bagian ekspresi yang sudah "terkunci" (misalnya `"20 + "`), sehingga layar bisa menampilkan seluruh proses perhitungan, bukan cuma angka terakhir.

### 3. Satu Event Handler untuk Semua Tombol Angka

```csharp
private void NumberButton_Click(object sender, EventArgs e)
{
    Button button = (Button)sender;

    if (currentInput == "0")
        currentInput = button.Text;
    else
        currentInput += button.Text;

    txtDisplay.Text = fullExpression + currentInput;
}
```

Semua tombol angka (0-9) memanggil method yang sama ini. Dengan mengambil `sender` dan meng-cast-nya menjadi `Button`, program bisa tahu **tombol mana** yang sedang diklik lewat `button.Text`, sehingga tidak perlu menulis 10 method terpisah untuk 10 tombol angka.

### 4. Perhitungan Saat Tombol `=` Ditekan

```csharp
private void btnEquals_Click(object sender, EventArgs e)
{
    try
    {
        secondNumber = double.Parse(currentInput.Replace(",", "."), CultureInfo.InvariantCulture);
        result = kalkulator.Hitung(firstNumber, secondNumber, operation);

        string entriLengkap = fullExpression + currentInput + " = " + result.ToString(CultureInfo.InvariantCulture).Replace(".", ",");
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
```

Bagian ini menangani parsing angka dari teks ke `double`, memanggil `CalculatorService` untuk menghitung, menampilkan hasil, sekaligus menyimpannya ke riwayat. Penggunaan `CultureInfo.InvariantCulture` dan `Replace(",", ".")` memastikan angka desimal dengan tanda koma tetap terhitung dengan benar, tidak peduli pengaturan regional komputer yang digunakan.

### 5. Fitur Tambahan: Backspace, ±, dan %

```csharp
private void btnBackspace_Click(object sender, EventArgs e)
{
    if (currentInput.Length > 1)
        currentInput = currentInput.Substring(0, currentInput.Length - 1);
    else
        currentInput = "0";

    txtDisplay.Text = fullExpression + currentInput;
}
```

Backspace menghapus karakter terakhir dari `currentInput` menggunakan `Substring`. Tombol ± dan % memanggil method `Negasi()` dan `Persen()` dari `CalculatorService` untuk mengubah nilai angka yang sedang ditampilkan.

### 6. Fitur Scientific Calculator

```csharp
private void HitungUnary(Func<double, double> operasi, string namaFungsi)
{
    try
    {
        double angka = double.Parse(currentInput.Replace(",", "."), CultureInfo.InvariantCulture);
        double hasil = operasi(angka);
        // ... tampilkan hasil dan simpan ke riwayat
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message, "Error");
    }
}
```

Keenam tombol scientific (`√`, `x²`, `sin`, `cos`, `tan`, `log`) menggunakan satu method bantu `HitungUnary`, yang menerima *delegate* `Func<double, double>` sebagai parameter. Dengan cara ini, kode tidak perlu ditulis berulang untuk setiap fungsi matematika — cukup dikirim method yang berbeda-beda (misalnya `kalkulator.Sin`, `kalkulator.Log`, dst).

### 7. Menampilkan Riwayat

```csharp
private void RefreshRiwayat()
{
    lstHistory.Items.Clear();
    foreach (string entri in kalkulator.Riwayat)
    {
        lstHistory.Items.Add(entri);
    }
}
```

Setiap kali ada perhitungan baru, riwayatnya disimpan ke `List<string>` di `CalculatorService`, lalu ditampilkan ulang ke `ListBox` (`lstHistory`) di form.

---

## Hasil Program

**1. Tampilan Utama Kalkulator**

<img width="427" height="311" alt="image" src="https://github.com/user-attachments/assets/41fddebc-80ad-4afc-afab-764fe689926f" />

**2. Operasi Aritmatika Dasar (contoh: 10 + 20)**

<img width="432" height="316" alt="image" src="https://github.com/user-attachments/assets/dbf00e1a-87b0-4f8b-ae5d-9057ff70424d" />

**3. Riwayat Perhitungan**

<img width="430" height="318" alt="image" src="https://github.com/user-attachments/assets/084bec4c-23fc-46a0-bebe-81d60f7aa194" />

**4. Fitur Scientific Calculator (√, x², sin, cos, tan, log)**

<img width="430" height="311" alt="image" src="https://github.com/user-attachments/assets/caa77cef-91b3-42bd-b40c-003274d3b02f" />

**5. Penanganan Error (Pembagian dengan Nol)**

<img width="697" height="316" alt="image" src="https://github.com/user-attachments/assets/7cbddbe1-e60a-4053-ac84-30ff2635fd1c" />

---

## Refleksi

### Apa fungsi object `sender` pada event handler?

`sender` adalah parameter yang secara otomatis dikirim oleh .NET setiap kali sebuah event (misalnya klik tombol) terjadi. Parameter ini berisi referensi ke **objek/kontrol yang memicu event tersebut** — dalam kasus ini, tombol mana yang sedang diklik. Dengan meng-cast `sender` menjadi `Button` (`Button button = (Button)sender;`), kode dapat mengetahui properti tombol tersebut, seperti `button.Text`, tanpa harus menuliskan logika terpisah untuk setiap tombol.

### Mengapa semua tombol angka dapat memakai satu `NumberButton_Click`?

Karena logika yang dijalankan untuk semua tombol angka pada dasarnya **sama persis** — hanya berbeda pada angka yang ditambahkan ke tampilan. Dengan memanfaatkan parameter `sender` untuk mengetahui tombol mana yang diklik (lewat `button.Text`), satu method saja sudah cukup untuk menangani tombol 0 sampai 9. Ini membuat kode jauh lebih ringkas dibandingkan menulis 10 method terpisah dengan isi yang hampir identik.

### Apa perbedaan `firstNumber`, `secondNumber`, dan `result`?

- **`firstNumber`**: menyimpan angka pertama yang diinput pengguna, yaitu sebelum tombol operator (+, −, ×, ÷) ditekan.
- **`secondNumber`**: menyimpan angka kedua yang diinput pengguna, yaitu setelah operator dipilih dan sebelum tombol `=` ditekan.
- **`result`**: menyimpan hasil akhir dari operasi antara `firstNumber` dan `secondNumber` setelah dihitung menggunakan operator yang dipilih.

Ketiganya diperlukan secara terpisah karena kalkulator perlu "mengingat" nilai-nilai ini di antara beberapa kali klik tombol yang berbeda, sebelum akhirnya digabung menjadi satu hasil perhitungan.

### Mengapa pembagian dengan nol perlu divalidasi?

Secara matematis, hasil pembagian dengan nol tidak terdefinisi. Dalam pemrograman, jika operasi ini dilakukan tanpa validasi, program bisa menghasilkan nilai `Infinity`, `NaN` (Not a Number), atau melempar exception yang tidak tertangani dan menyebabkan aplikasi **crash**. Dengan memvalidasi terlebih dahulu (`if (secondNumber == 0) throw new DivideByZeroException(...)`), program dapat menampilkan pesan error yang jelas dan tetap berjalan dengan normal, alih-alih berhenti mendadak.

### Bagaimana `try-catch` membantu menjaga aplikasi tetap stabil?

Blok `try-catch` memungkinkan program untuk **menangkap error yang terjadi saat runtime** (misalnya pembagian dengan nol, atau input yang gagal di-parse menjadi angka) tanpa menghentikan keseluruhan aplikasi. Kode di dalam blok `try` dijalankan seperti biasa; jika terjadi exception, eksekusi langsung berpindah ke blok `catch` yang menangani error tersebut (dalam kasus ini, menampilkan `MessageBox` berisi pesan error). Dengan begitu, aplikasi tetap responsif dan bisa terus digunakan meskipun terjadi kesalahan input, alih-alih tiba-tiba tertutup paksa.

---

## Kesimpulan

Melalui lab ini, dapat dipahami bagaimana membangun aplikasi desktop berbasis GUI dengan C# Windows Forms, mulai dari mendesain tampilan menggunakan Visual Designer, menangani event dari berbagai kontrol dengan konsep `sender` dan event handler, menerapkan validasi input serta penanganan error dengan `try-catch`, hingga menerapkan prinsip pemisahan tanggung jawab (*separation of concerns*) dengan memisahkan logika perhitungan ke dalam class tersendiri (`CalculatorService`). Pengembangan fitur tambahan seperti ±, %, backspace, riwayat, dan fungsi scientific juga memberikan pemahaman lebih dalam mengenai bagaimana satu event handler dapat digunakan kembali (*reusable*) untuk berbagai kontrol yang memiliki logika serupa.
