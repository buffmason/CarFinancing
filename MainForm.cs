using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// Load system that allows data to inported/outported
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Xml.Linq;

namespace CarFinancing
{
    public partial class MainForm : Form
    {
        // Creating a global dictionary called carPrice
        Dictionary <string, int> carPrice = new Dictionary <string, int> ();

        public MainForm()
        {
            InitializeComponent();
        }

        // Ensuring that data is read in as soon as Start button is clicked
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Creating a variable and array 
            string price;
            string[] priceData;
            
            // Ensuring that a car and a model is always selected before being able to purchase or see payments
            rdBasic.Checked = true;
            cmbSelection.SelectedIndex = 0;

            // Reading in the data from Moodle
            StreamReader data = new StreamReader("CarPricing.txt");

            // Creating a loop so that the data is read in line by line until the last line
            while (data.EndOfStream == false)
            {
                price = data.ReadLine();
                priceData = price.Split(',');
                carPrice[priceData[0]] = int.Parse(priceData[1]);
            }
        }

        private void btnPayments_Click(object sender, EventArgs e)
        {
            // Creating new variables and stores the values from combo box in a new variable
            int make, month = 1;
            int.TryParse(cmbSelection.Text, out make);
            string name, age;

            double modelType, downPayment, totalCost, remainingBalance = 0;

            // Storing values from textboxes in the new variables and trims whitespace
            name = txtName.Text.Trim();
            age = txtAge.Text.Trim();
 

            // Setting new int loanTerm and sets it to 60 for 60 months
            int loanTermMonths = 60;

            // Setting new int annualRate and setting it to 0.06 for 6%
            double annualInterestRate = 0.06;

            // Storing data from DownPayment textbox into a variable 
            double.TryParse(txtDownPayment.Text, out downPayment);

            // Ensures the Name textbox is filled out
            if (name.Length == 0)
            {
                MessageBox.Show("Please enter a valid name");
                txtName.Clear();
                txtName.Focus();
                return;
            }

            // Ensures the Age textbox is filled out
            if (age.Length == 0)
            {
                MessageBox.Show("Please enter a valid Age");
                txtAge.Clear();
                txtAge.Focus();
                return;
            }

            // Ensures the DownPayment textbox is filled out
            if (downPayment == 0)
            {
                MessageBox.Show("Please enter a valid Down Payment");
                txtDownPayment.Clear();
                txtDownPayment.Focus();
                return;
            }


            // Creates a loop to determine the value of modelType based on selection of radio button
            if (rdBasic.Checked)
            {
                modelType = 1;
            }
            else if (rdHybrid.Checked)
            {
                modelType = 1.2;
            }
            else
            {
                modelType = 1.3;
            }

 
            // Creating equation for total cost and storing it as a currency string
            totalCost = (int)(modelType * carPrice[cmbSelection.Text]);
            lstMonthlyPayments.Items.Add("Total cost: " + totalCost.ToString("C"));

            // Equations used to calculate each payment
            double monthlyInterestRate = annualInterestRate / 12;
            double loanAmount = totalCost - downPayment;
            double monthlyPayment = (loanAmount * monthlyInterestRate) / (1 - Math.Pow(1 +
            monthlyInterestRate, -loanTermMonths));

            // Adding lables to the listbox and clearning a line underneathe it
            lstMonthlyPayments.Items.Add(string.Format(string.Format("{0,5}{1,20}{2,24}", "Month", "Monthly Payment", "Remaining Balance")));
            lstMonthlyPayments.Items.Add("");

            // Using a while loop to show the values until the loan has nothing left before going into the negative
            while (loanAmount > 0)
            {
                // Creating a remaining balance variable and giving it an equation
                loanAmount = loanAmount - monthlyPayment; 
                // Display month, monthlyPayment, and remainingBalance in the MonthlyPayments listbox and not allowing loanAmount to go below 0; 
                if (loanAmount > 0)
                {
                    lstMonthlyPayments.Items.Add((month++, monthlyPayment.ToString("C"), loanAmount.ToString("C")));
                }
            }
      
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            // Closes application when "Exit" button is clicked
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            // Clearing the form when "Clear" button is clicked
            txtName.Text = string.Empty;
            txtAge.Text = string.Empty;
            cmbSelection.SelectedIndex = -1;
            txtDownPayment.Text = string.Empty;
            lstMonthlyPayments.Items.Clear();
            rdBasic.Checked = false;
            rdHybrid.Checked = false;   
            rdSport .Checked = false;   
        }

        private void btnPurchase_Click(object sender, EventArgs e)
        {
            // Creating new variables to be used
            string name, age, downPayment, modelType;
            // Creating a string variable and assinging it the value of which Selection of car is made
            string make = cmbSelection.Text;

            // Storing values from textboxes in the new variables and trims whitespace
            name = txtName.Text.Trim();
            age = txtAge.Text.Trim();   
            downPayment = txtDownPayment.Text.Trim();

            // Ensures the Name textbox is filled out
            if (name.Length == 0)
            {
                MessageBox.Show("Please enter a valid name");
                txtName.Clear();
                txtName.Focus();
                return;
            }

            // Ensures the Age textbox is filled out
            if (age.Length == 0)
            {
                MessageBox.Show("Please enter a valid Age");
                txtAge.Clear();
                txtAge.Focus();
                return;
            }

            // Ensures the DownPayment textbox is filled out
            if (downPayment.Length == 0)
            {
                MessageBox.Show("Please enter a valid Down Payment");
                txtDownPayment.Clear(); 
                txtDownPayment.Focus();
                return;
            }

            // Uses an if statement to change modelType based on which radio button is selected 
            if (rdBasic.Checked)
            {
                modelType = "Basic";
            }
            else if (rdHybrid.Checked)
            {
                modelType = "Hyrbid";
            }
            else
            {
                modelType = "Sport";
            }

            // Writes the data stored in form to an external file while adding a line for each new entry
            StreamWriter writer = new StreamWriter("Purchase.txt", append: true);
            writer.WriteLine("Name: " + name + " " + "Age: " + age + " " + "Make: " + make + " " + "Model type: " + modelType + " " + "Down Payment: " + downPayment + " ");
            writer.Close();
        }

        private void dataTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            {

                // Read the data from the Purchase.txt file
                StreamReader data = new StreamReader("Purchase.txt");

                // Creating string variable called Line
                string line;

                // Adding existing purchases to the listbox
                lstPurchasesMade.Items.Add("Existing Purchases");

                // Using a loop to add data up until the last line
                while (data.EndOfStream == false)
                {
                    line = data.ReadLine();
                    lstPurchasesMade.Items.Add(line);
                }

                // Allows the purchase button to be used even if the purchases made tab is selected before the car purchase tab
                data.Close();
            }

        }
    }
}
