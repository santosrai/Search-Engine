/*
 * Author : Santosh rai & Djante
 * date : 2024.12.01
 * email : srai3@students.solano.edu
 * class : CIS 022
 * desc : Search Engine App- Look
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace SearchEngine
{


    public partial class SearchForm : Form
    {
        // Declare class-level field
        private Panel resultsPanel;
        private Label welcomeLabel;

        private Panel topBarPanel;  // Topbar Panel
        private Panel sidebarPanel; // Sidebar Panel
        private Button toggleButton; // Button to toggle sidebar
        private Timer sidebarTimer; // Timer for animation
        private bool isSidebarVisible = true; // Track sidebar visibility


        private FlowLayoutPanel chatPanel;
        private bool isFirstSearch = true; // Track if it's the first search

        private Panel allResultsPanel;
        private List<string> _recentQueries = new List<string>();

        private Label loadingLabel;
        private Label recentLabel;
        private FlowLayoutPanel recentQueriesPanel;

        private List<string> allQueries = new List<string>
        {
            "How to learn C#?",
            "Best practices for API design",
            "Understanding async and await",
            "Top 10 programming languages in 2024",
            "What is dependency injection?"
        };

        private string query;

        // store and manage query and result history
        private List<(string, IEnumerable<SearchData>)> _queryHistory = new List<(string, IEnumerable<SearchData>)>();

        // For SerpiApi
        private SerpApiService _serpApiService;
        public SearchForm()
        {
            InitializeComponent();
            InitializeApiKey(); // initialize the Serp Api key
            SetupUI();
            InitializeSidebarToggle(); // Initialize the sidebar toggle
            PopulateRecentQueries(); // Populate recent queries

        }

        private void InitializeApiKey()
        {

            // Initialize SerpApi with API key
            string apiKey = Properties.Settings.Default.ApiKey;
            _serpApiService = new SerpApiService(apiKey);



        }

        private void InitializeSidebarToggle()
        {

            // Path to the sidebar icon
            string sidbarImgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img", "Dock.png");


            // Toggle Button
            PictureBox toggleIcon = new PictureBox
            {
                Width = 20,
                Height = 20,
                Location = new Point(10, 10), 
                Image = Image.FromFile(sidbarImgPath), 
                SizeMode = PictureBoxSizeMode.StretchImage, 
                Cursor = Cursors.Hand 
            };

            toggleIcon.BackColor = sidebarPanel.BackColor;
            toggleIcon.Click += ToggleButton_Click;
            
            // add to main panel
            this.Controls.Add(toggleIcon);
            toggleIcon.BringToFront();




            // Initialize the timer
            sidebarTimer = new Timer();
            sidebarTimer.Interval = 15; // Adjust for speed of animation
            sidebarTimer.Tick += SidebarTimer_Tick;



            // Path to the new chat icon
            string newChatImgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img", "new-chat.png");



            // New Search
            PictureBox newSearchChatIcon = new PictureBox
            {
                Width = 20,
                Height = 20,
                Location = new Point(toggleIcon.Right + 10, 10), // Position next to the toggle button
                Image = Image.FromFile(newChatImgPath), // Load the image
                SizeMode = PictureBoxSizeMode.StretchImage, // Adjust size mode
                Cursor = Cursors.Hand 
            };

            newSearchChatIcon.BackColor = sidebarPanel.BackColor;
            newSearchChatIcon.Click += NewSearchChatButton_Click; // Event handler for refresh

            this.Controls.Add(newSearchChatIcon); // Add to the form
            newSearchChatIcon.BringToFront();

            // Initialize the timer
            sidebarTimer = new Timer();
            sidebarTimer.Interval = 15; // Adjust for speed of animation
            sidebarTimer.Tick += SidebarTimer_Tick;
        }

        private void NewSearchChatButton_Click(object sender, EventArgs e)
        {
            // Logic to refresh chat results
            NewSearchChat();
        }

        private void NewSearchChat()
        {
            // Show the welcome message again
            welcomeLabel.Show();
            // clear the chat panel and reload data
            chatPanel.Controls.Clear();

            // update the queries 
            UpdateRecentQueriesSidebar(query);

            // if query isnot null
            if (query != null)
            {
                PopulateRecentQueries();
            }


        }

        private void ToggleButton_Click(object sender, EventArgs e)
        {
            // Start the animation
            sidebarTimer.Start();
        }

        private void SidebarTimer_Tick(object sender, EventArgs e)
        {
            if (isSidebarVisible)
            {
                // Animate sidebar collapsing
                if (sidebarPanel.Width > 0)
                {
                    sidebarPanel.Width -= 10; // Adjust for speed of collapse
                }
                else
                {
                    sidebarTimer.Stop();
                    isSidebarVisible = false;
                }
            }
            else
            {
                // Animate sidebar expanding
                if (sidebarPanel.Width < 200)
                {
                    sidebarPanel.Width += 10; // Adjust for speed of expand
                }
                else
                {
                    sidebarTimer.Stop();
                    isSidebarVisible = true;
                }
            }

        }

        private void SetupUI()
        {
            // Set form properties
            this.Text = "Look";
            this.Width = 850;
            this.Height = 650;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;


            // Top Header
            topBarPanel = new Panel
            {
                Width = 800,
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(45, 45, 45)
            };

            this.Controls.Add(topBarPanel);

            // Title Label
            Label titleLabel = new Label
            {
                Text = "Look",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            topBarPanel.Controls.Add(titleLabel);

            //Sidebar Panel
            sidebarPanel = new Panel
            {
                Width = 200,
                Dock = DockStyle.Left,

                BackColor = Color.FromArgb(45, 45, 45)
            };
            this.Controls.Add(sidebarPanel);

            // Recent Queries Section
            recentLabel = new Label
            {

                Text = "Recent",
                Height = 100,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 80, 0, 0),
                TextAlign = ContentAlignment.MiddleCenter, // Center the text


            };

            sidebarPanel.Controls.Add(recentLabel);
            recentLabel.BringToFront();

            // Panel to hold recent queries
            recentQueriesPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Location = new Point(10, 200),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown, // Controls flow vertically
                WrapContents = false,
                
                BackColor = Color.FromArgb(45, 45, 45)
            };
            sidebarPanel.Controls.Add(recentQueriesPanel);
            recentQueriesPanel.BringToFront();

            // Settings Icon
            string settingsImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img", "settings.png");

            // Check if the settings image file exists
            if (!File.Exists(settingsImagePath))
            {
                MessageBox.Show("Settings image not found: " + settingsImagePath);
                return; // Exit if the image is not found
            }

            // Settings Icon (using PictureBox)
            PictureBox settingsIcon = new PictureBox
            {
                Width = 20,
                Height = 20,
                Location = new Point(10, sidebarPanel.Height - 30),
                Image = Image.FromFile(settingsImagePath),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Cursor = Cursors.Hand
            };
            settingsIcon.Click += SettingsIcon_Click; 
            sidebarPanel.Controls.Add(settingsIcon); 
            settingsIcon.BringToFront();

            // Main content panel
            FlowLayoutPanel mainPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown, // Controls flow vertically
                WrapContents = false,
                AutoScroll = true,
                Visible = true,
                BackColor = Color.FromArgb(40, 40, 40) 
            };

            this.Controls.Add(mainPanel);



            // Add scrollable results panel
            resultsPanel = new Panel
            {

                Width = 600,
                Height = 450,
                AutoScroll = true,

            };
            mainPanel.Controls.Add(resultsPanel);

            // Welcome Label
            welcomeLabel = new Label
            {
                Text = "What can I help with?",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                ForeColor = Color.White
            };


            // Calculate center position
            int centerX = 200;
            int centerY = (this.ClientSize.Height - welcomeLabel.Height) / 2;

            // set position of welcome label
            welcomeLabel.Location = new Point(centerX, centerY);


            resultsPanel.Controls.Add(welcomeLabel);

            // Initialize a panel to hold all results
            allResultsPanel = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = Color.FromArgb(40, 40, 40),
                Padding = new Padding(10)
            };
            resultsPanel.Controls.Add(allResultsPanel);



            // Bottom input area
            Panel inputPanel = new Panel
            {

                Dock = DockStyle.Bottom, // Dock to the bottom

            };
            mainPanel.Controls.Add(inputPanel);


            // inputSearchBox
            TextBox inputSearchBox = new TextBox
            {

                Multiline = true,
                Font = new Font("Segoe UI", 12),
                Height = 100,

                MaximumSize = new Size(400, 100), // Set maximum height to 100
                ScrollBars = ScrollBars.None, // hidee scrollbars
                BorderStyle = BorderStyle.FixedSingle
            };


            inputSearchBox.Width = 400; // Set a fixed width

            // Calculate the vertical position to center it
            int inSearchBoxY = (inputPanel.Height - inputSearchBox.Height) / 2;

            // Calculate the horizontal position to center it
            int inSearchBoxX = (inputPanel.Width - inputSearchBox.Width) / 2;

            // Set the location of the inputSearchBox
            inputSearchBox.Location = new Point(50, inSearchBoxY); // Center both horizontally and vertically

            inputPanel.Controls.Add(inputSearchBox);


            // Send button
            Button searchButton = new Button
            {
                Text = "Search",
                Width = 80,
                Height = 35,
                Location = new Point(inputSearchBox.Right + 10, 12),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 120, 215), 
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold), 
                Cursor = Cursors.Hand 
            };
            searchButton.FlatAppearance.BorderSize = 0;


            searchButton.Click += (object sender, EventArgs e) =>
            {
                query = inputSearchBox.Text.Trim();

                if (string.IsNullOrEmpty(query))
                {
                    MessageBox.Show("Please enter a search query", "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                searchButton_Click(sender, e, query);
            };


            inputPanel.Controls.Add(searchButton);

            
            // Initialize chat panel
            chatPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Width = 600,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.FromArgb(40, 40, 40) // Background color for chat

            };
            resultsPanel.Controls.Add(chatPanel); // Add chatPanel to mainPanel

            mainPanel.BringToFront();

        }


        private void SettingsIcon_Click(object sender, EventArgs e)
        {
            // Open the settings form
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.ShowDialog(); // Show as a dialog

            // refresh the Serp Api Key
            InitializeApiKey();
        }

        // 
        private void AddChatMessage(string title, string url, string description, Image thumbnail, bool isUserQuery)
        {
            // Create a panel for the chat message
            Panel messagePanel = new Panel
            {
                Width = chatPanel.Width - 100,
                AutoSize = true,
                AutoScroll = true,
                BackColor = isUserQuery ? Color.FromArgb(0, 120, 215) : Color.FromArgb(50, 50, 50), 
                Margin = new Padding(10),
                Padding = new Padding(10),
                Dock = isUserQuery ? DockStyle.Right : DockStyle.Left // Position based on who sent the message
            };
          

            // Title Label (Top)
            Label titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Margin = new Padding(5),
                Location = new Point(10, 10),

            };
            messagePanel.Controls.Add(titleLabel);

            // Declare linkLabel variable
            LinkLabel linkLabel = null;

            // LinkLabel for URL (Middle)
            if (!string.IsNullOrEmpty(url))
            {
                linkLabel = new LinkLabel
                {
                    Text = url,
                    AutoSize = true,
                    ForeColor = Color.White,
                    Margin = new Padding(5),
                    Dock = DockStyle.None,
                    Location = new Point(10, titleLabel.Bottom + 2),
                    MaximumSize = new Size(chatPanel.Width - 100, 0), // Optional: Set a maximum width for wrapping
                };
                linkLabel.LinkClicked += (sender, e) =>
                {
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = url,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not open URL: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    };
                };
                messagePanel.Controls.Add(linkLabel);
            }

            // Description Label (Bottom)
            if (!string.IsNullOrEmpty(description))
            {
                Label descriptionLabel = new Label
                {
                    Text = description,
                    AutoSize = true,
                    MaximumSize = new Size(chatPanel.Width - 100, 0),
                    ForeColor = Color.White, 
                    Margin = new Padding(5),
                    Dock = DockStyle.None,
                    Location = linkLabel != null ? new Point(10, linkLabel.Bottom + 5) : new Point(0, 0)
                };
                messagePanel.Controls.Add(descriptionLabel);
            }

            // Thumbnail (if available)
            if (thumbnail != null)
            {
                PictureBox thumbnailBox = new PictureBox
                {
                    Image = thumbnail,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Width = 50, 
                    Height = 50, 
                    Margin = new Padding(5),
                    Dock = DockStyle.None,
                    Location = linkLabel != null ? new Point(10, linkLabel.Bottom + 5) : new Point(0, 0) 
                };
                messagePanel.Controls.Add(thumbnailBox);
            }

            // Add the message panel to the chatPanel
            chatPanel.Controls.Add(messagePanel);
            chatPanel.ScrollControlIntoView(messagePanel); 
        }

        private void UpdateRecentQueriesSidebar(string queries)
        {
            allQueries.Add(queries);
        }


        private void PopulateRecentQueries()
        {

            // Clear existing recent queries in the sidebar
            recentQueriesPanel.Controls.OfType<Label>().Where(l => l.Name == "RecentQueryLabel").ToList().ForEach(l => recentQueriesPanel.Controls.Remove(l));

            // Add mock recent queries to the sidebar
            foreach (var query in allQueries)
            {
                Label queryLabel = new Label
                {
                    Text = query,
                    AutoSize = true,
                    Margin = new Padding(5),
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(40, 40, 40),
                    Name = "RecentQueryLabel" // Set a name for easy identification
                };
                recentQueriesPanel.Controls.Add(queryLabel);
                queryLabel.BringToFront();
            }
        }

        private async void searchButton_Click(object sender, EventArgs e, string query)
        {

            // Display the user's query in the chat
            AddChatMessage($" {query}", "", "", null, true); // Pass true for user query


            // Hide welcome label on first search
            if (isFirstSearch)
            {
                welcomeLabel.Hide();
                isFirstSearch = false; // Set to false after the first search
            }

            try
            {
                // Perform search
                var results = await _serpApiService.SearchAsync(query);

                // Store the query and results in history
                _queryHistory.Add((query, results));


                // Check if results are empty
                if (results.Count == 0)
                {
                    AddChatMessage("No results found.", "", "", null, false);
                    welcomeLabel.Show(); // Show welcome label if no results
                }
                else
                {
                    // Display results in chat format
                    foreach (var result in results)
                    {
                        AddChatMessage(result.Title, result.Url, result.Description, result.ThumbnailImage, false);
                    }
                }
            }
            catch (Exception ex)
            {
                AddChatMessage($"Error: {ex.Message}", "", "", null, false);
            }
            finally
            {
                // hide welcomelabel
                welcomeLabel.Hide();

            }
        }



    }
}
