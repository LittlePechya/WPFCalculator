using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

using NCalc;
using System.Text.RegularExpressions;

namespace WPFCalculator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _errorFlag = false;
        private double memory = 0;

        public MainWindow()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            InitializeComponent();
            PreviewKeyDown += MainWindow_PreviewKeyDown;
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Calculate();
            }
            else if (IsNumericKey(e.Key))
            {
                char digit = GetDigitFromKey(e);
                Input.Text += digit.ToString();
            } 
            else if (e.Key == Key.Back)
            {
                Backspace();
            } 
            else if (e.Key == Key.Back)
            {
                Clear();
            }
            else if (e.Key == Key.OemPlus || e.Key == Key.Add)
            {
                AddToInput("+");
            }
            else if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
            {
                AddToInput("-");
            }
            else if (e.Key == Key.OemQuestion || e.Key == Key.Divide)
            {
                AddToInput("/");
            }
            else if (e.Key == Key.Multiply)
            {
                AddToInput("*");
            }
            else if (e.Key == Key.OemOpenBrackets || e.Key == Key.D8)
            {
                AddToInput("(");
            }
            else if (e.Key == Key.OemCloseBrackets || e.Key == Key.D9)
            {
                AddToInput(")");
            }
            else if (e.Key == Key.OemPeriod || e.Key == Key.Decimal)
            {
                AddToInput(".");
            }
            else if (e.Key == Key.OemComma)
            {
                AddToInput(",");
            }
        }
        
        private bool IsNumericKey(Key key)
        {
            return (key >= Key.NumPad0 && key <= Key.NumPad9) || (key >= Key.D0 && key <= Key.D9);
        }

        private char GetDigitFromKey(KeyEventArgs e)
        {
            if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                return (char)(e.Key - Key.NumPad0 + '0');
            }
            else if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                return (char)(e.Key - Key.D0 + '0');
            }

            throw new ArgumentException("Неверная клавиша");
        }

        private void EnableAndCleanInput()
        {
            if (_errorFlag)
            {
                Input.IsEnabled = true;
                Input.Text = "";
                _errorFlag = false;
            }
        }

        private void DisableInput()
        {
            _errorFlag = true;
            Input.IsEnabled = false;
        }

        private void Calculate(object sender, RoutedEventArgs e)
        {
            EnableAndCleanInput();
            string userInput = InputParse(Input.Text);

            if (userInput == "" || userInput == null)
            {
                Input.Text = "0";
                return;
            }

            NCalc.Expression expr = new NCalc.Expression(userInput);
            object result;

            try
            {
                result = expr.Evaluate();
                Input.Text = result.ToString().Replace(',', '.');

            }
            catch (Exception ex)
            {
                // Обработка других исключений
                Input.Text = "Error: " + ex.Message;
                DisableInput();
            }
        }

        private void Calculate()
        {
            EnableAndCleanInput();
            string userInput = InputParse(Input.Text);
            
            if (userInput == "" || userInput == null)
            {
                Input.Text = "0";
                return;
            }

            NCalc.Expression expr = new NCalc.Expression(userInput);
            object result;

            try
            {
                result = expr.Evaluate();
                Input.Text = result.ToString().Replace(',', '.');
            }
            catch (Exception ex)
            {
                // Обработка других исключений
                Input.Text = "Ошибка: " + ex.Message;
                DisableInput();
            }
        }

        private string InputParse(string input)
        {
            string result = Regex.Replace(input, @"\b\w+\b", match => char.ToUpper(match.Value[0]) + match.Value.Substring(1));
            // result = result.Replace(',', '.');
            return result;
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            EnableAndCleanInput();
            Input.Text = "";
        }

        private void Clear()
        {
            EnableAndCleanInput();
            Input.Text = "";
        }

        private void Backspace(object sender, RoutedEventArgs e)
        {
            EnableAndCleanInput();

            string input = Input.Text;
            if (!string.IsNullOrEmpty(input))
            {
                input = input.Remove(input.Length - 1);
                Input.Text = input;
            }
        }

        private void Backspace()
        {
            EnableAndCleanInput();

            string input = Input.Text;
            if (!string.IsNullOrEmpty(input))
            {
                input = input.Remove(input.Length - 1);
                Input.Text = input;
            }
        }

        private void AddButtonValue(object sender, RoutedEventArgs e)
        {
            EnableAndCleanInput();

            if (_errorFlag)
            {
                Input.IsEnabled = true;
                Input.Text = "";
                _errorFlag = false;
            }

            Button button = (Button)sender;
            string buttonText = button.Content.ToString();
            if (buttonText == "π")
            {
                Input.Text += Math.PI;
            } else if (buttonText == "e")
            {
                Input.Text += Math.E;
            } else
            {
                Input.Text += buttonText;
            }
            Input.Text = Input.Text.Replace(',', '.');
        }

        private void AddToInput(string character)
        {
            EnableAndCleanInput();
            Input.Text += character;
        }

        private void Functions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;

            if (selectedItem != null)
            {
                string function = selectedItem.Content.ToString();
                AddToInput(function);
            }
        }

        private void Functions_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            comboBox.SelectedItem = null;

            if (comboBox.Tag != null)
            {
                comboBox.Text = comboBox.Tag.ToString();
            }
        }

        private void MemoryAdd(object sender, RoutedEventArgs e)
        {
            try
            {
                memory += double.Parse(Input.Text, CultureInfo.InvariantCulture);
            } catch (Exception ex)
            {
                Input.Text = "Error: " + ex.Message;
                DisableInput();
            }
        }

        private void MemorySubtract(object sender, RoutedEventArgs e)
        {
            try
            {
                memory -= double.Parse(Input.Text, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Input.Text = "Error: " + ex.Message;
                DisableInput();
            }
        }

        private void MemoryRecall(object sender, RoutedEventArgs e)
        {
            EnableAndCleanInput();
            AddToInput(Convert.ToString(memory));
        }

        private void MemoryClean(object sender, RoutedEventArgs e)
        {
            EnableAndCleanInput();
            memory = 0;
        }

        private void MemorySave(object sender, RoutedEventArgs e)
        {
            try
            {
                memory = double.Parse(Input.Text, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Input.Text = "Error: " + ex.Message;
                DisableInput();
            }
        }
    }
}
