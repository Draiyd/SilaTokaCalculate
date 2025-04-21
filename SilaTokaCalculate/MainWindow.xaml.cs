using System;
using System.IO;
using System.Globalization;
using System.Windows;

namespace SilaTokaCalculate
{
    public partial class MainWindow : Window
    {
        private string _filePath = "data.txt";

        public MainWindow()
        {
            InitializeComponent();
            LoadDataFromFile();
        }

        private void LoadDataFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    ShowError($"Файл '{_filePath}' не найден!");
                    return;
                }

                string[] lines = File.ReadAllLines(_filePath);
                if (lines.Length < 2)
                {
                    ShowError("Файл должен содержать ровно две строки:\n1. Напряжение (В)\n2. Сопротивление (Ом)");
                    return;
                }

                // Проверка первой строки (напряжение)
                if (!IsValidNumber(lines[0], out double voltage) || voltage <= 0)
                {
                    ShowError($"Некорректное напряжение:\nДолжно быть положительное число\n(например: 12 или 220.5)\nПолучено: '{lines[0]}'");
                    return;
                }

                // Проверка второй строки (сопротивление)
                if (!IsValidNumber(lines[1], out double resistance) || resistance <= 0)
                {
                    ShowError($"Некорректное сопротивление:\nДолжно быть положительное число\n(например: 100 или 47.5)\nПолучено: '{lines[1]}'");
                    return;
                }

                txtFileData.Text = $"Напряжение (U): {voltage} В\nСопротивление (R): {resistance} Ом";
                ClearError();
            }
            catch (Exception ex)
            {
                ShowError($"Критическая ошибка:\n{ex.Message}");
            }
        }

        private bool IsValidNumber(string input, out double result)
        {
            // Проверяем, что строка содержит число
            bool isNumber = double.TryParse(
                input,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out result
            );

            
            if (isNumber)
            {
                string cleanedInput = input.Replace(",", ".").Trim();
                foreach (char c in cleanedInput)
                {
                    if (!char.IsDigit(c) && c != '.' && c != '-')
                    {
                        return false;
                    }
                }
            }

            return isNumber;
        }

        private void CalculateCurrent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    ShowError($"Файл '{_filePath}' не найден!");
                    return;
                }

                string[] lines = File.ReadAllLines(_filePath);
                if (lines.Length < 2)
                {
                    ShowError("Требуется две строки в файле:\n1. Напряжение\n2. Сопротивление");
                    return;
                }

              
                if (!IsValidNumber(lines[0], out double voltage) || voltage <= 0)
                {
                    ShowError($"Ошибка в напряжении:\n'{lines[0]}' — не число или значение ≤ 0");
                    return;
                }

                if (!IsValidNumber(lines[1], out double resistance) || resistance <= 0)
                {
                    ShowError($"Ошибка в сопротивлении:\n'{lines[1]}' — не число или значение ≤ 0");
                    return;
                }

                double current = voltage / resistance;
                ShowResult($"Ток (I) = {current.ToString("F3", CultureInfo.InvariantCulture)} А");
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка расчета:\n{ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            txtResult.Text = message;
            txtResult.Foreground = System.Windows.Media.Brushes.Red;
            txtResult.FontWeight = FontWeights.Bold;
        }

        private void ShowResult(string message)
        {
            txtResult.Text = message;
            txtResult.Foreground = System.Windows.Media.Brushes.Green;
            txtResult.FontWeight = FontWeights.Bold;
        }

        private void ClearError()
        {
            txtResult.Text = "";
        }
    }
}