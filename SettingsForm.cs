using System;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;

namespace SearchEngine
{
    public partial class SettingsForm : Form
    {
        private TextBox apiKeyTextBox;
    private Button saveButton;
        public SettingsForm()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Settings";
            this.Size = new Size(300, 200);
            this.StartPosition = FormStartPosition.CenterParent;

            // API Key Label
            Label apiKeyLabel = new Label
            {
                Text = "API Key:",
                Location = new Point(10, 20),
                AutoSize = true
            };
            this.Controls.Add(apiKeyLabel);

            // API Key TextBox
            apiKeyTextBox = new TextBox
            {
                Location = new Point(10, 50),
                Width = 260
            };
            this.Controls.Add(apiKeyTextBox);

            // Save Button
            saveButton = new Button
            {
                Text = "Save",
                Location = new Point(10, 100),
                Width = 80
            };
            saveButton.Click += SaveButton_Click; // Event handler for save button
            this.Controls.Add(saveButton);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Save the API key (you can save it to a config file or in memory)
            string apiKey = apiKeyTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(apiKey))
            {
                
                // Save the API key
                Properties.Settings.Default.ApiKey = apiKey;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();

                MessageBox.Show("API Key saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Close the settings form
           
            
            }
            else
            {
                MessageBox.Show("Please enter a valid API Key.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
