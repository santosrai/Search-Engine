using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchEngine
{
    public partial class SearchForm : Form
    {

        private Program _searchProgram;
        private Panel resultsPanel;
        public SearchForm()
        {
            InitializeComponent();
            InitializeSearchProgram();

        }

        private void InitializeSearchProgram()
        {
            //string sourceDirPath = Path.Combine(Environment.CurrentDirectory, "data");\
             var sourceDirPath = Directory.GetCurrentDirectory() + @"\data\search-data-M.txt";
            _searchProgram = new Program(sourceDirPath);
            SetupUI();

        }

        private void SetupUI()
        {
            // Set form properties
            this.Text = "SearchForm";
            this.Width = 800;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;


            // Welcome Label
            Label welcomeLabel = new Label
            {
                Text = "What can I help with?",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point((this.ClientSize.Width - 300) / 2, 100),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(welcomeLabel);

            // Add scrollable results panel
            Panel resultsPanel = new Panel
            {
                Location = new Point(20, 50),
                Width = 700,
                Height = 450,
                AutoScroll = true,
                //BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(resultsPanel);
            resultsPanel.Hide();

            // Buttons Panel
            Panel buttonsPanel = new Panel
            {
                Width = 600,
                Height = 60,
                Location = new Point((this.ClientSize.Width - 600) / 2, 200),
                BackColor = Color.Transparent
            };
            this.Controls.Add(buttonsPanel);

            // Bottom input area
            Panel inputPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Bottom,
            };
            this.Controls.Add(inputPanel);


            // TextBox for input
            TextBox inputBox = new TextBox
            {
                Width = 600,
                Location = new Point(20, 15),
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.FixedSingle
            };
            inputPanel.Controls.Add(inputBox);

            // Send button
            Button sendButton = new Button
            {
                Text = "Send",
                Width = 80,
                Height = 35,
                Location = new Point(630, 12),
                FlatStyle = FlatStyle.Flat
            };
            sendButton.FlatAppearance.BorderSize = 0;
            sendButton.Click += (sender, args) =>
            {
                string query = inputBox.Text;
                var searchResults = _searchProgram.FindWord(query);
                welcomeLabel.Hide();
                DisplayResults(resultsPanel,searchResults);
            };
            inputPanel.Controls.Add(sendButton);

            
            
        }

        //private void btnSearch_Click(object sender, EventArgs e)
        //{
        //    string searchQuery = txtSearchInput.Text;
        //    try
        //    {
        //        var results = _searchProgram.FindWord(searchQuery);
        //        if (results != null)
        //        {
        //            // display results
        //            DisplayResults(results);
        //        }
        //    } catch (Exception ex)
        //    {
        //        MessageBox.Show($"Search error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }



        //}


        private void DisplayResults(Panel resultsPanel, IEnumerable<SearchData> searchResults)
        {
            // Clear previous results
            resultsPanel.Controls.Clear();

            resultsPanel.Show();
     

            int yOffset = 0;

            foreach (var result in searchResults)
            {
                // Panel for each search result
                Panel resultPanel = new Panel
                {
                    Width = resultsPanel.Width - 20,
                    Height = 80,
                    Location = new Point(10, yOffset),
                    
                    BorderStyle = BorderStyle.None

                };

                // Clickable title (like a hyperlink)
                LinkLabel titleLink = new LinkLabel
                {
                    Text = result.Title,
                    Location = new Point(0, 0),
                    AutoSize = true,
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    LinkColor = Color.Blue
                };
                titleLink.Click += (sender, args) =>
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = result.Url,
                        UseShellExecute = true
                    });
                };

                // URL display
                Label urlLabel = new Label
                {
                    Text = result.Url,
                    Location = new Point(0, 25),
                    AutoSize = true,
                    ForeColor = Color.Green
                };

                // Snippet/description (using the word and frequency)
                Label descriptionLabel = new Label
                {
                    Text = $"Found '{result.Word}' with frequency {result.Frequency}",
                    Location = new Point(0, 45),
                    AutoSize = true,
                    ForeColor = Color.Gray
                };

                // Add elements to the panel
                resultPanel.Controls.Add(titleLink);
                resultPanel.Controls.Add(urlLabel);
                resultPanel.Controls.Add(descriptionLabel);

                // Add panel to the form
                resultsPanel.Controls.Add(resultPanel);

                // Adjust yOffset for the next result
                yOffset += 80;
            }
        }


    }
}
