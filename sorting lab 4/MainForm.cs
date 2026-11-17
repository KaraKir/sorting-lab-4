using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sorting_lab_4 // ВАЖНО: Убедитесь, что namespace совпадает с вашим проектом!
{
    public class MainForm : Form
    {
        // Поля для Задания 1
        private TextBox? txtInput1;
        private TextBox? txtLog1;
        private ComboBox? cmbAlgo;
        private NumericUpDown? numDelay;
        private Button? btnSort1;
        private bool isSorting = false;

        // Поля для Задания 2
        private TextBox? txtLog2;
        private ComboBox? cmbExtMethod;
        private ComboBox? cmbSortField;
        private string[]? currentDataLines;

        // Поля для Задания 3
        private RichTextBox? rtbTextLog;
        private DataGridView? dgvResults;
        private ComboBox? cmbTextAlgo1;
        private ComboBox? cmbTextAlgo2;
        private string[]? loadedWords = null;

        public MainForm()
        {
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Лабораторная работа: Алгоритмы сортировки";
            this.Size = new Size(1000, 800);

            TabControl tabControl = new TabControl { Dock = DockStyle.Fill };

            TabPage page1 = new TabPage("Задание 1. Внутренняя сортировка");
            TabPage page2 = new TabPage("Задание 2. Внешняя сортировка");
            TabPage page3 = new TabPage("Задание 3. Текст и эксперименты");

            tabControl.TabPages.Add(page1);
            tabControl.TabPages.Add(page2);
            tabControl.TabPages.Add(page3);

            SetupTask1(page1);
            SetupTask2(page2);
            SetupTask3(page3);

            this.Controls.Add(tabControl);
        }

        #region Задание 1: Внутренняя сортировка

        private void SetupTask1(TabPage page)
        {
            Label lblIn = new Label { Text = "Исходный массив (через запятую):", Location = new Point(10, 10), AutoSize = true };
            txtInput1 = new TextBox { Location = new Point(10, 30), Width = 400, Text = "5, 2, 9, 1, 5, 6, 3, 8, 4, 7" };

            Button btnGenArr = new Button { Text = "Случайный массив", Location = new Point(420, 28), Width = 140 };
            btnGenArr.Click += (s, e) => {
                Random rnd = new Random();
                txtInput1.Text = string.Join(", ", Enumerable.Range(0, 15).Select(_ => rnd.Next(1, 100)));
            };

            Label lblAlgo = new Label { Text = "Алгоритм:", Location = new Point(10, 60), AutoSize = true };
            cmbAlgo = new ComboBox { Location = new Point(10, 80), Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };

            // Оставляем старые и добавляем новые
            cmbAlgo.Items.AddRange(new string[] { 
                // Квадратичные / Субквадратичные
                "Квадратичная: Сортировка выбором (SelectSort)",
                "Квадратичная: Пузырьковая (BubbleSort)",
                "Квадратичная: Вставками (InsertSort)",
                "Субквадратичная: Шелла (ShellSort)",
                "Квадратичная: Шейкерная (CocktailSort)",
                // Усовершенствованные
                "Усовершенствованная: Быстрая (QuickSort)",
                "Усовершенствованная: Пирамидальная (HeapSort)",
                "Усовершенствованная: Слиянием (MergeSort)",
                "Усовершенствованная: Гномья (GnomeSort)"
            });
            cmbAlgo.SelectedIndex = 0;

            Label lblDelay = new Label { Text = "Задержка (мс):", Location = new Point(340, 60), AutoSize = true };
            numDelay = new NumericUpDown { Location = new Point(340, 80), Width = 60, Value = 50, Maximum = 1000 };

            btnSort1 = new Button { Text = "Сортировать", Location = new Point(410, 78), Width = 110 };
            btnSort1.Click += BtnSort1_Click;

            Button btnSaveLog1 = new Button { Text = "Сохранить лог в файл", Location = new Point(530, 78), Width = 150 };
            btnSaveLog1.Click += (s, e) => SaveLogToFile(txtLog1?.Text, "Log_Task1.txt");

            txtLog1 = new TextBox { Location = new Point(10, 110), Width = 950, Height = 600, Multiline = true, ScrollBars = ScrollBars.Vertical, ReadOnly = true, Font = new Font("Consolas", 10) };

            page.Controls.Add(lblIn); page.Controls.Add(txtInput1); page.Controls.Add(btnGenArr);
            page.Controls.Add(lblAlgo); page.Controls.Add(cmbAlgo);
            page.Controls.Add(lblDelay); page.Controls.Add(numDelay);
            page.Controls.Add(btnSort1); page.Controls.Add(btnSaveLog1); page.Controls.Add(txtLog1);
        }

        private async void BtnSort1_Click(object? sender, EventArgs e)
        {
            if (isSorting) return;

            Log1($"\r\n======================================================");
            Log1($"НОВЫЙ ЗАПУСК: {DateTime.Now:HH:mm:ss}");
            Log1($"======================================================");

            try
            {
                if (txtInput1 == null || cmbAlgo == null || numDelay == null || btnSort1 == null) return;

                int[] arr = txtInput1.Text.Split(',').Select(s => int.Parse(s.Trim())).ToArray();
                int delay = (int)numDelay.Value;
                string algo = cmbAlgo.SelectedItem?.ToString() ?? "";

                Log1($"Исходный массив: {string.Join(", ", arr)}");
                Log1($"Алгоритм: {algo}");
                Log1("--------------------------------------------------");

                long comparisons = 0;
                long swaps = 0;
                isSorting = true;
                btnSort1.Enabled = false;

                await Task.Run(() =>
                {
                    if (algo.Contains("SelectSort")) SelectSort(arr, ref comparisons, ref swaps, delay);
                    else if (algo.Contains("BubbleSort")) BubbleSort(arr, ref comparisons, ref swaps, delay);
                    else if (algo.Contains("InsertSort")) InsertSort(arr, ref comparisons, ref swaps, delay);
                    else if (algo.Contains("ShellSort")) ShellSort(arr, ref comparisons, ref swaps, delay);
                    else if (algo.Contains("CocktailSort")) CocktailSort(arr, ref comparisons, ref swaps, delay);
                    else if (algo.Contains("QuickSort")) QuickSort(arr, 0, arr.Length - 1, ref comparisons, ref swaps, delay);
                    else if (algo.Contains("HeapSort")) HeapSort(arr, ref comparisons, ref swaps, delay);
                    else if (algo.Contains("MergeSort")) MergeSort(arr, 0, arr.Length - 1, ref comparisons, ref swaps, delay);
                    else if (algo.Contains("GnomeSort")) GnomeSort(arr, ref comparisons, ref swaps, delay);
                });

                Log1("--------------------------------------------------");
                Log1($"Отсортированный массив: {string.Join(", ", arr)}");
                Log1($"Всего сравнений: {comparisons}");
                Log1($"Всего перестановок: {swaps}");
                Log1("--------------------------------------------------");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка ввода: " + ex.Message);
            }
            finally
            {
                isSorting = false;
                if (btnSort1 != null) btnSort1.Enabled = true;
            }
        }

        private void Log1(string msg)
        {
            if (txtLog1 == null) return;
            if (txtLog1.InvokeRequired)
                txtLog1.Invoke(new Action(() => txtLog1.AppendText(msg + Environment.NewLine)));
            else
                txtLog1.AppendText(msg + Environment.NewLine);
        }

        private void Swap(int[] a, int i, int j) { int t = a[i]; a[i] = a[j]; a[j] = t; }

        // --- Старые алгоритмы ---

        private void SelectSort(int[] a, ref long comp, ref long swaps, int delay)
        {
            for (int i = 0; i < a.Length - 1; i++)
            {
                int minIdx = i;
                for (int j = i + 1; j < a.Length; j++)
                {
                    comp++;
                    if (a[j] < a[minIdx]) minIdx = j;
                }
                if (i != minIdx)
                {
                    Swap(a, i, minIdx);
                    swaps++;
                    Log1($"[Шаг Select] {string.Join(", ", a)}");
                    Thread.Sleep(delay);
                }
            }
        }

        private void BubbleSort(int[] a, ref long comp, ref long swaps, int delay)
        {
            for (int i = 0; i < a.Length; i++)
            {
                for (int j = 0; j < a.Length - i - 1; j++)
                {
                    comp++;
                    if (a[j] > a[j + 1])
                    {
                        Swap(a, j, j + 1);
                        swaps++;
                        Log1($"[Шаг Bubble] {string.Join(", ", a)}");
                        Thread.Sleep(delay);
                    }
                }
            }
        }

        private void InsertSort(int[] a, ref long comp, ref long swaps, int delay)
        {
            for (int i = 1; i < a.Length; i++)
            {
                int key = a[i];
                int j = i - 1;
                while (j >= 0 && a[j] > key)
                {
                    comp++;
                    a[j + 1] = a[j];
                    j--;
                    swaps++;
                    Log1($"[Шаг Insert] {string.Join(", ", a)}");
                    Thread.Sleep(delay);
                }
                a[j + 1] = key;
            }
        }

        private void ShellSort(int[] a, ref long comp, ref long swaps, int delay)
        {
            int n = a.Length;
            for (int gap = n / 2; gap > 0; gap /= 2)
            {
                for (int i = gap; i < n; i += 1)
                {
                    int temp = a[i];
                    int j;
                    for (j = i; j >= gap && a[j - gap] > temp; j -= gap)
                    {
                        comp++;
                        a[j] = a[j - gap];
                        swaps++;
                    }
                    a[j] = temp;
                    Log1($"[Шаг Shell] Gap={gap}: {string.Join(", ", a)}");
                    Thread.Sleep(delay);
                }
            }
        }

        private void QuickSort(int[] a, int low, int high, ref long comp, ref long swaps, int delay)
        {
            if (low < high)
            {
                int pi = Partition(a, low, high, delay, ref comp, ref swaps);
                QuickSort(a, low, pi - 1, ref comp, ref swaps, delay);
                QuickSort(a, pi + 1, high, ref comp, ref swaps, delay);
            }
        }

        private int Partition(int[] a, int low, int high, int delay, ref long comp, ref long swaps)
        {
            int pivot = a[high];
            int i = (low - 1);

            for (int j = low; j < high; j++)
            {
                comp++;
                if (a[j] < pivot)
                {
                    i++;
                    Swap(a, i, j);
                    swaps++;
                    Log1($"[Шаг QS] {string.Join(", ", a)}");
                    Thread.Sleep(delay);
                }
            }
            Swap(a, i + 1, high);
            swaps++;
            return i + 1;
        }

        private void HeapSort(int[] a, ref long comp, ref long swaps, int delay)
        {
            int n = a.Length;
            for (int i = n / 2 - 1; i >= 0; i--)
                Heapify(a, n, i, delay, ref comp, ref swaps);

            for (int i = n - 1; i > 0; i--)
            {
                Swap(a, 0, i);
                swaps++;
                Log1($"[Шаг Heap] {string.Join(", ", a)}");
                Thread.Sleep(delay);
                Heapify(a, i, 0, delay, ref comp, ref swaps);
            }
        }

        private void Heapify(int[] a, int n, int i, int delay, ref long comp, ref long swaps)
        {
            int largest = i;
            int l = 2 * i + 1;
            int r = 2 * i + 2;

            if (l < n && a[l] > a[largest]) { largest = l; comp++; }
            if (r < n && a[r] > a[largest]) { largest = r; comp++; }

            if (largest != i)
            {
                Swap(a, i, largest);
                swaps++;
                Log1($"[Шаг Heapify] {string.Join(", ", a)}");
                Thread.Sleep(delay);
                Heapify(a, n, largest, delay, ref comp, ref swaps);
            }
        }

        // --- НОВЫЕ ДОБАВЛЕННЫЕ АЛГОРИТМЫ ---

        // Шейкерная сортировка (улучшенный пузырек)
        private void CocktailSort(int[] a, ref long comp, ref long swaps, int delay)
        {
            bool swapped = true;
            int start = 0;
            int end = a.Length;

            while (swapped)
            {
                swapped = false;
                for (int i = start; i < end - 1; ++i)
                {
                    comp++;
                    if (a[i] > a[i + 1])
                    {
                        Swap(a, i, i + 1);
                        swaps++;
                        Log1($"[Шаг Cocktail] {string.Join(", ", a)}");
                        Thread.Sleep(delay);
                        swapped = true;
                    }
                }
                if (!swapped) break;
                swapped = false;
                end--;
                for (int i = end - 1; i >= start; i--)
                {
                    comp++;
                    if (a[i] > a[i + 1])
                    {
                        Swap(a, i, i + 1);
                        swaps++;
                        Log1($"[Шаг Cocktail] {string.Join(", ", a)}");
                        Thread.Sleep(delay);
                        swapped = true;
                    }
                }
                start++;
            }
        }

        // Сортировка слиянием
        private void MergeSort(int[] a, int left, int right, ref long comp, ref long swaps, int delay)
        {
            if (left < right)
            {
                int mid = (left + right) / 2;
                MergeSort(a, left, mid, ref comp, ref swaps, delay);
                MergeSort(a, mid + 1, right, ref comp, ref swaps, delay);
                Merge(a, left, mid, right, ref comp, ref swaps, delay);
            }
        }

        private void Merge(int[] a, int left, int mid, int right, ref long comp, ref long swaps, int delay)
        {
            int[] temp = new int[a.Length];
            int i = left, j = mid + 1, k = left;

            while (i <= mid && j <= right)
            {
                comp++;
                if (a[i] <= a[j])
                {
                    temp[k++] = a[i++];
                }
                else
                {
                    temp[k++] = a[j++];
                }
            }

            while (i <= mid) temp[k++] = a[i++];
            while (j <= right) temp[k++] = a[j++];

            for (i = left; i <= right; i++)
            {
                a[i] = temp[i];
                swaps++;
            }
            Log1($"[Шаг Merge] {string.Join(", ", a)}");
            Thread.Sleep(delay);
        }

        // Гномья сортировка
        private void GnomeSort(int[] a, ref long comp, ref long swaps, int delay)
        {
            int index = 0;
            while (index < a.Length)
            {
                if (index == 0) index++;
                comp++;
                if (a[index] >= a[index - 1])
                {
                    index++;
                }
                else
                {
                    Swap(a, index, index - 1);
                    swaps++;
                    Log1($"[Шаг Gnome] {string.Join(", ", a)}");
                    Thread.Sleep(delay);
                    index--;
                }
            }
        }

        #endregion

        #region Задание 2: Внешняя сортировка

        private void SetupTask2(TabPage page)
        {
            Label lblInfo = new Label { Text = "Демонстрация внешней сортировки", Location = new Point(10, 10), AutoSize = true };

            Button btnGenData = new Button { Text = "Сгенерировать данные", Location = new Point(10, 40), Width = 180 };
            btnGenData.Click += (s, e) => GenerateExternalData();

            Button btnLoadFile = new Button { Text = "Загрузить из файла", Location = new Point(200, 40), Width = 150 };
            btnLoadFile.Click += BtnLoadFile_Click;

            Label lblMethod = new Label { Text = "Метод:", Location = new Point(10, 80), AutoSize = true };
            cmbExtMethod = new ComboBox { Location = new Point(10, 100), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbExtMethod.Items.AddRange(new string[] { "Прямое слияние", "Естественное слияние" });
            cmbExtMethod.SelectedIndex = 0;

            Label lblField = new Label { Text = "Сортировать по:", Location = new Point(200, 80), AutoSize = true };
            cmbSortField = new ComboBox { Location = new Point(200, 100), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSortField.Items.AddRange(new string[] { "Название", "Население" });
            cmbSortField.SelectedIndex = 0;

            Button btnSort = new Button { Text = "Выполнить сортировку", Location = new Point(360, 98), Width = 180 };
            btnSort.Click += BtnExternalSort_Click;

            Button btnSaveLog2 = new Button { Text = "Сохранить лог в файл", Location = new Point(550, 98), Width = 150 };
            btnSaveLog2.Click += (s, e) => SaveLogToFile(txtLog2?.Text, "Log_Task2.txt");

            txtLog2 = new TextBox { Location = new Point(10, 140), Width = 950, Height = 550, Multiline = true, ScrollBars = ScrollBars.Vertical, ReadOnly = true, Font = new Font("Consolas", 10) };

            page.Controls.Add(lblInfo);
            page.Controls.Add(btnGenData); page.Controls.Add(btnLoadFile);
            page.Controls.Add(lblMethod); page.Controls.Add(cmbExtMethod);
            page.Controls.Add(lblField); page.Controls.Add(cmbSortField);
            page.Controls.Add(btnSort); page.Controls.Add(btnSaveLog2); page.Controls.Add(txtLog2);
        }

        private void GenerateExternalData()
        {
            Random rnd = new Random();
            // Генерируем 50 уникальных стран с случайным населением
            currentDataLines = Enumerable.Range(1, 50).Select(i =>
            {
                string countryName = $"Страна {i}";
                long population = rnd.Next(1000000, 1500000000); // Случайное население от 1 млн до 1.5 млрд
                return $"{countryName};{population}";
            }).OrderBy(x => Guid.NewGuid()).ToArray(); // Перемешиваем, чтобы изначально они были не по порядку

            if (txtLog2 != null)
            {
                txtLog2.AppendText($"\r\n[{DateTime.Now:HH:mm:ss}] Данные сгенерированы (50 уникальных записей).\r\n");
                txtLog2.AppendText("До сортировки:\r\n");
                foreach (var line in currentDataLines.Take(50))
                {
                    txtLog2.AppendText(line + "\r\n");
                }
                txtLog2.AppendText("...\r\n");
            }
        }

        private void BtnLoadFile_Click(object? sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                currentDataLines = File.ReadAllLines(ofd.FileName);
                if (txtLog2 != null)
                    txtLog2.AppendText($"\r\n[{DateTime.Now:HH:mm:ss}] Файл загружен: {ofd.FileName}\r\n");
            }
        }

        private async void BtnExternalSort_Click(object? sender, EventArgs e)
        {
            if (currentDataLines == null || currentDataLines.Length == 0)
            {
                MessageBox.Show("Сначала сгенерируйте данные или загрузите файл.");
                return;
            }
            if (cmbExtMethod == null || cmbSortField == null) return;

            string method = cmbExtMethod.SelectedItem?.ToString() ?? "";
            string field = cmbSortField.SelectedItem?.ToString() ?? "";

            Log2($"\r\n======================================================");
            Log2($"ВНЕШНЯЯ СОРТИРОВКА: {DateTime.Now:HH:mm:ss}");
            Log2($"Метод: {method}, Поле: {field}");
            Log2("======================================================");

            List<string> sortedLines = new List<string>();

            await Task.Run(() =>
            {
                int chunkSize = 5;
                List<string[]> chunks = new List<string[]>();
                for (int i = 0; i < currentDataLines.Length; i += chunkSize)
                {
                    string[] chunk = currentDataLines.Skip(i).Take(chunkSize).ToArray();
                    Array.Sort(chunk, (x, y) => CompareLines(x, y, field));
                    chunks.Add(chunk);
                    Log2($"[Чанк {chunks.Count}] отсортирован.");
                }

                Log2("Начало слияния чанков...");
                sortedLines = MergeChunks(chunks, field);
            });

            Log2("--- РЕЗУЛЬТАТ ---");
            foreach (var line in sortedLines)
            {
                Log2(line);
            }

            // Сохранение результата в файл (как требует ТЗ)
            string resultPath = "result_sorted.txt";
            File.WriteAllLines(resultPath, sortedLines);
            Log2($"\r\nРезультат сохранен в файл: {resultPath}");
        }

        private int CompareLines(string x, string y, string field)
        {
            string[] partsX = x.Split(';');
            string[] partsY = y.Split(';');

            if (field == "Название")
            {
                return string.Compare(partsX[0], partsY[0], StringComparison.Ordinal);
            }
            else
            {
                if (long.TryParse(partsX[1], out long popX) && long.TryParse(partsY[1], out long popY))
                {
                    return popX.CompareTo(popY);
                }
                return 0;
            }
        }

        private List<string> MergeChunks(List<string[]> chunks, string field)
        {
            List<string> result = new List<string>();
            while (chunks.Any(c => c.Length > 0))
            {
                string[]? minChunk = null;
                int minChunkIdx = -1;

                for (int i = 0; i < chunks.Count; i++)
                {
                    if (chunks[i].Length > 0)
                    {
                        if (minChunk == null || CompareLines(chunks[i][0], minChunk[0], field) < 0)
                        {
                            minChunk = chunks[i];
                            minChunkIdx = i;
                        }
                    }
                }

                if (minChunk != null)
                {
                    result.Add(minChunk[0]);
                    chunks[minChunkIdx] = chunks[minChunkIdx].Skip(1).ToArray();
                }
            }
            return result;
        }

        private void Log2(string msg)
        {
            if (txtLog2 == null) return;
            if (txtLog2.InvokeRequired)
                txtLog2.Invoke(new Action(() => txtLog2.AppendText(msg + Environment.NewLine)));
            else
                txtLog2.AppendText(msg + Environment.NewLine);
        }

        #endregion

        #region Задание 3: Текст и эксперименты

        private void SetupTask3(TabPage page)
        {
            Label lblDesc = new Label { Text = "Анализ текста, сортировка слов и замер времени.", Location = new Point(10, 10), AutoSize = true };

            Button btnLoadText = new Button { Text = "Загрузить текст", Location = new Point(10, 40), Width = 150 };
            btnLoadText.Click += BtnLoadText_Click;

            Button btnRunExp = new Button { Text = "Запустить эксперименты", Location = new Point(170, 40), Width = 180 };
            btnRunExp.Click += BtnRunExperiment_Click;

            Label lblA1 = new Label { Text = "Алгоритм 1:", Location = new Point(360, 20), AutoSize = true };
            cmbTextAlgo1 = new ComboBox { Location = new Point(360, 40), Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTextAlgo1.Items.AddRange(new string[] { "BubbleSort", "InsertSort" });
            cmbTextAlgo1.SelectedIndex = 0;

            Label lblA2 = new Label { Text = "Алгоритм 2:", Location = new Point(510, 20), AutoSize = true };
            cmbTextAlgo2 = new ComboBox { Location = new Point(510, 40), Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTextAlgo2.Items.AddRange(new string[] { "QuickSort", "RadixSort" });
            cmbTextAlgo2.SelectedIndex = 0;

            Button btnSaveLog3 = new Button { Text = "Сохранить лог", Location = new Point(660, 38), Width = 120 };
            btnSaveLog3.Click += (s, e) => SaveLogToFile(rtbTextLog?.Text, "Log_Task3.txt");

            rtbTextLog = new RichTextBox { Location = new Point(10, 80), Width = 460, Height = 600, ReadOnly = true, Font = new Font("Consolas", 9) };

            dgvResults = new DataGridView { Location = new Point(480, 80), Width = 480, Height = 300, AllowUserToAddRows = false, ReadOnly = true };
            dgvResults.Columns.Add("Size", "Размер (слов)");
            dgvResults.Columns.Add("Algo1Time", "Время 1 (мс)");
            dgvResults.Columns.Add("Algo2Time", "Время 2 (мс)");

            page.Controls.Add(lblDesc); page.Controls.Add(btnLoadText); page.Controls.Add(btnRunExp);
            page.Controls.Add(lblA1); page.Controls.Add(cmbTextAlgo1);
            page.Controls.Add(lblA2); page.Controls.Add(cmbTextAlgo2);
            page.Controls.Add(btnSaveLog3);
            page.Controls.Add(rtbTextLog); page.Controls.Add(dgvResults);
        }

        private void BtnLoadText_Click(object? sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "Text Files|*.txt" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string text = File.ReadAllText(ofd.FileName);
                char[] delimiters = new char[] { ' ', '\r', '\n', '.', ',', '!', '?', ';', ':', '-', '"', '\'' };
                loadedWords = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                if (rtbTextLog != null)
                {
                    rtbTextLog.Clear();
                    rtbTextLog.AppendText($"Загружен файл: {ofd.FileName}\r\n");
                    rtbTextLog.AppendText($"Всего слов: {loadedWords.Length}\r\n");
                    rtbTextLog.AppendText("Первые 20 слов:\r\n");
                    rtbTextLog.AppendText(string.Join(", ", loadedWords.Take(20)) + "\r\n\r\n");
                }
            }
        }

        private void BtnRunExperiment_Click(object? sender, EventArgs e)
        {
            if (loadedWords == null || loadedWords.Length == 0)
            {
                MessageBox.Show("Сначала загрузите текстовый файл!");
                return;
            }
            if (cmbTextAlgo1 == null || cmbTextAlgo2 == null || dgvResults == null || rtbTextLog == null) return;

            dgvResults.Rows.Clear();
            rtbTextLog.AppendText($"[{DateTime.Now:HH:mm:ss}] Запуск экспериментов...\r\n");
            int[] sizes = { 100, 500, 1000, 2000, 5000 };

            foreach (int size in sizes)
            {
                if (size > loadedWords.Length) break;

                string[] testArray = loadedWords.Take(size).ToArray();
                string[] arrForAlgo1 = (string[])testArray.Clone();
                string[] arrForAlgo2 = (string[])testArray.Clone();

                Stopwatch sw1 = new Stopwatch();
                sw1.Start();
                RunStringSort(arrForAlgo1, cmbTextAlgo1.SelectedItem?.ToString() ?? "");
                sw1.Stop();

                Stopwatch sw2 = new Stopwatch();
                sw2.Start();
                RunStringSort(arrForAlgo2, cmbTextAlgo2.SelectedItem?.ToString() ?? "");
                sw2.Stop();

                double time1 = Math.Round(sw1.Elapsed.TotalMilliseconds, 3);
                double time2 = Math.Round(sw2.Elapsed.TotalMilliseconds, 3);
                dgvResults.Rows.Add(size, time1, time2);
                rtbTextLog.AppendText($"Размер {size}: {cmbTextAlgo1.SelectedItem} = {sw1.ElapsedMilliseconds} мс; {cmbTextAlgo2.SelectedItem} = {sw2.ElapsedMilliseconds} мс\r\n");
            }

            rtbTextLog.AppendText("\r\nЧастотный анализ (Топ-20 слов):\r\n");
            var freq = loadedWords.GroupBy(w => w.ToLower())
                                  .Select(g => new { Word = g.Key, Count = g.Count() })
                                  .OrderByDescending(x => x.Count)
                                  .Take(20);

            foreach (var item in freq)
            {
                rtbTextLog.AppendText($"{item.Word}: {item.Count}\r\n");
            }
        }

        private void RunStringSort(string[] arr, string algoName)
        {
            switch (algoName)
            {
                case "BubbleSort": BubbleSortStr(arr); break;
                case "InsertSort": InsertSortStr(arr); break;
                case "QuickSort": QuickSortStr(arr, 0, arr.Length - 1); break;
                case "RadixSort": RadixSortStr(arr); break;
            }
        }

        private void BubbleSortStr(string[] a)
        {
            for (int i = 0; i < a.Length; i++)
                for (int j = 0; j < a.Length - i - 1; j++)
                    if (string.Compare(a[j], a[j + 1], StringComparison.Ordinal) > 0)
                    { var t = a[j]; a[j] = a[j + 1]; a[j + 1] = t; }
        }

        private void InsertSortStr(string[] a)
        {
            for (int i = 1; i < a.Length; i++)
            {
                string key = a[i];
                int j = i - 1;
                while (j >= 0 && string.Compare(a[j], key, StringComparison.Ordinal) > 0)
                {
                    a[j + 1] = a[j];
                    j--;
                }
                a[j + 1] = key;
            }
        }

        private void QuickSortStr(string[] a, int low, int high)
        {
            if (low < high)
            {
                int pi = PartitionStr(a, low, high);
                QuickSortStr(a, low, pi - 1);
                QuickSortStr(a, pi + 1, high);
            }
        }

        private int PartitionStr(string[] a, int low, int high)
        {
            string pivot = a[high];
            int i = (low - 1);
            for (int j = low; j < high; j++)
            {
                if (string.Compare(a[j], pivot, StringComparison.Ordinal) < 0)
                {
                    i++;
                    var t = a[i]; a[i] = a[j]; a[j] = t;
                }
            }
            var t2 = a[i + 1]; a[i + 1] = a[high]; a[high] = t2;
            return i + 1;
        }

        private void RadixSortStr(string[] arr)
        {
            if (arr.Length == 0) return;
            int maxLen = arr.Max(s => s.Length);
            for (int i = maxLen - 1; i >= 0; i--)
            {
                CountingSortByChar(arr, i);
            }
        }

        private void CountingSortByChar(string[] arr, int charIndex)
        {
            int n = arr.Length;
            string[] output = new string[n];
            int[] count = new int[256];

            for (int i = 0; i < 256; i++) count[i] = 0;
            for (int i = 0; i < n; i++)
            {
                int charVal = (charIndex < arr[i].Length) ? arr[i][charIndex] : 0;
                count[charVal]++;
            }
            for (int i = 1; i < 256; i++) count[i] += count[i - 1];
            for (int i = n - 1; i >= 0; i--)
            {
                int charVal = (charIndex < arr[i].Length) ? arr[i][charIndex] : 0;
                output[count[charVal] - 1] = arr[i];
                count[charVal]--;
            }
            for (int i = 0; i < n; i++) arr[i] = output[i];
        }

        #endregion

        #region Общие методы

        private void SaveLogToFile(string? text, string defaultFileName)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Лог пуст, нечего сохранять.");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Text Files|*.txt",
                FileName = defaultFileName
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(sfd.FileName, text);
                    MessageBox.Show("Лог успешно сохранен!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении: " + ex.Message);
                }
            }
        }

        #endregion
    }
}